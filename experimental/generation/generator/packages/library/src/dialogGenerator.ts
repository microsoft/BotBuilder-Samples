#!/usr/bin/env node
/*!
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
export * from './dialogGenerator'
import * as crypto from 'crypto'
import * as expressions from 'adaptive-expressions'
import * as fs from 'fs-extra'
import * as merger from './mergeAssets'
import * as lg from 'botbuilder-lg'
import * as os from 'os'
import * as ppath from 'path'
import * as ph from './generatePhrases'
import * as ps from './processSchemas'
import * as s from './schema'
import {SubstitutionsEvaluator} from './substitutions'

export enum FeedbackType {
    message,
    info,
    warning,
    error,
    debug
}

export type Feedback = (type: FeedbackType, message: string) => void

/**
 * Generate examples for a list of enum values by breaking on case space or underscore and generating subphrases.
 * @param values List of enum values to generate examples.
 */
export function examples(values: string[]): any {
    const examples: any = {}
    const engine = getExpressionEngine()
    for (const choice of values) {
        const values: string[] = []
        const {value, error} = engine.parse(`phrases('${choice}')`).tryEvaluate({})
        if (error) {
            throw new Error(error)
        }
        for (const phrase of value) {
            values.push(phrase)
        }
        examples[choice] = values
    }
    return examples
}

function templatePath(name: string, dir: string): string {
    return ppath.join(dir, name)
}

function computeHash(val: string): string {
    return crypto.createHash('md5').update(val).digest('hex')
}

// Normalize to OS line endings
function normalizeEOL(val: string): string {
    if (val.startsWith('#!/')) {
        // For linux shell scripts want line feed only
        val = val.replace(/\r/g, '')
    } else if (os.EOL === '\r\n') {
        val = val.replace(/\r\n/g, '\n').replace(/\n/g, os.EOL)
    } else {
        val = val.replace(/\r\n/g, os.EOL)
    }
    return val
}

// Stringify JSON with optional replacer
export function stringify(val: any, replacer?: any): string {
    if (typeof val === 'object') {
        val = normalizeEOL(JSON.stringify(val, replacer, '  '))
    }
    return val
}

function computeJSONHash(json: any): string {
    return computeHash(stringify(json))
}

const CommentHashExtensions = ['.lg', '.lu', '.qna']
const JSONHashExtensions = ['.dialog']
const GeneratorPattern = /\r?\n> Generator: ([a-zA-Z0-9]+)/
const ReplaceGeneratorPattern = /\r?\n> Generator: ([a-zA-Z0-9]+)/g
function addHash(path: string, val: any): any {
    const ext = ppath.extname(path)
    if (CommentHashExtensions.includes(ext)) {
        val = val.replace(ReplaceGeneratorPattern, '')
        if (!val.endsWith(os.EOL)) {
            val += os.EOL
        }
        val += `${os.EOL}> Generator: ${computeHash(val)}`
    } else if (JSONHashExtensions.includes(ext)) {
        const json = JSON.parse(val)
        delete json.$Generator
        json.$Generator = computeJSONHash(json)
        val = stringify(json)
    }
    return val
}

// Check to see if the contents of path are unchanged since generated
export async function isUnchanged(path: string): Promise<boolean> {
    let result = false
    const ext = ppath.extname(path)
    let file = await fs.readFile(path, 'utf8')
    if (CommentHashExtensions.includes(ext)) {
        const match = file.match(GeneratorPattern)
        if (match) {
            const oldHash = match[1]
            file = file.replace(GeneratorPattern, '')
            const hash = computeHash(file)
            result = oldHash === hash
        }
    } else if (JSONHashExtensions.includes(ext)) {
        const json = JSON.parse(file)
        const oldHash = json.$Generator
        if (oldHash) {
            delete json.$Generator
            const hash = computeJSONHash(json)
            result = oldHash === hash
        }
    }
    return result
}

// Get hashcode of the file
export async function getHashCode(path: string): Promise<string> {
    let oldHash
    const ext = ppath.extname(path)
    const file = await fs.readFile(path, 'utf8')
    if (CommentHashExtensions.includes(ext)) {
        const match = file.match(GeneratorPattern)
        if (match) {
            oldHash = match[1]
        }
    } else if (JSONHashExtensions.includes(ext)) {
        const json = JSON.parse(file)
        oldHash = json.$Generator
    }
    return oldHash
}

// Write file with error checking and hash code generation.
export async function writeFile(path: string, val: string, feedback: Feedback, skipHash?: boolean) {
    try {
        const dir = ppath.dirname(path)
        await fs.ensureDir(dir)
        val = normalizeEOL(val)
        if (!skipHash) {
            val = addHash(path, val)
        }
        await fs.writeFile(path, val)
    } catch (e) {
        const match = /position ([0-9]+)/.exec(e.message)
        if (match) {
            const offset = Number(match[1])
            val = `${val.substring(0, offset)}^^^${val.substring(offset)}`
        }
        feedback(FeedbackType.error, `${e.message}${os.EOL}${val}`)
    }
}

// Return template directories by combining explicit ones with form ones
export async function templateDirectories(templateDirs?: string[]): Promise<string[]> {
    // Fully expand all directories
    templateDirs = resolveDir(templateDirs || [])
    let startDirs = [...templateDirs]
    if (startDirs.length === 0) {
        // Default to children of built-in templates
        const templates = normalize(ppath.join(__dirname, '../templates'))
        for (const dirName of await fs.readdir(templates)) {
            const dir = ppath.join(templates, dirName)
            if ((await fs.lstat(dir)).isDirectory()) {
                // Add templates subdirectories as templates
                startDirs.push(dir)
            }
        }
    }
    return startDirs
}

async function generateFile(path: string, val: any, force: boolean, feedback: Feedback) {
    if (force || !await fs.pathExists(path)) {
        feedback(FeedbackType.info, `Generating ${path}`)
        await writeFile(path, val, feedback)
    } else {
        feedback(FeedbackType.warning, `Skipping already existing ${path}`)
    }
}

let expressionEngine: expressions.ExpressionParser
function getExpressionEngine(): expressions.ExpressionParser {
    if (!expressionEngine) {
        expressionEngine = new expressions.ExpressionParser((func: string): any => {
            switch (func) {
                case 'phrase': return ph.PhraseEvaluator
                case 'phrases': return ph.PhrasesEvaluator
                case 'substitutions': return SubstitutionsEvaluator
                default:
                    return expressions.ExpressionFunctions.standardFunctions.get(func)
            }
        })
    }
    return expressionEngine
}

// Generator template used in expanding schema
let generatorTemplate: lg.Templates

/**
 * Return directory to put asset in as driven by generator template.
 * @param extension File extension like .lg or .lu
 * @returns Directory where extension should be located.
 */
export function assetDirectory(extension: string): string {
    let dir = ''
    switch (extension) {
        case '.dialog': dir = 'dialogDir'
            break
        case '.lg': dir = 'generationDir'
            break
        case '.lu': dir = 'understandingDir'
            break
        case '.qna': dir = 'knowledgeDir'
            break
    }
    return dir ? generatorTemplate.evaluate(dir, {}) : ''
}

// Walk over JSON object, stopping if true from walker.
// Walker gets the current value, the parent object and full path to that object
// and returns false to continue, true to stop going deeper.
async function walkJSON(elt: any, fun: (val: any, obj?: any, path?: string) => Promise<boolean>, obj?: any, path?: any): Promise<void> {
    const done = await fun(elt, obj, path)
    if (!done) {
        if (typeof elt === 'object' || Array.isArray(elt)) {
            for (const key in elt) {
                await walkJSON(elt[key], fun, elt, pathName(path, key))
            }
        }
    }
}

function pathName(path: string | undefined, extension: string): string {
    return path ? `${path}/${extension}` : extension
}

function setPath(obj: any, path: string, value: any) {
    const key = path.substring(path.lastIndexOf('/') + 1)
    obj[key] = value
}

type Plain = {source: string, template: string}
type Template = lg.Templates | Plain | undefined
export const TemplateCache: Map<string, Template> = new Map<string, Template>()

async function findTemplate(name: string, templateDirs: string[]): Promise<Template> {
    let template: Template
    for (const dir of templateDirs) {
        let loc = templatePath(name, dir)
        template = TemplateCache.get(loc)
        if (!template) {
            if (await fs.pathExists(loc)) {
                // Direct file
                template = {source: loc, template: await fs.readFile(loc, 'utf8')}
                TemplateCache.set(loc, template)
                break
            } else {
                // LG file
                loc = templatePath(name + '.lg', dir)
                template = TemplateCache.get(loc)
                if (template) {
                    break
                } else if (await fs.pathExists(loc)) {
                    template = lg.Templates.parseFile(loc, undefined, getExpressionEngine())
                    TemplateCache.set(loc, template)
                    break
                }
            }
        } else {
            break
        }
    }
    return template
}

// Add prefix to [] imports in constant .lg files
const RefPattern = /^[ \t]*\[[^\]\n\r]*\].*$/gm
function addPrefixToImports(template: string, scope: any): string {
    return template.replace(RefPattern, (match: string) => {
        const ref = match.substring(match.indexOf('[') + 1, match.indexOf(']'))
        return `[${scope.prefix}-${ref}](${scope.prefix}-${ref})`
    })
}

function addPrefix(prefix: string, name: string): string {
    const dir = name.lastIndexOf('/')
    if (dir >= 0) {
        return `${name.substring(0, dir)}/${prefix}-${name.substring(dir + 1)}`
    } else {
        return `${prefix}-${name}`
    }
}

// Add information about a newly generated file.
// This also ensures the file does not exist already.
type FileRef = {name: string, shortName: string, fallbackName: string, fullName: string, relative: string}
function addFileRef(fullPath: string, outDir: string, prefix: string, tracker: any): FileRef | undefined {
    let ref: FileRef | undefined
    const basename = ppath.basename(fullPath, '.dialog')
    const ext = ppath.extname(fullPath).substring(1)
    const arr: FileRef[] = tracker[ext]
    if (!arr.find(ref => ref.name === basename)) {
        const shortName = basename.substring(prefix.length + 1, basename.indexOf('.'))
        ref = {
            name: basename,
            shortName: shortName,
            // Fallback is only used for .lg files
            fallbackName: basename.replace(/\.[^.]+\.lg/, '.lg'),
            fullName: ppath.basename(fullPath),
            relative: ppath.relative(outDir, fullPath)
        }
    }
    return ref
}

function existingRef(name: string, tracker: any): FileRef | undefined {
    const ext = ppath.extname(name).substring(1)
    let arr: FileRef[] = tracker[ext]
    if (!arr) {
        arr = []
        tracker[ext] = arr
    }
    return arr.find(ref => ref.fullName === name)
}

type Transform = {name: string, template: lg.Templates}
async function processTemplate(
    templateName: string,
    templateDirs: string[],
    outDir: string,
    scope: any,
    force: boolean,
    feedback: Feedback,
    ignorable: boolean,
    transforms: Transform[]): Promise<string> {
    let outPath = ''
    const oldDir = process.cwd()
    try {
        const ref = existingRef(templateName, scope.templates)
        if (ref) {
            // Simple file already existed
            feedback(FeedbackType.debug, `Reusing ${templateName}`)
            outPath = ppath.join(outDir, ref.relative)
        } else {
            const foundTemplate = await findTemplate(templateName, templateDirs)
            if (foundTemplate !== undefined) {
                const lgTemplate: lg.Templates | undefined = foundTemplate instanceof lg.Templates ? foundTemplate as lg.Templates : undefined
                const plainTemplate: Plain | undefined = !lgTemplate ? foundTemplate as Plain : undefined
                // Ignore templates that are defined, but are empty
                if (plainTemplate?.source || lgTemplate?.allTemplates.some(f => f.name === 'template')) {
                    // Constant file or .lg template so output
                    feedback(FeedbackType.debug, `Using template ${plainTemplate ? plainTemplate.source : lgTemplate?.id}`)

                    let filename = addPrefix(scope.prefix, templateName)
                    if (lgTemplate?.allTemplates.some(t => t.name === 'filename')) {
                        try {
                            filename = lgTemplate.evaluate('filename', scope) as string
                        } catch (e) {
                            throw new Error(`${templateName}: ${e.message}`)
                        }
                    } else {
                        // Infer name
                        const locale = filename.includes(scope.locale) ? `${scope.locale}/` : ''
                        filename = `${assetDirectory(ppath.extname(filename))}${locale}${scope.property ?? 'form'}/${ppath.basename(filename)}`
                    }

                    outPath = ppath.join(outDir, filename)
                    const ref = addFileRef(outPath, outDir, scope.prefix, scope.templates)
                    if (ref) {
                        // This is a new file
                        if (force || !await fs.pathExists(outPath)) {
                            feedback(FeedbackType.info, `Generating ${outPath}`)
                            // Add prefix to constant imports
                            let result = plainTemplate ? addPrefixToImports(plainTemplate.template, scope) : undefined
                            if (lgTemplate) {
                                process.chdir(ppath.dirname(lgTemplate.allTemplates[0].sourceRange.source))
                                result = lgTemplate.evaluate('template', scope) as string
                                process.chdir(oldDir)
                                if (Array.isArray(result)) {
                                    result = result.join(os.EOL)
                                }
                            }

                            // See if generated file has been overridden in templates
                            const existing = await findTemplate(filename, templateDirs) as Plain
                            if (existing && existing.source.endsWith(ppath.normalize(filename))) {
                                feedback(FeedbackType.info, `  Overridden by ${existing.source}`)
                                result = existing.template
                            }

                            // Ignore empty templates
                            if (result) {
                                const extension = ppath.extname(outPath).substring(1)
                                if (transforms.length > 0) {
                                    let body = result
                                    debugger
                                    try {
                                        body = JSON.parse(result)
                                    } catch (e) {
                                    }
                                    for (const transform of transforms) {
                                        body = transform.template.evaluate(
                                            transform.name, 
                                            {...scope, extension, body})
                                    }
                                }
                                const resultString = result as string
                                if (resultString.includes('**MISSING**')) {
                                    feedback(FeedbackType.error, `${outPath} has **MISSING** data`)
                                } else {
                                    const match = resultString.match(/\*\*([^0-9\s]+)[0-9]+\*\*/)
                                    if (match) {
                                        feedback(FeedbackType.warning, `Replace **${match[1]}<N>** with values in ${outPath}`)
                                    }
                                }
                                await writeFile(outPath, resultString, feedback)
                                scope.templates[extension].push(ref)
                            }
                        } else {
                            feedback(FeedbackType.warning, `Skipping already existing ${outPath}`)
                        }
                    }
                } else if (lgTemplate) {
                    // Pick up # transforms
                    if (lgTemplate.allTemplates.some(f => f.name == 'transforms')) {
                        let newTransforms = lgTemplate.evaluate('transforms', scope)
                        if (!Array.isArray(newTransforms)) {
                            newTransforms = [newTransforms]
                        }
                        for (const transform of newTransforms) {
                            feedback(FeedbackType.debug, `Adding transform ${transform}`)
                            transforms.push({name: transform, template: lgTemplate})
                        }
                    }

                    // Expand # templates
                    if (lgTemplate.allTemplates.some(f => f.name === 'templates')) {
                        feedback(FeedbackType.debug, `Expanding template ${lgTemplate.id}`)
                        let generated = lgTemplate.evaluate('templates', scope)
                        if (!Array.isArray(generated)) {
                            generated = [generated]
                        }
                        for (const generate of generated as any as string[]) {
                            feedback(FeedbackType.debug, `  ${generate}`)
                        }
                        for (const generate of generated as any as string[]) {
                            await processTemplate(generate, templateDirs, outDir, scope, force, feedback, false, transforms)
                        }
                    }

                }
            } else if (!ignorable) {
                feedback(FeedbackType.error, `Missing template ${templateName}`)
            }
        }
    } catch (e) {
        feedback(FeedbackType.error, e.message)
    } finally {
        process.chdir(oldDir)
    }
    return outPath
}

// Walk over locale .lu files and extract examples in >> var: comment blocks
async function globalExamples(outDir: string, scope: any): Promise<object> {
    const examples = {}
    const luFiles = scope.templates.lu
    for (const file of luFiles) {
        const path = ppath.join(outDir, file.relative)
        const contents = await fs.readFile(path, 'utf8')
        let lines = contents.split(os.EOL)
        if (lines.length < 2) {
            // Windows uses CRLF and that is how it is checked-in, but when an npm
            // package is built it switches to just LF.
            lines = contents.split('\n')
        }
        let collect: string | undefined
        for (const line of lines) {
            if (line.startsWith('>>')) {
                collect = line.substring(2, line.indexOf(':')).trim()
                if (!examples[collect]) {
                    examples[collect] = []
                }
            } else if (line.startsWith('>')) {
                collect = undefined
            } else if (collect && line.startsWith('-')) {
                examples[collect].push(line.substring(1).trim())
            }
        }
    }
    return examples
}

// Verify all $examples entries are entities or phrases
async function verifyExamples(schema: any, feedback: Feedback): Promise<void> {
    const allEntities = new Set<string>()
    await walkJSON(schema, async (elt, _obj, path) => {
        if (elt.$entities) {
            for (const entity of elt.$entities) {
                allEntities.add(entity)
            }
        }
        return false
    })
    await walkJSON(schema, async (elt, _obj, path) => {
        if (elt.$examples && !path?.endsWith('/items')) {
            if (path) {
                if (!elt.$entities) {
                    feedback(FeedbackType.error, `${path} is missing $entities`)
                } else {
                    for (const [locale, examples] of Object.entries(elt.$examples)) {
                        for (const entityOrPhrases of Object.keys(examples as object)) {
                            if (!entityOrPhrases.endsWith('Phrases') && !elt.$entities.includes(entityOrPhrases)) {
                                feedback(FeedbackType.warning, `Unknown $examples ${locale}/${entityOrPhrases} in ${path}`)
                            }
                        }
                    }
                }
            } else {
                for (const [locale, examples] of Object.entries(elt.$examples)) {
                    for (const entityOrPhrases of Object.keys(examples as object)) {
                        if (!entityOrPhrases.endsWith('Phrases') && !allEntities.has(entityOrPhrases)) {
                            feedback(FeedbackType.warning, `Unknown global $examples ${locale}/${entityOrPhrases}`)
                        }
                    }
                }
            }
        }
        return false
    })
}

// Verify enum and examples match
function verifyEnumAndGetExamples(schema: any, property: string, locale: string, entity: string, feedback: Feedback): string[] | object {
    const propertySchema = schema.properties[property]
    let examples = propertySchema.$examples?.[locale]?.[entity]
    let usedLocale = true
    let global = false
    if (!examples) {
        usedLocale = true
        global = true
        examples = schema.$examples?.[locale]?.[entity]
    }
    if (!examples) {
        usedLocale = false
        global = false
        examples = propertySchema.$examples?.['']?.[entity]
    }
    if (!examples) {
        usedLocale = false
        global = true
        examples = schema.$examples?.['']?.[entity]
    }
    if (!usedLocale) {
        locale = ''
    }

    const enums = propertySchema.enum ?? propertySchema.items?.enum
    if (examples && enums && entity.endsWith('Value')) {
        const exampleSet = new Set<string>(Object.keys(examples))
        const enumSet = new Set<string>(enums)
        const enumOnly = [...enumSet].filter(e => !exampleSet.has(e))
        const exampleOnly = [...exampleSet].filter(e => !enumSet.has(e))
        const description = `${global ? 'global ' : ''}${locale}/${entity} $examples`
        if (enumOnly.length > 0) {
            feedback(FeedbackType.error, `${property} has enums [${enumOnly.join(', ')}] not in ${description}.`)
        }
        if (exampleOnly.length > 0) {
            feedback(FeedbackType.error, `${property} has ${description} [${exampleOnly.join(', ')}] not in enum.`)
        }
    }
    return examples
}

async function processTemplates(
    schema: s.Schema,
    templateDirs: string[],
    locales: string[],
    outDir: string,
    scope: any,
    force: boolean,
    feedback: Feedback): Promise<void> {
    await verifyExamples(schema.schema, feedback)
    scope.templates = {}
    // We expand each template in the context of a combination of locale, property, type (schema name) and entity.
    // Since we only generate each filename once this means property only templates don't care about the entity and entity templates will be expanded for each entity.
    // If a template author wants to do different things with different entities they will need explicit templates for each desired entity.
    for (const locale of locales) {
        scope.locale = locale
        for (const property of schema.schemaProperties()) {
            scope.property = property.path
            scope.template = property.schema.$template
            scope.propertySchema = property.schema
            const entities = property.schema.$entities
            const templates = property.schema.$templates
            if (!entities || !templates) {
                feedback(FeedbackType.error, `'${property.path}' does not define $template, $entities or $templates.`)
            } else if (scope.property.includes('-') || scope.property.includes(' ')) {
                feedback(FeedbackType.error, `'${property.path}' cannot include space or dash.`)
            } else {
                // Pick up examples from property schema if unique entity
                if (!property.schema.$examples && (property.schema.examples || property.schema.items?.examples)) {
                    const entity = entities.filter((e: string) => e !== 'utterance')
                    if (entity.length == 1) {
                        const entityExamples = {}
                        entityExamples[entity[0]] = property.schema.examples ?? property.schema.items?.examples
                        property.schema.$examples = {
                            '': entityExamples
                        }
                    } else {
                        feedback(FeedbackType.warning, `For property ${property.path} use $examples rather than examples.`)
                    }
                }

                // Assume non-array are expressions to be interpreted in expandSchema
                const transforms = []
                for (const entityName of entities) {
                    // If expression will get handled by expandSchema
                    if (!entityName.startsWith('$')) {
                        scope.entity = entityName
                        feedback(FeedbackType.debug, `=== ${scope.locale} ${scope.property} ${scope.entity} ===`)
                        scope.examples = verifyEnumAndGetExamples(schema.schema, property.path, locale, entityName, feedback)
                        for (const template of templates) {
                            await processTemplate(template, templateDirs, outDir, scope, force, feedback, false, transforms)
                        }

                        delete scope.entity
                        delete scope.examples
                    }
                }
            }
            delete scope.property
            delete scope.type
            delete scope.propertySchema
        }

        // Process templates found at the top which should not depend on locale/property/entity
        if (schema.schema.$templates) {
            feedback(FeedbackType.debug, `=== Global templates ===`)
            scope.examples = await globalExamples(outDir, scope)
            for (const templateName of schema.schema.$templates) {
                await processTemplate(templateName, templateDirs, outDir, scope, force, feedback, false, [])
            }
        }

        // Reset locale specific files
        scope.templates.lu = []
        scope.templates.lg = []
        scope.templates.qna = []
    }
    delete scope.locale
}

// Ensure every property has $entities and $templates from $template
async function ensureEntitiesAndTemplates(
    schema: s.Schema,
    templateDirs: string[],
    scope: any,
    feedback: Feedback)
    : Promise<void> {
    for (const property of schema.schemaProperties()) {
        if (!property.schema.$entities || !property.schema.$templates) {
            const template = property.schema.$template
            if (!template) {
                feedback(FeedbackType.error, `${property.path} has no $template`)
            } else {
                try {
                    scope.property = property.path
                    scope.propertySchema = property.schema
                    const rootTemplate = await findTemplate(template, templateDirs)
                    const lgTemplate: lg.Templates | undefined = rootTemplate instanceof lg.Templates ? rootTemplate as lg.Templates : undefined
                    if (!lgTemplate) {
                        feedback(FeedbackType.error, `Cannot find ${rootTemplate} for ${property.path}`)
                    } else {
                        if (!property.schema.$entities) {
                            feedback(FeedbackType.debug, `Expanding template ${lgTemplate.id} for ${property.path} $entities`)
                            property.schema.$entities = lgTemplate.evaluate('entities', scope) as string[]
                            if (!property.schema.$entities) {
                                feedback(FeedbackType.error, `${property.path} has no $entities`)
                            }
                        }
                        if (!property.schema.$templates) {
                            feedback(FeedbackType.debug, `Expanding template ${lgTemplate.id} for ${property.path} $templates`)
                            property.schema.$templates = lgTemplate.evaluate('templates', scope) as string[]
                            if (!property.schema.$templates) {
                                feedback(FeedbackType.error, `${property.path} has no $templates`)
                            }
                        }
                    }
                } catch (e) {
                    feedback(FeedbackType.error, e.message)
                }
            }
        }
    }
}

// Expand strings with ${} expression in them by evaluating and then interpreting as JSON.
function expandSchema(schema: any, scope: any, path: string, inProperties: boolean, missingIsError: boolean, feedback: Feedback): any {
    let newSchema = schema
    const isTopLevel = inProperties && !scope.propertySchema
    if (Array.isArray(schema)) {
        newSchema = []
        let isExpanded = false
        for (const val of schema) {
            const isExpr = typeof val === 'string' && val.startsWith('${')
            const newVal = expandSchema(val, scope, path, false, missingIsError, feedback)
            isExpanded = isExpanded || (isExpr && (typeof newVal !== 'string' || !val.startsWith('${')))
            if (newVal !== null) {
                if (Array.isArray(newVal)) {
                    newSchema = [...newSchema, ...newVal]
                } else {
                    newSchema.push(newVal)
                }
            }
        }
        if (isExpanded && newSchema.length > 0 && !path.includes('.')) {
            // Assume top-level arrays are merged across schemas
            newSchema = Array.from(new Set(newSchema.flat(1)))
            if (typeof newSchema[0] === 'object') {
                // Merge into single object
                let obj = {}
                for (const elt of newSchema) {
                    obj = {...obj, ...elt}
                }
                newSchema = obj
            }
        }
    } else if (typeof schema === 'object') {
        newSchema = {}
        for (const [key, val] of Object.entries(schema)) {
            let newPath = path
            if (inProperties) {
                newPath += newPath === '' ? key : '.' + key
            }
            if (key === '$parameters') {
                newSchema[key] = val
            } else {
                if (isTopLevel) {
                    // Bind property schema to use when expanding
                    scope.propertySchema = val
                }
                const newVal = expandSchema(val, {...scope, property: newPath}, newPath, key === 'properties', missingIsError, feedback)
                newSchema[key] = newVal
                if (isTopLevel) {
                    delete scope.propertySchema
                }
            }
        }
    } else if (typeof schema === 'string' && schema.startsWith('${')) {
        try {
            const value = generatorTemplate.evaluateText(schema, scope)
            if (value && value !== 'null') {
                newSchema = value
            } else {
                if (missingIsError) {
                    feedback(FeedbackType.error, `Could not evaluate ${schema} in schema`)
                }
            }
        } catch (e) {
            if (missingIsError) {
                feedback(FeedbackType.error, e.message)
            }
        }
    }
    return newSchema
}

// Get all files recursively in root
async function allFiles(root: string): Promise<Map<string, string>> {
    const files = new Map<string, string>()
    async function walker(dir: string) {
        if ((await fs.lstat(dir)).isDirectory()) {
            for (const child of await fs.readdir(dir)) {
                await walker(ppath.join(dir, child))
            }
        } else {
            files.set(ppath.basename(dir), dir)
        }
    }
    await walker(root)
    return files
}

// Generate a singleton dialog by pulling in all dialog refs
async function generateSingleton(schema: string, inDir: string, outDir: string, feedback: Feedback) {
    const files = await allFiles(inDir)
    const mainName = `${schema}.dialog`
    const main = await fs.readJSON(files.get(mainName) as string)
    const used = new Set<string>()
    await walkJSON(main, async (elt, obj, key) => {
        if (typeof elt === 'string') {
            const ref = `${elt}.dialog`
            const path = files.get(ref)
            // Keep recognizers as a reference
            if (ref !== mainName && path && key && !ref.endsWith('.lu.dialog')) {
                // Replace reference with inline object
                const newElt = await fs.readJSON(path)
                let id = ppath.basename(path)
                id = id.substring(0, id.indexOf('.dialog'))
                delete newElt.$schema
                delete newElt.$Generator
                newElt.$source = id
                newElt.$Generator = computeJSONHash(newElt)
                setPath(obj, key, newElt)
                used.add(ref)
            }
        }
        return false
    })
    delete main.$Generator
    main.$Generator = computeJSONHash(main)
    for (const [name, path] of files) {
        if (!used.has(name)) {
            const outPath = ppath.join(outDir, ppath.relative(inDir, path))
            feedback(FeedbackType.info, `Generating ${outPath}`)
            if (name === mainName && path) {
                await fs.writeJSON(outPath, main, {spaces: '  '})
            } else {
                await fs.copy(path, outPath)
            }
        }
    }
}

const templatePrefix: string = 'template:'

function resolveDir(dirs: string[]): string[] {
    const expanded: string[] = []
    for (let dir of dirs) {
        if (dir.startsWith(templatePrefix)) {
            // Expand template:<name> relative to built-in templates
            expanded.push(ppath.resolve(ppath.join(__dirname, '../templates', dir.substring(templatePrefix.length))))
        } else {
            dir = ppath.resolve(dir)
            expanded.push(normalize(dir))
        }
    }
    return expanded
}

// Convert to the right kind of slash. 
// ppath.normalize did not do this properly on the mac.
function normalize(path: string): string {
    if (ppath.sep === '/') {
        path = path.replace(/\\/g, ppath.sep)
    } else {
        path = path.replace(/\//g, ppath.sep)
    }
    return ppath.normalize(path)
}

/**
 * Iterate through the locale templates and generate per property/locale files.
 * Each template file will map to <filename>_<property>.<ext>.
 * @param schemaPath Path to JSON Schema to use for generation.
 * @param prefix Prefix to use for generated files.
 * @param outDir Where to put generated files.
 * @param metaSchema Schema to use when generating .dialog files
 * @param allLocales Locales to generate.
 * @param templateDirs Where templates are found.
 * @param force True to force overwriting existing files.
 * @param merge Merge generated results into target directory.
 * @param singleton Merge .dialog into a single .dialog.
 * @param feedback Callback function for progress and errors.
 * @returns True if successful.
 */
export async function generate(
    schemaPath: string,
    prefix?: string,
    outDir?: string,
    metaSchema?: string,
    allLocales?: string[],
    templateDirs?: string[],
    force?: boolean,
    merge?: boolean,
    singleton?: boolean,
    feedback?: Feedback)
    : Promise<boolean> {
    const start = process.hrtime.bigint()

    if (!feedback) {
        feedback = (_info, _message) => true
    }
    let error = false
    const externalFeedback = feedback
    feedback = (info, message) => {
        if (info === FeedbackType.error) {
            error = true
        }
        externalFeedback(info, message)
    }

    if (!prefix) {
        prefix = ppath.basename(schemaPath)
        const lastDot = prefix.lastIndexOf('.')
        if (lastDot >= 0) {
            prefix = prefix.substring(0, lastDot)
        }
    }

    if (!outDir) {
        outDir = ppath.join(prefix + '-resources')
    }

    if (!metaSchema) {
        metaSchema = 'https://raw.githubusercontent.com/microsoft/botbuilder-samples/main/experimental/generation/runbot/RunBot.schema'
    } else if (!metaSchema.startsWith('http')) {
        // Adjust relative to outDir
        metaSchema = ppath.relative(outDir, metaSchema)
    }

    if (!allLocales) {
        allLocales = ['en-us']
    }

    if (!templateDirs) {
        templateDirs = []
    }

    if (force) {
        merge = false
    }

    try {
        if (!await fs.pathExists(outDir)) {
            // If directory is new, no force or merge necessary
            force = false
            merge = false
            await fs.ensureDir(outDir)
        }

        const existingFiles = await fs.readdir(outDir)
        if (existingFiles.length === 0) {
            force = false
            merge = false
        }

        let op = 'Regenerating'
        if (!force) {
            force = false
            if (merge) {
                op = 'Merging'
            } else {
                op = 'Generating'
            }
        }
        feedback(FeedbackType.message, `${op} resources for ${ppath.basename(schemaPath, '.schema')} in ${outDir}`)
        feedback(FeedbackType.message, `Locales: ${JSON.stringify(allLocales)} `)
        feedback(FeedbackType.message, `Templates: ${JSON.stringify(templateDirs)} `)
        feedback(FeedbackType.message, `App.schema: ${metaSchema} `)

        let outPath = outDir
        let outPathSingle = outDir
        if (merge || singleton) {
            // Redirect to temporary path
            outPath = ppath.join(os.tmpdir(), 'tempNew')
            outPathSingle = ppath.join(os.tmpdir(), 'tempNewSingle')
            await fs.emptyDir(outPath)
            await fs.emptyDir(outPathSingle)
        }

        const startDirs = await templateDirectories(templateDirs)

        // Find generator.lg for schema expansion
        for (const dir of startDirs) {
            const loc = ppath.join(dir, '../generator.lg')
            if (await fs.pathExists(loc)) {
                generatorTemplate = lg.Templates.parseFile(loc, undefined, getExpressionEngine())
                break
            }
        }
        if (!generatorTemplate) {
            feedback(FeedbackType.error, 'Templates must include a parent generator.lg')
        }

        const schema = await ps.processSchemas(schemaPath, startDirs, feedback)

        // User templates + used schema template directories
        const schemaDirs = schema.schema.$templateDirs.map(d => normalize(d))
        templateDirs = resolveDir(templateDirs)
        templateDirs = [
            ...templateDirs,
            ...schemaDirs.filter(d => !(templateDirs as string[]).includes(d))
        ]

        // Expand root $template and computed schema
        await ensureEntitiesAndTemplates(schema, templateDirs, {}, feedback)
        schema.schema = expandSchema(schema.schema, {}, '', false, false, feedback)

        // Process templates
        let scope: any = {
            locales: allLocales,
            prefix: (prefix ?? schema.name()).replace('-', '_'),
            schema: schema.schema,
            operations: schema.schema.$operations,
            properties: schema.schema.$public,
            triggerIntent: schema.triggerIntent(),
            appSchema: metaSchema,
            utterances: new Set<string>()
        }

        if (schema.schema.$parameters) {
            scope = {...scope, ...schema.schema.$parameters}
        }

        scope = {...scope, entities: schema.entityToProperties()}
        await processTemplates(schema, templateDirs, allLocales, outPath, scope, force, feedback)

        // Expand all remaining schema expressions
        const expanded = expandSchema(schema.schema, scope, '', false, true, feedback)

        if (!error) {
            // Write final schema
            const body = stringify(expanded, (key: any, val: any) => (key === '$templates' || key === '$requires' || key === '$templateDirs' || key === '$examples' || key === '$template' || key === '$generator') ? undefined : val)
            await generateFile(ppath.join(outPath, `${prefix}.json`), body, force, feedback)

            // Merge together all dialog files
            if (singleton) {
                if (!merge) {
                    feedback(FeedbackType.info, 'Combining into singleton .dialog')
                    await generateSingleton(scope.prefix, outPath, outDir, feedback)
                } else {
                    await generateSingleton(scope.prefix, outPath, outPathSingle, feedback)
                }
            }

            // Merge old and new directories
            if (merge) {
                if (singleton) {
                    await merger.mergeAssets(prefix, outDir, outPathSingle, outDir, allLocales, feedback)
                } else {
                    await merger.mergeAssets(prefix, outDir, outPath, outDir, allLocales, feedback)
                }
            }

            // Copy source schema
            const outSchemaPath = ppath.join(outDir, ppath.basename(schemaPath))
            feedback(FeedbackType.info, `Copying to ${outSchemaPath}`)
            await fs.copyFile(schemaPath, outSchemaPath)
        }
    } catch (e) {
        feedback(FeedbackType.error, e.message)
    }

    const end = process.hrtime.bigint()
    const elapsed = Number(end - start) / 1000000000

    if (error) {
        externalFeedback(FeedbackType.error, '*** Errors prevented generation ***')
    } else {
        externalFeedback(FeedbackType.message, `Successfully generated in ${elapsed} seconds`)
    }

    return !error
}

/**
 * Expand a property definition by resolving $ref including template:, removing allOf and generating $entities if missing.
 * @param property Name of property.
 * @param schema Schema definition.
 * @param templateDirs Optional directories for overriding templates.
 */
export async function expandPropertyDefinition(property: string, schema: any, templateDirs?: string[]): Promise<any> {
    templateDirs = await templateDirectories(templateDirs)
    schema = await ps.expandPropertyDefinition(schema, templateDirs)
    const fullSchema = {
        properties: {}
    }
    fullSchema.properties[property] = schema
    schema = expandSchema(fullSchema, {}, '', false, false, ps.feedbackException).properties[property]
    if (!schema.$entities) {
        const scope = {
            property,
            propertySchema: schema,
            type: ps.typeName(schema)
        }
        const foundTemplate = await findTemplate(scope.type, templateDirs)
        const lgTemplate: lg.Templates | undefined = foundTemplate instanceof lg.Templates ? foundTemplate as lg.Templates : undefined
        if (lgTemplate && lgTemplate.allTemplates.some(f => f.name === 'entities')) {
            const entities = lgTemplate.evaluate('entities', scope) as string[]
            if (entities) {
                schema.$entities = entities
            }
        }
    }
    return schema
}
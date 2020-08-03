#!/usr/bin/env node
/*!
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
export * from './dialogGenerator'
import * as s from './schema'
import * as crypto from 'crypto'
import * as expressions from 'adaptive-expressions'
import * as fs from 'fs-extra'
import * as merger from './mergeAssets'
import * as lg from 'botbuilder-lg'
import * as os from 'os'
import * as ppath from 'path'
import * as ph from './generatePhrases'
import {SubstitutionsEvaluator} from './substitutions'
import * as ps from './processSchemas'

export enum FeedbackType {
    message,
    info,
    warning,
    error,
    debug
}

export type Feedback = (type: FeedbackType, message: string) => void

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
        val = val.replace(/(^|[^\r])\n/g, `$1${os.EOL}`)
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
    let ext = ppath.extname(path)
    if (CommentHashExtensions.includes(ext)) {
        val = val.replace(ReplaceGeneratorPattern, '')
        if (!val.endsWith(os.EOL)) {
            val += os.EOL
        }
        val += `${os.EOL}> Generator: ${computeHash(val)}`
    } else if (JSONHashExtensions.includes(ext)) {
        let json = JSON.parse(val)
        delete json.$Generator
        json.$Generator = computeJSONHash(json)
        val = stringify(json)
    }
    return val
}

// Check to see if the contents of path are unchanged since generated
export async function isUnchanged(path: string): Promise<boolean> {
    let result = false
    let ext = ppath.extname(path)
    let file = await fs.readFile(path, 'utf8')
    if (CommentHashExtensions.includes(ext)) {
        let match = file.match(GeneratorPattern)
        if (match) {
            let oldHash = match[1]
            file = file.replace(GeneratorPattern, '')
            let hash = computeHash(file)
            result = oldHash === hash
        }
    } else if (JSONHashExtensions.includes(ext)) {
        let json = JSON.parse(file)
        let oldHash = json.$Generator
        if (oldHash) {
            delete json.$Generator
            let hash = computeJSONHash(json)
            result = oldHash === hash
        }
    }
    return result
}

// Write file with error checking and hash code generation.
export async function writeFile(path: string, val: string, feedback: Feedback, skipHash?: boolean) {
    try {
        let dir = ppath.dirname(path)
        await fs.ensureDir(dir)
        val = normalizeEOL(val)
        if (!skipHash) {
            val = addHash(path, val)
        }
        await fs.writeFile(path, val)
    } catch (e) {
        let match = /position ([0-9]+)/.exec(e.message)
        if (match) {
            let offset = Number(match[1])
            val = `${val.substring(0, offset)}^^^${val.substring(offset)}`
        }
        feedback(FeedbackType.error, `${e.message}${os.EOL}${val}`)
    }
}

// Return template directories by combining explicit ones with library ones
export async function templateDirectories(templateDirs?: string[]): Promise<string[]> {
    // Fully expand all directories
    templateDirs = resolveDir(templateDirs || [])

    // User templates + cli templates to find schemas
    let startDirs = [...templateDirs]
    let templates = normalize(ppath.join(__dirname, '../templates'))
    for (let dirName of await fs.readdir(templates)) {
        let dir = ppath.join(templates, dirName)
        if ((await fs.lstat(dir)).isDirectory() && !startDirs.includes(dir)) {
            // Add templates subdirectories as templates
            startDirs.push(dir)
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

let generatorTemplate: lg.Templates
function getGeneratorTemplate(): lg.Templates {
    if (!generatorTemplate) {
        generatorTemplate = lg.Templates.parseFile(ppath.join(__dirname, '../templates/', 'generator.lg'), undefined, getExpressionEngine())
    }
    return generatorTemplate
}

// Walk over JSON object, stopping if true from walker.
// Walker gets the current value, the parent object and full path to that object
// and returns false to continue, true to stop going deeper.
async function walkJSON(elt: any, fun: (val: any, obj?: any, path?: string) => Promise<boolean>, obj?: any, path?: any): Promise<void> {
    let done = await fun(elt, obj, path)
    if (!done) {
        if (typeof elt === 'object' || Array.isArray(elt)) {
            for (let key in elt) {
                await walkJSON(elt[key], fun, elt, pathName(path, key))
            }
        }
    }
}

function pathName(path: string | undefined, extension: string): string {
    return path ? `${path}/${extension}` : extension
}

function setPath(obj: any, path: string, value: any) {
    let key = path.substring(path.lastIndexOf('/') + 1)
    obj[key] = value
}

type Plain = {source: string, template: string}
type Template = lg.Templates | Plain | undefined

async function findTemplate(name: string, templateDirs: string[]): Promise<Template> {
    let template: Template
    for (let dir of templateDirs) {
        let loc = templatePath(name, dir)
        if (await fs.pathExists(loc)) {
            // Direct file
            template = {source: loc, template: await fs.readFile(loc, 'utf8')}
            break
        } else {
            // LG file
            loc = templatePath(name + '.lg', dir)
            if (await fs.pathExists(loc)) {
                template = lg.Templates.parseFile(loc, undefined, getExpressionEngine())
                break
            }
        }
    }
    return template
}

// Add prefix to [] imports in constant .lg files
const RefPattern = /^[ \t]*\[[^\]\n\r]*\][ \t]*$/gm
function addPrefixToImports(template: string, scope: any): string {
    return template.replace(RefPattern, (match: string) => {
        let ref = match.substring(match.indexOf('[') + 1, match.indexOf(']'))
        return `[${scope.prefix}-${ref}](${scope.prefix}-${ref})${os.EOL}`
    })
}

function addPrefix(prefix: string, name: string): string {
    return `${prefix}-${name}`
}

// Add entry to the .lg generation context and return it.  
// This also ensures the file does not exist already.
type FileRef = {name: string, fallbackName: string, fullName: string, relative: string}
function addEntry(fullPath: string, outDir: string, tracker: any): FileRef | undefined {
    let ref: FileRef | undefined
    let basename = ppath.basename(fullPath, '.dialog')
    let ext = ppath.extname(fullPath).substring(1)
    let arr: FileRef[] = tracker[ext]
    if (!arr.find(ref => ref.name === basename)) {
        ref = {
            name: basename,
            fallbackName: basename.replace(/\.[^.]+\.lg/, '.lg'),
            fullName: ppath.basename(fullPath),
            relative: ppath.relative(outDir, fullPath)
        }
    }
    return ref
}

function existingRef(name: string, tracker: any): FileRef | undefined {
    let ext = ppath.extname(name).substring(1)
    let arr: FileRef[] = tracker[ext]
    if (!arr) {
        arr = []
        tracker[ext] = arr
    }
    return arr.find(ref => ref.fullName === name)
}

async function processTemplate(
    templateName: string,
    templateDirs: string[],
    outDir: string,
    scope: any,
    force: boolean,
    feedback: Feedback,
    ignorable: boolean): Promise<string> {
    let outPath = ''
    let oldDir = process.cwd()
    try {
        let ref = existingRef(templateName, scope.templates)
        if (ref) {
            // Simple file already existed
            feedback(FeedbackType.debug, `Reusing ${templateName}`)
            outPath = ppath.join(outDir, ref.relative)
        } else {
            let foundTemplate = await findTemplate(templateName, templateDirs)
            if (foundTemplate === undefined && templateName.includes('Entity')) {
                // If we can't find an entity, try for a generic definition
                feedback(FeedbackType.debug, `Generic of ${templateName}`)
                templateName = templateName.replace(/.*Entity/, 'generic')
                foundTemplate = await findTemplate(templateName, templateDirs)
            }
            if (foundTemplate !== undefined) {
                let lgTemplate: lg.Templates | undefined = foundTemplate instanceof lg.Templates ? foundTemplate as lg.Templates : undefined
                let plainTemplate: Plain | undefined = !lgTemplate ? foundTemplate as Plain : undefined
                // Ignore templates that are defined, but are empty
                if (plainTemplate?.source || lgTemplate?.allTemplates.some(f => f.name === 'template')) {
                    // Constant file or .lg template so output
                    feedback(FeedbackType.debug, `Using template ${plainTemplate ? plainTemplate.source : lgTemplate?.id}`)

                    let filename = addPrefix(scope.prefix, templateName)
                    if (lgTemplate?.allTemplates.some(f => f.name === 'filename')) {
                        try {
                            filename = lgTemplate.evaluate('filename', scope) as string
                        } catch (e) {
                            throw new Error(`${templateName}: ${e.message}`)
                        }
                    } else if (filename.includes(scope.locale)) {
                        // Move constant files into locale specific directories
                        let prop = templateName.startsWith('library') ? 'library' : (filename.endsWith('.qna') ? 'QnA' : scope.property)
                        filename = `${scope.locale}/${prop}/${filename}`
                    } else if (filename.includes('library-')) {
                        // Put library stuff in its own folder by default
                        filename = `library/${filename}`
                    }

                    // Add prefix to constant imports
                    if (plainTemplate) {
                        plainTemplate.template = addPrefixToImports(plainTemplate.template, scope)
                    }

                    outPath = ppath.join(outDir, filename)
                    let ref = addEntry(outPath, outDir, scope.templates)
                    if (ref) {
                        // This is a new file
                        if (force || !await fs.pathExists(outPath)) {
                            feedback(FeedbackType.info, `Generating ${outPath}`)
                            let result = plainTemplate?.template
                            if (lgTemplate) {
                                process.chdir(ppath.dirname(lgTemplate.allTemplates[0].sourceRange.source))
                                result = lgTemplate.evaluate('template', scope) as string
                                process.chdir(oldDir)
                                if (Array.isArray(result)) {
                                    result = result.join(os.EOL)
                                }
                            }

                            // See if generated file has been overridden in templates
                            let existing = await findTemplate(filename, templateDirs) as Plain
                            if (existing?.source) {
                                feedback(FeedbackType.info, `  Overridden by ${existing.source}`)
                                result = existing.template
                            }

                            let resultString = result as string
                            if (resultString.includes('**MISSING**')) {
                                feedback(FeedbackType.error, `${outPath} has **MISSING** data`)
                            } else {
                                let match = resultString.match(/\*\*([^0-9\s]+)[0-9]+\*\*/)
                                if (match) {
                                    feedback(FeedbackType.warning, `Replace **${match[1]}<N>** with values in ${outPath}`)
                                }
                            }
                            await writeFile(outPath, resultString, feedback)
                            scope.templates[ppath.extname(outPath).substring(1)].push(ref)

                        } else {
                            feedback(FeedbackType.warning, `Skipping already existing ${outPath}`)
                        }
                    }
                } else if (lgTemplate) {
                    if (lgTemplate.allTemplates.some(f => f.name === 'entities') && !scope.schema.properties[scope.property].$entities) {
                        let entities = lgTemplate.evaluate('entities', scope) as string[]
                        if (entities) {
                            scope.schema.properties[scope.property].$entities = entities
                        }
                    }
                    if (lgTemplate.allTemplates.some(f => f.name === 'templates')) {
                        feedback(FeedbackType.debug, `Expanding template ${lgTemplate.id}`)
                        let generated = lgTemplate.evaluate('templates', scope)
                        if (!Array.isArray(generated)) {
                            generated = [generated]
                        }
                        for (let generate of generated as any as string[]) {
                            feedback(FeedbackType.debug, `  ${generate}`)
                        }
                        for (let generate of generated as any as string[]) {
                            await processTemplate(generate, templateDirs, outDir, scope, force, feedback, false)
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

async function processTemplates(
    schema: s.Schema,
    templateDirs: string[],
    locales: string[],
    outDir: string,
    scope: any,
    force: boolean,
    feedback: Feedback): Promise<void> {
    scope.templates = {}
    for (let locale of locales) {
        scope.locale = locale
        for (let property of schema.schemaProperties()) {
            scope.property = property.path
            scope.type = property.typeName()
            let templates = property.schema.$templates
            if (!templates) {
                templates = [scope.type]
            }
            for (let template of templates) {
                await processTemplate(template, templateDirs, outDir, scope, force, feedback, false)
            }
            let entities = property.schema.$entities
            if (!entities) {
                feedback(FeedbackType.error, `${property.path} does not have $entities defined in schema or template.`)
            } else if (!property.schema.$templates) {
                for (let entity of entities) {
                    let [entityName, role] = entity.split(':')
                    scope.entity = entityName
                    scope.role = role
                    if (entityName === `${scope.property}Entity`) {
                        entityName = `${scope.type}`
                    }

                    // Look for examples in global $examples
                    if (schema.schema.$examples) {
                        scope.examples = schema.schema.$examples[entityName]
                    }

                    // Pick up examples from property schema
                    if (!scope.examples && property.schema.examples && entities.length === 1) {
                        scope.examples = property.schema.examples
                    }

                    // If neither specify, then it is up to templates

                    await processTemplate(`${entityName}Entity-${scope.type}`, templateDirs, outDir, scope, force, feedback, false)
                }
                delete scope.entity
                delete scope.role
                delete scope.examples
            }
        }
        delete scope.property
        delete scope.type

        // Process templates found at the top
        if (schema.schema.$templates) {
            scope.entities = schema.entityTypes()
            for (let templateName of schema.schema.$templates) {
                await processTemplate(templateName, templateDirs, outDir, scope, force, feedback, false)
            }
        }
    }
    delete scope.locale
}

// Expand strings with ${} expression in them by evaluating and then interpreting as JSON.
function expandSchema(schema: any, scope: any, path: string, inProperties: boolean, missingIsError: boolean, feedback: Feedback): any {
    let newSchema = schema
    if (Array.isArray(schema)) {
        newSchema = []
        let isExpanded = false
        for (let val of schema) {
            let isExpr = typeof val === 'string' && val.startsWith('${')
            let newVal = expandSchema(val, scope, path, false, missingIsError, feedback)
            isExpanded = isExpanded || (isExpr && (typeof newVal !== 'string' || !val.startsWith('${')))
            newSchema.push(newVal)
        }
        if (isExpanded && newSchema.length > 0 && !path.includes('.')) {
            // Assume top-level arrays are merged across schemas
            newSchema = Array.from(new Set(newSchema.flat(1)))
            if (typeof newSchema[0] === 'object') {
                // Merge into single object
                let obj = {}
                for (let elt of newSchema) {
                    obj = {...obj, ...elt}
                }
                newSchema = obj
            }
        }
    } else if (typeof schema === 'object') {
        newSchema = {}
        for (let [key, val] of Object.entries(schema)) {
            let newPath = path
            if (inProperties) {
                newPath += newPath === '' ? key : '.' + key
            }
            if (key === '$parameters') {
                newSchema[key] = val
            } else {
                let newVal = expandSchema(val, {...scope, property: newPath}, newPath, key === 'properties', missingIsError, feedback)
                newSchema[key] = newVal
            }
        }
    } else if (typeof schema === 'string' && schema.startsWith('${')) {
        try {
            let value = getGeneratorTemplate().evaluateText(schema, scope)
            if (value && value !== 'null') {
                newSchema = value
            } else {
                if (missingIsError) {
                    feedback(FeedbackType.error, `Could not evaluate ${schema}`)
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
    let files = new Map<string, string>()
    async function walker(dir: string) {
        if ((await fs.lstat(dir)).isDirectory()) {
            for (let child of await fs.readdir(dir)) {
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
// NOTE: This does not pull in the recognizers in part because they are only generated when
// publishing.
async function generateSingleton(schema: string, inDir: string, outDir: string) {
    let files = await allFiles(inDir)
    let mainName = `${schema}.dialog`
    let main = await fs.readJSON(files.get(mainName) as string)
    let used = new Set<string>()
    await walkJSON(main, async (elt, obj, key) => {
        if (typeof elt === 'string') {
            let ref = `${elt}.dialog`
            let path = files.get(ref)
            if (path && key) {
                // Replace reference with inline object
                let newElt = await fs.readJSON(path)
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
    for (let [name, path] of files) {
        if (!used.has(name)) {
            let outPath = ppath.join(outDir, ppath.relative(inDir, path))
            if (name === mainName && path) {
                await fs.writeJSON(outPath, main, {spaces: '  '})
            } else {
                await fs.copy(path, outPath)
            }
        }
    }
}

function resolveDir(dirs: string[]): string[] {
    let expanded: string[] = []
    for (let dir of dirs) {
        dir = ppath.resolve(dir)
        expanded.push(normalize(dir))
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
    : Promise<void> {
    if (!feedback) {
        feedback = (_info, _message) => true
    }

    if (!prefix) {
        prefix = ppath.basename(schemaPath, '.schema')
    }

    if (!outDir) {
        outDir = ppath.join(prefix + '-resources')
    }

    if (!metaSchema) {
        metaSchema = 'https://raw.githubusercontent.com/microsoft/botbuilder-samples/master/experimental/generation/runbot/runbot.schema'
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

        let op = 'Regenerating'
        if (!force) {
            force = false
            if (merge) {
                op = 'Merging'
            } else {
                merge = false
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

        let startDirs = await templateDirectories(templateDirs)
        let schema = await ps.processSchemas(schemaPath, startDirs, feedback)
        schema.schema = expandSchema(schema.schema, {}, '', false, false, feedback)

        // User templates + used schema template directories
        let schemaDirs = schema.schema.$templateDirs.map(d => normalize(d))
        templateDirs = resolveDir(templateDirs)
        templateDirs = [
            ...templateDirs,
            ...schemaDirs.filter(d => !(templateDirs as string[]).includes(d))
        ]

        // Process templates
        let scope: any = {
            locales: allLocales,
            prefix: prefix || schema.name(),
            schema: schema.schema,
            properties: schema.schema.$public,
            triggerIntent: schema.triggerIntent(),
            appSchema: metaSchema
        }

        if (schema.schema.$parameters) {
            scope = {...scope, ...schema.schema.$parameters}
        }

        await processTemplates(schema, templateDirs, allLocales, outPath, scope, force, feedback)

        // Expand schema expressions
        let expanded = expandSchema(schema.schema, scope, '', false, true, feedback)

        // Write final schema
        let body = stringify(expanded, (key: any, val: any) => (key === '$templates' || key === '$requires' || key === '$templateDirs' || key === '$examples') ? undefined : val)
        await generateFile(ppath.join(outPath, `${prefix}.json`), body, force, feedback)

        // Merge together all dialog files
        if (singleton) {
            if (!merge) {
                feedback(FeedbackType.info, 'Combining into singleton .dialog')
                await generateSingleton(scope.prefix, outPath, outDir)
            } else {
                await generateSingleton(scope.prefix, outPath, outPathSingle)
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
    } catch (e) {
        feedback(FeedbackType.error, e.message)
    }
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
    let fullSchema = { 
        properties: {}
    }
    fullSchema.properties[property] = schema
    schema = expandSchema(fullSchema, {}, '', false, false, ps.feedbackException).properties[property]
    if (!schema.$entities) {
        let scope = {
            property,
            type: ps.typeName(schema)
        }
        let foundTemplate = await findTemplate(scope.type, templateDirs)
        let lgTemplate: lg.Templates | undefined = foundTemplate instanceof lg.Templates ? foundTemplate as lg.Templates : undefined
        if (lgTemplate && lgTemplate.allTemplates.some(f => f.name === 'entities')) {
            let entities = lgTemplate.evaluate('entities', scope) as string[]
            if (entities) {
                schema.$entities = entities
            }
        }
    }
    return schema
}
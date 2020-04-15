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
import { SubstitutionsEvaluator } from './substitutions'
import { processSchemas } from './processSchemas'

export enum FeedbackType {
    message,
    info,
    warning,
    error
}

export type Feedback = (type: FeedbackType, message: string) => void

// This is the Windows EOL
export const EOL = '\r\n'

function templatePath(name: string, dir: string): string {
    return ppath.join(dir, name)
}

function computeHash(val: string): string {
    return crypto.createHash('md5').update(val).digest('hex')
}

function stringify(val: any): string {
    if (typeof val === 'object') {
        val = JSON.stringify(val, null, 4)
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
        // TODO: Remove this test
        if (val.match(ReplaceGeneratorPattern)) {
            val = val.replace(ReplaceGeneratorPattern, '')
        }
        if (!val.endsWith(EOL)) {
            val += EOL
        }
        val += `${EOL}> Generator: ${computeHash(val)}`
    } else if (JSONHashExtensions.includes(ext)) {
        let json = JSON.parse(val)
        delete json.$Generator
        json.$Generator = computeJSONHash(json)
        val = stringify(json)
    }
    return val
}

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

export async function writeFile(path: string, val: string, feedback: Feedback, skipHash?: boolean) {
    try {
        let dir = ppath.dirname(path)
        await fs.ensureDir(dir)
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
        feedback(FeedbackType.error, `${e.message}${EOL}${val}`)
    }
}

async function generateFile(path: string, val: any, force: boolean, feedback: Feedback) {
    if (force || !await fs.pathExists(path)) {
        feedback(FeedbackType.info, `Generating ${path}`)
        await writeFile(path, val, feedback)
    } else {
        feedback(FeedbackType.warning, `Skipping already existing ${path}`)
    }
}

const expressionEngine = new expressions.ExpressionParser((func: string): any => {
    switch (func) {
        case 'phrase': return ph.PhraseEvaluator
        case 'phrases': return ph.PhrasesEvaluator
        case 'substitutions': return SubstitutionsEvaluator
        default:
            return expressions.ExpressionFunctions.standardFunctions.get(func)
    }
})

type Template = lg.Templates | string | undefined

async function findTemplate(name: string, templateDirs: string[]): Promise<Template> {
    let template: Template
    for (let dir of templateDirs) {
        let loc = templatePath(name, dir)
        if (await fs.pathExists(loc)) {
            // Direct file
            template = await fs.readFile(loc, 'utf8')
        } else {
            // LG file
            loc = templatePath(name + '.lg', dir)
            if (await fs.pathExists(loc)) {
                template = lg.Templates.parseFile(loc, undefined, expressionEngine)
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
        return `[${scope.prefix}-${ref}](${scope.prefix}-${ref})${EOL}`
    })
}

function addPrefix(prefix: string, name: string): string {
    return `${prefix}-${name}`
}

// Add entry to the .lg generation context and return it.  
// This also ensures the file does not exist already.
type FileRef = { name: string, fallbackName: string, fullName: string, relative: string }
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
            outPath = ppath.join(outDir, ref.relative)
        } else {
            let template = await findTemplate(templateName, templateDirs)
            if (template !== undefined) {
                // Ignore templates that are defined, but are empty
                if (template) {
                    if (typeof template !== 'object' || template.allTemplates.some(f => f.name === 'template')) {
                        // Constant file or .lg template so output
                        let filename = addPrefix(scope.prefix, templateName)
                        if (typeof template === 'object' && template.allTemplates.some(f => f.name === 'filename')) {
                            try {
                                filename = template.evaluate('filename', scope) as string
                            } catch (e) {
                                throw new Error(`${templateName}: ${e.message}`)
                            }
                        } else if (filename.includes(scope.locale)) {
                            // Move constant files into locale specific directories
                            filename = `${scope.locale}/${filename}`
                        }

                        // Add prefix to constant imports
                        if (typeof template !== 'object') {
                            template = addPrefixToImports(template, scope)
                        }

                        outPath = ppath.join(outDir, filename)
                        let ref = addEntry(outPath, outDir, scope.templates)
                        if (ref) {
                            // This is a new file
                            if (force || !await fs.pathExists(outPath)) {
                                feedback(FeedbackType.info, `Generating ${outPath}`)
                                let result = template
                                if (typeof template === 'object') {
                                    process.chdir(ppath.dirname(template.allTemplates[0].source))
                                    result = template.evaluate('template', scope) as string
                                    if (Array.isArray(result)) {
                                        result = result.join(EOL)
                                    }
                                }

                                // See if generated file has been overridden in templates
                                let existing = await findTemplate(filename, templateDirs)
                                if (existing) {
                                    result = existing
                                }

                                await writeFile(outPath, result as string, feedback)
                                scope.templates[ppath.extname(outPath).substring(1)].push(ref)

                            } else {
                                feedback(FeedbackType.warning, `Skipping already existing ${outPath}`)
                            }
                        }
                    }

                    if (typeof template === 'object') {
                        if (template.allTemplates.some(f => f.name === 'entities') && !scope.schema.properties[scope.property].$entities) {
                            let entities = template.evaluate('entities', scope) as string[]
                            if (entities) {
                                scope.schema.properties[scope.property].$entities = entities
                            }
                        }
                        if (template.allTemplates.some(f => f.name === 'templates')) {
                            let generated = template.evaluate('templates', scope)
                            if (!Array.isArray(generated)) {
                                generated = [generated]
                            }
                            for (let generate of generated as any as string[]) {
                                await processTemplate(generate, templateDirs, outDir, scope, force, feedback, false)
                            }
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
                    await processTemplate(`${entityName}Entity-${scope.type}`, templateDirs, outDir, scope, force, feedback, false)
                }
            }
        }

        // Process templates found at the top
        if (schema.schema.$templates) {
            scope.entities = schema.entityTypes()
            for (let templateName of schema.schema.$templates) {
                await processTemplate(templateName, templateDirs, outDir, scope, force, feedback, false)
            }
        }
    }
}

// Expand strings with ${} expression in them by evaluating and then interpreting as JSON.
function expandSchema(schema: any, scope: any, path: string, inProperties: boolean, missingIsError: boolean, feedback: Feedback): any {
    let newSchema = schema
    if (Array.isArray(schema)) {
        newSchema = []
        for (let val of schema) {
            let newVal = expandSchema(val, scope, path, false, missingIsError, feedback)
            newSchema.push(newVal)
        }
    } else if (typeof schema === 'object') {
        newSchema = {}
        for (let [key, val] of Object.entries(schema)) {
            let newPath = path
            if (inProperties) {
                newPath += newPath === '' ? key : '.' + key
            }
            let newVal = expandSchema(val, { ...scope, property: newPath }, newPath, key === 'properties', missingIsError, feedback)
            newSchema[key] = newVal
        }
    } else if (typeof schema === 'string' && schema.startsWith('${')) {
        let expr = schema.substring(2, schema.length - 1)
        try {
            let { value, error } = expressionEngine.parse(expr).tryEvaluate(scope)
            if (!error && value) {
                newSchema = value
            } else {
                if (missingIsError) {
                    feedback(FeedbackType.error, `${expr}: ${error}`)
                }
            }
        } catch (e) {
            feedback(FeedbackType.error, `${expr}: ${e.message}`)
        }
    }
    return newSchema
}

function expandStandard(dirs: string[]): string[] {
    let expanded: string[] = []
    for (let dir of dirs) {
        if (dir === 'standard') {
            dir = ppath.join(__dirname, '../../templates')
        } else {
            dir = ppath.resolve(dir)
        }
        expanded.push(dir)
    }
    return expanded
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
        metaSchema = 'https://raw.githubusercontent.com/microsoft/botbuilder-dotnet/master/schemas/sdk.schema'
    } else if (!metaSchema.startsWith('http')) {
        // Adjust relative to outDir
        metaSchema = ppath.relative(outDir, metaSchema)
    }

    if (!allLocales) {
        allLocales = ['en-us']
    }

    if (!templateDirs) {
        templateDirs = ['standard']
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
        if (merge) {
            // Redirect to temporary path
            outPath = ppath.join(os.tmpdir(), 'tempNew')
            await fs.emptyDir(outPath)
        }

        templateDirs = expandStandard(templateDirs)

        let schema = await processSchemas(schemaPath, templateDirs, feedback)
        schema.schema = expandSchema(schema.schema, {}, '', false, false, feedback)

        // Process templates
        let scope: any = {
            locales: allLocales,
            prefix: prefix || schema.name(),
            schema: schema.schema,
            properties: schema.schema.$public,
            triggerIntent: schema.triggerIntent(),
            appSchema: metaSchema
        }
        await processTemplates(schema, templateDirs, allLocales, outPath, scope, force, feedback)

        // Expand schema expressions
        let expanded = expandSchema(schema.schema, scope, '', false, true, feedback)

        // Write final schema
        let body = JSON.stringify(expanded, (key, val) => (key === '$templates' || key === '$requires') ? undefined : val, 4)
        await generateFile(ppath.join(outPath, `${prefix}.schema.dialog`), body, force, feedback)

        if (merge) {
            await merger.mergeAssets(prefix, outDir, outPath, outDir, allLocales, feedback)
        }
    } catch (e) {
        feedback(FeedbackType.error, e.message)
    }
}

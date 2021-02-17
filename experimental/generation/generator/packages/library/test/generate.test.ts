/*!
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
// tslint:disable:no-console
// tslint:disable:no-object-literal-type-assertion

import * as fs from 'fs-extra'
import 'mocha'
import * as os from 'os'
import * as ppath from 'path'
import * as ft from '../src/schema'
import * as gen from '../src/dialogGenerator'
import * as glob from 'globby'
import { generateTest } from '../src/testGenerator'
import * as ps from '../src/processSchemas'
import * as assert from 'assert'
import { Templates, DiagnosticSeverity } from 'botbuilder-lg'
import * as luFile from '@microsoft/bf-lu/lib/utils/filehelper'
import * as LuisBuilder from '@microsoft/bf-lu/lib/parser/luis/luisCollate'
import { ComponentRegistration } from 'botbuilder-core'
import { AdaptiveComponentRegistration } from 'botbuilder-dialogs-adaptive'
import { ResourceExplorer } from 'botbuilder-dialogs-declarative'
import { LuisComponentRegistration, QnAMakerComponentRegistration } from 'botbuilder-ai'

// Output temp directory
let tempDir = ppath.join(os.tmpdir(), 'generate.out')

function feedback(type: gen.FeedbackType, msg: string) {
    if (type !== gen.FeedbackType.debug) {
        console.log(`${type}: ${msg}`)
    }
}

// NOTE: If you update dialog:merge functionality you need to execute the makeOracles.cmd to update them
async function compareToOracle(name: string, oraclePath?: string): Promise<object> {
    let generatedPath = ppath.join(tempDir, name)
    let generated = await fs.readJSON(generatedPath)
    oraclePath = oraclePath ? ppath.join(tempDir, oraclePath) : ppath.join('test/oracles', name)
    let oracle = await fs.readJSON(oraclePath)
    let oracles = JSON.stringify(oracle)
    let generateds = JSON.stringify(generated)
    if (oracles !== generateds) {
        console.log(`Oracle   : ${oracles.length}`)
        console.log(`Generated: ${generateds.length}`)
        let max = oracles.length
        if (max > generateds.length) {
            max = generateds.length
        }
        let idx: number
        for (idx = 0; idx < max; ++idx) {
            if (oracles[idx] !== generateds[idx]) {
                break;
            }
        }
        let start = idx - 40
        if (start < 0) {
            start = 0
        }
        let end = idx + 40
        if (end > max) {
            end = max
        }
        console.log(`Oracle   : ${oracles.substring(start, end)}`)
        console.log(`Generated: ${generateds.substring(start, end)}`)
        assert(false,
            `${ppath.resolve(generatedPath)} does not match oracle ${ppath.resolve(oraclePath)}`)
    }
    return generated
}

async function checkDirectory(path: string, files: number, directories: number): Promise<void> {
    let dirList: string[] = []
    let fileList: string[] = []
    for (const child of await fs.readdir(path)) {
        if ((await fs.stat(ppath.join(path, child))).isDirectory()) {
            dirList.push(child)
        } else {
            fileList.push(child)
        }
    }
    if (fileList.length != files) {
        assert.fail(`${path} has ${fileList.length} files != ${files}: ${os.EOL}${fileList.join(os.EOL)}`)
    }
    if (dirList.length != directories) {
        assert.fail(`${path} has ${dirList.length} directories != ${directories}: ${os.EOL}${fileList.join(os.EOL)}`)
    }
}

async function checkPattern(pattern: string, files: number): Promise<void> {
    const matches = await glob(pattern.replace(/\\/g, '/'))
    if (matches.length != files) {
        assert.fail(`${pattern} has ${matches.length} files != ${files}:${os.EOL}${matches.join(os.EOL)}`)
    }
}

async function includes(path: string, content: string): Promise<void> {
    const file = await fs.readFile(path, 'utf-8')
    assert(file.includes(content), `${path} does not contain ${content}`)
}

describe('dialog:generate library', async () => {
    let output = tempDir
    let schemaPath = 'test/forms/sandwich.form'
    let unitTestSchemaPath = 'test/forms/unittest_'
    let badSchema = 'test/forms/bad-schema.form'
    let notObject = 'test/forms/not-object.form'
    let override = 'test/templates/override'
    let unittestSchemaNames = [
        'age_with_units',
        'age',
        'array_enum',
        'array_personName',
        'boolean',
        'date-time',
        'date',
        'datetime',
        'dimension_with_units',
        'dimension',
        'email',
        'enum',
        'geography',
        'integer_with_limits',
        'integer',
        'iri',
        'keyPhrase_with_pattern',
        'keyPhrase',
        'money_with_units',
        'money',
        'number_with_limits',
        'number',
        'ordinal',
        'percentage_with_limits',
        'percentage',
        'personName_with_pattern',
        'personName',
        'phonenumber',
        'string',
        'temperature_with_units',
        'temperature',
        'time',
        'uri'
    ]

    beforeEach(async () => {
        await fs.remove(output)
    })

    it('Transcript test', async () => {
        try {
            console.log('\n\nTranscript test')
            assert.ok(await generateTest('test/transcripts/sandwich.transcript', 'sandwich', output, false), 'Could not generate test script')
            await compareToOracle('sandwich.test.dialog')
        } catch (e) {
            assert.fail(e.message)
        }
    })

    it('Transcript test click button on adaptive card', async () => {
        try {
            console.log('\n\nTranscript test')
            assert.ok(await generateTest('test/transcripts/addItemWithButton.transcript', 'msmeeting-actions', output, false), 'Could not generate test script')
            await compareToOracle('addItemWithButton.test.dialog')
        } catch (e) {
            assert.fail(e.message)
        }
    })

    it('Hash text', async () => {
        let lu = `> LU File${os.EOL}# Intent${os.EOL}- This is an .lu file`
        let lufile = ppath.join(os.tmpdir(), 'test.lu')

        await gen.writeFile(lufile, lu, feedback)
        assert(await gen.isUnchanged(lufile))

        lu = await fs.readFile(lufile, 'utf-8')
        lu += `${os.EOL}- another line`
        await fs.writeFile(lufile, lu)
        assert(!await gen.isUnchanged(lufile))

        await gen.writeFile(lufile, lu, feedback, true)
        assert(!await gen.isUnchanged(lufile))

        await gen.writeFile(lufile, lu, feedback)
        assert(await gen.isUnchanged(lufile))
        lu = await fs.readFile(lufile, 'utf-8')
        assert((lu.match(/Generator:/g) || []).length === 1)
    })

    it('Hash JSON', async () => {
        let dialog = { $comment: 'this is a .dialog file' }
        let dialogFile = ppath.join(os.tmpdir(), 'test.dialog')

        await gen.writeFile(dialogFile, JSON.stringify(dialog), feedback)
        assert(await gen.isUnchanged(dialogFile))

        // Test json hashing
        dialog = JSON.parse(await fs.readFile(dialogFile, 'utf-8'))
        dialog['foo'] = 3
        await fs.writeFile(dialogFile, JSON.stringify(dialog))
        assert(!await gen.isUnchanged(dialogFile))

        await gen.writeFile(dialogFile, JSON.stringify(dialog), feedback)
        assert(await gen.isUnchanged(dialogFile))
    })

    it('Generation with override', async () => {
        try {
            console.log('\n\nGeneration with override')
            await gen.generate(schemaPath, undefined, output, undefined, ['en-us'], [override, 'template:standard'], false, false, undefined, feedback)
            let lg = await fs.readFile(ppath.join(output, 'language-generation/en-us/Bread', 'sandwich-Bread.en-us.lg'))
            assert.ok(lg.toString().includes('What kind of bread?'), 'Did not override locale generated file')
            let dialog = await fs.readFile(ppath.join(output, 'dialogs/Bread/sandwich-Bread-missing.dialog'))
            assert.ok(!dialog.toString().includes('priority'), 'Did not override top-level file')
        } catch (e) {
            assert.fail(e.message)
        }
    })

    it('Singleton', async () => {
        try {
            console.log('\n\nSingleton Generation')
            await gen.generate(schemaPath, undefined, output, undefined, ['en-us'], undefined, false, false, true, feedback)
            assert.ok(!await fs.pathExists(ppath.join(output, 'dialogs/Bread')), 'Generated non-singleton directories')
            assert.ok(await fs.pathExists(ppath.join(output, 'sandwich.dialog')), 'Did not generate root dialog')
        } catch (e) {
            assert.fail(e.message)
        }
    })

    it('Non singleton', async () => {
        try {
            console.log('\n\nNon singleton Generation')
            await gen.generate(schemaPath, undefined, output, undefined, ['en-us'], undefined, false, false, false, feedback)
            await checkDirectory(output, 8, 4)
            await checkDirectory(ppath.join(output, 'dialogs'), 0, 10)
            await checkDirectory(ppath.join(output, 'recognizers'), 2, 0)
            await checkDirectory(ppath.join(output, 'language-generation'), 0, 1)
            await checkDirectory(ppath.join(output, 'language-understanding'), 0, 1)
            await checkDirectory(ppath.join(output, 'language-generation', 'en-us'), 1, 10)
            await checkDirectory(ppath.join(output, 'language-understanding', 'en-us'), 1, 10)
            await checkPattern(ppath.join(output, '**'), 136)
        } catch (e) {
            assert.fail(e.message)
        }
    })

    for (let i = 0; i < unittestSchemaNames.length; i++) {
        const name = unittestSchemaNames[i]
        const prefix = name.replace('-', '_').replace(' ', '_')
        const description = `Unit test ${name}`
        it(description, async () => {
            const testOutput = ppath.join(os.tmpdir(), 'unitTests', name)
            console.log(`\n\n${description}`)
            await fs.remove(testOutput)
            const success = await gen.generate(`${unitTestSchemaPath}${name}.form`, undefined, testOutput, undefined, ['en-us'], undefined, false, false, true, feedback)
            if (success) {
                try {
                    console.log(`LG Testing schema ${name}`)
                    const templates = Templates.parseFile(`${testOutput}/language-generation/en-us/unittest_${prefix}.en-us.lg`)
                    const allDiagnostics = templates.allDiagnostics
                    if (allDiagnostics) {
                        let errors = allDiagnostics.filter((u): boolean => u.severity === DiagnosticSeverity.Error)
                        if (errors && errors.length > 0) {
                            let errorList: string[] = []
                            for (let j = 0; j < allDiagnostics.length; j++) {
                                const error = allDiagnostics[j]
                                errorList.push(`${error.message}: ${error.source} ${error.range}`)
                            }
                            assert.fail(errorList.join('\n'))
                        }
                    }
                } catch (e) {
                    assert.fail(e.message)
                }

                try {
                    console.log(`LU Testing schema ${name}`)
                    let result: any
                    const luFiles = await luFile.getLuObjects(undefined, `${testOutput}/language-understanding/en-us/unittest_${prefix}.en-us.lu`, true, '.lu')
                    result = await LuisBuilder.build(luFiles, true, 'en-us', undefined)
                    result.validate()
                } catch (e) {
                    assert.fail(`${e.source}: ${e.message}`)
                }

                try {
                    console.log(`Dialog Testing schema ${name}`)
                    ComponentRegistration.add(new AdaptiveComponentRegistration())
                    ComponentRegistration.add(new LuisComponentRegistration())
                    ComponentRegistration.add(new QnAMakerComponentRegistration())
                    const resourceExplorer = new ResourceExplorer()
                    resourceExplorer.addFolder(`${testOutput}`, true, false)
                    resourceExplorer.loadType(`unittest_${prefix}.dialog`)
                } catch (e) {
                    assert.fail(e.message)
                }
            } else {
                assert.fail('Did not generate')
            }
        })
    }

    it('Not object type', async () => {
        try {
            await ft.Schema.readSchema(notObject)
            assert.fail('Did not detect bad schema')
        } catch (e) {
            assert(e.message.includes('must be of type object'), 'Missing type object')
        }
    })

    it('Illegal schema', async () => {
        try {
            await ft.Schema.readSchema(badSchema)
            assert.fail('Did not detect bad schema')
        } catch (e) {
            assert(e.message.includes('is not a valid JSON Schema'), 'Missing valid JSON schema')
        }
    })

    it('Schema discovery', async () => {
        try {
            const schemas = await ps.schemas()
            assert.strictEqual(Object.keys(schemas).length, 25, `Expected 25 schemas and found ${Object.keys(schemas).length}`)
            let global = 0
            let property = 0
            for (let [_, schema] of Object.entries(schemas)) {
                if (ps.isGlobalSchema(schema)) {
                    ++global
                } else {
                    ++property
                }
            }
            assert.strictEqual(global, 3, `Expected 3 global schemas and found ${global}`)
            assert.strictEqual(property, 22, `Expected 22 property schemas and found ${property}`)
        } catch (e) {
            assert.fail(e.message)
        }
    })

    it('Expand simple property definition', async () => {
        try {
            let schema = {
                type: 'number'
            }
            let expansion = await gen.expandPropertyDefinition('simple', schema)
            assert(expansion.$entities, 'Did not generate $entities')
            assert.strictEqual(expansion.$entities.length, 1, 'Wrong number of entities')
        } catch (e) {
            assert.fail(e.message)
        }
    })

    it('Bad property names', async () => {
        try {
            let errors = 0
            assert(!(await gen.generate('test/forms/bad-propertyNames.form', undefined, output, undefined, undefined, undefined, true, false, false,
                (type, msg) => {
                    feedback(type, msg)
                    if (type === gen.FeedbackType.error) ++errors
                })))
            assert.strictEqual(errors, 4)
        } catch (e) {
            assert.fail(e.message)
        }
    })

    it('Examples verification', async () => {
        try {
            const testOutput = `${output}/enum`
            let errors = 0
            assert(!(await gen.generate('test/forms/enum.form', undefined, testOutput, undefined, undefined, undefined, true, false, false,
                (type, msg) => {
                    feedback(type, msg)
                    if (type === gen.FeedbackType.error) ++errors
                })))
            assert.strictEqual(errors, 7)
            await includes(`${testOutput}/language-understanding/en-us/examples/enum-examples-examplesEntity.en-us.lu`, 'why not')
            await includes(`${testOutput}/language-understanding/en-us/examplesArray/enum-examplesArray-examplesArrayEntity.en-us.lu`, 'repent again')
        } catch (e) {
            assert.fail(e.message)
        }
    })

    type FullTemplateName = string
    type TemplateName = string
    type Source = string
    type SourceToReferences = Map<Source, TemplateName[]>
    type TemplateToReferences = Map<FullTemplateName, SourceToReferences>
    const SourceToReferences = <{ new (): SourceToReferences}> Map
    const TemplateToReferences = <{new (): TemplateToReferences}> Map

    /**
     * Given a list of source LG templates return the references for each template inside.
     * Each imported file is only analyzed once.
     * @param templates List of templates to analyze.
     * @returns {Source:TemplateName} -> {source -> [references]}
     */
    function templateUsage(templates: Templates[]): TemplateToReferences {
        const usage = new TemplateToReferences()
        const analyzed = new Set<Source>()
        for (const source of templates) {
            // Map from simple to full name and initialize template usage
            const nameToFullname = new Map<string, string>()
            for (const template of source.allTemplates) {
                const fullName = `${template.sourceRange.source}:${template.name}`
                nameToFullname.set(template.name, fullName)
                if (!usage.get(fullName)) {
                    usage.set(fullName, new SourceToReferences())
                }
            }

            // Add references from each template that is in an unanalyzed source file
            for (const template of source.allTemplates) {
                // Analyze each original source template only once
                if (!analyzed.has(template.sourceRange.source)) {
                    const info = source.analyzeTemplate(template.name)
                    for (const reference of info.TemplateReferences) {
                        const source = template.sourceRange.source as string
                        const referenceSources = usage.get(nameToFullname.get(reference) as string) as Map<string, string[]>
                        let referenceSource = referenceSources.get(source)
                        if (!referenceSource) {
                            referenceSource = []
                            referenceSources.set(source, referenceSource)
                        }
                        referenceSource.push(template.name)
                    }
                }
            }

            // Add in the newly analyzed sources
            for (const imported of source.imports) {
                analyzed.add(ppath.resolve(ppath.dirname(source.source), imported.id))
            }
            analyzed.add(source.source)
        }
        return usage
    }

    /**
     * Return the simple template name from a full template name.
     * @param fullname Full template name including source.
     */
    function templateName(fullname: string): string {
        const colon = fullname.lastIndexOf(':')
        return fullname.substring(colon + 1)
    }

    /**
     * Simplify full template name which is source:template to just filename:template.
     * @param fullname Full template name including source.
     * @returns filename:template
     */
    function shortTemplateName(fullname: string): string {
        const colon = fullname.lastIndexOf(':')
        return ppath.basename(fullname.substring(0, colon)) + fullname.substring(colon)
    }

    // This is only meaningful after running unit tests
    it('Unit test unused standard templates', async () => {
        // Compare cache to all standard template files
        const allTemplates = (await glob('templates/standard/**/*.lg')).map(t => ppath.resolve(t))
        const unused = allTemplates.filter(t => !gen.TemplateCache.has(t))
        // These are files that are only imported
        const excludeFiles = ['standard.en-us.lg']
        let unusedCount = 0
        for (const path of unused) {
            if (!excludeFiles.includes(ppath.basename(path))) {
                feedback(gen.FeedbackType.warning, `Unused template ${path}`)
                ++unusedCount
            }
        }

        // Analyze LG templates for usage
        const templates: Templates[] = []
        for (let template of gen.TemplateCache.values()) {
            if (template instanceof Templates) {
                templates.push(template)
            }
        }
        const usage = templateUsage(templates)
        
        // Dump out all template usage
        for (const [template, templateUsage] of usage) {
            feedback(gen.FeedbackType.debug, `${shortTemplateName(template)} references:`)
            for (const [source, sourceUsage] of templateUsage) {
                feedback(gen.FeedbackType.debug, `    ${ppath.basename(source)}: ${sourceUsage.join(', ')}`)
            }
        }

        // Identify unused templates
        // Exclusions are top-level templates called by the generator in standard.schema
        const exclude = ['filename', 'template', 'entities', 'templates', 'knowledgeDir', 'schemaOperations', 'schemaDefaultOperation']
        let unusedTemplates = 0
        for (const [template, templateUsage] of usage) {
            const name = templateName(template)
            if (!exclude.includes(name) && templateUsage.size === 0) {
                feedback(gen.FeedbackType.error, `${shortTemplateName(template)} is unused`)
                ++unusedTemplates
            }
        }

        assert.strictEqual(unusedCount, 0, `Found ${unusedCount} unused template files`)
        assert.strictEqual(unusedTemplates, 0, `Found ${unusedTemplates} unused templates`)
    })
})

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
import {generateTest} from '../src/testGenerator'
import * as ps from '../src/processSchemas'
import * as assert from 'assert'
import {Templates, DiagnosticSeverity} from 'botbuilder-lg'
import * as luFile from '@microsoft/bf-lu/lib/utils/filehelper'
import * as LuisBuilder from '@microsoft/bf-lu/lib/parser/luis/luisCollate'

import {AdaptiveBotComponent} from 'botbuilder-dialogs-adaptive'
import {LuisBotComponent, QnAMakerBotComponent} from 'botbuilder-ai'
import {ComponentDeclarativeTypes, ResourceExplorer} from 'botbuilder-dialogs-declarative'
import {ServiceCollection, noOpConfiguration} from 'botbuilder-dialogs-adaptive-runtime-core'

// Output temp directory
const tempDir = ppath.join(os.tmpdir(), 'generate.out')

function feedback(type: gen.FeedbackType, msg: string) {
    if (type !== gen.FeedbackType.debug) {
        console.log(`${type}: ${msg}`)
    }
}

// Remove hash because they differ depending on os.EOL
function removeHash(val: string): string {
    return val
        .replace(/"\$Generator": .*/g, '')
        .replace(/> Generator:.*/, '')
}

// NOTE: If you update dialog:merge functionality you need to execute the makeOracles.cmd to update them
export async function compareToOracle(path: string, oraclePath?: string): Promise<void> {
    const name = ppath.basename(path)
    const generateds = removeHash(await fs.readFile(path, 'utf8'))
    oraclePath = oraclePath ? ppath.join(tempDir, oraclePath) : ppath.join('test/oracles', name)
    const oracle = await fs.readFile(oraclePath, 'utf8')
    const oracles = removeHash(gen.normalizeEOL(oracle))
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
            `${ppath.resolve(path)} does not match oracle ${ppath.resolve(oraclePath)}`)
    }
}

async function checkDirectory(path: string, files: number, directories: number): Promise<void> {
    const dirList: string[] = []
    const fileList: string[] = []
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
    const output = tempDir
    const schemaPath = 'test/forms/sandwich.form'
    const unitTestSchemaPath = 'test/forms/unittest_'
    const badSchema = 'test/forms/bad-schema.form'
    const notObject = 'test/forms/not-object.form'
    const override = 'test/templates/override'
    const unittestSchemaNames = [
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
        'dynamiclist',
        'dynamiclist_empty',
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
        'multiple_enum',
        'multiple_string',
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
        'transforms',
        'uri'
    ]

    beforeEach(async () => {
        await fs.remove(output)
    })

    it('Transcript test', async () => {
        try {
            console.log('\n\nTranscript test')
            assert.ok(await generateTest('test/transcripts/sandwich.transcript', 'sandwich', output, false), 'Could not generate test script')
            await compareToOracle(ppath.join(tempDir, 'sandwich.test.dialog'))
        } catch (e) {
            assert.fail(e.message)
        }
    })

    it('Transcript test click button on adaptive card', async () => {
        try {
            console.log('\n\nTranscript test')
            assert.ok(await generateTest('test/transcripts/addItemWithButton.transcript', 'msmeeting-actions', output, false), 'Could not generate test script')
            await compareToOracle(ppath.join(tempDir, 'addItemWithButton.test.dialog'))
        } catch (e) {
            assert.fail(e.message)
        }
    })

    it('Hash text', async () => {
        let lu = `> LU File${os.EOL}# Intent${os.EOL}- This is an .lu file`
        const lufile = ppath.join(os.tmpdir(), 'test.lu')

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
        let dialog = {$comment: 'this is a .dialog file'}
        const dialogFile = ppath.join(os.tmpdir(), 'test.dialog')

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
            await gen.generate(schemaPath,
                {
                    outDir: output,
                    templateDirs: [override, 'template:standard'],
                    feedback
                })
            const lg = await fs.readFile(ppath.join(output, 'language-generation/en-us/Bread', 'sandwich-Bread.en-us.lg'))
            assert.ok(lg.toString().includes('What kind of bread?'), 'Did not override locale generated file')
            const dialog = await fs.readFile(ppath.join(output, 'dialogs/Bread/sandwich-Bread-missing.dialog'))
            assert.ok(!dialog.toString().includes('priority'), 'Did not override top-level file')
        } catch (e) {
            assert.fail(e.message)
        }
    })

    it('Singleton', async () => {
        try {
            console.log('\n\nSingleton Generation')
            await gen.generate(schemaPath, {
                outDir: output,
                singleton: true,
                feedback
            })
            assert.ok(!await fs.pathExists(ppath.join(output, 'dialogs/Bread')), 'Generated non-singleton directories')
            assert.ok(await fs.pathExists(ppath.join(output, 'sandwich.dialog')), 'Did not generate root dialog')
        } catch (e) {
            assert.fail(e.message)
        }
    })

    it('Non singleton', async () => {
        try {
            console.log('\n\nNon singleton Generation')
            await gen.generate(schemaPath, {
                outDir: output,
                feedback
            })
            await checkDirectory(output, 8, 4)
            await checkDirectory(ppath.join(output, 'dialogs'), 0, 10)
            await checkDirectory(ppath.join(output, 'recognizers'), 2, 0)
            await checkDirectory(ppath.join(output, 'language-generation'), 0, 1)
            await checkDirectory(ppath.join(output, 'language-understanding'), 0, 1)
            await checkDirectory(ppath.join(output, 'language-generation', 'en-us'), 1, 15)
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
            const success = await gen.generate(`${unitTestSchemaPath}${name}.form`, {
                outDir: testOutput,
                singleton: true,
                feedback
            })
            if (success) {
                try {
                    console.log(`LG Testing schema ${name}`)
                    const templates = Templates.parseFile(`${testOutput}/language-generation/en-us/unittest_${prefix}.en-us.lg`)
                    const allDiagnostics = templates.allDiagnostics
                    if (allDiagnostics) {
                        const errors = allDiagnostics.filter((u): boolean => u.severity === DiagnosticSeverity.Error)
                        if (errors && errors.length > 0) {
                            const errorList: string[] = []
                            for (let j = 0; j < allDiagnostics.length; j++) {
                                const error = allDiagnostics[j]
                                errorList.push(`${error.message}: ${error.source} ${error.range}`)
                            }
                            assert.fail(errorList.join('\n'))
                        }
                    }

                    // Ensure all top-level LG uses activities
                    for (const path of await glob(`${testOutput.replace(/\\/g, '/')}/language-generation/en-us/*/*.lg`)) {
                        if (!path.includes('/form/')) {
                            const templates = Templates.parseFile(path)
                            const badTemplates = templates.allTemplates.filter(t =>
                                ppath.basename(t.sourceRange.source) === ppath.basename(path) &&
                                !t.name.endsWith('_text') &&
                                !t.name.endsWith('_Name') &&
                                !t.name.endsWith('_Value') &&
                                t.body.indexOf('[Activity') < 0)
                                .map(t => t.name)
                            assert.strictEqual(badTemplates.length, 0, `Missing activity ${badTemplates}`)
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

                    // Setup declarations
                    const services = new ServiceCollection({
                        declarativeTypes: [],
                    })
                    new AdaptiveBotComponent().configureServices(services, noOpConfiguration)
                    new LuisBotComponent().configureServices(services, noOpConfiguration)
                    new QnAMakerBotComponent().configureServices(services, noOpConfiguration)
                    const declarativeTypes = services.mustMakeInstance<ComponentDeclarativeTypes[]>('declarativeTypes')
                    const resourceExplorer = new ResourceExplorer({declarativeTypes})

                    // Add folder and load type
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
            const totalExpected = 26
            const globalExpected = 3
            const propertyExpected = 23
            assert.strictEqual(Object.keys(schemas).length, totalExpected, `Expected ${totalExpected} schemas and found ${Object.keys(schemas).length}`)
            let global = 0
            let property = 0
            for (let [_, schema] of Object.entries(schemas)) {
                if (ps.isGlobalSchema(schema)) {
                    ++global
                } else {
                    ++property
                    assert(schema.$generator.title, `${schema.$templateDirs} missing title`)
                    assert(schema.$generator.description, `${schema.$templateDirs} missing description`)
                }
            }
            assert.strictEqual(global, globalExpected, `Expected ${globalExpected} global schemas and found ${global}`)
            assert.strictEqual(property, propertyExpected, `Expected ${propertyExpected} property schemas and found ${property}`)
        } catch (e) {
            assert.fail(e.message)
        }
    })

    it('Expand simple property definition', async () => {
        try {
            const schema = {
                type: 'number'
            }
            const expansion = await gen.expandPropertyDefinition('simple', schema)
            assert(expansion.$entities, 'Did not generate $entities')
            assert.strictEqual(expansion.$entities.length, 1, 'Wrong number of entities')
        } catch (e) {
            assert.fail(e.message)
        }
    })

    it('Bad property names', async () => {
        try {
            let errors = 0
            assert(!(await gen.generate('test/forms/bad-propertyNames.form', {
                outDir: output,
                force: true,
                feedback: (type, msg) => {
                    feedback(type, msg)
                    if (type === gen.FeedbackType.error) ++errors
                }
            })), 'Should have failed generation')
            assert.strictEqual(errors, 4, 'Wrong number of errors')
        } catch (e) {
            assert.fail(e.message)
        }
    })

    it('Examples verification', async () => {
        try {
            const testOutput = `${output}/enum`
            let errors = 0
            let warnings = 0
            assert(!(await gen.generate('test/forms/enum.form', {
                outDir: testOutput,
                force: true,
                feedback: (type, msg) => {
                    feedback(type, msg)
                    if (type === gen.FeedbackType.error) ++errors
                    if (type === gen.FeedbackType.warning) ++warnings
                }
            })), 'Should have failed generation')
            assert.strictEqual(errors, 7, 'Wrong number of errors')
            assert.strictEqual(warnings, 8, 'Wrong number of warnings')
            await includes(`${testOutput}/language-understanding/en-us/ok/enum-ok-okValue.en-us.lu`, 'this is ok')
            await includes(`${testOutput}/language-understanding/en-us/ok/enum-ok-okValue.en-us.lu`, 'ok phrases')
            await includes(`${testOutput}/language-understanding/en-us/okArray/enum-okArray-okArrayValue.en-us.lu`, 'this is okArray')
            await includes(`${testOutput}/language-understanding/en-us/examples/enum-examples-examplesValue.en-us.lu`, 'why not')
            await includes(`${testOutput}/language-understanding/en-us/examplesArray/enum-examplesArray-examplesArrayValue.en-us.lu`, 'repent again')
        } catch (e) {
            assert.fail(e.message)
        }
    })

    it('Examples generation', () => {
        const examples = gen.examples(['abcDef', 'ghi jkl', 'MnoPQR', 'stu_vwx'])
        assert.deepStrictEqual(examples['abcDef'], ['abc', 'def', 'abc def'])
        assert.deepStrictEqual(examples['ghi jkl'], ['ghi', 'jkl', 'ghi jkl'])
        assert.deepStrictEqual(examples['MnoPQR'], ['mno', 'pqr', 'mno pqr'])
        assert.deepStrictEqual(examples['stu_vwx'], ['stu', 'vwx', 'stu vwx'])
    })

    it('Missing template directory', async () => {
        try {
            let errors = 0
            let warnings = 0
            assert(!(await gen.generate('test/forms/unittest_transforms.form', {
                outDir: tempDir,
                templateDirs: ['test/templates/overrides', 'template:standard'],
                feedback: (type, msg) => {
                    feedback(type, msg)
                    if (type === gen.FeedbackType.error) ++errors
                    if (type === gen.FeedbackType.warning) ++warnings
                }
            })), 'Should have failed generation')
            assert.strictEqual(errors, 2, 'Wrong number of errors')
            assert.strictEqual(warnings, 0, 'Wrong number of warnings')
        } catch (e) {
            assert.fail(e.message)
        }
    })

    it('Global transform', async () => {
        try {
            let errors = 0
            let warnings = 0
            assert((await gen.generate('test/forms/unittest_transforms.form', {
                outDir: tempDir,
                templateDirs: ['test/templates', override, 'template:standard'],
                transforms: ['addOne'],
                force: true,
                singleton: true,
                feedback: (type, msg) => {
                    feedback(type, msg)
                    if (type === gen.FeedbackType.error) ++errors
                    if (type === gen.FeedbackType.warning) ++warnings
                }
            })), 'Should not have failed generation')
            assert.strictEqual(errors, 0, 'Wrong number of errors')
            assert.strictEqual(warnings, 3, 'Wrong number of warnings')
            await compareToOracle(ppath.join(tempDir, 'unittest_transforms.dialog'))
        } catch (e) {
            assert.fail(e.message)
        }
    })

    type FullTemplateName = string
    type TemplateName = string
    type Source = string
    type SourceToReferences = Map<Source, TemplateName[]>
    type TemplateToReferences = Map<FullTemplateName, SourceToReferences>
    const SourceToReferences = <{new(): SourceToReferences}>Map
    const TemplateToReferences = <{new(): TemplateToReferences}>Map

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
                    try {
                        const references = source.analyzeTemplate(template.name).TemplateReferences
                        const templateSource = template.sourceRange.source
                        for (const reference of references) {
                            const referenceSources = usage.get(nameToFullname.get(reference) as string) as Map<string, string[]>
                            let referenceSource = referenceSources.get(templateSource)
                            if (!referenceSource) {
                                referenceSource = []
                                referenceSources.set(templateSource, referenceSource)
                            }
                            referenceSource.push(template.name)
                        }
                    } catch (e) {
                        // If you have a recursive template like sortNumber you will get an exception
                    }
                }
            }

            if (source.allTemplates.some(t => t.name == 'transforms')) {
                // Mark all templates in transforms as being used.
                // They are not picked up because they are called using template
                for (const template of source.allTemplates) {
                    const referenceSources = usage.get(nameToFullname.get(template.name) as string) as Map<string, string[]>
                    let referenceSource = referenceSources.get('transforms')
                    if (!referenceSource) {
                        referenceSource = []
                        referenceSources.set('transforms', referenceSource)
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
    function generatorName(fullname: string): string {
        const colon = fullname.lastIndexOf(':')
        return fullname.substring(colon + 1)
    }

    /**
     * Simplify full template name which is source:template to just filename:template.
     * @param fullname Full template name including source.
     * @returns filename:template
     */
    function shortGeneratorName(fullname: string): string {
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
        for (const template of gen.TemplateCache.values()) {
            if (template instanceof Templates) {
                templates.push(template)
            }
        }
        const usage = templateUsage(templates)

        // Dump out all template usage
        for (const [template, templateUsage] of usage) {
            feedback(gen.FeedbackType.debug, `${shortGeneratorName(template)} references:`)
            for (const [source, sourceUsage] of templateUsage) {
                feedback(gen.FeedbackType.debug, `    ${ppath.basename(source)}: ${sourceUsage.join(', ')}`)
            }
        }

        // Identify unused templates
        // Exclusions are top-level templates called by the generator in standard.schema or called through template
        const exclude = ['filename', 'generator', 'entities', 'generators', 'transforms', 'knowledgeDir', 'schemaOperations', 'schemaDefaultOperation', 'isSetProperty', 'sortPosition', 'triggerNames']
        let unusedTemplates = 0
        for (const [template, templateUsage] of usage) {
            const name = generatorName(template)
            if (!exclude.includes(name) && templateUsage.size === 0) {
                feedback(gen.FeedbackType.error, `${shortGeneratorName(template)} is unused`)
                ++unusedTemplates
            }
        }

        assert.strictEqual(unusedCount, 0, `Found ${unusedCount} unused template files`)
        assert.strictEqual(unusedTemplates, 0, `Found ${unusedTemplates} unused templates`)
    })
})

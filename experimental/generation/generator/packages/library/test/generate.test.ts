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

    // This is only meaningful after running unit tests
    it('Unit test unused standard templates', async () => {
        // Compare cache to all standard template files
        const allTemplates = (await glob('templates/standard/**/*.lg')).map(t => ppath.resolve(t))
        const unused = allTemplates.filter(t => !gen.TemplateCache.has(t))
        for (const path of unused) {
            feedback(gen.FeedbackType.warning, `Unused template ${ppath.relative('templates/standard', path)}`)
        }

        // After running unit tests this should be 1 because of standard.en-us.lg
        assert.strictEqual(unused.length, 1)
    })
})

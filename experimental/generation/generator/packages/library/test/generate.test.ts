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
import { generateTest } from '../src/testGenerator'
import * as ps from '../src/processSchemas'
import * as assert from 'assert'
const { Templates, DiagnosticSeverity } = require('botbuilder-lg')
const file = require('@microsoft/bf-lu/lib/utils/filehelper')
const LuisBuilder = require('@microsoft/bf-lu/lib/parser/luis/luisCollate')

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

describe('dialog:generate library', async () => {
    let output = tempDir
    let schemaPath = 'test/forms/sandwich.schema'
    let unitTestSchemaPath = 'test/forms/unittest_'
    let badSchema = 'test/forms/bad-schema.schema'
    let notObject = 'test/forms/not-object.schema'
    let override = 'test/templates/override'
    let unittestSchemaNames = ['number', "number_with_limitation", 'integer', 'integer_with_limitation', 'boolean', 'array_personName', 'enum', 'array_enum', 'email', 'uri', 'iri', 'date-time', 'date', 'time', 'personName', 'personName_with_pattern', 'phonenumber', 'keyPhrase', 'keyPhrase_with_pattern', 'percentage', 'age', 'age_with_units', 'ordinal', 'geography', 'money', 'money_with_units', 'temperature', 'temperature_with_units', 'dimension', 'dimension_with_units', 'datetime']

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
            await gen.generate(schemaPath, undefined, output, undefined, ['en-us'], [override, 'standard'], false, false, undefined, feedback)
            let lg = await fs.readFile(ppath.join(output, 'en-us/bread', 'sandwich-Bread.en-us.lg'))
            assert.ok(lg.toString().includes('What kind of bread?'), 'Did not override locale generated file')
            let dialog = await fs.readFile(ppath.join(output, 'bread/sandwich-Bread-missing.dialog'))
            assert.ok(!dialog.toString().includes('priority'), 'Did not override top-level file')
        } catch (e) {
            assert.fail(e.message)
        }
    })

    it('Singleton', async () => {
        try {
            console.log('\n\nSingleton Generation')
            await gen.generate(schemaPath, undefined, output, undefined, ['en-us'], undefined, false, false, true, feedback)
            assert.ok(!await fs.pathExists(ppath.join(output, 'Bread')), 'Did not generate singleton directories')
            assert.ok(await fs.pathExists(ppath.join(output, 'sandwich.dialog')), 'Did not root dialog')
        } catch (e) {
            assert.fail(e.message)
        }
    })

    it('LU and LG Unit Test', async () => {
        for (let i = 0; i < unittestSchemaNames.length; i++) {
            await gen.generate(`${unitTestSchemaPath}${unittestSchemaNames[i]}.schema`, undefined, output, undefined, ['en-us'], undefined, false, false, true, feedback)
            try {
                console.log(`\n\nLG Testing schema ${unittestSchemaNames[i]}`)
                const templates = Templates.parseFile(`${output}/en-us/unittest${i}.en-us.lg`)
                const allDiagnostics = templates.allDiagnostics
                if (allDiagnostics) {
                    let errors = allDiagnostics.filter((u): boolean => u.severity === DiagnosticSeverity.Error)
                    if (errors && errors.length > 0) { throw new Error('Surface errors.') }
                }
            } catch (e) {
                assert(e.message)
            }
            try {
                console.log(`\n\nLU Testing schema ${unittestSchemaNames[i]}`)
                let result: any
                const luFiles = await file.getLuObjects(undefined, `${output}/en-us/unittest${i}.en-us.lu`, true, ".lu")
                result = await LuisBuilder.build(luFiles, true, "en-us")
                result.validate()
                if (!hasContent(result)) {
                    throw new Error('Surface errors.')
                }
            } catch (e) {
                assert(e.message)
            }
            await fs.remove(output)
        }
    })

    function hasContent(luisInstance: any) {
        for (let prop in luisInstance) {
            if (Array.isArray(luisInstance[prop]) && luisInstance[prop].length > 0) return true
        }
        return false
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

    it('Schema discovery', async() => {
        try {
            let schemas = await ps.schemas()
            assert.equal(Object.keys(schemas).length, 10, 'Wrong number of schemas discovered')
            let global = 0
            let property = 0
            for (let [_, schema] of Object.entries(schemas)) {
                if (ps.isGlobalSchema(schema)) {
                    ++global
                } else {
                    ++property
                }
            }
            assert.equal(global, 3, 'Wrong number of global schemas')
            assert.equal(property, 7, 'Wrong number of property schemas')
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
            assert.equal(expansion.$entities.length, 1, 'Wrong number of entities')
        } catch (e) {
            assert.fail(e.message)
        }
    })

    it('Expand $ref property definition', async () => {
        try {
            let schema = {
                $ref: "template:dimension.schema"
            }
            let expansion = await gen.expandPropertyDefinition('ref', schema)
            assert(expansion.$entities, 'Did not generate $entities')
            assert.equal(expansion.$entities.length, 1, 'Wrong number of entities')
        } catch (e) {
            assert.fail(e.message)
        }
    })
})

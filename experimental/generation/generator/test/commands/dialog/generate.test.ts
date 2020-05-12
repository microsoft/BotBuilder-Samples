/*!
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
// tslint:disable:no-console
// tslint:disable:no-object-literal-type-assertion

import { expect, test } from '@oclif/test';
import * as fs from 'fs-extra'
import 'mocha'
import * as os from 'os'
import * as ppath from 'path'
import * as ft from '../../../src/library/schema'
import * as gen from '../../../src/library/dialogGenerator'
import * as assert from 'assert';

function feedback(type: gen.FeedbackType, msg: string) {
    console.log(`${type}: ${msg}`)
}

describe('dialog:generate', async () => {
    let output = ppath.join(os.tmpdir(), 'sandwich.out')
    let schemaPath = 'test/commands/dialog/forms/sandwich.schema'
    let badSchema = 'test/commands/dialog/forms/bad-schema.schema'
    let notObject = 'test/commands/dialog/forms/not-object.schema'
    let override = 'test/commands/dialog/templates/override'

    beforeEach(async () => {
        await fs.remove(output)
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
            await gen.generate(schemaPath, undefined, output, undefined, ['en-us'], [override, 'standard'], false, false, feedback)
            let lg = await fs.readFile(ppath.join(output, 'en-us', 'sandwich-Bread.en-us.lg'))
            assert.ok(lg.toString().includes('What kind of bread?'), 'Did not override locale generated file')
            let dialog = await fs.readFile(ppath.join(output, 'sandwich-Bread-missing.dialog'))
            assert.ok(!dialog.toString().includes('priority'), 'Did not override top-level file')
        } catch (e) {
            assert.fail(e.message)
        }
    })

    it('Generation', async () => {
        try {
            console.log('\n\nGeneration')
            await gen.generate(schemaPath, undefined, output, undefined, ['en-us'], undefined, false, false, feedback)
        } catch (e) {
            assert.fail(e.message)
        }
    })

    it('Not object type', async () => {
        try {
            await ft.Schema.readSchema(notObject)
            assert.fail('Did not detect bad schema');
        } catch (e) {
            expect(e.message).to.contain('must be of type object')
        }
    })

    it('Illegal schema', async () => {
        try {
            await ft.Schema.readSchema(badSchema)
            assert.fail('Did not detect bad schema');
        } catch (e) {
            expect(e.message).to.contain('is not a valid JSON Schema')
        }
    })

    test
        .stdout()
        .stderr()
        .command(['dialog:generate', `${badSchema}`])
        .it('Detect bad schema', ctx => {
            expect(ctx.stderr)
                .to.contain('not a valid JSON Schema')
        })

    test
        .stdout()
        .stderr()
        .command(['dialog:generate', `${schemaPath}`, '-o', `${output}`, '--verbose'])
        .it('Detect success', ctx => {
            expect(ctx.stderr)
                .to.contain('Generating')
        })
})

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

describe('generate', async () => {
    let output = ppath.join(os.tmpdir(), 'sandwich.out')
    let schemaPath = 'test/commands/generate/forms/sandwich.schema'
    let badSchema = 'test/commands/generate/forms/bad-schema.schema'
    let notObject = 'test/commands/generate/forms/not-object.schema'
    let override = 'test/commands/generate/templates/override'

    beforeEach(async () => {
        await fs.remove(output)
    })

    it('Generation with override', async () => {
        try {
            await gen.generate(schemaPath, undefined, output, undefined, ['en-us'], [override, 'standard'], false, (type, msg) => {
                console.log(`${type}: ${msg}`)
            })
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
            await gen.generate(schemaPath, undefined, output, undefined, ['en-us'], undefined, false, (type, msg) => {
                console.log(`${type}: ${msg}`)
            })
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
        .command(['generate', `${badSchema}`])
        .it('Detect bad schema', ctx => {
            expect(ctx.stderr)
                .to.contain('not a valid JSON Schema')
        })

    test
        .stdout()
        .stderr()
        .command(['generate', `${schemaPath}`, '-o', `${output}`, '--verbose'])
        .it('Detect success', ctx => {
            expect(ctx.stderr)
                .to.contain('Generating')
        })
})

/*!
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
// tslint:disable:no-console
// tslint:disable:no-object-literal-type-assertion

import {expect, test} from '@oclif/test';
import * as fs from 'fs-extra'
import 'mocha'
import * as os from 'os'
import * as ppath from 'path'
import * as gen from '@microsoft/bf-generate-library'

function feedback(type: gen.FeedbackType, msg: string) {
    if (type !== gen.FeedbackType.debug) {
        console.log(`${type}: ${msg}`)
    }
}

describe('dialog:generate', async () => {
    let output = ppath.join(os.tmpdir(), 'test.out')
    let schemaPath = 'test/commands/dialog/forms/sandwich.schema'
    let badSchema = 'test/commands/dialog/forms/bad-schema.schema'
    let swaggerPath = 'test/commands/dialog/forms/petSwagger.json'
    let method = 'post'
    let route = '/store/order'
    let schemaName = 'petOrder.schema'

    beforeEach(async () => {
        await fs.remove(output)
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
        .it('Generated successfully', ctx => {
            expect(ctx.stderr)
                .not.to.contain('Error')
        })
/*
    test
        .stdout()
        .stderr()
        .command(['dialog:generate:swagger', `${swaggerPath}`, 
            '-o', `${output}`, 
            '-m', `${method}`, 
            '-n', `${schemaName}`,
            '-r', `${route}`,
            '--verbose'])
        .it('Detect success', ctx => {
            expect(ctx.stderr)
                .to.contain('Generating')
        })
        */
})

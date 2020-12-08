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
import * as gen from '../src/dialogGenerator'
import * as genSwagger from '../src/swaggerDialogGenerator'
import * as assert from 'assert';

function feedback(type: gen.FeedbackType, msg: string) {
    if (type !== gen.FeedbackType.debug) {
        console.log(`${type}: ${msg}`)
    }
}

describe('dialog:generate:swagger', async () => {
    let output = ppath.join(os.tmpdir(), 'swagger')
    let swaggerPath = 'test/forms/petSwagger.json'
    let method = 'post'
    let route = '/store/order'
    let schemaName = 'petOrder.schema'

    before(async () => {
        await fs.remove(output)        
    })

    it('Generate JSON Schema', async () => {
        try {
            console.log('\n\nJSON schema Generation')
            await genSwagger.swaggerGenerate(swaggerPath, output, method, route, schemaName, feedback)
            let schemaFile = await fs.readFile(ppath.join(output, schemaName))
            assert.ok(schemaFile.toString().includes('$parameters'), 'Did not generate parameters')
        } catch (e) {
            assert.fail(e.message)
        }
    })

    it('Generate Swagger Bot Asset', async () => {
        try {
            let schemaPath = ppath.join(output, schemaName)
            let outDir = ppath.join(output, 'out')
            console.log('\n\nBot Asset Generation')
            await gen.generate(schemaPath, undefined, outDir, undefined, ['en-us'], undefined, false, false, false, feedback)
            assert.ok(await fs.pathExists(ppath.join(outDir, 'dialogs', 'form', 'petOrder-form-HttpRequestIntent.dialog')), 'Did not generate http request dialog')
        } catch (e) {
            assert.fail(e.message)
        }
    })

})

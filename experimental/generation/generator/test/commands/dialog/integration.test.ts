/*!
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
// tslint:disable:no-console
// tslint:disable:no-object-literal-type-assertion

import { expect, test } from '@oclif/test';
import * as fs from 'fs-extra'
import 'mocha'
import * as ppath from 'path'
import * as os from 'os'
import * as assert from 'assert';

import * as integ from '../../../src/library/integration'

describe('dialog:integrate', async () => {
    let output_dir = ppath.join(os.tmpdir(), 'sandwichBuy3')
    let schemaName = 'sandwichBuy'
    let oldPath = 'test/commands/dialog/integrate_test_data/sandwichBuy1'
    let newPath = 'test/commands/dialog/integrate_test_data/sandwichBuy2'
    let locale = 'en-us'

    beforeEach(async () => {
        await fs.remove(output_dir)
    })

    it('integrate: Merge dialog files', async () => {
        try {
            await integ.integrateAssets(schemaName, oldPath, newPath, output_dir, locale, (type, msg) => {console.log(`${type}: ${msg}`)})

            let resultDialog = `${output_dir}/sandwichBuy.main.dialog`
            let dialog = await fs.readFile(resultDialog)
            assert.ok(dialog.toString().includes('sandwichBuy-Hobby-missing'), 'Did not merge dialog files')
        } catch (e) {
            assert.fail(e.message)
        }
    })

    // it('integrate: Merge other files', async () => {
    //     try {
    //         await integ.integrateAssets(schemaName, oldPath, newPath, output_dir, locale, (type, msg) => {console.log(`${type}: ${msg}`)})

    //         let resultOther = `${output_dir}/sandwichBuy-Bread.qna`
    //         let other = await fs.readFile(resultOther)
    //         assert.ok(other.toString().includes('old sandwich Bread'), 'Did not merge otehr files, e.g. .qna')
    //     } catch (e) {
    //         assert.fail(e.message)
    //     }
    // })

    it('integrate: Merge lg files', async () => {
        try {
            await integ.integrateAssets(schemaName, oldPath, newPath, output_dir, locale)
            
            let resultLG = `${output_dir}/${locale}/sandwichBuy-BreadEntity.en-us.lg`
            let lg = await fs.readFile(resultLG)
            assert.ok(lg.toString().includes('black'), 'Did not merge lg files')
        } catch (e) {
            assert.fail(e.message)
        }
    })

    it('integrate: Merge lu files', async () => {
        try {
            await integ.integrateAssets(schemaName, oldPath, newPath, output_dir, locale)

            let resultLU = `${output_dir}/${locale}/sandwichBuy-BreadEntity.en-us.lu`
            let lu = await fs.readFile(resultLU)
            assert.ok(lu.toString().includes('black'), 'Did not merge lu files')
        } catch (e) {
            assert.fail(e.message)
        }
    })

    test
        .stdout()
        .stderr()
        .command(['dialog:integrate', `${schemaName}`, '-o', `${oldPath}`, '-n', `${newPath}`, '-m', `${output_dir}`, '-l', `${locale}`])
        .it('Detect success', ctx => {
            expect(ctx.stderr)
                .to.contain('Create output dir')
        })
})

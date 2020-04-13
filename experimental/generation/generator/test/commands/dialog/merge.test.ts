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
import * as gen from '../../../src/library/dialogGenerator';

type Comparison = { dir1: string, dir1Files: string[], dir1Only: string[], dir2: string, dir2Files: string[], dir2Only: string[], same: string[], different: string[] }

function displayCompare(comparison: Comparison) {
    console.log(`Compare ${comparison.dir1} to ${comparison.dir2}`)
    console.log(`  dir1Only:\n    ${comparison.dir1Only.join(',\n    ')}`)
    console.log(`  dir2Only:\n    ${comparison.dir2Only.join(',\n    ')}`)
    console.log(`  same:\n    ${comparison.same.join(',\n    ')}`)
    console.log(`  different:\n    ${comparison.different.join(',\n    ')}`)
}

function assertCheck(comparison: Comparison, errors: string[]) {
    if (errors.length > 0) {
        displayCompare(comparison)
        assert.fail(errors.join(gen.EOL))
    }
}

async function allFiles(base: string, path: string, files: Set<string>) {
    let stats = await fs.lstat(path)
    if (stats.isDirectory()) {
        for (let child of await fs.readdir(path)) {
            await allFiles(base, ppath.join(path, child), files)
        }
    } else {
        files.add(ppath.relative(base, path))
    }
}

async function compareDirs(dir1: string, dir2: string): Promise<Comparison> {
    let comparison: Comparison = { dir1: dir1, dir1Files: [], dir2: dir2, dir2Files: [], dir1Only: [], dir2Only: [], same: [], different: [] }
    let dir1Files = new Set<string>()
    let dir2Files = new Set<string>()
    await allFiles(dir1, dir1, dir1Files)
    await allFiles(dir2, dir2, dir2Files)
    comparison.dir1Files = Array.from(dir1Files)
    comparison.dir2Files = Array.from(dir2Files)
    for (let file1 of dir1Files) {
        if (dir2Files.has(file1)) {
            // See if files are the same
            let val1 = await fs.readFile(ppath.join(dir1, file1), 'utf-8')
            let val2 = await fs.readFile(ppath.join(dir2, file1), 'utf-8')
            if (val1 === val2) {
                comparison.same.push(file1)
            } else {
                comparison.different.push(file1)
            }
        } else {
            comparison.dir1Only.push(file1)
        }
    }
    comparison.dir2Only = [...dir2Files].filter(x => !dir1Files.has(x))
    return comparison
}

function entryCompare(comparison: Comparison, name: string, expected: number | string[] | undefined, errors: string[]) {
    if (expected) {
        let value = comparison[name] as string[]
        if (typeof expected === 'number') {
            if (value.length !== expected) {
                errors.push(`${name}: ${value.length} != ${expected}`)
            }
        } else {
            for (let expect of expected as string[]) {
                if (!value.includes(expect)) {
                    errors.push(`${name} does not contain ${expect}`)
                }
            }
        }
    }
}

function assertCompare(comparison: Comparison,
    errors: string[],
    same?: number | string[],
    different?: number | string[],
    dir1Only?: number | string[],
    dir2Only?: number | string[]): boolean {
    entryCompare(comparison, 'same', same, errors)
    entryCompare(comparison, 'different', different, errors)
    entryCompare(comparison, 'dir1Only', dir1Only, errors)
    entryCompare(comparison, 'dir2Only', dir2Only, errors)
    return errors.length > 0
}

describe('dialog:merge', async () => {
    let output_dir = ppath.join(os.tmpdir(), 'mergeTest')
    let originalSchema = 'test/commands/dialog/merge_data/sandwichMerge.schema'
    let modifiedSchema = 'test/commands/dialog/merge_data/sandwichMerge-modified.schema'
    let locales = ['en-us']
    let originalDir = ppath.join(output_dir, 'sandwichMerge-original')
    let modifiedDir = ppath.join(output_dir, 'sandwichMerge-modified')
    let mergedDir = ppath.join(output_dir, 'sandwichMerge-merged')
    let feedback = (type: gen.FeedbackType, msg: string): void => {
        if (type === gen.FeedbackType.warning || type === gen.FeedbackType.error) {
            assert.fail(`${type}: ${msg}`)
        }
    }

    before(async () => {
        try {
            await fs.remove(output_dir)
            await gen.generate(originalSchema, 'sandwichMerge', originalDir, undefined, locales, undefined, false, undefined, feedback)
            await gen.generate(modifiedSchema, 'sandwichMerge', modifiedDir, undefined, locales, undefined, undefined, false, feedback)
        } catch (e) {
            assert.fail(e.message)
        }
    })

    beforeEach(async () => {
        try {
            await fs.remove(mergedDir)
            await fs.copy(originalDir, mergedDir)
        } catch (e) {
            assert.fail(e.message)
        }
    })

    it('merge: self', async () => {
        try {
            await gen.generate(originalSchema, undefined, mergedDir, undefined, locales, undefined, undefined, true, feedback)
            let comparison = await compareDirs(originalDir, mergedDir)
            let errors: string[] = []
            assertCompare(comparison, errors, comparison.dir1Files.length)
            assertCheck(comparison, errors)
        } catch (e) {
            assert.fail(e.message)
        }
    })


    it('merge: modified', async () => {
        try {
            await gen.generate(modifiedSchema, 'sandwichMerge', mergedDir, undefined, locales, undefined, undefined, true, feedback)
            let comparison = await compareDirs(originalDir, mergedDir)
            let errors: string[] = []
            assertCompare(comparison, errors, comparison.dir1Files.length)
            assertCheck(comparison, errors)
        } catch (e) {
            assert.fail(e.message)
        }
    })
    /*
    it('merge: Merge dialog files', async () => {
        try {
            await integ.mergeAssets(schemaName, oldPath, newPath, output_dir, locale, (type, msg) => { console.log(`${type}: ${msg}`) })

            let resultDialog = `${output_dir}/sandwichMerge.main.dialog`
            let dialog = await fs.readFile(resultDialog)
            assert.ok(dialog.toString().includes('sandwichMerge-Hobby-missing'), 'Did not merge dialog files')
        } catch (e) {
            assert.fail(e.message)
        }
    })

    it('merge: Merge lg files', async () => {
        try {
            await integ.mergeAssets(schemaName, oldPath, newPath, output_dir, locale)

            let resultLG = `${output_dir}/${locale}/sandwichMerge-BreadEntity.en-us.lg`
            let lg = await fs.readFile(resultLG)
            assert.ok(lg.toString().includes('black'), 'Did not merge lg files')
        } catch (e) {
            assert.fail(e.message)
        }
    })

    it('merge: Merge lu files', async () => {
        try {
            await integ.mergeAssets(schemaName, oldPath, newPath, output_dir, locale)

            let resultLU = `${output_dir}/${locale}/sandwichMerge-BreadEntity.en-us.lu`
            let lu = await fs.readFile(resultLU)
            assert.ok(lu.toString().includes('black'), 'Did not merge lu files')
        } catch (e) {
            assert.fail(e.message)
        }
    })

    test
        .stdout()
        .stderr()
        .command(['dialog:generate', `${schemaName}`, '-o', `${oldPath}`, '--merge', '-l', `${locale}`])
        .it('Detect success', ctx => {
            expect(ctx.stderr)
                .to.contain('Create output dir')
        })
        */
})

/*!
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
// tslint:disable:no-console
// tslint:disable:no-object-literal-type-assertion

import * as fs from 'fs-extra'
import * as glob from 'globby'
import 'mocha'
import * as ppath from 'path'
import * as os from 'os'
import * as assert from 'assert';
import * as gen from '../src/dialogGenerator';

type Diff = {
    file: string,
    position: number
}
type Comparison = {
    original: string, originalFiles: string[], originalOnly: string[],
    merged: string, mergedFiles: string[], mergedOnly: string[],
    same: string[],
    different: Diff[]
}

function filePaths(files: string[], base: string): string {
    return files.map(f => ppath.resolve(base, f)).join('\n    ')
}

function displayCompare(comparison: Comparison) {
    console.log(`Compare ${comparison.original} to ${comparison.merged}`)
    console.log(`  originalOnly:\n    ${filePaths(comparison.originalOnly, comparison.original)}`)
    console.log(`  mergedOnly:\n    ${filePaths(comparison.mergedOnly, comparison.merged)}`)
    console.log(`  same:\n    ${comparison.same.join('\n    ')}`)
    console.log(`  different:\n    ${comparison.different.map(d => `${d.position}: ${ppath.resolve(comparison.original, d.file)} != ${ppath.resolve(comparison.merged, d.file)}`).join('\n    ')}`)
}

function assertCheck(comparison: Comparison, errors: string[]) {
    if (errors.length > 0) {
        displayCompare(comparison)
        assert.fail(os.EOL + errors.join(os.EOL))
    }
}

async function allFiles(base: string, path: string, files: Set<string>) {
    const stats = await fs.lstat(path)
    if (stats.isDirectory()) {
        for (const child of await fs.readdir(path)) {
            await allFiles(base, ppath.join(path, child), files)
        }
    } else {
        files.add(ppath.relative(base, path))
    }
}

async function compareDirs(original: string, merged: string): Promise<Comparison> {
    const comparison: Comparison = {original, originalFiles: [], merged, mergedFiles: [], originalOnly: [], mergedOnly: [], same: [], different: []}
    const originalFiles = new Set<string>()
    const mergedFiles = new Set<string>()
    await allFiles(original, original, originalFiles)
    await allFiles(merged, merged, mergedFiles)
    comparison.originalFiles = Array.from(originalFiles)
    comparison.mergedFiles = Array.from(mergedFiles)
    for (const file1 of originalFiles) {
        if (mergedFiles.has(file1)) {
            // See if files are the same
            const originalVal = await fs.readFile(ppath.join(original, file1), 'utf-8')
            const mergedVal = await fs.readFile(ppath.join(merged, file1), 'utf-8')
            if (originalVal === mergedVal) {
                comparison.same.push(file1)
            } else {
                let pos = 0;
                while (pos < originalVal.length && pos < mergedVal.length) {
                    if (originalVal[pos] !== mergedVal[pos]) {
                        break
                    }
                    ++pos
                }
                comparison.different.push({file: file1, position: pos})
            }
        } else {
            comparison.originalOnly.push(file1)
        }
    }
    comparison.mergedOnly = [...mergedFiles].filter(x => !originalFiles.has(x))
    return comparison
}

function entryCompare(comparison: Comparison, name: string, expected: number | string[] | undefined, errors: string[]) {
    if (expected) {
        const value = comparison[name] as string[]
        if (typeof expected === 'number') {
            if (value.length !== expected) {
                errors.push(`${name}: ${value.length} != ${expected}`)
            }
        } else {
            for (const expect of expected as string[]) {
                if (!value.includes(expect)) {
                    errors.push(`${name} does not contain ${expect}`)
                }
            }
        }
    }
}

function assertCompare(
    comparison: Comparison,
    errors: string[],
    same?: number | string[],
    different?: number | string[],
    originalOnly?: number | string[],
    mergedOnly?: number | string[]): boolean {
    entryCompare(comparison, 'same', same, errors)
    entryCompare(comparison, 'different', different, errors)
    entryCompare(comparison, 'originalOnly', originalOnly, errors)
    entryCompare(comparison, 'mergedOnly', mergedOnly, errors)
    return errors.length > 0
}

function assertRemoved(comparison: Comparison, file: string, errors: string[]) {
    if (comparison.mergedFiles.includes(file)) {
        errors.push(`Did not expect ${file} in merged`)
    }
}

function assertAddedProperty(comparison: Comparison, added: string, errors: string[]): string[] {
    const found: string[] = []
    for (const file of comparison.mergedFiles) {
        if (file.includes(added)) {
            found.push(file)
        }
    }
    if (found.length === 0) {
        errors.push(`Missing ${added} in merged files`)
    }
    return found;
}

function assertRemovedProperty(comparison: Comparison, removed: string, errors: string[]): string[] {
    const found: string[] = []
    for (const file of comparison.mergedFiles) {
        if (file.includes(removed)) {
            found.push(file)
        }
    }
    if (found.length > 0) {
        errors.push(`Found ${removed} in merged files`)
    }
    return found;
}

describe('dialog:generate --merge library', async function () {
    const output_dir = ppath.join(os.tmpdir(), 'mergeTest')
    const merge_data = 'test/merge_data'
    const modified_data = `${merge_data}/modified`
    const originalSchema = ppath.join(merge_data, 'sandwichMerge.form')
    const modifiedSchema = ppath.join(merge_data, 'sandwichMerge-modified.form')
    const originalDir = ppath.join(output_dir, 'sandwichMerge-original')
    const modifiedDir = ppath.join(output_dir, 'sandwichMerge-modified')
    const mergedDir = ppath.join(output_dir, 'sandwichMerge-merged')

    function errorOnly(type: gen.FeedbackType, msg: string): void {
        if ((type === gen.FeedbackType.warning && !msg.startsWith('Replace')) || type === gen.FeedbackType.error) {
            assert.fail(`${type}: ${msg}`)
        }
    }

    function feedback(type: gen.FeedbackType, msg: string): void {
        if (type !== gen.FeedbackType.debug) {
            console.log(`${type}: ${msg}`)
        }
        if ((type === gen.FeedbackType.warning && !msg.startsWith('Replace')) || type === gen.FeedbackType.error) {
            assert.fail(`${type}: ${msg}`)
        }
    }

    async function assertContains(file: string, regex: RegExp, errors: string[]) {
        const val = await fs.readFile(ppath.join(mergedDir, file), 'utf8')
        if (!val.match(regex)) {
            errors.push(`${file} does not contain expected ${regex}`)
        }
    }

    async function assertMissing(file: string, regex: RegExp, errors: string[]) {
        const val = await fs.readFile(ppath.join(mergedDir, file), 'utf8')
        if (val.match(regex)) {
            errors.push(`${file} contains unexpected ${regex}`)
        }
    }

    async function assertUnchanged(file: string, expected: boolean, errors: string[]) {
        const unchanged = await gen.isUnchanged(ppath.join(mergedDir, file))
        if (unchanged !== expected) {
            errors.push(`${file} is unexpectedly ${expected ? 'changed' : 'unchanged'}`)
        }
    }

    async function copyToMerged(pattern: string) {
        const pathPattern = ppath.join(modified_data, pattern).replace(/\\/g, '/')
        for (const path of await glob(pathPattern)) {
            const file = ppath.relative(modified_data, path)
            const dest = ppath.join(mergedDir, file)
            console.log(`Modifying ${dest}`)
            await fs.copyFile(path, dest)
        }
    }

    async function deleteMerged(pattern: string) {
        const pathPattern = ppath.join(mergedDir, pattern).replace(/\\/g, '/')
        for (const file of await glob(pathPattern)) {
            console.log(`Deleting ${file}`)
            await fs.unlink(file)
        }
    }

    before(async function () {
        try {
            console.log('Deleting output directory')
            await fs.remove(output_dir)
            console.log('Generating original files')
            await gen.generate(originalSchema, {
                prefix: 'sandwichMerge',
                outDir: originalDir,
                feedback: errorOnly
            })
            console.log('Generating modified files')
            await gen.generate(modifiedSchema, {
                prefix: 'sandwichMerge',
                outDir: modifiedDir,
                feedback: errorOnly
            })
        } catch (e) {
            assert.fail(e.message)
        }
    })

    beforeEach(async function () {
        try {
            console.log('\n\nCopying original generated to merged')
            await fs.remove(mergedDir)
            await fs.copy(originalDir, mergedDir)
        } catch (e) {
            assert.fail(e.message)
        }
    })

    // Ensure merge with no changes is unchanged
    it('merge: self', async function () {
        try {
            console.log('Self merging')
            await gen.generate(originalSchema, {
                outDir: mergedDir,
                merge: true,
                feedback
            })
            const comparison = await compareDirs(originalDir, mergedDir)
            const errors: string[] = []
            assertCompare(comparison, errors, comparison.originalFiles.length)
            assertCheck(comparison, errors)
        } catch (e) {
            assert.fail(e.message)
        }
    })

    // Ensure merge with modified schema changes as expected
    it('merge: modified', async function () {
        try {
            console.log('Modified merge')
            await gen.generate(modifiedSchema, {
                prefix: 'sandwichMerge', 
                outDir: mergedDir, 
                merge: true, 
                feedback})
            const comparison = await compareDirs(originalDir, mergedDir)
            const errors = []
            assertAddedProperty(comparison, 'Hobby', errors)
            assertRemovedProperty(comparison, 'Meat', errors)
            assertRemovedProperty(comparison, 'Toppings', errors)
            assertRemovedProperty(comparison, 'Sauces', errors)
            await assertContains('language-generation/en-us/BreadValue/sandwichMerge-BreadValue.en-us.lg', /black/, errors)
            await assertMissing('language-generation/en-us/BreadValue/sandwichMerge-BreadValue.en-us.lg', /white/, errors)
            //sandwichMerge
            await assertContains('language-generation/en-us/CheeseValue/sandwichMerge-CheeseValue.en-us.lg', /brie/, errors)

            // Unchanged hash + optional enum fixes = hash updated
            await assertUnchanged('language-generation/en-us/BreadValue/sandwichMerge-BreadValue.en-us.lg', false, errors)
            await assertUnchanged('language-generation/en-us/Name/sandwichMerge-Name.en-us.lg', true, errors)
            assertCheck(comparison, errors)
        } catch (e) {
            assert.fail(e.message)
        }
    })

    // Respect user changes
    it('merge: respect changes', async function () {
        try {
            // Modify a dialog file it should stay unchanged except for main.dialog which should be updated, but not hash updated
            // Remove a dialog file and it should not come back
            // Modify an .lu file and it should have enum updated, but not hash
            // Modify an .lg file and it should have enum updated, but not hash
            console.log('Respect changes merge')
            await copyToMerged('**/language-generation/en-us/Bread/*')
            await copyToMerged('**/language-generation/en-us/BreadValue/*')
            await copyToMerged('**/language-understanding/en-us/Bread/*')
            await copyToMerged('**/language-understanding/en-us/form/*')
            await copyToMerged('sandwichMerge.dialog')
            await copyToMerged('dialogs/sandwichMerge-foo-missing.dialog')
            await deleteMerged('dialogs/Price/sandwichMerge-price-remove-money.dialog')
            await gen.generate(modifiedSchema, {
                prefix: 'sandwichMerge', 
                outDir: mergedDir, 
                merge: true,
                feedback})
            const comparison = await compareDirs(originalDir, mergedDir)
            const errors = []

            // Changed + optional enum fixes = hash not updated, so still changed
            await assertUnchanged('language-generation/en-us/BreadValue/sandwichMerge-BreadValue.en-us.lg', false, errors)
            await assertUnchanged('sandwichMerge.dialog', false, errors)

            // Despite enum update, hash updated so unchanged
            await assertUnchanged('language-generation/en-us/CheeseValue/sandwichMerge-CheeseValue.en-us.lg', false, errors)

            // Main should still be updated
            await assertContains('sandwichMerge.dialog', /sandwichMerge-foo/, errors)
            await assertMissing('sandwichMerge.dialog', /sandwichMerge-price-remove-money/, errors)

            // Removed should stay removed
            assertRemoved(comparison, 'dialogs/sandwichMerged-price-remove-money.dialog', errors)

            // Still get enum updates
            await assertContains('language-generation/en-us/BreadValue/sandwichMerge-BreadValue.en-us.lg', /black/, errors)
            await assertMissing('language-generation/en-us/BreadValue/sandwichMerge-BreadValue.en-us.lg', /white/, errors)
            //sandwichMerge
            await assertMissing('language-understanding/en-us/form/sandwichMerge-custom.en-us.lu', /pulled/, errors)
            await assertContains('language-understanding/en-us/Bread/sandwichMerge-Bread-BreadValue.en-us.lu', />- {@BreadProperty={@BreadValue=rye}}/, errors)
            await assertContains('language-understanding/en-us/Bread/sandwichMerge-Bread-BreadValue.en-us.lu', /black/, errors)

            assertCheck(comparison, errors)
        } catch (e) {
            assert.fail(e.message)
        }
    })
})

/*!
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

import * as fs from 'fs-extra';
import * as ppath from 'path';
import * as os from 'os';
import {assetDirectory, Feedback, FeedbackType, getHashCode, isUnchanged, writeFile, stringify} from './dialogGenerator'

const {Templates, SwitchCaseBodyContext} = require('botbuilder-lg');
const LUParser = require('@microsoft/bf-lu/lib/parser/lufile/luParser');
const sectionOperator = require('@microsoft/bf-lu/lib/parser/lufile/sectionOperator');
const lusectiontypes = require('@microsoft/bf-lu/lib/parser/utils/enums/lusectiontypes')

const GeneratorPattern = /\r?\n> Generator: ([a-zA-Z0-9]+)/

/**
 * @description：Detect if the old file was not changed.
 * @param oldFileList Paths to the old asset.
 * @param fileName File name of the .lu, .lg, .dialog and .qna file.
 */
async function isOldUnchanged(oldFileList: string[], fileName: string): Promise<boolean> {
    const filePaths = oldFileList.filter(file => file.endsWith(fileName))
    const filePath = filePaths[0]
    return !filePath || isUnchanged(filePath)
}

/**
 * @description：Get hashcode of the file
 * @param fileList Path to the asset.
 * @param fileName File name of the .lu, .lg, .dialog and .qna file.
 */
async function getHashCodeFromFile(fileList: string[], fileName: string): Promise<string> {
    const filePaths = fileList.filter(file => file.endsWith(fileName))
    const path = filePaths[0]
    return getHashCode(path)
}

/**
 * @description：Copy the single file including .lu .lg and .dialog.
 * @param sourcePath Path to the folder where the file is copied from.
 * @param destPath Path to the folder where the file is copied to.
 * @param fileName File name of the .lu, .lg, .dialog and .qna file.
 * @param sourceFileList List of source file paths.
 * @param feedback Callback function for progress and errors.
 */
async function copySingleFile(sourcePath: string, destPath: string, fileName: string, sourceFileList: string[], feedback: Feedback): Promise<void> {
    let filePaths = sourceFileList.filter(file => file.match(fileName))
    if (filePaths.length !== 0) {
        let sourceFilePath = filePaths[0]
        let destFilePath = sourceFilePath.replace(sourcePath, destPath)
        let destDirPath = ppath.dirname(destFilePath)
        await fs.ensureDir(destDirPath)
        await fs.copyFile(sourceFilePath, destFilePath)
        feedback(FeedbackType.info, `Copying ${fileName} from ${sourcePath}`)
    }
}

/**
 * @description：Write file to the specific path.
 * @param sourcePath Path to the folder where the file is copied from.
 * @param destPath Path to the folder where the file is copied to.
 * @param fileName File name of the .lu, .lg, .dialog and .qna file.
 * @param sourceFileList List of source file paths.
 * @param var File content.
 * @param feedback Callback function for progress and errors.
 */
async function writeToFile(sourcePath: string, destPath: string, fileName: string, sourceFileList: string[], val: string, feedback: Feedback): Promise<void> {
    let filePaths = sourceFileList.filter(file => file.match(fileName))
    if (filePaths.length !== 0) {
        let sourceFilePath = filePaths[0]
        let destFilePath = sourceFilePath.replace(sourcePath, destPath)
        let destDirPath = ppath.dirname(destFilePath)
        await fs.ensureDir(destDirPath)
        await writeFile(destFilePath, val, feedback, true)
        feedback(FeedbackType.info, `Merging ${fileName}`)
    }
}

/**
 * @description：Show up message.
 * @param fileName File name of the .lu, .lg, .dialog and .qna file.
 * @param feedback Callback function for progress and errors.
 */
function changedMessage(fileName: string, feedback: Feedback): void{
    feedback(FeedbackType.info, `*** Old and new both changed, manually merge from ${fileName} ***`)
}

/**
 * @description：Get all file paths from the specific dir.
 * @param dir Root dir.
 * @param fileList List of file paths.
 */
async function getFiles(dir: string, fileList?: string[]): Promise<string[]> {
    fileList = fileList ?? []
    let files = await fs.readdir(dir)
    for (let file of files) {
        let name = dir + '/' + file
        if ((await fs.stat(name)).isDirectory()) {
            await getFiles(name, fileList)
        } else {
            fileList.push(name)
        }
    }
    return fileList
}

function refFilename(ref: string, feedback: Feedback): string {
    let file = ''
    let matches = ref.match(/([^/]*)\)/)
    if (matches && matches.length == 2) {
        file = matches[1]
    } else {
        feedback(FeedbackType.error, `Could not parse ref ${ref}`)
    }
    return file
}

/**
 * Merge two bot assets to generate one merged bot asset. 
 *
 * Rules for merging:
 * 1) A file unchanged since last generated will be overwritten by the new file.
 * 2) A changed file will have its .lg/.lu enum or .dialog triggers overwritten,
 *    but nothing else and its hash code should not be updated.
 * 3) If a property existed in the old schema, but does not exist in the new
 *    schema all files for that property should be deleted and have references
 *    removed.
 * 4) If a property exists in both old and new schema, but a file is not present
 *    in the new directory, the file should not be copied over again and
 *    references should not be added.
 * 5) The order of .dialog triggers should be respected, i.e. if changed by the
 *    user it should remain the same. 
 * 6) If a file has changed and cannot be updated there will be a message to
 *    merge manually.
 *
 * @param schemaName Name of the .schema file.
 * @param oldPath Path to the folder of the old asset.
 * @param newPath Path to the folder of the new asset.
 * @param mergedPath Path to the folder of the merged asset.
 * @param locales Locales.
 * @param feedback Callback function for progress and errors.
 *
 */
export async function mergeAssets(schemaName: string, oldPath: string, newPath: string, mergedPath: string, locales: string[], feedback?: Feedback): Promise<boolean> {
    if (!feedback) {
        feedback = (_info, _message) => true
    }

    if (oldPath === mergedPath) {
        let tempOldPath = `${os.tmpdir()}/tempOld/`
        await fs.emptyDir(tempOldPath)
        await fs.copy(oldPath, tempOldPath)
        await fs.emptyDir(oldPath)
        oldPath = tempOldPath
    }

    try {
        for (let locale of locales) {
            let oldFileList = await getFiles(oldPath)
            let newFileList = await getFiles(newPath)

            const {oldPropertySet, newPropertySet} = await parseSchemas(schemaName, oldPath, newPath, newFileList, mergedPath, feedback)

            await mergeDialogs(schemaName, oldPath, oldFileList, newPath, newFileList, mergedPath, locale, oldPropertySet, newPropertySet, feedback)
            await mergeRootLUFile(schemaName, oldPath, oldFileList, newPath, newFileList, mergedPath, locale, feedback)
            await mergeRootFile(schemaName, oldPath, oldFileList, newPath, newFileList, mergedPath, locale, '.lg', oldPropertySet, newPropertySet, feedback)
            await mergeOtherFiles(oldPath, oldFileList, newPath, newFileList, mergedPath, feedback)
        }
    } catch (e) {
        feedback(FeedbackType.error, e.message)
    }

    return true
}

/**
 * @description: Merge other types of files, e.g qna files.
 * @param oldPath Path to the folder of the old asset.
 * @oldFileList List of old file paths.
 * @param newPath Path to the folder of the new asset.
 * @newFileList List of new file paths.
 * @param mergedPath Path to the folder of the merged asset.
 * @param feedback Callback function for progress and errors.
 */
async function mergeOtherFiles(oldPath: string, oldFileList: string[], newPath: string, newFileList: string[], mergedPath: string, feedback: Feedback): Promise<void> {
    for (let file of oldFileList) {
        if ((file.endsWith('.lu.dialog') || !file.endsWith('.dialog')) && !file.endsWith('.lu') && !file.endsWith('.lg')) {
            let index = file.lastIndexOf('/')
            let fileName = file.substring(index)
            await copySingleFile(oldPath, mergedPath, fileName, oldFileList, feedback)
        }
    }

    for (let file of newFileList) {
        if ((file.endsWith('.lu.dialog') || !file.endsWith('.dialog')) && !file.endsWith('.lu') && !file.endsWith('.lg')) {
            let index = file.lastIndexOf('/')
            let fileName = file.substring(index)
            let files = oldFileList.filter(f => f.match(file))
            if (files.length === 0) {
                await copySingleFile(newPath, mergedPath, fileName, newFileList, feedback)
            }
        }

    }
}

/**
 * @description: Merge root lg file from two assets based on the new and old root files.
 * @param schemaName Name of the .schema file.
 * @param oldPath Path to the folder of the old asset.
 * @param oldFileList List of old file paths.
 * @param newPath Path to the folder of the new asset.
 * @param newFileList List of new file paths.
 * @param mergedPath Path to the folder of the merged asset.
 * @param locale Locale.
 * @param extension .lu or .lg
 * @param oldPropertySet Property Set from the old .schema file.
 * @param newPropertySet Property Set from the new .schema file.
 * @param feedback Callback function for progress and errors.
 */
async function mergeRootFile(schemaName: string, oldPath: string, oldFileList: string[], newPath: string, newFileList: string[], mergedPath: string, locale: string, extension: string, oldPropertySet: Set<string>, newPropertySet: Set<string>, feedback: Feedback): Promise<void> {
    const outDir = assetDirectory(extension)
    let oldText = await fs.readFile(ppath.join(oldPath, outDir, locale, `${schemaName}.${locale}${extension}`), 'utf8')
    let oldRefs = oldText.split(os.EOL)
    let newText = await fs.readFile(ppath.join(newPath, outDir, locale, `${schemaName}.${locale}${extension}`), 'utf8')
    let newRefs = newText.split(os.EOL)

    let resultRefs: string[] = []
    let oldRefSet = new Set<string>()

    for (let ref of oldRefs) {
        if (ref.match('> Generator:')) {
            if (resultRefs.length !== 0 && resultRefs[resultRefs.length - 1] === '') {
                resultRefs.pop()
            }
            break
        }
        if (!ref.startsWith('[')) {
            resultRefs.push(ref)
            continue
        }
        oldRefSet.add(ref)
        let extractedProperty = equalPattern(ref, oldPropertySet, schemaName)
        if (extractedProperty !== undefined) {
            if (newPropertySet.has(extractedProperty)) {
                resultRefs.push(ref)
                let file = refFilename(ref, feedback)
                if (file.match(`${extractedProperty}Value`)) {
                    if (extension === '.lg') {
                        await changeEntityEnumLG(oldPath, oldFileList, newFileList, mergedPath, file, feedback)
                    }
                } else {
                    if (await !isOldUnchanged(oldFileList, file) && await getHashCodeFromFile(oldFileList, file) !== await getHashCodeFromFile(newFileList, file)) {
                        changedMessage(file, feedback)
                    } else {
                        await copySingleFile(oldPath, mergedPath, file, oldFileList, feedback)
                    }
                }
            }
        } else {
            resultRefs.push(ref)
            let file = refFilename(ref, feedback)
            if (newText.match(file) && !await isOldUnchanged(oldFileList, file) && await getHashCodeFromFile(oldFileList, file) !== await getHashCodeFromFile(newFileList, file)) {
                changedMessage(file, feedback)
            } else {
                await copySingleFile(oldPath, mergedPath, file, oldFileList, feedback)
            }
        }
    }

    for (let ref of newRefs) {
        if (!ref.startsWith('[')) {
            continue
        }
        if (!oldRefSet.has(ref)) {
            resultRefs.push(ref)
            let file = refFilename(ref, feedback)
            await copySingleFile(newPath, mergedPath, file, newFileList, feedback)
        }
    }

    let val = resultRefs.join(os.EOL)

    let patternIndex = oldText.search(GeneratorPattern)
    if (patternIndex !== -1) {
        val = val + os.EOL + oldText.substring(patternIndex)
    }

    await writeToFile(oldPath, mergedPath, `${schemaName}.${locale}${extension}`, oldFileList, val, feedback)
}

/**
 * @description: Merge root lu file from two assets based on the new and old root files.
 * @param schemaName Name of the .schema file.
 * @param oldPath Path to the folder of the old asset.
 * @param oldFileList List of old file paths.
 * @param newPath Path to the folder of the new asset.
 * @param newFileList List of new file paths.
 * @param mergedPath Path to the folder of the merged asset.
 * @param locale Locale.
 * @param feedback Callback function for progress and errors.
 */
async function mergeRootLUFile(schemaName: string, oldPath: string, oldFileList: string[], newPath: string, newFileList: string[], mergedPath: string, locale: string, feedback: Feedback): Promise<void> {
    const outDir = assetDirectory('.lu')
    let oldText = await fs.readFile(ppath.join(oldPath, outDir, locale, `${schemaName}.${locale}.lu`), 'utf8')
    let newText = await fs.readFile(ppath.join(newPath, outDir, locale, `${schemaName}.${locale}.lu`), 'utf8')
    let newRefs = newText.split(os.EOL)

    let resultRefs: string[] = []

    for (let ref of newRefs) {
        if (ref.match('> Generator:')) {
            if (resultRefs.length !== 0 && resultRefs[resultRefs.length - 1] === '') {
                resultRefs.pop()
            }
            break
        }
        else if (!ref.startsWith('[')) {
            resultRefs.push(ref)
            continue
        }
        else if (!ref.match('custom')) {
            resultRefs.push(ref)
            let file = refFilename(ref, feedback)
            await copySingleFile(newPath, mergedPath, file, newFileList, feedback)
        }
        else {
            resultRefs.push(ref)
            await updateCustomLUFile(schemaName, oldPath, newPath, oldFileList, mergedPath, locale, feedback)
        }
    }

    let val = resultRefs.join(os.EOL)

    let patternIndex = oldText.search(GeneratorPattern)
    if (patternIndex !== -1) {
        val = val + os.EOL + oldText.substring(patternIndex)
    }

    await writeToFile(oldPath, mergedPath, `${schemaName}.${locale}.lu`, oldFileList, val, feedback)
}

const enumPattern = /\{@([a-zA-Z0-9]+)Value=([a-zA-Z0-9\s]+)\}/g
/**
 * @description: Update custom lu file if the schema has been changed.
 * @param schemaName Name of the .schema file.
 * @param oldPath Path to the folder of the old bot asset.
 * @param newPath Path to the folder of the new bot asset.
 * @param oldFileList List of old file paths.
 * @param mergedPath Path to the folder of the merged asset.
 * @param locale Locale.
 * @param feedback Callback function for progress and errors.
 */
async function updateCustomLUFile(schemaName: string, oldPath: string, newPath: string, oldFileList: string[], mergedPath: string, locale: string, feedback: Feedback): Promise<void> {
    let customLuFilePath = oldFileList.filter(file => file.match(`${schemaName}-custom.${locale}.lu`))[0]
    let text = await fs.readFile(customLuFilePath, 'utf8')
    let lines = text.split(os.EOL)
    let resultLines: string[] = []

    let propertyValueSynonyms = await getSynonyms(schemaName, newPath)
    for (let line of lines) {
        if (line.match(enumPattern)) {
            let newLine = await replaceLine(line, propertyValueSynonyms)
            resultLines.push(newLine)

        } else {
            resultLines.push(line)
        }
    }

    let val = resultLines.join(os.EOL)
    await writeToFile(oldPath, mergedPath, `${schemaName}-custom.${locale}.lu`, oldFileList, val, feedback)
}

/**
 * @description: Replace the non-existing synonym in the utterance with the exisiting one.
 * @param line Example utterance.
 * @param propertyValueSynonyms Map of property value to its synonyms.
 */
async function replaceLine(line: string, propertyValueSynonyms: Map<string, Set<string>>): Promise<string> {
    let matches = line.match(enumPattern)
    if (matches !== undefined && matches !== null) {
        for (let i = 0; i < matches.length; i++) {
            let phrases = matches[i].split('=')
            let key = phrases[0].replace('{@', '')
            let value = phrases[1].replace('}', '')
            if (propertyValueSynonyms.has(key)) {
                let synonymsSet = propertyValueSynonyms.get(key)
                if (synonymsSet !== undefined && !synonymsSet.has(value)) {
                    let items = Array.from(synonymsSet)
                    let randomSynomym = items[Math.floor(Math.random() * items.length)]
                    let replacePattern = `{@${key}=${randomSynomym}}`
                    line = line.replace(matches[i], replacePattern)
                }
            }
        }
    }
    return line
}

/**
 * @description: Merge individual lg files which have the template with SWITCH ENUM.
 * @param oldPath Path to the folder of the old asset.
 * @param oldFileList List of old file paths.
 * @param newFileList List of new file paths.
 * @param mergedPath Path to the folder of the merged asset.
 * @param filename File name of the .lg file.
 * @param feedback Callback function for progress and errors.
 */
async function changeEntityEnumLG(oldPath: string, oldFileList: string[], newFileList: string[], mergedPath: string, filename: string, feedback: Feedback): Promise<void> {
    let oldFilePath = oldFileList.filter(file => file.match(filename))[0]
    let oldText = await fs.readFile(oldFilePath, 'utf8')
    let oldStatements = oldText.split(os.EOL)
    let oldTemplates = Templates.parseText(oldText)

    let newFilePath = newFileList.filter(file => file.match(filename))[0]
    let newText = await fs.readFile(newFilePath, 'utf8')
    let newStatements = newText.split(os.EOL)
    let newTemplates = Templates.parseText(newText)

    let mergedStatements: string[] = []

    let recordPart: object[] = []

    for (let oldTemplate of oldTemplates) {
        let oldBody = oldTemplate.templateBodyParseTree
        if (oldBody === undefined) {
            continue
        }
        if (oldBody instanceof SwitchCaseBodyContext) {
            for (let newTemplate of newTemplates) {
                if (newTemplate.name !== oldTemplate.name) {
                    continue
                }
                let newBody = newTemplate.templateBodyParseTree
                if (newBody instanceof SwitchCaseBodyContext) {
                    let newSwitchStatements: string[] = []
                    let newEnumValueMap = new Map<string, number>()
                    let oldEnumEntitySet = new Set<string>()
                    let newRules = newBody.switchCaseTemplateBody().switchCaseRule()
                    for (let rule of newRules) {
                        let state = rule.switchCaseStat()
                        // get enumEntity and its following statements
                        if (state.text.match('\s*-\s*CASE:')) {
                            let enumEntity = state.text.replace('-CASE:${', '').replace('}', '')
                            let start = state.start.line + newTemplate.sourceRange.range.start.line
                            newEnumValueMap.set(enumEntity, start)
                        }
                    }
                    const {startIndex, endIndex} = parseLGTemplate(oldTemplate, oldBody, oldStatements, newStatements, newEnumValueMap, oldEnumEntitySet, newSwitchStatements)
                    let statementInfo = {
                        start: startIndex, end: endIndex, newSStatements: newSwitchStatements
                    }
                    recordPart.push(statementInfo)
                }
            }
        }
    }

    if (recordPart.length !== 0) {
        let startSplit = 0
        let arrList: [string[]] = [[]]
        for (let obj of recordPart) {
            let arr = oldStatements.slice(startSplit, obj['start'])
            arrList.push(arr)
            arrList.push(obj['newSStatements'])
            startSplit = obj['end']
        }

        if (startSplit !== oldStatements.length) {
            let arr = oldStatements.slice(startSplit)
            arrList.push(arr)
        }

        for (let arr of arrList) {
            mergedStatements = mergedStatements.concat(arr)
        }
        let val = mergedStatements.join(os.EOL)
        await writeToFile(oldPath, mergedPath, filename, oldFileList, val, feedback)
    } else {
        await writeToFile(oldPath, mergedPath, filename, oldFileList, oldText, feedback)
    }
}

/**
 * @description: Update old LG Template which has SWITCH ENUM.
 * @param oldTemplate Template from the old .lg file. 
 * @param oldBody   Body from the old .lg file.
 * @param oldStatements Statement array from the old .lg file.
 * @param newStatements Statement array from the new .lg file.
 * @param newEnumValueMap Map for Enum Entity key-value pair from the new .lg file.
 * @param oldEnumEntitySet Set for Enum Entity from the old .lg file.
 * @param newSwitchStatements Merged switch statement array.
 */
function parseLGTemplate(oldTemplate: any, oldBody: any, oldStatements: string[], newStatements: string[], newEnumValueMap: Map<string, number>, oldEnumEntitySet: Set<string>, newSwitchStatements: string[]): {startIndex: number, endIndex: number} {
    let startIndex = 0
    let endIndex = 0
    let oldRules = oldBody.switchCaseTemplateBody().switchCaseRule()
    for (let rule of oldRules) {
        let state = rule.switchCaseStat()
        if (state.text.match('\s*-\s*SWITCH')) {
            startIndex = state.start.line + oldTemplate.sourceRange.range.start.line - 1
            newSwitchStatements.push(oldStatements[startIndex])
            let i = startIndex + 1
            while (i < oldStatements.length && !oldStatements[i].toLowerCase().match('case') && !oldStatements[i].toLowerCase().match('default')) {
                newSwitchStatements.push(oldStatements[i])
                i++
            }
        } else if (state.text.match('\s*-\s*CASE')) {
            let enumEntity = state.text.replace('-CASE:${', '').replace('}', '')
            oldEnumEntitySet.add(enumEntity)
            if (newEnumValueMap.has(enumEntity)) {
                let k = state.start.line + oldTemplate.sourceRange.range.start.line - 1
                newSwitchStatements.push(oldStatements[k])
                k++
                while (k < oldStatements.length && !oldStatements[k].toLowerCase().match('case') && !oldStatements[k].toLowerCase().match('default')) {
                    newSwitchStatements.push(oldStatements[k])
                    k++
                }
            }
        } else if (state.text.match('\s*-\s*DEFAULT')) {
            for (let [key, value] of newEnumValueMap) {
                if (!oldEnumEntitySet.has(key)) {
                    let k = value - 1
                    newSwitchStatements.push(newStatements[k])
                    k++
                    while (k < newStatements.length && !newStatements[k].toLowerCase().match('case') && !newStatements[k].toLowerCase().match('default')) {
                        newSwitchStatements.push(newStatements[k])
                        k++
                    }

                }
            }
            let m = state.start.line + oldTemplate.sourceRange.range.start.line - 1
            newSwitchStatements.push(oldStatements[m])
            m++
            while (m < oldStatements.length && !oldStatements[m].match('#') && !oldStatements[m].startsWith('[')) {
                newSwitchStatements.push(oldStatements[m])
                m++
            }
            endIndex = m
        }
    }

    return {startIndex, endIndex}
}

/**
 * @description: Merge two main .dialog files following the trigger ordering rule.
 * @param schemaName Name of the .schema file.
 * @param oldPath Path to the folder of the old asset.
 * @param oldFileList List of old file paths.
 * @param newPath Path to the folder of the new asset.
 * @param newFileList List of new file paths.
 * @param mergedPath Path to the folder of the merged asset.
 * @param locale Locale
 * @param oldPropertySet Property Set from the old .schema file.
 * @param newPropertySet Property Set from the new .schema file.
 * @param feedback Callback function for progress and errors.
 */
async function mergeDialogs(schemaName: string, oldPath: string, oldFileList: string[], newPath: string, newFileList: string[], mergedPath: string, locale: string, oldPropertySet: Set<string>, newPropertySet: Set<string>, feedback: Feedback): Promise<void> {
    let template = await fs.readFile(ppath.join(oldPath, schemaName + '.dialog')
        , 'utf8')
    let oldObj = JSON.parse(template)
    template = await fs.readFile(ppath.join(newPath, schemaName + '.dialog')
        , 'utf8')
    let newObj = JSON.parse(template)

    let newTriggers: string[] = []
    let newTriggerMap = new Map<string, any>()

    // remove triggers whose property does not exist in new property set
    let reducedOldTriggers: string[] = []
    let reducedOldTriggerMap = new Map<string, any>()

    let mergedTriggers: any[] = []

    for (let trigger of oldObj['triggers']) {
        let triggerName = getTriggerName(trigger)
        let extractedProperty = equalPattern(triggerName, oldPropertySet, schemaName)
        if (extractedProperty !== undefined) {
            if (newPropertySet.has(extractedProperty)) {
                reducedOldTriggers.push(triggerName)
                reducedOldTriggerMap.set(triggerName, trigger)
            }
        } else {
            reducedOldTriggers.push(triggerName)
            reducedOldTriggerMap.set(triggerName, trigger)
        }
    }

    for (let trigger of newObj['triggers']) {
        let triggerName = getTriggerName(trigger)
        let extractedProperty = equalPattern(triggerName, oldPropertySet, schemaName)
        if (extractedProperty !== undefined && !reducedOldTriggerMap.has(triggerName)) {
            continue
        }
        newTriggers.push(triggerName)
        newTriggerMap.set(triggerName, trigger)
    }

    let i = 0
    while (!reducedOldTriggerMap.has(newTriggers[i]) && i < newTriggers.length) {
        let resultMergedTrigger = newTriggerMap.get(newTriggers[i])
        mergedTriggers.push(resultMergedTrigger)
        if (typeof resultMergedTrigger === 'string') {
            await copySingleFile(newPath, mergedPath, newTriggers[i] + '.dialog', newFileList, feedback)
        }
        i++
    }

    let j = 0

    while (j < reducedOldTriggers.length) {
        let resultReducedOldTrigger = reducedOldTriggerMap.get(reducedOldTriggers[j])
        mergedTriggers.push(resultReducedOldTrigger)
        if (typeof resultReducedOldTrigger === 'string') {
            if (newTriggers.includes(reducedOldTriggers[j]) && !await isOldUnchanged(oldFileList, reducedOldTriggers[j] + '.dialog') && await getHashCodeFromFile(oldFileList, reducedOldTriggers[j] + '.dialog') !== await getHashCodeFromFile(newFileList, reducedOldTriggers[j] + '.dialog')) {
                changedMessage(reducedOldTriggers[j] + '.dialog', feedback)
            } else {
                await copySingleFile(oldPath, mergedPath, reducedOldTriggers[j] + '.dialog', oldFileList, feedback)
            }
        }
        let index = newTriggers.indexOf(reducedOldTriggers[j])
        if (index !== -1) {
            index++
            while (index < newTriggers.length && !reducedOldTriggerMap.has(newTriggers[index])) {
                let resultMergedTrigger = newTriggerMap.get(newTriggers[index])
                mergedTriggers.push(resultMergedTrigger)
                if (typeof resultMergedTrigger === 'string') {
                    await copySingleFile(newPath, mergedPath, newTriggers[index] + '.dialog', newFileList, feedback)
                }
                index++
            }
        }
        j++
    }

    oldObj['triggers'] = mergedTriggers
    await writeToFile(oldPath, mergedPath, schemaName + '.dialog', oldFileList, stringify(oldObj), feedback)
    await copySingleFile(newPath, mergedPath, schemaName + '.' + locale + '.lu.dialog', newFileList, feedback)
}

/**
 * @description: Get the trigger name
 * @param trigger trigger from main.dialog file
 */
function getTriggerName(trigger: any): string {
    let triggerName: string
    if (typeof trigger !== 'string') {
        triggerName = trigger['$source']
    } else {
        triggerName = trigger
    }
    return triggerName
}

/**
 * @description: Compare the filename pattern for .lu file.
 * @param filename File name of .lu, .lg, or .dialog file.
 * @param propertySet Property set from the .schema file.
 * @param schemaName Name of the .schema file.
 */
function equalPattern(filename: string, propertySet: Set<string>, schemaName: string): string | undefined {
    let result: string | undefined

    for (let property of propertySet) {
        let pattern1 = schemaName + '-' + property + '-'
        let pattern2 = schemaName + '-' + property + 'Value'
        let pattern3 = schemaName + '-' + property + '.'
        if (filename.match(pattern1) || filename.match(pattern2) || filename.match(pattern3)) {
            result = property
            break
        }
    }
    return result
}

/**
 * @description: Get the old property set and new property set from schema files.
 * @param schemaName Name of the .schema file.
 * @param oldPath Path to the folder of the old asset.
 * @param newPath Path to the folder of the new asset.
 * @param newFileList List of new file paths.
 * @param mergedPath Path to the folder of the merged asset.
 * @param feedback Callback function for progress and errors.
 */
async function parseSchemas(schemaName: string, oldPath: string, newPath: string, newFileList: string[], mergedPath: string, feedback: Feedback): Promise<{oldPropertySet: Set<string>, newPropertySet: Set<string>}> {
    let oldPropertySet = new Set<string>()
    let newPropertySet = new Set<string>()

    let template = await fs.readFile(ppath.join(oldPath, schemaName + '.json'), 'utf8')
    let oldObj = JSON.parse(template)

    template = await fs.readFile(ppath.join(newPath, schemaName + '.json'), 'utf8')
    let newObj = JSON.parse(template)

    for (let property in oldObj['properties']) {
        oldPropertySet.add(property)
    }
    for (let property in newObj['properties']) {
        newPropertySet.add(property)
    }

    await copySingleFile(newPath, mergedPath, schemaName + '.json', newFileList, feedback)
    return {oldPropertySet, newPropertySet}
}

/**
 * @description: Get the synonyms of the enum entity.
 * @param schemaName Name of the .schema file.
 * @param newPath Path to the folder of the new asset.
 */
async function getSynonyms(schemaName: string, newPath: string): Promise<Map<string, Set<string>>> {
    let template = await fs.readFile(ppath.join(newPath, schemaName + '.json'), 'utf8')
    let newObj = JSON.parse(template)
    let propertyValueSynonyms = new Map<string, Set<string>>()

    for (let property in newObj['properties']) {
        let examples = newObj['properties'][property]['$examples']
        if (examples !== undefined && examples[''][`${property}Value`] !== undefined) {
            let synonymsSet = new Set<string>()
            for (let enumEntity in examples[''][`${property}Value`]) {
                synonymsSet.add(enumEntity)
                for (let synonym in examples[''][`${property}Value`][enumEntity]) {
                    synonymsSet.add(synonym)
                }
            }
            propertyValueSynonyms.set(`${property}Value`, synonymsSet)
        }
    }

    return propertyValueSynonyms
}
/*!
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

import * as fs from 'fs-extra';
import * as ppath from 'path';
import * as os from 'os';
import { Feedback, FeedbackType, isUnchanged, writeFile, stringify } from './dialogGenerator'

const { Templates, SwitchCaseBodyContext } = require('botbuilder-lg');
const LUParser = require('@microsoft/bf-lu/lib/parser/lufile/luParser');
const sectionOperator = require('@microsoft/bf-lu/lib/parser/lufile/sectionOperator');
const lusectiontypes = require('@microsoft/bf-lu/lib/parser/utils/enums/lusectiontypes')

const GeneratorPattern = /\r?\n> Generator: ([a-zA-Z0-9]+)/

/**
 * @description：Detect if the old file was not changed.
 * @param oldPath Path to the folder of the old asset.
 * @param fileName File name of the .lu, .lg, .dialog and .qna file.
 */
async function isOldUnchanged(oldPath: string, fileName: string): Promise<boolean> {
    return isUnchanged(ppath.join(oldPath, fileName))
}

/**
 * @description：Copy the single file including .lu .lg and .dialog.
 * @param sourcePath Path to the folder where the file is copied from.
 * @param destPath Path to the folder where the file is copied to.
 * @param fileName File name of the .lu, .lg, .dialog and .qna file.
 * @param feedback Callback function for progress and errors.
 */
async function copySingleFile(sourcePath: string, destPath: string, fileName: string, feedback: Feedback): Promise<void> {
    await fs.copyFile(ppath.join(sourcePath, fileName), ppath.join(destPath, fileName))
    feedback(FeedbackType.info, `Copying ${fileName} from ${sourcePath}`)
}

function changedMessage(path: string, fileName: string, feedback: Feedback) {
    feedback(FeedbackType.info, `*** Old and new both changed, manually merge from ${ppath.join(path, fileName)} ***`)
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
 * @param locale Locale.
 * @param feedback Callback function for progress and errors.
 *
 */
export async function mergeAssets(schemaName: string, oldPath: string, newPath: string, mergedPath: string, locales: string[], feedback?: Feedback): Promise<boolean> {
    if (!feedback) {
        feedback = (_info, _message) => true
    }

    if (oldPath === mergedPath) {
        let tempOldPath = `${os.tmpdir}/tempOld/`
        await fs.emptyDir(tempOldPath)
        await fs.copy(oldPath, tempOldPath)
        await fs.emptyDir(oldPath)
        oldPath = tempOldPath
    }

    try {
        for (let locale of locales) {

            await fs.ensureDir(ppath.join(mergedPath, locale))
            feedback(FeedbackType.message, `Create output dir : ${mergedPath} `)

            const { oldPropertySet, newPropertySet } = await parseSchemas(schemaName, oldPath, newPath, mergedPath, feedback)

            await mergeDialogs(schemaName, oldPath, newPath, mergedPath, locale, oldPropertySet, newPropertySet, feedback)
            await mergeLUFiles(schemaName, oldPath, newPath, mergedPath, locale, oldPropertySet, newPropertySet, feedback)
            await mergeLGFiles(schemaName, oldPath, newPath, mergedPath, locale, oldPropertySet, newPropertySet, feedback)
            await mergeOtherFiles(oldPath, newPath, mergedPath, locale, feedback)
        }
    } catch (e) {
        feedback(FeedbackType.error, e.message)
    }

    return true
}

/**
 * @description: Merge other types of files, e.g qna files.
 * @param oldPath Path to the folder of the old asset.
 * @param newPath Path to the folder of the new asset.
 * @param mergedPath Path to the folder of the merged asset.
 * @param locale Locale.
 * @param feedback Callback function for progress and errors.
 */
async function mergeOtherFiles(oldPath: string, newPath: string, mergedPath: string, locale: string, feedback: Feedback): Promise<void> {
    let dirList = [oldPath, oldPath + '/' + locale]
    let fileSet = new Set<string>()

    for (let dir of dirList) {
        let tempDir = mergedPath
        let compareDir = newPath
        if (dir.endsWith(locale)) {
            tempDir = mergedPath + '/' + locale
            compareDir = newPath + '/' + locale
        }
        let files = await fs.readdir(dir)
        for (let file of files) {
            let stat = fs.lstatSync(ppath.join(dir, file))
            if (stat.isFile()) {
                if (!file.endsWith('.dialog') && !file.endsWith('.lu') && !file.endsWith('.lg')) {
                    let newFile = ppath.join(compareDir, file)
                    if (fs.existsSync(newFile)) {
                        await copySingleFile(dir, tempDir, file, feedback)
                    }
                    fileSet.add(file)
                }
            }
        }
    }

    let newDirList = [newPath, newPath + '/' + locale]
    for (let dir of newDirList) {
        let tempDir = mergedPath
        if (dir.endsWith(locale)) {
            tempDir = mergedPath + '/' + locale
        }
        let files = await fs.readdir(dir)
        for (let file of files) {
            let stat = fs.lstatSync(ppath.join(dir, file))
            if (stat.isFile()) {
                if (!file.endsWith('.dialog') && !file.endsWith('.lu') && !file.endsWith('.lg') && !fileSet.has(file)) {
                    await copySingleFile(dir, tempDir, file, feedback)
                }
            }
        }
    }
}

/**
 * @description: Merge lu files from two assets based on the new and old root lu files.
 * @param schemaName Name of the .schema file.
 * @param oldPath Path to the folder of the old asset.
 * @param newPath Path to the folder of the new asset.
 * @param mergedPath Path to the folder of the merged asset.
 * @param locale Locale.
 * @param oldPropertySet Property Set from the old .schema file.
 * @param newPropertySet Property Set from the new .schema file.
 * @param feedback Callback function for progress and errors.
 */
async function mergeLUFiles(schemaName: string, oldPath: string, newPath: string, mergedPath: string, locale: string, oldPropertySet: Set<string>, newPropertySet: Set<string>, feedback: Feedback): Promise<void> {
    let oldText = await fs.readFile(ppath.join(oldPath, locale, schemaName + '.' + locale + '.lu'), 'utf8')
    let oldRefs = oldText.split(os.EOL)
    let newText = await fs.readFile(ppath.join(newPath, locale, schemaName + '.' + locale + '.lu'), 'utf8')
    let newRefs = newText.split(os.EOL)

    let localeOldPath = ppath.join(oldPath, locale)
    let localeNewPath = ppath.join(newPath, locale)
    let localeMergedPath = ppath.join(mergedPath, locale)

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
                let refStr = ref.split('.lu')
                let luFile = refStr[0].replace('[', '') + '.lu'
                if (luFile.match(extractedProperty + 'Entity')) {
                    await changeEntityEnumLU(schemaName, oldPath, newPath, mergedPath, luFile, locale, feedback)
                } else {
                    if (await isOldUnchanged(localeOldPath, luFile)) {
                        await copySingleFile(localeOldPath, localeMergedPath, luFile, feedback)
                    } else {
                        changedMessage(localeOldPath, luFile, feedback)
                    }
                }
            }
        } else {
            resultRefs.push(ref)
            let refStr = ref.split('.lu')
            let luFile = refStr[0].replace('[', '') + '.lu'
            if (newText.match(luFile) && !await isOldUnchanged(localeOldPath, luFile)) {
                changedMessage(localeOldPath, luFile, feedback)
            } else {
                await copySingleFile(localeOldPath, localeMergedPath, luFile, feedback)
            }
        }
    }

    for (let ref of newRefs) {
        if (!ref.startsWith('[')) {
            continue
        }
        if (!oldRefSet.has(ref)) {
            resultRefs.push(ref)
            let refStr = ref.split('.lu')
            let luFile = refStr[0].replace('[', '') + '.lu'
            await copySingleFile(localeNewPath, localeMergedPath, luFile, feedback)
        }
    }

    let val = resultRefs.join(os.EOL)

    let patternIndex = oldText.search(GeneratorPattern)
    if (patternIndex !== -1) {
        val = val + os.EOL + oldText.substring(patternIndex)
    }

    await writeFile(ppath.join(mergedPath, locale, schemaName + '.' + locale + '.lu'), val, feedback, true)
    feedback(FeedbackType.info, `Merging ${schemaName}.${locale}.lu`)
}

/**
 * @description: Merge individual lu files which have List Entity Section.
 * @param schemaName Schema Name
 * @param oldPath Path to the folder of the old asset.
 * @param newPath Path to the folder of the new asset.
 * @param mergedPath Path to the folder of the merged asset.
 * @param filename File name of .lu file.
 * @param locale Locale.
 * @param feedback Callback function for progress and errors.
 */
async function changeEntityEnumLU(schemaName: string, oldPath: string, newPath: string, mergedPath: string, filename: string, locale: string, feedback: Feedback): Promise<void> {
    let text = await fs.readFile(ppath.join(newPath, locale, filename), 'utf8')
    let newLUResource = LUParser.parse(text)
    let newEntitySections = newLUResource.Sections.filter(s => s.SectionType === lusectiontypes.NEWENTITYSECTION)

    text = await fs.readFile(ppath.join(oldPath, locale, filename), 'utf8')
    let oldLUResource = LUParser.parse(text)
    let oldEntitySections = oldLUResource.Sections.filter(s => s.SectionType === lusectiontypes.NEWENTITYSECTION)
    let oldIntentSections = oldLUResource.Sections.filter(s => s.SectionType === lusectiontypes.SIMPLEINTENTSECTION && s.Name === schemaName)

    let odlSectionOp = new sectionOperator(oldLUResource)
    let updatedLUResource: any = null

    let oldListEntitySections = oldEntitySections.filter(s => s.Type === 'list')
    for (let oldListEntitySection of oldListEntitySections) {
        if (!oldListEntitySection.Name.match('Entity')) {
            continue
        }

        for (let newEntitySection of newEntitySections) {
            if (newEntitySection.Name !== oldListEntitySection.Name) {
                continue
            }

            let keepEnumValue = new Set<string>()
            let deletedEnumValue = new Set<string>()

            let enumValueMap = new Map<string, string[]>()
            let enumSet = new Set<string>()
            let resultStatements: string[] = []

            //get new enum value set
            for (let i = 0; i < newEntitySection.ListBody.length; i++) {
                // if the string has : (e.g., - multiGrainWheat :), parse it as an enum entity
                if (newEntitySection.ListBody[i].match(':')) {
                    let enumEntity = newEntitySection.ListBody[i].replace('-', '').replace(':', '').trim()
                    // add all statements following current enum entity
                    let temp: string[] = []
                    let j = i + 1
                    while (j < newEntitySection.ListBody.length) {
                        if (!newEntitySection.ListBody[j].match(':')) {
                            temp.push(newEntitySection.ListBody[j])
                            j++
                            if (j === newEntitySection.ListBody.length) {
                                enumValueMap.set(enumEntity, temp)
                            }
                        } else {
                            enumValueMap.set(enumEntity, temp)
                            i = j - 1
                            break
                        }
                    }
                }
            }

            //parse old lu entity list and delete the enum entity which does not exist in new lu file
            for (let i = 0; i < oldListEntitySection.ListBody.length; i++) {
                // if the string has : (e.g., - multiGrainWheat :), parse it as an enum entity
                if (oldListEntitySection.ListBody[i].match(':')) {
                    let enumEntity = oldListEntitySection.ListBody[i].replace('-', '').replace(':', '').trim()
                    enumSet.add(enumEntity)
                    if (enumValueMap.has(enumEntity)) {
                        resultStatements.push(oldListEntitySection.ListBody[i])
                    }
                    let j = i + 1
                    while (j < oldListEntitySection.ListBody.length) {
                        if (!oldListEntitySection.ListBody[j].match(':')) {
                            let enumSyn = oldListEntitySection.ListBody[j].replace('-', '').trim()
                            if (enumValueMap.has(enumEntity)) {
                                resultStatements.push(oldListEntitySection.ListBody[j])
                                keepEnumValue.add(enumSyn)
                            } else {
                                deletedEnumValue.add(enumSyn)
                            }
                            j++
                        } else {
                            i = j - 1
                            break
                        }
                    }
                }
            }

            // add  new enum entity in the new  lu file 
            for (let [key, values] of enumValueMap) {
                if (!enumSet.has(key)) {
                    resultStatements.push('\t- ' + key + ' :')
                    for (let newStatement of values) {
                        resultStatements.push(newStatement)
                    }
                }
            }

            // update content 
            let entityLUContent = resultStatements.join(os.EOL)
            let entityLUName = '@ ' + oldListEntitySection.Type + ' ' + oldListEntitySection.Name + ' ='
            let sectionBody = entityLUName + os.EOL + entityLUContent
            updatedLUResource = odlSectionOp.updateSection(oldListEntitySection.Id, sectionBody)

            // update intent content
            if (oldIntentSections.length === 0) {
                continue
            }

            let oldIntentSection = oldIntentSections[0]
            let removedEnumValue = new Set<string>()
            for (let enumSyn of deletedEnumValue) {
                if (!keepEnumValue.has(enumSyn)) {
                    removedEnumValue.add(enumSyn)
                }
            }

            let intentBodyStatements = oldIntentSection.Body.split(os.EOL)
            let intentResult: string[] = []
            for (let intentBodyStatement of intentBodyStatements) {
                let matching = false
                for (let enumSyn of removedEnumValue) {
                    if (intentBodyStatement.match(enumSyn)) {
                        matching = true
                        break
                    }
                }
                if (!matching) {
                    intentResult.push(intentBodyStatement)
                }
            }

            let intentSectionBody = '# ' + schemaName + os.EOL + intentResult.join(os.EOL)
            let updateSectionOp = new sectionOperator(updatedLUResource)
            updatedLUResource = updateSectionOp.updateSection(oldIntentSection.Id, intentSectionBody)
        }
    }
    let content = (updatedLUResource || oldLUResource).Content
    await writeFile(ppath.join(mergedPath, locale, filename), content, feedback, true)
    feedback(FeedbackType.info, `Merging ${filename}`)
}

/**
 * @description: Merge lg files of two assets based on the new and old root lg files.
 * @param schemaName Name of the .schema file.
 * @param oldPath Path to the folder of the old asset.
 * @param newPath Path to the folder of the new asset.
 * @param mergedPath Path to the folder of the merged asset.
 * @param locale Locale.
 * @param oldPropertySet Property Set from the old .schema file.
 * @param newPropertySet Property Set from the new .schema file.
 * @param feedback Callback function for progress and errors.
 */
async function mergeLGFiles(schemaName: string, oldPath: string, newPath: string, mergedPath: string, locale: string, oldPropertySet: Set<string>, newPropertySet: Set<string>, feedback: Feedback): Promise<void> {
    let oldText = await fs.readFile(ppath.join(oldPath, locale, schemaName + '.' + locale + '.lg'), 'utf8')
    let oldRefs = oldText.split(os.EOL)
    let newText = await fs.readFile(ppath.join(newPath, locale, schemaName + '.' + locale + '.lg'), 'utf8')
    let newRefs = newText.split(os.EOL)

    let localeOldPath = ppath.join(oldPath, locale)
    let localeNewPath = ppath.join(newPath, locale)
    let localeMergedPath = ppath.join(mergedPath, locale)

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
                let refStr = ref.split('.lg')
                let lgFile = refStr[0].replace('[', '') + '.lg'
                if (lgFile.match(extractedProperty + 'Entity')) {
                    await changeEntityEnumLG(oldPath, newPath, mergedPath, lgFile, locale, feedback)
                } else {
                    if (await isOldUnchanged(localeOldPath, lgFile)) {
                        await copySingleFile(localeOldPath, localeMergedPath, lgFile, feedback)
                    } else {
                        changedMessage(localeOldPath, lgFile, feedback)
                    }
                }
            }
        } else {
            resultRefs.push(ref)
            let refStr = ref.split('.lg')
            let lgFile = refStr[0].replace('[', '') + '.lg'
            if (newText.match(lgFile) && !await isOldUnchanged(localeOldPath, lgFile)) {
                changedMessage(localeOldPath, lgFile, feedback)
            } else {
                await copySingleFile(localeOldPath, localeMergedPath, lgFile, feedback)
            }
        }
    }

    for (let ref of newRefs) {
        if (!ref.startsWith('[')) {
            continue
        }
        if (!oldRefSet.has(ref)) {
            resultRefs.push(ref)
            let refStr = ref.split('.lg')
            let lgFile = refStr[0].replace('[', '') + '.lg'
            await copySingleFile(localeNewPath, localeMergedPath, lgFile, feedback)
        }
    }

    let val = resultRefs.join(os.EOL)

    let patternIndex = oldText.search(GeneratorPattern)
    if (patternIndex !== -1) {
        val = val + os.EOL + oldText.substring(patternIndex)
    }

    await writeFile(ppath.join(mergedPath, locale, schemaName + '.' + locale + '.lg'), val, feedback, true)
    feedback(FeedbackType.info, `Merging ${schemaName}.${locale}.lg`)
}

/**
 * @description: Merge individual lg files which have the template with SWITCH ENUM.
 * @param oldPath Path to the folder of the old asset.
 * @param newPath Path to the folder of the new asset.
 * @param mergedPath Path to the folder of the merged asset.
 * @param filename File name of the .lg file.
 * @param locale Locale.
 * @param feedback Callback function for progress and errors.
 */
async function changeEntityEnumLG(oldPath: string, newPath: string, mergedPath: string, filename: string, locale: string, feedback: Feedback): Promise<void> {
    let oldText = await fs.readFile(ppath.join(oldPath, locale, filename), 'utf8')
    let oldStatements = oldText.split(os.EOL)
    let oldTemplates = Templates.parseText(oldText)

    let newText = await fs.readFile(ppath.join(newPath, locale, filename), 'utf8')
    let newStatements = newText.split(os.EOL)
    let newTemplates = Templates.parseText(newText)

    let mergedStatements: string[] = []

    let recordPart: object[] = []

    for (let oldTemplate of oldTemplates) {
        let oldBody = oldTemplate.parseTree.templateBody()
        if (oldBody === undefined) {
            continue
        }
        if (oldBody instanceof SwitchCaseBodyContext) {
            for (let newTemplate of newTemplates) {
                if (newTemplate.name !== oldTemplate.name) {
                    continue
                }
                let newBody = newTemplate.parseTree.templateBody()
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
                            let start = state.start.line
                            newEnumValueMap.set(enumEntity, start)
                        }
                    }
                    const { startIndex, endIndex } = parseLGTemplate(oldBody, oldStatements, newStatements, newEnumValueMap, oldEnumEntitySet, newSwitchStatements)
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
        await writeFile(ppath.join(mergedPath, locale, filename), val, feedback, true)
        feedback(FeedbackType.info, `Merging ${filename}`)
    } else {
        await writeFile(ppath.join(mergedPath, locale, filename), oldText, feedback, true)
        feedback(FeedbackType.info, `Merging ${filename}`)
    }
}

/**
 * @description: Update old LG Template which has SWITCH ENUM.
 * @param oldBody  Template Body from the old .lg file.
 * @param oldStatements Statement array from the old .lg file.
 * @param newStatements Statement array from the new .lg file.
 * @param newEnumValueMap Map for Enum Entity key-value pair from the new .lg file.
 * @param oldEnumEntitySet Set for Enum Entity from the old .lg file.
 * @param newSwitchStatements Merged switch statement array.
 */
function parseLGTemplate(oldBody: any, oldStatements: string[], newStatements: string[], newEnumValueMap: Map<string, number>, oldEnumEntitySet: Set<string>, newSwitchStatements: string[]): { startIndex: number, endIndex: number } {
    let startIndex = 0
    let endIndex = 0
    let oldRules = oldBody.switchCaseTemplateBody().switchCaseRule()
    for (let rule of oldRules) {
        let state = rule.switchCaseStat()
        if (state.text.match('\s*-\s*SWITCH')) {
            startIndex = state.start.line - 1;
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
                let k = state.start.line - 1
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
            let m = state.start.line - 1
            newSwitchStatements.push(oldStatements[m])
            m++
            while (m < oldStatements.length && !oldStatements[m].match('#') && !oldStatements[m].startsWith('[')) {
                newSwitchStatements.push(oldStatements[m])
                m++
            }
            endIndex = m
        }
    }

    return { startIndex, endIndex }
}

/**
 * @description: Merge two .main.dialog files following the trigger ordering rule.
 * @param schemaName Name of the .schema file.
 * @param oldPath Path to the folder of the old asset.
 * @param newPath Path to the folder of the new asset.
 * @param mergedPath Path to the folder of the merged asset.
 * @param oldPropertySet Property Set from the old .schema file.
 * @param newPropertySet Property Set from the new .schema file.
 * @param feedback Callback function for progress and errors.
 */
async function mergeDialogs(schemaName: string, oldPath: string, newPath: string, mergedPath: string, locale: string, oldPropertySet: Set<string>, newPropertySet: Set<string>, feedback: Feedback): Promise<void> {
    let template = await fs.readFile(ppath.join(oldPath, schemaName + '.main.dialog')
        , 'utf8')
    let oldObj = JSON.parse(template)
    template = await fs.readFile(ppath.join(newPath, schemaName + '.main.dialog')
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
        if (extractedProperty !== undefined && !reducedOldTriggerMap.has(trigger)) {
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
            await copySingleFile(newPath, mergedPath, newTriggers[i] + '.dialog', feedback)
        }
        i++
    }

    let j = 0

    while (j < reducedOldTriggers.length) {
        let resultReducedOldTrigger = reducedOldTriggerMap.get(reducedOldTriggers[j])
        mergedTriggers.push(resultReducedOldTrigger)
        if (typeof resultReducedOldTrigger === 'string') {
            if (newTriggers.includes(reducedOldTriggers[j]) && !await isOldUnchanged(oldPath, reducedOldTriggers[j] + '.dialog')) {
                changedMessage(oldPath, reducedOldTriggers[j] + '.dialog', feedback)
            } else {
                await copySingleFile(oldPath, mergedPath, reducedOldTriggers[j] + '.dialog', feedback)
            }
        }
        let index = newTriggers.indexOf(reducedOldTriggers[j])
        if (index !== -1) {
            index++
            while (index < newTriggers.length && !reducedOldTriggerMap.has(newTriggers[index])) {
                let resultMergedTrigger = newTriggerMap.get(newTriggers[index])
                mergedTriggers.push(resultMergedTrigger)
                if (typeof resultMergedTrigger === 'string') {
                    await copySingleFile(newPath, mergedPath, newTriggers[index] + '.dialog', feedback)
                }
                index++
            }
        }
        j++
    }

    oldObj['triggers'] = mergedTriggers
    await writeFile(ppath.join(mergedPath, schemaName + '.main.dialog'), stringify(oldObj), feedback, true)
    feedback(FeedbackType.info, `Merging ${schemaName}.main.dialog`)

    await copySingleFile(newPath, mergedPath, schemaName + '.' + locale + '.lu.dialog', feedback)
}

/**
 * @description: Get the trigger name
 * @param trigger trigger from main.dialog file
 */
function getTriggerName(trigger: any): string {
    let triggerName: string
    if (typeof trigger !== 'string') {
        triggerName = trigger['id']
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
        let pattern2 = schemaName + '-' + property + 'Entity'
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
 * @param mergedPath Path to the folder of the merged asset.
 * @param feedback Callback function for progress and errors.
 */
async function parseSchemas(schemaName: string, oldPath: string, newPath: string, mergedPath: string, feedback: Feedback): Promise<{ oldPropertySet: Set<string>, newPropertySet: Set<string> }> {
    let oldPropertySet = new Set<string>()
    let newPropertySet = new Set<string>()

    let template = await fs.readFile(ppath.join(oldPath, schemaName + '.schema.dialog'), 'utf8')
    let oldObj = JSON.parse(template)

    template = await fs.readFile(ppath.join(newPath, schemaName + '.schema.dialog'), 'utf8')
    let newObj = JSON.parse(template)

    for (let property in oldObj['properties']) {
        oldPropertySet.add(property)
    }
    for (let property in newObj['properties']) {
        newPropertySet.add(property)
    }

    await copySingleFile(newPath, mergedPath, schemaName + '.schema.dialog', feedback)
    return { oldPropertySet, newPropertySet }
}

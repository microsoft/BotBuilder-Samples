/*!
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

import * as fs from 'fs-extra';
import * as ppath from 'path';
import * as os from 'os';

import * as gen from './dialogGenerator'

const { Templates, SwitchCaseBodyContext } = require('botbuilder-lg');
const LUParser = require('@microsoft/bf-lu/lib/parser/lufile/luParser');
const sectionOperator = require('@microsoft/bf-lu/lib/parser/lufile/sectionOperator');
const lusectiontypes = require('@microsoft/bf-lu/lib/parser/utils/enums/lusectiontypes')

export enum FeedbackType {
    message,
    info,
    warning,
    error
}

export type Feedback = (type: FeedbackType, message: string) => void

const GeneratorPattern = /\r?\n> Generator: (.*)/m
const commentHash = ['.lg', '.lu', '.qna']
const jsonHash = ['.dialog']

/**
 * @description：Detect if two files have the same content by hashcode
 * @param oldPath 
 * @param newPath
 * @param fileName
 */
function isUnchanged(oldPath: string, newPath: string, fileName: string): boolean {
    let result = false
    let ext = ppath.extname(fileName)
    let oldFile = fs.readFileSync(ppath.join(oldPath, fileName), 'utf8')
    let newFile = fs.readFileSync(ppath.join(newPath, fileName), 'utf8')

    if (commentHash.includes(ext)) {
        let matchOld = oldFile.match(GeneratorPattern)
        let matchNew = newFile.match(GeneratorPattern)
        if (matchOld && matchNew) {
            let oldHash = matchOld[1]
            let newHash = matchNew[1]
            result = oldHash === newHash
        }
    } else if (jsonHash.includes(ext)) {
        let jsonOld = JSON.parse(oldFile)
        let oldHash = jsonOld.$Generator
        let jsonNew = JSON.parse(newFile)
        let newHash = jsonNew.$Generator
        if (oldHash && newHash) {
            result = oldHash === newHash
        }
    }
    return result
}

/**
 * @description：Copy the single file including .lu .lg and .dialog 
 * @param sourcePath 
 * @param destPath
 * @param fileName
 * @param feedback
 */
async function copySingleFile(sourcePath: string, destPath: string, fileName: string, feedback: Feedback): Promise<void> {
    fs.copyFile(ppath.join(sourcePath, fileName), ppath.join(destPath, fileName))
    feedback(FeedbackType.info, `Copying ${fileName} from ${sourcePath}`)
}

/**
 * @description: Integrate two bot assets to generate one merged bot asset
 * @param schemaName 
 * @param oldPath
 * @param newPath
 * @param mergedPath
 * @param locale
 * @param feedback
 */
export async function integrateAssets(schemaName: string, oldPath: string, newPath: string, mergedPath: string, locale: string, feedback?: Feedback): Promise<boolean> {
    if (!feedback) {
        feedback = (_info, _message) => true
    }

    try {
        fs.ensureDir(ppath.join(mergedPath, locale))
        fs.ensureDir(ppath.join(mergedPath, 'luis'))
        feedback(FeedbackType.message, `Create output dir : ${mergedPath} `)

        const { oldPropertySet, newPropertySet } = await parseSchemas(schemaName, oldPath, newPath, mergedPath, feedback)

        await mergeDialogs(schemaName, oldPath, newPath, mergedPath, oldPropertySet, newPropertySet, feedback)
        await mergeLUFiles(schemaName, oldPath, newPath, mergedPath, locale, oldPropertySet, newPropertySet, feedback)
        await mergeLGFiles(schemaName, oldPath, newPath, mergedPath, locale, oldPropertySet, newPropertySet, feedback)
        await mergeOtherFiles(oldPath, newPath, mergedPath, locale, feedback)
    } catch (e) {
        feedback(FeedbackType.error, e.message)
    }

    return true
}

/**
 * @description: merge other types of files, e.g qna files
 * @param oldPath
 * @param newPath
 * @param mergedPath
 * @param locale
 * @param feedback
 */
async function mergeOtherFiles(oldPath: string, newPath: string, mergedPath: string, locale: string, feedback: Feedback): Promise<void> {
    let dirList = [oldPath, oldPath + '/luis', oldPath + '/' + locale]
    let fileSet = new Set<string>()

    for (let dir of dirList) {
        let tempDir = mergedPath
        let compareDir = newPath
        if (dir.endsWith('luis')) {
            tempDir = mergedPath + '/luis'
            compareDir = newPath + '/luis'
        } else if (dir.endsWith(locale)) {
            tempDir = mergedPath + '/' + locale
            compareDir = newPath + '/' + locale
        }
        let files = await fs.readdir(dir)
        for (let file of files) {
            let stat = fs.lstatSync(ppath.join(dir, file))
            if (stat.isFile()) {
                if (!file.endsWith('.dialog') && !file.endsWith('.lu') && !file.endsWith('.lg')) {
                    if (fs.existsSync(ppath.join(compareDir, file)) && !isUnchanged(dir, compareDir, file)) {
                        feedback(FeedbackType.info, `***** Old and new ${file} are changed, please manually merge the resource `)
                    } else {
                        copySingleFile(dir, tempDir, file, feedback)
                    }
                    fileSet.add(file)
                }
            }
        }
    }

    let newDirList = [newPath, newPath + '/luis', newPath + '/' + locale]
    for (let dir of newDirList) {
        let tempDir = mergedPath
        if (dir.endsWith('luis')) {
            tempDir = mergedPath + '/luis'
        } else if (dir.endsWith(locale)) {
            tempDir = mergedPath + '/' + locale
        }
        let files = await fs.readdir(dir)
        for (let file of files) {
            let stat = fs.lstatSync(ppath.join(dir, file))
            if (stat.isFile()) {
                if (!file.endsWith('.dialog') && !file.endsWith('.lu') && !file.endsWith('.lg') && !fileSet.has(file)) {
                    copySingleFile(dir, tempDir, file, feedback)
                }
            }
        }
    }
}

/**
 * @description: Merge lu files from two assets based on the new and old root lu files
 * @param schemaName 
 * @param oldPath
 * @param newPath
 * @param mergedPath
 * @param locale
 * @param oldPropertySet
 * @param newPropertySet
 * @param feedback
 */
async function mergeLUFiles(schemaName: string, oldPath: string, newPath: string, mergedPath: string, locale: string, oldPropertySet: Set<string>, newPropertySet: Set<string>, feedback: Feedback): Promise<void> {
    let oldText = await fs.readFile(ppath.join(oldPath, 'luis', schemaName + '.' + locale + '.lu'), 'utf8')
    let oldLUResource = LUParser.parse(oldText)
    let oldImportSections = oldLUResource.Sections.filter(s => s.SectionType === lusectiontypes.IMPORTSECTION)

    let newText = await fs.readFile(ppath.join(newPath, 'luis', schemaName + '.' + locale + '.lu'), 'utf8')
    let newLUResource = LUParser.parse(newText)
    let newImportSections = newLUResource.Sections.filter(s => s.SectionType === lusectiontypes.IMPORTSECTION)

    let resultRefs: string[] = []
    let oldRefSet = new Set<string>()

    let localeOldPath = ppath.join(oldPath, locale)
    let localeNewPath = ppath.join(newPath, locale)
    let localeMergedPath = ppath.join(mergedPath, locale)

    for (let oldImportSection of oldImportSections) {
        let oldRef = oldImportSection.Description
        oldRefSet.add(oldRef)
        let extractedProperty = equalPattern(oldRef, oldPropertySet, schemaName)
        if (extractedProperty !== undefined) {
            if (newPropertySet.has(extractedProperty)) {
                resultRefs.push(oldRef + '(' + oldImportSection.Path + ')')
                let refStr = oldRef.split(']')
                let luFile = refStr[0].replace('[', '')
                // handle with lu file has enums 
                if (luFile.match(extractedProperty + 'Entity')) {
                    if (isUnchanged(localeOldPath, localeNewPath, luFile)) {
                        copySingleFile(localeOldPath, localeMergedPath, luFile, feedback)
                    } else {
                        changeEntityEnumLU(oldPath, newPath, mergedPath, luFile, locale, feedback)
                    }
                } else {
                    if (isUnchanged(localeOldPath, localeNewPath, luFile)) {
                        copySingleFile(localeOldPath, localeMergedPath, luFile, feedback)
                    } else {
                        feedback(FeedbackType.info, `***** Old and new ${luFile} are changed, please manually merge the resource `)
                    }
                }
            }
        } else {
            resultRefs.push(oldRef + '(' + oldImportSection.Path + ')')
            let refStr = oldRef.split(']')
            let luFile = refStr[0].replace('[', '')
            if (newText.match(luFile) && !isUnchanged(localeOldPath, localeNewPath, luFile)) {
                feedback(FeedbackType.info, `***** Old and new ${luFile} are changed, please manually merge the resource `)
            } else {
                copySingleFile(localeOldPath, localeMergedPath, luFile, feedback)
            }
        }
    }

    // integrate new lu files which do not exist in old lu assets
    for (let newImportSection of newImportSections) {
        let newRef = newImportSection.Description
        if (!oldRefSet.has(newRef)) {
            resultRefs.push(newRef + '(' + newImportSection.Path + ')')
            let refStr = newRef.split(']')
            let luFile = refStr[0].replace('[', '')
            copySingleFile(localeNewPath, localeMergedPath, luFile, feedback)
        }
    }

    let library = resultRefs.join(os.EOL)
    if (oldText.match('>>>')) {
        library = '>>> Library' + os.EOL + library
    }

    // write merged root lu file
    if (library.match(GeneratorPattern)) {
        library = library.replace(GeneratorPattern, '')
    }
    await gen.writeFile(ppath.join(mergedPath, 'luis', schemaName + '.' + locale + '.lu'), library, feedback)
    feedback(FeedbackType.info, `Generating ${schemaName}.${locale}.lu`)

    // copy .lu.dialog file
    copySingleFile(ppath.join(newPath, 'luis'), ppath.join(mergedPath, 'luis'), schemaName + '.' + locale + '.lu.dialog', feedback)
}

/**
 * @description: Merge individual lu files which have List Entity Section 
 * @param oldPath
 * @param newPath
 * @param mergedPath
 * @param filename
 * @param locale
 * @param feedback 
 */
async function changeEntityEnumLU(oldPath: string, newPath: string, mergedPath: string, filename: string, locale: string, feedback: Feedback): Promise<void> {
    let text = await fs.readFile(ppath.join(newPath, locale, filename), 'utf8')
    let newLUResource = LUParser.parse(text)
    let newEntitySections = newLUResource.Sections.filter(s => s.SectionType === lusectiontypes.NEWENTITYSECTION)

    text = await fs.readFile(ppath.join(oldPath, locale, filename), 'utf8')
    let oldLUResource = LUParser.parse(text)
    let oldEntitySections = oldLUResource.Sections.filter(s => s.SectionType === lusectiontypes.NEWENTITYSECTION)
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
                    if (!enumValueMap.has(enumEntity)) {
                        continue
                    }
                    resultStatements.push(oldListEntitySection.ListBody[i])
                    let j = i + 1
                    while (j < oldListEntitySection.ListBody.length) {
                        if (!oldListEntitySection.ListBody[j].match(':')) {
                            resultStatements.push(oldListEntitySection.ListBody[j])
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
        }
    }
    if (updatedLUResource === null) {
        if (oldLUResource.Content.match(GeneratorPattern)) {
            oldLUResource.Content = oldLUResource.Content.replace(GeneratorPattern, '')
        }
        await gen.writeFile(ppath.join(mergedPath, locale, filename), oldLUResource.Content, feedback)
        feedback(FeedbackType.info, `Generating ${filename}`)
    } else {
        if (updatedLUResource.Content.match(GeneratorPattern)) {
            updatedLUResource.Content = updatedLUResource.Content.replace(GeneratorPattern, '')
        }
        await gen.writeFile(ppath.join(mergedPath, locale, filename), updatedLUResource.Content, feedback)
        feedback(FeedbackType.info, `Generating ${filename}`)
    }
}

/**
 * @description: Merge lg files of two assets based on the new and old root lg files
 * @param schemaName
 * @param oldPath
 * @param newPath
 * @param mergedPath
 * @param locale
 * @param oldPropertySet
 * @param newPropertySet
 * @param feedback
 */
async function mergeLGFiles(schemaName: string, oldPath: string, newPath: string, mergedPath: string, locale: string, oldPropertySet: Set<string>, newPropertySet: Set<string>, feedback: Feedback): Promise<void> {
    let oldText = await fs.readFile(ppath.join(oldPath, locale, schemaName + '.' + locale + '.lg'), 'utf8')
    let oldRefs = oldText.split('\n')
    let newText = await fs.readFile(ppath.join(newPath, locale, schemaName + '.' + locale + '.lg'), 'utf8')
    let newRefs = newText.split('\n')

    let localeOldPath = ppath.join(oldPath, locale)
    let localeNewPath = ppath.join(newPath, locale)
    let localeMergedPath = ppath.join(mergedPath, locale)

    let resultRefs: string[] = []
    let oldRefSet = new Set<string>()
    for (let ref of oldRefs) {
        if (!ref.startsWith('[') && !ref.startsWith('>>>')) {
            continue
        }
        if (ref.startsWith('>>>')) {
            resultRefs.push(ref)
            continue
        }
        ref = ref.replace('\r', '')
        oldRefSet.add(ref)
        let extractedProperty = equalPattern(ref, oldPropertySet, schemaName)
        if (extractedProperty !== undefined) {
            if (newPropertySet.has(extractedProperty)) {
                resultRefs.push(ref)
                let refStr = ref.split('.lg')
                let lgFile = refStr[0].replace('[', '') + '.lg'
                if (lgFile.match(extractedProperty + 'Entity')) {
                    if (isUnchanged(localeOldPath, localeNewPath, lgFile)) {
                        copySingleFile(localeOldPath, localeMergedPath, lgFile, feedback)
                    } else {
                        changeEntityEnumLG(oldPath, newPath, mergedPath, lgFile, locale, feedback)
                    }
                } else {
                    if (isUnchanged(localeOldPath, localeNewPath, lgFile)) {
                        copySingleFile(localeOldPath, localeMergedPath, lgFile, feedback)
                    } else {
                        feedback(FeedbackType.info, `***** Old and new ${lgFile} are changed, please manually merge the resource `)
                    }
                }
            }
        } else {
            resultRefs.push(ref)
            let refStr = ref.split('.lg')
            let lgFile = refStr[0].replace('[', '') + '.lg'
            if (newText.match(lgFile) && !isUnchanged(localeOldPath, localeNewPath, lgFile)) {
                feedback(FeedbackType.info, `***** Old and new ${lgFile} are changed, please manually merge the resource `)
            } else {
                copySingleFile(localeOldPath, localeMergedPath, lgFile, feedback)
            }
        }
    }

    for (let ref of newRefs) {
        if (!ref.startsWith('[') && !ref.startsWith('>>>')) {
            continue
        }
        if (ref.startsWith('>>>')) {
            resultRefs.push(ref)
            continue
        }
        ref = ref.replace('\r', '')
        if (!oldRefSet.has(ref)) {
            resultRefs.push(ref)
            let refStr = ref.split('.lg')
            let lgFile = refStr[0].replace('[', '') + '.lg'
            copySingleFile(localeNewPath, localeMergedPath, lgFile, feedback)
        }
    }

    let val = resultRefs.join(os.EOL)
    if (val.match(GeneratorPattern)) {
        val = val.replace(GeneratorPattern, '')
    }
    await gen.writeFile(ppath.join(mergedPath, locale, schemaName + '.' + locale + '.lg'), val, feedback)
    feedback(FeedbackType.info, `Generating ${schemaName}.${locale}.lg`)
}

/**
 * @description: Merge individual lg files which have the template with SWITCH ENUM
 * @param oldPath
 * @param newPath
 * @param mergedPath
 * @param filename
 * @param locale
 * @param feedback
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
                    //parse old lg template and delete the enum entity which does not exist in new lg file and add new enum entity
                    const { startIndex, endIndex } =  parseLGTemplate(oldBody, oldStatements, newStatements, newEnumValueMap, oldEnumEntitySet,newSwitchStatements)
                    recordPart.push({ 'startIndex': startIndex, 'endIndex': endIndex, 'newSwitchStatements': newSwitchStatements })
                }
            }
        }
    }

    if (recordPart.length !== 0) {
        let startSplit = 0
        let arrList: [string[]] = [[]]
        for (let obj of recordPart) {
            let arr = oldStatements.slice(startSplit, obj['startIndex'])
            arrList.push(arr)
            arrList.push(obj['newSwitchStatements'])
            startSplit = obj['endIndex']
        }

        if (startSplit !== oldStatements.length) {
            let arr = oldStatements.slice(startSplit)
            arrList.push(arr)
        }

        for (let arr of arrList) {
            mergedStatements = mergedStatements.concat(arr)
        }
        let val = mergedStatements.join(os.EOL)
        if (val.match(GeneratorPattern)) {
            val = val.replace(GeneratorPattern, '')
        }
        await gen.writeFile(ppath.join(mergedPath, locale, filename), val, feedback)
        feedback(FeedbackType.info, `Generating ${filename}`)
    } else {
        await gen.writeFile(ppath.join(mergedPath, locale, filename), oldText, feedback)
        feedback(FeedbackType.info, `Generating ${filename}`)
    }
}

/**
 * @description: Update old LG Templte which has SWITCH ENUM
 * @param oldBody 
 * @param oldStatements
 * @param newStatements
 * @param newEnumValueMap
 * @param oldEnumEntitySet
 * @param newSwitchStatements
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
            while (!oldStatements[i].toLowerCase().match('case') && !oldStatements[i].toLowerCase().match('default')) {
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
                while (!oldStatements[k].toLowerCase().match('case') && !oldStatements[k].toLowerCase().match('default')) {
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
                    while (!newStatements[k].toLowerCase().match('case') && !newStatements[k].toLowerCase().match('default')) {
                        newSwitchStatements.push(newStatements[k])
                        k++
                    }

                }
            }
            let m = state.start.line - 1
            newSwitchStatements.push(oldStatements[m])
            m++
            while (!oldStatements[m].match('#') && !oldStatements[m].startsWith('[')) {
                newSwitchStatements.push(oldStatements[m])
                m++
            }
            endIndex = m
        }
    }

    return { startIndex, endIndex }
}

/**
 * @description: Merge two .main.dialog files following the trigger ordering rule
 * @param schemaName
 * @param oldPath
 * @param newPath
 * @param mergedPath
 * @param oldPropertySet
 * @param newPropertySet
 * @param feedback
 */
async function mergeDialogs(schemaName: string, oldPath: string, newPath: string, mergedPath: string, oldPropertySet: Set<string>, newPropertySet: Set<string>, feedback: Feedback): Promise<void> {
    let template = await fs.readFile(ppath.join(oldPath, schemaName + '.main.dialog')
        , 'utf8')
    let oldObj = JSON.parse(template)
    template = await fs.readFile(ppath.join(newPath, schemaName + '.main.dialog')
        , 'utf8')
    let newObj = JSON.parse(template)

    let newTriggers: string[] = []
    let newTriggerSet = new Set<string>()

    // remove triggers whose property does not exist in new property set
    let reducedOldTriggers: string[] = []
    let reducedOldTriggerSet = new Set<string>()

    let mergedTriggers: string[] = []

    for (let trigger of newObj['triggers']) {
        if (typeof trigger !== 'string') {
            // todo inline object
            continue
        }
        newTriggers.push(trigger)
        newTriggerSet.add(trigger)
    }

    for (let trigger of oldObj['triggers']) {
        if (typeof trigger !== 'string') {
            // todo inline object
            continue
        }
        let extractedProperty = equalPattern(trigger, oldPropertySet, schemaName)
        if (extractedProperty !== undefined) {
            if (newPropertySet.has(extractedProperty)) {
                reducedOldTriggers.push(trigger)
                reducedOldTriggerSet.add(trigger)
            }
        } else {
            reducedOldTriggers.push(trigger)
            reducedOldTriggerSet.add(trigger)
        }
    }

    let i = 0
    while (!reducedOldTriggerSet.has(newTriggers[i]) && i < newTriggers.length) {
        mergedTriggers.push(newTriggers[i])
        copySingleFile(newPath, mergedPath, newTriggers[i] + '.dialog', feedback)
        i++
    }

    let j = 0

    while (j < reducedOldTriggers.length) {
        mergedTriggers.push(reducedOldTriggers[j])
        if (newTriggers.includes(reducedOldTriggers[j]) && !isUnchanged(oldPath, newPath, reducedOldTriggers[j] + '.dialog')) {
            feedback(FeedbackType.info, `***** Old and new ${reducedOldTriggers[j]}.dialog are changed, please manually merge the resource `)
        } else {
            copySingleFile(oldPath, mergedPath, reducedOldTriggers[j] + '.dialog', feedback)
        }
        let index = newTriggers.indexOf(reducedOldTriggers[j])
        if (index !== -1) {
            index++
            while (index < newTriggers.length && !reducedOldTriggerSet.has(newTriggers[index])) {
                mergedTriggers.push(newTriggers[index])
                copySingleFile(newPath, mergedPath, newTriggers[index] + '.dialog', feedback)
                index++
            }
        }
        j++
    }

    oldObj['triggers'] = mergedTriggers
    await gen.writeFile(ppath.join(mergedPath, schemaName + '.main.dialog'), JSON.stringify(oldObj), feedback)
    feedback(FeedbackType.info, `Generating ${schemaName}.main.dialog`)
}

/**
 * @description: Compare the filename pattern for .lu file
 * @param filename
 * @param propertySet
 * @param schemaName 
 */
function equalPattern(filename: string, propertySet: Set<string>, schemaName: string): string | undefined {
    for (let property of propertySet) {
        let pattern1 = schemaName + '-' + property + '-'
        let pattern2 = schemaName + '-' + property + 'Entity'
        let pattern3 = schemaName + '-' + property + '.'
        if (filename.match(pattern1) || filename.match(pattern2) || filename.match(pattern3)) {
            return property
        }
    }
    return undefined
}

/**
 * @description: Get the old property set and new property set from schema files
 * @param schemaName
 * @param oldPath
 * @param newPath
 * @param mergedPath
 * @param feedback
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

    copySingleFile(newPath, mergedPath, schemaName + '.schema.dialog', feedback)
    return { oldPropertySet, newPropertySet }
}

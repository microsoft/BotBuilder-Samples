"use strict";
/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
Object.defineProperty(exports, "__esModule", { value: true });
const vscode_1 = require("vscode");
const botbuilder_lg_1 = require("botbuilder-lg");
const dataStorage_1 = require("./dataStorage");
const adaptive_expressions_1 = require("adaptive-expressions");
const buildinFunctions_1 = require("./buildinFunctions");
function isLgFile(fileName) {
    if (fileName === undefined || !fileName.toLowerCase().endsWith('.lg')) {
        return false;
    }
    return true;
}
exports.isLgFile = isLgFile;
function isLuFile(fileName) {
    if (fileName === undefined || !fileName.toLowerCase().endsWith('.lu')) {
        return false;
    }
    return true;
}
exports.isLuFile = isLuFile;
function isInFencedCodeBlock(doc, position) {
    let textBefore = doc.getText(new vscode_1.Range(new vscode_1.Position(0, 0), position));
    let matches = textBefore.match(/```[\w ]*$/gm);
    if (matches == null) {
        return false;
    }
    else {
        return matches.length % 2 != 0;
    }
}
exports.isInFencedCodeBlock = isInFencedCodeBlock;
function getAllTemplatesFromCurrentLGFile(lgFileUri) {
    let result = new botbuilder_lg_1.Templates();
    let engineEntity = dataStorage_1.DataStorage.templatesMap.get(lgFileUri.fsPath);
    if (engineEntity !== undefined && engineEntity.templates.toArray().length > 0) {
        result = engineEntity.templates;
    }
    return result;
}
exports.getAllTemplatesFromCurrentLGFile = getAllTemplatesFromCurrentLGFile;
function getreturnTypeStrFromReturnType(returnType) {
    let result = '';
    switch (returnType) {
        case adaptive_expressions_1.ReturnType.Boolean:
            result = "boolean";
            break;
        case adaptive_expressions_1.ReturnType.Number:
            result = "number";
            break;
        case adaptive_expressions_1.ReturnType.Object:
            result = "any";
            break;
        case adaptive_expressions_1.ReturnType.String:
            result = "string";
            break;
    }
    return result;
}
exports.getreturnTypeStrFromReturnType = getreturnTypeStrFromReturnType;
function getAllFunctions(lgFileUri) {
    const functions = new Map();
    for (const func of buildinFunctions_1.buildInfunctionsMap) {
        functions.set(func[0], func[1]);
    }
    const templates = getAllTemplatesFromCurrentLGFile(lgFileUri);
    const s = dataStorage_1.DataStorage.templatesMap;
    for (const template of templates) {
        var functionEntity = new buildinFunctions_1.FunctionEntity(template.parameters, adaptive_expressions_1.ReturnType.Object, 'Template reference');
        functions.set('lg.' + template.name, functionEntity);
    }
    return functions;
}
exports.getAllFunctions = getAllFunctions;
function getFunctionEntity(lgFileUri, name) {
    const allFunctions = getAllFunctions(lgFileUri);
    if (allFunctions.has(name)) {
        return allFunctions.get(name);
    }
    else {
        const lgWordName = 'lg.' + name;
        if (allFunctions.has(lgWordName)) {
            return allFunctions.get(lgWordName);
        }
    }
    return undefined;
}
exports.getFunctionEntity = getFunctionEntity;
//# sourceMappingURL=util.js.map
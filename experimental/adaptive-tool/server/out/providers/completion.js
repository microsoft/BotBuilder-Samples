"use strict";
/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
Object.defineProperty(exports, "__esModule", { value: true });
exports.provideCompletionItems = void 0;
const vscode_languageserver_1 = require("vscode-languageserver");
const util = require("../util");
const templatesStatus_1 = require("../templatesStatus");
const path = require("path");
function provideCompletionItems(_textDocumentPosition, documents) {
    const document = documents.get(_textDocumentPosition.textDocument.uri);
    const fspath = vscode_languageserver_1.Files.uriToFilePath(document.uri);
    const position = _textDocumentPosition.position;
    const lineTextBefore = document === null || document === void 0 ? void 0 : document.getText({
        start: {
            line: position.line,
            character: 0
        },
        end: position
    }).toString();
    if (/\[[^\]]*\]\([^)]*$/.test(lineTextBefore) && !util.isInFencedCodeBlock(document, position)) {
        // []() import suggestion
        const paths = Array.from(new Set(templatesStatus_1.TemplatesStatus.lgFilesOfWorkspace));
        return paths.filter(u => fspath !== u).reduce((prev, curr) => {
            const relativePath = path.relative(path.dirname(fspath), curr);
            const item = {
                label: relativePath,
                kind: vscode_languageserver_1.CompletionItemKind.Reference,
                detail: curr
            };
            item.detail = curr;
            prev.push(item);
            return prev;
        }, []);
    }
    else if (/\$\{[^}]*$/.test(lineTextBefore)) {
        // buildin function prompt in expression
        const items = [];
        const functions = util.getAllFunctions(document.uri);
        functions.forEach((value, key) => {
            const completionItem = vscode_languageserver_1.CompletionItem.create(key);
            const returnType = util.getreturnTypeStrFromReturnType(value.returntype);
            completionItem.detail = `${key}(${value.params.join(", ")}): ${returnType}`;
            completionItem.documentation = value.introduction;
            completionItem.insertText = `${key}(${value.params.map(u => u.split(':')[0].trim()).join(", ")})`;
            items.push(completionItem);
        });
        return items;
    }
    else if (isStartStructure(document, position)) {
        // structure name and key suggestion
        const items = [];
        util.cardTypes.forEach(value => {
            const completionItem = vscode_languageserver_1.CompletionItem.create(value);
            completionItem.detail = `create ${value} structure`;
            let insertTextArray = util.cardPropDict.Others;
            // if (value === 'CardAction' || value === 'Suggestions' || value === 'Attachment' || value === 'Activity') {
            if (value === 'CardAction' || value === 'Attachment' || value === 'Activity') {
                insertTextArray = util.cardPropDict[value];
            }
            else if (value.endsWith('Card')) {
                insertTextArray = util.cardPropDict.Cards;
            }
            completionItem.insertText = value + '\r\n' + insertTextArray.map(u => `\t${u.name}=${u.placeHolder}`).join('\r\n') + '\r\n';
            items.push(completionItem);
        });
        return items;
    }
    else if (isInStructure(document, position).isInStruct && /^\s*$/.test(lineTextBefore)) {
        const structureName = isInStructure(document, position).struType;
        const items = [];
        const nameToPropertiesMapping = Object.entries(util.cardPropDictFull);
        const propertiesMapping = nameToPropertiesMapping.find(u => u[0].toLowerCase() === structureName.toLowerCase());
        if (propertiesMapping !== undefined) {
            const properties = propertiesMapping[1];
            properties.forEach(propertyItem => {
                const completionItem = vscode_languageserver_1.CompletionItem.create(propertyItem.name);
                completionItem.detail = `create property ${propertyItem.name}`;
                const placeHolder = 'placeHolder' in propertyItem ? propertyItem['placeHolder'] : `{${propertyItem.name}}`;
                completionItem.insertText = propertyItem.name + '=' + placeHolder;
                items.push(completionItem);
            });
            return items;
        }
    }
    else if (/^>\s!#/.test(lineTextBefore)) {
        // options suggestion following "> !#"
        const items = [];
        if (/>\s!#\s*$/.test(lineTextBefore)) {
            AddToCompletion(util.optionsMap.options, items);
        }
        else {
            if (/>\s!#\s*@strict\s*=$/.test(lineTextBefore)) {
                AddToCompletion(util.optionsMap.strictOptions, items);
            }
            else if (/>\s!#\s*@replaceNull\s*=$/.test(lineTextBefore)) {
                AddToCompletion(util.optionsMap.replaceNullOptions, items);
            }
            else if (/>\s!#\s*@lineBreakStyle\s*=$/.test(lineTextBefore)) {
                AddToCompletion(util.optionsMap.lineBreakStyleOptions, items);
            }
            else if (/>\s!#\s*@Exports\s*=$/.test(lineTextBefore) || (/>\s!#\s*@Exports\s*=/.test(lineTextBefore) && /,\s*$/.test(lineTextBefore))) {
                const templatesOptions = templatesStatus_1.TemplatesStatus.templatesMap.get(fspath).templates.toArray();
                // const templatesOptions = Templates.parseFile(fspath!).toArray();
                for (let i = 0; i < templatesOptions.length; ++i) {
                    const templateName = templatesOptions[i].name;
                    const completionItem = vscode_languageserver_1.CompletionItem.create(" " + templateName);
                    completionItem.detail = " " + templateName;
                    completionItem.insertText = " " + templateName;
                    items.push(completionItem);
                }
            }
        }
        return items;
    }
    return [];
}
exports.provideCompletionItems = provideCompletionItems;
function isStartStructure(document, position) {
    const lineTextBefore = document === null || document === void 0 ? void 0 : document.getText({
        start: {
            line: position.line,
            character: 0
        },
        end: position
    }).toString();
    if (util.isInFencedCodeBlock(document, position)
        || !(/^\s*\[[^\]]*$/.test(lineTextBefore))
        || position.line <= 0)
        return false;
    let previourLine = position.line - 1;
    const currentOffset = document.offsetAt({
        line: position.line,
        character: 0
    });
    while (previourLine >= 0) {
        const previousLineText = document === null || document === void 0 ? void 0 : document.getText({
            start: {
                line: previourLine,
                character: 0
            },
            end: document.positionAt(currentOffset - 1)
        }).toString().trim();
        if (previousLineText === '') {
            previourLine--;
            continue;
        }
        else if (previousLineText.startsWith('#')) {
            return true;
        }
        else {
            return false;
        }
    }
    return false;
}
function isInStructure(document, position) {
    if (util.isInFencedCodeBlock(document, position)
        || position.line <= 0)
        return { isInStruct: false, struType: undefined };
    let previourLine = position.line - 1;
    const currentOffset = document.offsetAt({
        line: position.line,
        character: 0
    });
    while (previourLine >= 0) {
        const previousLineText = document === null || document === void 0 ? void 0 : document.getText({
            start: {
                line: previourLine,
                character: 0
            },
            end: document.positionAt(currentOffset - 1)
        }).toString().trim();
        if (previousLineText.startsWith('#')
            || previousLineText === ']'
            || previousLineText.startsWith('-')
            || previousLineText.endsWith('```')) {
            return { isInStruct: false, struType: undefined };
        }
        else if (previousLineText.startsWith('[')) {
            const structureType = previousLineText.substr(1).trim();
            return { isInStruct: true, struType: structureType };
        }
        previourLine--;
    }
    return { isInStruct: false, struType: undefined };
}
function AddToCompletion(options, items) {
    for (const option in options) {
        const completionItem = vscode_languageserver_1.CompletionItem.create(" " + option);
        completionItem.detail = options[option].detail;
        completionItem.documentation = options[option].documentation;
        completionItem.insertText = options[option].insertText;
        items.push(completionItem);
    }
}
//# sourceMappingURL=completion.js.map
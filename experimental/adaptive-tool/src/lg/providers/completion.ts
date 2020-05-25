/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

import * as vscode from 'vscode';
import * as util from '../util';
import { TemplatesStatus } from '../templatesStatus';
import * as path from 'path';

/**
 * Code completions provide context sensitive suggestions to the user.
 * @see https://code.visualstudio.com/api/language-extensions/programmatic-language-features#show-code-completion-proposals
 * @export
 * @class LGCompletionItemProvider
 * @implements [CompletionItemProvider](#vscode.CompletionItemProvider)
 */

export function activate(context: vscode.ExtensionContext) {
    context.subscriptions.push(vscode.languages.registerCompletionItemProvider('*', new LGCompletionItemProvider(), '{', '(', '[', '.', '\n'));
}

class LGCompletionItemProvider implements vscode.CompletionItemProvider {
    provideCompletionItems(document: vscode.TextDocument,
        position: vscode.Position,
        token: vscode.CancellationToken,
        context: vscode.CompletionContext): vscode.ProviderResult<vscode.CompletionItem[] | vscode.CompletionList> {
        if (!util.isLgFile(document.fileName)) {
            return;
        }

        const lineTextBefore = document.lineAt(position.line).text.substring(0, position.character);

        if (/\[[^\]]*\]\([^\)]*$/.test(lineTextBefore) && !util.isInFencedCodeBlock(document, position)) {
            // []() import suggestion
            const paths = Array.from(new Set(TemplatesStatus.lgFilesOfWorkspace));

            return paths.filter(u => document.uri.fsPath !== u).reduce((prev, curr) => {
                var relativePath = path.relative(path.dirname(document.uri.fsPath), curr);
                    let item = new vscode.CompletionItem(relativePath, vscode.CompletionItemKind.Reference);
                    item.detail = curr;
                    prev.push(item);
                    return prev;
            }, []);
        } else if (/\$\{[^\}]*$/.test(lineTextBefore)) {
            // buildin function prompt in expression
            let items: vscode.CompletionItem[] = [];
            var functions = util.getAllFunctions(document.uri);
            functions.forEach((value, key) => {
                let completionItem = new vscode.CompletionItem(key);
                const returnType = util.getreturnTypeStrFromReturnType(value.returntype);
                completionItem.detail = `${key}(${value.params.join(", ")}): ${returnType}`;
                completionItem.documentation = value.introduction;
                completionItem.insertText = `${key}(${value.params.map(u => u.split(':')[0].trim()).join(", ")})`;
                items.push(completionItem);
            });

            return items;
        }  else if (this.isStartStructure(document, position)) {
            // structure name and key suggestion
            let items: vscode.CompletionItem[] = [];
            util.cardTypes.forEach(value => {
                let completionItem = new vscode.CompletionItem(value);
                completionItem.detail = `create ${value} structure`;
                let insertTextArray = util.cardPropDict.Others;
                if (value === 'CardAction' || value === 'Suggestions' || value === 'Attachment' || value === 'Activity') {
                    insertTextArray = util.cardPropDict[value];
                } else if (value.endsWith('Card')){
                    insertTextArray = util.cardPropDict.Cards;
                }

                completionItem.insertText = value + '\r\n' + insertTextArray.map(u => `\t${u.name}=${u.placeHolder}`).join('\r\n') + '\r\n';
                items.push(completionItem);
            });

            return items;
        } else if (this.isInStructure(document, position).isInStruct && /^\s*$/.test(lineTextBefore)) {
            const structureName = this.isInStructure(document, position).struType;
            let items: vscode.CompletionItem[] = []; 

            const nameToPropertiesMapping = Object.entries(util.cardPropDictFull);
            const propertiesMapping = nameToPropertiesMapping.find(u => u[0].toLowerCase() === structureName.toLowerCase());
            if (propertiesMapping !== undefined) {
                const properties = propertiesMapping[1];
                properties.forEach(propertyItem => {
                    let completionItem = new vscode.CompletionItem(propertyItem.name);
                    completionItem.detail = `create property ${propertyItem.name}`;
                    const placeHolder = 'placeHolder' in propertyItem ? propertyItem['placeHolder'] : `{${propertyItem.name}}`
                    completionItem.insertText = propertyItem.name + '=' + placeHolder;
                    items.push(completionItem);
                })
                return items;
            }
        } else {
            return [];
        }
    }

    isStartStructure(document: vscode.TextDocument,
        position: vscode.Position) {
            const lineTextBefore = document.lineAt(position.line).text.substring(0, position.character);
            if (util.isInFencedCodeBlock(document, position)
                    || !(/^\s*\[[^\]]*$/.test(lineTextBefore))
                    || position.line <= 0)
                return false;
            
            var previourLine = position.line - 1;
            while(previourLine >= 0) {
                var previousLineText = document.lineAt(previourLine).text.trim();
                if (previousLineText === '') {
                    previourLine--;
                    continue;
                } else if (previousLineText.startsWith('#')){
                    return true;
                } else {
                    return false;
                }
            }
            return false;
    }

    isInStructure(document: vscode.TextDocument,
        position: vscode.Position): { isInStruct:boolean; struType:string } {
            if (util.isInFencedCodeBlock(document, position)
                    || position.line <= 0)
                return {isInStruct:false, struType:undefined};

            var previourLine = position.line - 1;
            while(previourLine >= 0) {
                var previousLineText = document.lineAt(previourLine).text.trim();
                if (previousLineText.startsWith('#')
                || previousLineText === ']'
                || previousLineText.startsWith('-')
                || previousLineText.endsWith('```')) {
                    return {isInStruct:false, struType:undefined};
                } else if (previousLineText.startsWith('[')){
                    const structureType = previousLineText.substr(1).trim();
                    return {isInStruct:true, struType:structureType};
                }
                previourLine--;
            }
            return {isInStruct:false, struType:undefined};
    }
}



"use strict";
/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
Object.defineProperty(exports, "__esModule", { value: true });
const vscode = require("vscode");
const util = require("../util");
const dataStorage_1 = require("../dataStorage");
const path = require("path");
/**
 * Code completions provide context sensitive suggestions to the user.
 * @see https://code.visualstudio.com/api/language-extensions/programmatic-language-features#show-code-completion-proposals
 * @export
 * @class LGCompletionItemProvider
 * @implements [CompletionItemProvider](#vscode.CompletionItemProvider)
 */
function activate(context) {
    context.subscriptions.push(vscode.languages.registerCompletionItemProvider('*', new LGCompletionItemProvider(), '{', '(', '[', '.'));
}
exports.activate = activate;
class LGCompletionItemProvider {
    provideCompletionItems(document, position, token, context) {
        if (!util.isLgFile(document.fileName)) {
            return;
        }
        const lineTextBefore = document.lineAt(position.line).text.substring(0, position.character);
        const lineTextAfter = document.lineAt(position.line).text.substring(position.character);
        if (/\[[^\]]*\]\([^\)]*$/.test(lineTextBefore) && !util.isInFencedCodeBlock(document, position)) {
            // []() import suggestion
            return new Promise((res, _) => {
                let paths = [];
                dataStorage_1.DataStorage.templatesMap.forEach(u => paths = paths.concat(u.templates.toArray().map(u => u.source)));
                paths = Array.from(new Set(paths));
                const headingCompletions = paths.reduce((prev, curr) => {
                    var relativePath = path.relative(path.dirname(document.uri.fsPath), curr);
                    let item = new vscode.CompletionItem(relativePath, vscode.CompletionItemKind.Reference);
                    item.detail = curr;
                    prev.push(item);
                    return prev;
                }, []);
                res(headingCompletions);
            });
        }
        else if (/\$\{[^\}]*$/.test(lineTextBefore)) {
            // buildin function prompt in expression
            let items = [];
            var functions = util.getAllFunctions(document.uri);
            functions.forEach((value, key) => {
                let completionItem = new vscode.CompletionItem(key);
                const returnType = util.getreturnTypeStrFromReturnType(value.returntype);
                completionItem.detail = `${key}(${value.params.join(", ")}): ${returnType}`;
                completionItem.documentation = value.introduction;
                items.push(completionItem);
            });
            return items;
        }
        else {
            return [];
        }
    }
}
//# sourceMappingURL=completion.js.map
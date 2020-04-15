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
    context.subscriptions.push(vscode.languages.registerCompletionItemProvider('*', new LGCompletionItemProvider(), '{', '(', '[', '.'));
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
            return new Promise((res, _) => {
                const paths = Array.from(new Set(TemplatesStatus.lgFilesOfWorkspace));

                const headingCompletions = paths.reduce((prev, curr) => {
                    var relativePath = path.relative(path.dirname(document.uri.fsPath), curr);
                    let item = new vscode.CompletionItem(relativePath, vscode.CompletionItemKind.Reference);
                    item.detail = curr;
                    prev.push(item);
                    return prev;
                }, []);

                res(headingCompletions);
            });
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
        }  else {
            return [];
        }
    }
}



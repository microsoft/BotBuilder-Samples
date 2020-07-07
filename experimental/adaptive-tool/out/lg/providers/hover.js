"use strict";
/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
Object.defineProperty(exports, "__esModule", { value: true });
exports.activate = void 0;
const vscode = require("vscode");
const util = require("../util");
/**
 * Hovers show information about the symbol/object that's below the mouse cursor. This is usually the type of the symbol and a description.
 * @see https://code.visualstudio.com/api/language-extensions/programmatic-language-features#show-hovers
 * @export
 * @class LGHoverProvider
 * @implements [HoverProvider ](#vscode.HoverProvider )
 */
function activate(context) {
    context.subscriptions.push(vscode.languages.registerHoverProvider('*', new LGHoverProvider()));
}
exports.activate = activate;
class LGHoverProvider {
    provideHover(document, position, token) {
        if (!util.isLgFile(document.fileName)) {
            return;
        }
        const wordRange = document.getWordRangeAtPosition(position, /[a-zA-Z0-9_.]+/);
        if (!wordRange) {
            return undefined;
        }
        const wordName = document.getText(wordRange);
        const functionEntity = util.getFunctionEntity(document.uri, wordName);
        if (functionEntity !== undefined) {
            const returnType = util.getreturnTypeStrFromReturnType(functionEntity.returntype);
            const functionIntroduction = `${wordName}(${functionEntity.params.join(", ")}): ${returnType}`;
            const contents = [];
            contents.push(new vscode.MarkdownString(functionIntroduction));
            contents.push(new vscode.MarkdownString(functionEntity.introduction));
            return new vscode.Hover(contents, wordRange);
        }
    }
}
//# sourceMappingURL=hover.js.map
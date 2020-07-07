"use strict";
/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
Object.defineProperty(exports, "__esModule", { value: true });
exports.activate = void 0;
const vscode_1 = require("vscode");
const util_1 = require("../util");
const vscode = require("vscode");
function activate(context) {
    context.subscriptions.push(vscode_1.commands.registerCommand('lg.extension.onEnterKey', onEnterKey));
}
exports.activate = activate;
function onEnterKey() {
    const editor = vscode_1.window.activeTextEditor;
    const cursorPos = editor.selection.active;
    const line = editor.document.lineAt(cursorPos.line);
    const textBeforeCursor = line.text.substr(0, cursorPos.character);
    const lineBreakPos = cursorPos;
    let matches;
    if ((matches = /^(\s*-)\s?(IF|ELSE|SWITCH|CASE|DEFAULT|(ELSE\\s*IF))\s*:.*/i.exec(textBeforeCursor)) !== null && !util_1.isInFencedCodeBlock(editor.document, cursorPos)) {
        // -IF: new line would indent 
        return editor.edit(editBuilder => {
            const emptyNumber = matches;
            const dashIndex = '\n    ' + emptyNumber[1];
            editBuilder.insert(lineBreakPos, dashIndex);
        }).then(() => { editor.revealRange(editor.selection); });
    }
    else if ((matches = /^(\s*[#-]).*\S+.*/.exec(textBeforeCursor)) !== null && !util_1.isInFencedCodeBlock(editor.document, cursorPos)) {
        // in '- ' or '# ' line
        return editor.edit(editBuilder => {
            const replacedStr = `\n${matches[1].replace('#', '-')} `;
            editBuilder.insert(lineBreakPos, replacedStr);
        }).then(() => { editor.revealRange(editor.selection); });
    }
    else if ((matches = /^(\s*-)\s*/.exec(textBeforeCursor)) !== null && !util_1.isInFencedCodeBlock(editor.document, cursorPos)) {
        // in '-' empty line, enter would delete the head dash
        return editor.edit(editBuilder => {
            const range = new vscode.Range(lineBreakPos.line, 0, lineBreakPos.line, lineBreakPos.character);
            editBuilder.delete(range);
        }).then(() => { editor.revealRange(editor.selection); });
    }
    else {
        return vscode_1.commands.executeCommand('type', { source: 'keyboard', text: '\n' });
    }
}
//# sourceMappingURL=keyBinding.js.map
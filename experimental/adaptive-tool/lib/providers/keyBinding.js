"use strict";
/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
Object.defineProperty(exports, "__esModule", { value: true });
const vscode_1 = require("vscode");
const util_1 = require("../util");
function activate(context) {
    context.subscriptions.push(vscode_1.commands.registerCommand('lg.extension.onEnterKey', onEnterKey));
}
exports.activate = activate;
function onEnterKey(modifiers) {
    let editor = vscode_1.window.activeTextEditor;
    let cursorPos = editor.selection.active;
    let line = editor.document.lineAt(cursorPos.line);
    let textBeforeCursor = line.text.substr(0, cursorPos.character);
    let textAfterCursor = line.text.substr(cursorPos.character);
    let lineBreakPos = cursorPos;
    let matches;
    if ((matches = /^(\s*[#-]).*\S+.*/.exec(textBeforeCursor)) !== null && !util_1.isInFencedCodeBlock(editor.document, cursorPos)) {
        // in '- ' or '# ' line
        return editor.edit(editBuilder => {
            editBuilder.insert(lineBreakPos, `\n${matches[1].replace('#', '-')}`);
        }).then(() => { editor.revealRange(editor.selection); });
    }
    else {
        return asNormal('enter', modifiers);
    }
}
function asNormal(key, modifiers) {
    switch (key) {
        case 'enter':
            if (modifiers === 'ctrl') {
                return vscode_1.commands.executeCommand('editor.action.insertLineAfter');
            }
            else {
                return vscode_1.commands.executeCommand('type', { source: 'keyboard', text: '\n' });
            }
        case 'tab':
            if (vscode_1.workspace.getConfiguration('emmet').get('triggerExpansionOnTab')) {
                return vscode_1.commands.executeCommand('editor.emmet.action.expandAbbreviation');
            }
            else if (modifiers === 'shift') {
                return vscode_1.commands.executeCommand('editor.action.outdentLines');
            }
            else {
                return vscode_1.commands.executeCommand('tab');
            }
        case 'backspace':
            return vscode_1.commands.executeCommand('deleteLeft');
    }
}
//# sourceMappingURL=keyBinding.js.map
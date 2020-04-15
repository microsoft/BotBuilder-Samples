/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

import { commands, ExtensionContext, Position, window, workspace} from 'vscode';
import { isInFencedCodeBlock } from '../util';
import * as vscode from 'vscode';

export function activate(context: ExtensionContext) {
    context.subscriptions.push(
        commands.registerCommand('lg.extension.onEnterKey', onEnterKey),
    );
}

function onEnterKey(modifiers?: string) {
    let editor = window.activeTextEditor;
    let cursorPos: Position = editor.selection.active;
    let line = editor.document.lineAt(cursorPos.line);
    let textBeforeCursor = line.text.substr(0, cursorPos.character);
    let textAfterCursor = line.text.substr(cursorPos.character);

    let lineBreakPos = cursorPos;

    let matches: RegExpExecArray | { replace: (arg0: string, arg1: string) => void; }[];
    if ((matches = /^(\s*-)\s?(IF|ELSE|SWITCH|CASE|DEFAULT|(ELSE\\s*IF))\s*:.*/i.exec(textBeforeCursor)) !== null && !isInFencedCodeBlock(editor.document, cursorPos)) {
        // -IF: new line would indent 
        return editor.edit(editBuilder => {
            var emptyNumber  = matches as RegExpExecArray;
            var dashIndex = '\n    ' + emptyNumber[1];
            editBuilder.insert(lineBreakPos, dashIndex);
        }).then(() => { editor.revealRange(editor.selection); });
    } else if ((matches = /^(\s*[#-]).*\S+.*/.exec(textBeforeCursor)) !== null && !isInFencedCodeBlock(editor.document, cursorPos)) {
        // in '- ' or '# ' line
        return editor.edit(editBuilder => {
            const replacedStr = `\n${matches[1].replace('#', '-')} `;
            editBuilder.insert(lineBreakPos, replacedStr);
        }).then(() => { editor.revealRange(editor.selection); });
    } else if ((matches = /^(\s*-)\s*/.exec(textBeforeCursor)) !== null && !isInFencedCodeBlock(editor.document, cursorPos)) {
        // in '-' empty line, enter would delete the head dash
        return editor.edit(editBuilder => {
            const range = new vscode.Range(lineBreakPos.line, 0, lineBreakPos.line, lineBreakPos.character);
            editBuilder.delete(range);
        }).then(() => { editor.revealRange(editor.selection); });
    } else {
        return asNormal('enter', modifiers);
    }
}

function asNormal(key: string, modifiers?: string) {
    switch (key) {
        case 'enter':
            if (modifiers === 'ctrl') {
                return commands.executeCommand('editor.action.insertLineAfter');
            } else {
                return commands.executeCommand('type', { source: 'keyboard', text: '\n' });
            }
        case 'tab':
            if (workspace.getConfiguration('emmet').get<boolean>('triggerExpansionOnTab')) {
                return commands.executeCommand('editor.emmet.action.expandAbbreviation');
            } else if (modifiers === 'shift') {
                return commands.executeCommand('editor.action.outdentLines');
            } else {
                return commands.executeCommand('tab');
            }
        case 'backspace':
            return commands.executeCommand('deleteLeft');
    }
}
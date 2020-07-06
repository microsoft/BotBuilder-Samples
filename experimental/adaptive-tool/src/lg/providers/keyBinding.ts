/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

import { commands, ExtensionContext, Position, window} from 'vscode';
import { isInFencedCodeBlock } from '../util';
import * as vscode from 'vscode';

export function activate(context: ExtensionContext) {
    context.subscriptions.push(
        commands.registerCommand('lg.extension.onEnterKey', onEnterKey),
    );
}

function onEnterKey() {
    const editor = window.activeTextEditor;
    const cursorPos: Position = editor.selection.active;
    const line = editor.document.lineAt(cursorPos.line);
    const textBeforeCursor = line.text.substr(0, cursorPos.character);

    const lineBreakPos = cursorPos;

    let matches: RegExpExecArray | { replace: (arg0: string, arg1: string) => void; }[];
    if ((matches = /^(\s*-)\s?(IF|ELSE|SWITCH|CASE|DEFAULT|(ELSE\\s*IF))\s*:.*/i.exec(textBeforeCursor)) !== null && !isInFencedCodeBlock(editor.document, cursorPos)) {
        // -IF: new line would indent 
        return editor.edit(editBuilder => {
            const emptyNumber  = matches as RegExpExecArray;
            const dashIndex = '\n    ' + emptyNumber[1];
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
        return commands.executeCommand('type', { source: 'keyboard', text: '\n' });
    }
}
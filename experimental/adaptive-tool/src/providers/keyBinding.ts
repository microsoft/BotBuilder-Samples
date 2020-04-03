/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

import { commands, ExtensionContext, Position, window, workspace} from 'vscode';
import { isInFencedCodeBlock } from '../util';

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
    if ((matches = /^(\s*[#-]).*\S+.*/.exec(textBeforeCursor)) !== null && !isInFencedCodeBlock(editor.document, cursorPos)) {
        // in '- ' or '# ' line
        return editor.edit(editBuilder => {
            editBuilder.insert(lineBreakPos, `\n${matches[1].replace('#', '-')}`);
        }).then(() => { editor.revealRange(editor.selection); });
    }  else {
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
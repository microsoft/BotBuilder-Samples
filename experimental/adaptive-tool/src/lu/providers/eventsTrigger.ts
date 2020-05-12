/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

import * as vscode from 'vscode';
import { LuFilesStatus } from '../luFilesStatus';

export function activate(context: vscode.ExtensionContext) {
    if (vscode.window.activeTextEditor) {
        triggerLUFileFinder();
    }

    context.subscriptions.push(vscode.workspace.onDidCreateFiles(e => {
        triggerLUFileFinder();
    }));

    context.subscriptions.push(vscode.workspace.onDidDeleteFiles(e => {
        triggerLUFileFinder();
    }));

    // workspace changed
    context.subscriptions.push(vscode.workspace.onDidChangeWorkspaceFolders(e => {
        triggerLUFileFinder();
    }));

}

function triggerLUFileFinder() {
    vscode.workspace.findFiles('**/*.lu').then(uris => {
        LuFilesStatus.luFilesOfWorkspace = [];
        uris.forEach(uri => {
            LuFilesStatus.luFilesOfWorkspace.push(uri.fsPath);
        });
    });
}
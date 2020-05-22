/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

import * as vscode from 'vscode';
import { TemplatesStatus, TemplatesEntity } from '../templatesStatus';

export function activate(context: vscode.ExtensionContext) {
    if (vscode.window.activeTextEditor) {
        triggerLGFileFinder();
    }

    context.subscriptions.push(vscode.workspace.onDidCreateFiles(e => {
        triggerLGFileFinder();
    }));

    context.subscriptions.push(vscode.workspace.onDidDeleteFiles(e => {
        triggerLGFileFinder();
        // delete templates from map
        e.files.forEach(u => {
            if (TemplatesStatus.templatesMap.has(u.fsPath)) {
                TemplatesStatus.templatesMap.delete(u.fsPath);
            }
        })
    }));

    // workspace changed
    context.subscriptions.push(vscode.workspace.onDidChangeWorkspaceFolders(e => {
        triggerLGFileFinder();
    }));

}

function triggerLGFileFinder() {
    vscode.workspace.findFiles('**/*.lg').then(uris => {
        TemplatesStatus.lgFilesOfWorkspace = [];
        uris.forEach(uri => {
            TemplatesStatus.lgFilesOfWorkspace.push(uri.fsPath);
        });
    });
}

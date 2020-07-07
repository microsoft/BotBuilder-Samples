"use strict";
/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
Object.defineProperty(exports, "__esModule", { value: true });
exports.activate = void 0;
const vscode = require("vscode");
const templatesStatus_1 = require("../templatesStatus");
function activate(context) {
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
            if (templatesStatus_1.TemplatesStatus.templatesMap.has(u.fsPath)) {
                templatesStatus_1.TemplatesStatus.templatesMap.delete(u.fsPath);
            }
        });
    }));
    // workspace changed
    context.subscriptions.push(vscode.workspace.onDidChangeWorkspaceFolders(e => {
        triggerLGFileFinder();
    }));
}
exports.activate = activate;
function triggerLGFileFinder() {
    vscode.workspace.findFiles('**/*.lg').then(uris => {
        templatesStatus_1.TemplatesStatus.lgFilesOfWorkspace = [];
        uris.forEach(uri => {
            templatesStatus_1.TemplatesStatus.lgFilesOfWorkspace.push(uri.fsPath);
        });
    });
}
//# sourceMappingURL=eventsTrigger.js.map
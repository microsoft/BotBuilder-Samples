"use strict";
/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
Object.defineProperty(exports, "__esModule", { value: true });
exports.activate = void 0;
const vscode = require("vscode");
const luFilesStatus_1 = require("../luFilesStatus");
function activate(context) {
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
exports.activate = activate;
function triggerLUFileFinder() {
    vscode.workspace.findFiles('**/*.lu').then(uris => {
        luFilesStatus_1.LuFilesStatus.luFilesOfWorkspace = [];
        uris.forEach(uri => {
            luFilesStatus_1.LuFilesStatus.luFilesOfWorkspace.push(uri.fsPath);
        });
    });
}
//# sourceMappingURL=eventsTrigger.js.map
/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

import * as vscode from 'vscode';
import { Templates } from 'botbuilder-lg';
import * as util from '../util';
import { TemplatesStatus, TemplatesEntity } from '../templatesStatus';

export function activate(context: vscode.ExtensionContext) {
    if (vscode.window.activeTextEditor) {
        triggerLGFileFinder();
    }

    // each 3 second, would re-parse current file
    setInterval(() => {
        const editer = vscode.window.activeTextEditor;
        if (editer !== undefined && util.isLgFile(editer.document.fileName)) {
            updateTemplateEngine(editer.document.uri, editer.document.getText());
         }
    }, 3000);

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

    // if the current file is saved, re-parse file
    context.subscriptions.push(vscode.workspace.onDidSaveTextDocument(e => {
        if (util.isLgFile(e.fileName))
        {
            updateTemplateEngine(e.uri, e.getText());
        }
    }));

    // workspace changed
    context.subscriptions.push(vscode.workspace.onDidChangeWorkspaceFolders(e => {
        triggerLGFileFinder();
    }));

    // if the file is opened, parse-it
    context.subscriptions.push(vscode.workspace.onDidOpenTextDocument(e => {
        if (util.isLgFile(e.fileName))
        {
            updateTemplateEngine(e.uri, e.getText());
        }
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

function updateTemplateEngine(uri: vscode.Uri, fileContent: string) {
    let engine: Templates = Templates.parseText(fileContent, uri.fsPath);

    TemplatesStatus.templatesMap.set(uri.fsPath, new TemplatesEntity(uri, engine));
}
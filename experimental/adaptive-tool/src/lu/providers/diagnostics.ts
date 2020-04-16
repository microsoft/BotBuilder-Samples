/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

import * as vscode from 'vscode';
import * as util from '../util';
import { DataStorage } from '../dataStorage';

/**
 * Diagnostics are a way to indicate issues with the code.
 * @see https://code.visualstudio.com/api/language-extensions/programmatic-language-features#provide-diagnostics
 */
export function activate(context: vscode.ExtensionContext) {
    const collection: vscode.DiagnosticCollection = vscode.languages.createDiagnosticCollection('lu');

    setInterval(() => {
        const editer = vscode.window.activeTextEditor;
        if (editer !== undefined && util.isLuFile(editer.document.fileName)) {
            updateDiagnostics(editer.document, collection);
         }
    }, 3000);

    // if you want to trigger the event for each text change, use: vscode.workspace.onDidChangeTextDocument
    context.subscriptions.push(vscode.workspace.onDidOpenTextDocument(e => {
        if (util.isLuFile(vscode.window.activeTextEditor.document.fileName))
        {
            updateDiagnostics(e, collection);
        }
    }));
    
    context.subscriptions.push(vscode.workspace.onDidSaveTextDocument(e => {
        if (util.isLuFile(e.fileName))
        {
            updateDiagnostics(e, collection);
        }
    }));

    context.subscriptions.push(vscode.window.onDidChangeActiveTextEditor(e => {
        if (util.isLuFile(e.document.fileName))
        {
            updateDiagnostics(e.document, collection);
        }
    }));
}

function updateDiagnostics(document: vscode.TextDocument, collection: vscode.DiagnosticCollection): void {
    if(!util.isLuFile(document.fileName) || !DataStorage.LuResourceMap.has(document.uri.fsPath)) {
        collection.clear();
        return;
    }

    const luResource: any = DataStorage.LuResourceMap.get(document.uri.fsPath);

    var diagnostics = luResource.Errors;
    var vscodeDiagnostics: vscode.Diagnostic[] = [];

    const severityConverter = {
        ERROR: 'Error',
        WARN: 'Warning' 
    }

    diagnostics.forEach(u => {
        const diagItem = new vscode.Diagnostic(
            new vscode.Range(
                new vscode.Position(u.Range.Start.Line - 1, u.Range.Start.Character),
                new vscode.Position(u.Range.End.Line - 1, u.Range.End.Character)),
            u.Message,
            severityConverter[u.Severity]
        );
        vscodeDiagnostics.push(diagItem);
    });

    collection.set(document.uri, vscodeDiagnostics);
}
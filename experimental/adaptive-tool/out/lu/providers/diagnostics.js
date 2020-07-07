"use strict";
/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
Object.defineProperty(exports, "__esModule", { value: true });
exports.activate = void 0;
const vscode = require("vscode");
const util = require("../util");
// eslint-disable-next-line @typescript-eslint/no-var-requires
const LUParser = require('@microsoft/bf-lu/lib/parser/lufile/luParser');
/**
 * Diagnostics are a way to indicate issues with the code.
 * @see https://code.visualstudio.com/api/language-extensions/programmatic-language-features#provide-diagnostics
 */
function activate(context) {
    const collection = vscode.languages.createDiagnosticCollection('lu');
    if (vscode.window.activeTextEditor) {
        if (util.isLuFile(vscode.window.activeTextEditor.document.fileName)) {
            updateDiagnostics(vscode.window.activeTextEditor.document, collection);
        }
    }
    context.subscriptions.push(vscode.window.onDidChangeActiveTextEditor(e => {
        if (e && util.isLuFile(e.document.fileName)) {
            updateDiagnostics(e.document, collection);
        }
    }));
    context.subscriptions.push(vscode.workspace.onDidChangeTextDocument(e => {
        if (e && util.isLuFile(e.document.fileName)) {
            updateDiagnostics(e.document, collection);
        }
    }));
    context.subscriptions.push(vscode.workspace.onDidCloseTextDocument(doc => {
        if (doc && util.isLuFile(doc.fileName)) {
            collection.delete(doc.uri);
        }
    }));
}
exports.activate = activate;
function updateDiagnostics(document, collection) {
    if (!util.isLuFile(document.fileName)) {
        collection.clear();
        return;
    }
    const luResource = LUParser.parse(document.getText());
    const diagnostics = luResource.Errors;
    const vscodeDiagnostics = [];
    const severityConverter = {
        ERROR: 0,
        WARN: 1
    };
    diagnostics.forEach(u => {
        const diagItem = new vscode.Diagnostic(new vscode.Range(new vscode.Position(u.Range.Start.Line - 1, u.Range.Start.Character), new vscode.Position(u.Range.End.Line - 1, u.Range.End.Character)), u.Message, severityConverter[u.Severity]);
        vscodeDiagnostics.push(diagItem);
    });
    collection.set(document.uri, vscodeDiagnostics);
}
//# sourceMappingURL=diagnostics.js.map
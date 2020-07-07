"use strict";
/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
Object.defineProperty(exports, "__esModule", { value: true });
exports.activate = void 0;
const vscode = require("vscode");
const botbuilder_lg_1 = require("botbuilder-lg");
const util = require("../util");
const templatesStatus_1 = require("../templatesStatus");
/**
 * Diagnostics are a way to indicate issues with the code.
 * @see https://code.visualstudio.com/api/language-extensions/programmatic-language-features#provide-diagnostics
 */
function activate(context) {
    const collection = vscode.languages.createDiagnosticCollection('lg');
    if (vscode.window.activeTextEditor) {
        if (util.isLgFile(vscode.window.activeTextEditor.document.fileName)) {
            updateDiagnostics(vscode.window.activeTextEditor.document, collection);
        }
    }
    context.subscriptions.push(vscode.window.onDidChangeActiveTextEditor(e => {
        if (e && util.isLgFile(e.document.fileName)) {
            updateDiagnostics(e.document, collection);
        }
    }));
    context.subscriptions.push(vscode.workspace.onDidChangeTextDocument(e => {
        if (e && util.isLgFile(e.document.fileName)) {
            updateDiagnostics(e.document, collection);
        }
    }));
    context.subscriptions.push(vscode.workspace.onDidCloseTextDocument(doc => {
        if (doc && util.isLgFile(doc.fileName)) {
            collection.delete(doc.uri);
        }
    }));
}
exports.activate = activate;
function updateDiagnostics(document, collection) {
    if (!util.isLgFile(document.fileName)) {
        collection.clear();
        return;
    }
    const engine = botbuilder_lg_1.Templates.parseText(document.getText(), document.uri.fsPath);
    const diagnostics = engine.diagnostics;
    templatesStatus_1.TemplatesStatus.templatesMap.set(document.uri.fsPath, new templatesStatus_1.TemplatesEntity(document.uri, engine));
    const vscodeDiagnostics = [];
    const confDiagLevel = vscode.workspace.getConfiguration().get('LG.Expression.ignoreUnknownFunction');
    const confCustomFuncListSetting = vscode.workspace.getConfiguration().get('LG.Expression.customFunctionList');
    let customFunctionList = [];
    if (confCustomFuncListSetting.length >= 1) {
        customFunctionList = confCustomFuncListSetting.split(",").map(u => u.trim());
    }
    diagnostics.forEach(u => {
        const isUnkownFuncDiag = u.message.includes("it's not a built-in function or a custom function");
        if (isUnkownFuncDiag === true) {
            let ignored = false;
            const funcName = extractFuncName(u.message);
            if (customFunctionList.includes(funcName)) {
                ignored = true;
            }
            else {
                switch (confDiagLevel) {
                    case "ignore":
                        if (isUnkownFuncDiag) {
                            ignored = true;
                        }
                        break;
                    case "warn":
                        if (isUnkownFuncDiag) {
                            u.severity = botbuilder_lg_1.DiagnosticSeverity.Warning;
                        }
                        break;
                    default:
                        break;
                }
            }
            if (ignored === false) {
                const diagItem = new vscode.Diagnostic(new vscode.Range(new vscode.Position(u.range.start.line - 1 < 0 ? 0 : u.range.start.line - 1, u.range.start.character), new vscode.Position(u.range.end.line - 1 < 0 ? 0 : u.range.end.line - 1, u.range.end.character)), u.message, u.severity);
                vscodeDiagnostics.push(diagItem);
            }
        }
        else {
            const diagItem = new vscode.Diagnostic(new vscode.Range(new vscode.Position(u.range.start.line - 1 < 0 ? 0 : u.range.start.line - 1, u.range.start.character), new vscode.Position(u.range.end.line - 1 < 0 ? 0 : u.range.end.line - 1, u.range.end.character)), u.message, u.severity);
            vscodeDiagnostics.push(diagItem);
        }
    });
    collection.set(document.uri, vscodeDiagnostics);
}
function extractFuncName(errorMessage) {
    const message = errorMessage.match(/'\.\s([\w][\w0\-.9_]*)\s+does\snot\shave/)[1];
    return message;
}
//# sourceMappingURL=diagnostics.js.map
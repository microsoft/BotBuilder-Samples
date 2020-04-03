"use strict";
/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
Object.defineProperty(exports, "__esModule", { value: true });
const vscode = require("vscode");
const botbuilder_lg_1 = require("botbuilder-lg");
const util = require("../util");
const dataStorage_1 = require("../dataStorage");
/**
 * Diagnostics are a way to indicate issues with the code.
 * @see https://code.visualstudio.com/api/language-extensions/programmatic-language-features#provide-diagnostics
 */
function activate(context) {
    const collection = vscode.languages.createDiagnosticCollection('lg');
    setInterval(() => {
        const editer = vscode.window.activeTextEditor;
        if (editer !== undefined && util.isLgFile(editer.document.fileName)) {
            updateDiagnostics(editer.document, collection);
        }
    }, 3000);
    // if you want to trigger the event for each text change, use: vscode.workspace.onDidChangeTextDocument
    context.subscriptions.push(vscode.workspace.onDidOpenTextDocument(e => {
        if (util.isLgFile(vscode.window.activeTextEditor.document.fileName)) {
            updateDiagnostics(e, collection);
        }
    }));
    context.subscriptions.push(vscode.workspace.onDidSaveTextDocument(e => {
        if (util.isLgFile(e.fileName)) {
            updateDiagnostics(e, collection);
        }
    }));
    context.subscriptions.push(vscode.window.onDidChangeActiveTextEditor(e => {
        if (util.isLgFile(e.document.fileName)) {
            updateDiagnostics(e.document, collection);
        }
    }));
}
exports.activate = activate;
function updateDiagnostics(document, collection) {
    if (!util.isLgFile(document.fileName) || !dataStorage_1.DataStorage.templatesMap.has(document.uri.fsPath)) {
        collection.clear();
        return;
    }
    const templateEntity = dataStorage_1.DataStorage.templatesMap.get(document.uri.fsPath);
    var diagnostics = templateEntity.templates.diagnostics;
    var vscodeDiagnostics = [];
    const confDiagLevel = vscode.workspace.getConfiguration().get('LG.Expression.ignoreUnknownFunction');
    const confCustomFuncListSetting = vscode.workspace.getConfiguration().get('LG.Expression.customFunctionList');
    var customFunctionList = [];
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
                const diagItem = new vscode.Diagnostic(new vscode.Range(new vscode.Position(u.range.start.line - 1, u.range.start.character), new vscode.Position(u.range.end.line - 1, u.range.end.character)), u.message, u.severity);
                vscodeDiagnostics.push(diagItem);
            }
        }
        else {
            const diagItem = new vscode.Diagnostic(new vscode.Range(new vscode.Position(u.range.start.line - 1, u.range.start.character), new vscode.Position(u.range.end.line - 1, u.range.end.character)), u.message, u.severity);
            vscodeDiagnostics.push(diagItem);
        }
    });
    collection.set(document.uri, vscodeDiagnostics);
}
function extractFuncName(errorMessage) {
    const message = errorMessage.match(/'\.\s([\w][\w0\-\.9_]*)\s+does\snot\shave/)[1];
    return message;
}
//# sourceMappingURL=diagnostics.js.map
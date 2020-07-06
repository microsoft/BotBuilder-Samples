/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

import * as vscode from 'vscode';
import { DiagnosticSeverity, Templates} from 'botbuilder-lg';
import * as util from '../util';
import { TemplatesStatus, TemplatesEntity } from '../templatesStatus';

/**
 * Diagnostics are a way to indicate issues with the code.
 * @see https://code.visualstudio.com/api/language-extensions/programmatic-language-features#provide-diagnostics
 */
export function activate(context: vscode.ExtensionContext) {
    const collection: vscode.DiagnosticCollection = vscode.languages.createDiagnosticCollection('lg');
    if (vscode.window.activeTextEditor) {
        if (util.isLgFile(vscode.window.activeTextEditor.document.fileName))
        {
            updateDiagnostics(vscode.window.activeTextEditor.document, collection);
        }
    }
    
    context.subscriptions.push(vscode.window.onDidChangeActiveTextEditor(e => {
        if (e && util.isLgFile(e.document.fileName))
        {
            updateDiagnostics(e.document, collection);
        }
    }));

    context.subscriptions.push(vscode.workspace.onDidChangeTextDocument(e => {
        if (e && util.isLgFile(e.document.fileName))
        {
            updateDiagnostics(e.document, collection);
        }
    }));

    context.subscriptions.push(vscode.workspace.onDidCloseTextDocument(doc => {
        if (doc && util.isLgFile(doc.fileName))
        {
            collection.delete(doc.uri)
        }
    }));
}

function updateDiagnostics(document: vscode.TextDocument, collection: vscode.DiagnosticCollection): void {
    if(!util.isLgFile(document.fileName) ) {
        collection.clear();
        return;
    }
    
    const engine: Templates = Templates.parseText(document.getText(), document.uri.fsPath);
    const diagnostics = engine.diagnostics;

    TemplatesStatus.templatesMap.set(document.uri.fsPath, new TemplatesEntity(document.uri, engine));

    const vscodeDiagnostics: vscode.Diagnostic[] = [];
    const confDiagLevel = vscode.workspace.getConfiguration().get('LG.Expression.ignoreUnknownFunction');
    const confCustomFuncListSetting: string = vscode.workspace.getConfiguration().get('LG.Expression.customFunctionList');
    let customFunctionList: string[] = [];
    if (confCustomFuncListSetting.length >= 1) {
        customFunctionList = confCustomFuncListSetting.split(",").map(u => u.trim());
    }
    diagnostics.forEach(u => {
        const isUnkownFuncDiag: boolean = u.message.includes("it's not a built-in function or a custom function");
        if (isUnkownFuncDiag === true){
            let ignored = false;
            const funcName = extractFuncName(u.message);
            if (customFunctionList.includes(funcName)) {
                ignored = true;
            } else {
                switch (confDiagLevel) {
                    case "ignore":
                        if (isUnkownFuncDiag) {
                            ignored = true;
                        }
                        break;
                
                    case "warn":
                            if (isUnkownFuncDiag) {
                                u.severity = DiagnosticSeverity.Warning;
                            }
                        break;
                    default:
                        break;
                }
            }

            if (ignored === false){
                const diagItem = new vscode.Diagnostic(
                    new vscode.Range(
                        new vscode.Position(u.range.start.line - 1 < 0 ? 0 : u.range.start.line - 1, u.range.start.character),
                        new vscode.Position(u.range.end.line - 1 < 0 ? 0 : u.range.end.line - 1, u.range.end.character)),
                    u.message,
                    u.severity
                );
                vscodeDiagnostics.push(diagItem);
            }
        } else {
            const diagItem = new vscode.Diagnostic(
                new vscode.Range(
                    new vscode.Position(u.range.start.line - 1 < 0 ? 0 : u.range.start.line - 1, u.range.start.character),
                    new vscode.Position(u.range.end.line - 1 < 0 ? 0 : u.range.end.line - 1, u.range.end.character)),
                u.message,
                u.severity
            );
            vscodeDiagnostics.push(diagItem);
        }
    });
    collection.set(document.uri, vscodeDiagnostics);
}

function extractFuncName(errorMessage: string): string {
    const message = errorMessage.match(/'\.\s([\w][\w0\-.9_]*)\s+does\snot\shave/)[1];
    return message;
}
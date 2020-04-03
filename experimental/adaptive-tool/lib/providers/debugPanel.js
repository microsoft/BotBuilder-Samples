"use strict";
/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
Object.defineProperty(exports, "__esModule", { value: true });
const vscode = require("vscode");
const botbuilder_lg_1 = require("botbuilder-lg");
const path = require("path");
const fs = require("fs");
const dataStorage_1 = require("../dataStorage");
const util = require("../util");
function activate(context) {
    context.subscriptions.push(vscode.commands.registerCommand('lgLiveTest.start', () => {
        LGDebugPanel.createOrShow(context.extensionPath);
    }));
}
exports.activate = activate;
/**
 * VebView for LG debugger
 *
 * @export
 * @class LGDebugPanel
 */
class LGDebugPanel {
    constructor(panel, extensionPath) {
        this._disposables = [];
        this._panel = panel;
        this._extensionPath = extensionPath;
        this._update();
        this._panel.onDidDispose(() => this.dispose(), null, this._disposables);
        this._panel.onDidChangeViewState(e => {
            if (this._panel.visible) {
                this._update();
            }
        }, null, this._disposables);
        // receive maeeage from webview
        this._panel.webview.onDidReceiveMessage(message => {
            const currentUri = vscode.window.visibleTextEditors[0].document.uri;
            switch (message.command) {
                case 'passScope': {
                    try {
                        const scope = JSON.parse(message.scopeValue);
                        const templateName = message.templateName;
                        const iterations = message.iterations;
                        let results = [];
                        const fsPath = currentUri.fsPath;
                        if (!dataStorage_1.DataStorage.templatesMap.has(fsPath)) {
                            vscode.window.showErrorMessage("something was wrong.");
                        }
                        else {
                            let engineEntity = dataStorage_1.DataStorage.templatesMap.get(fsPath);
                            if (engineEntity.templates.diagnostics.some(u => u.severity === botbuilder_lg_1.DiagnosticSeverity.Error)) {
                                vscode.window.showErrorMessage("please fix errors first.");
                            }
                            else {
                                const engine = engineEntity.templates;
                                const evaledResults = engine.expandTemplate(templateName, scope);
                                if (iterations >= evaledResults.length) {
                                    results = evaledResults;
                                }
                                else {
                                    results = evaledResults.slice(0, iterations);
                                }
                                // send result to webview
                                this._panel.webview.postMessage({ command: 'evaluateResults', results: results });
                            }
                        }
                    }
                    catch (e) {
                        vscode.window.showErrorMessage(e.message);
                    }
                    break;
                }
                case 'getTemplates': {
                    try {
                        const source = message.source;
                        let result = [];
                        let templates = util.getAllTemplatesFromCurrentLGFile(currentUri).toArray();
                        if (templates.length === 0) {
                            vscode.window.showErrorMessage(`there is no valid template in ${currentUri.fsPath}.`);
                        }
                        else {
                            for (const template of templates) {
                                if (!result.includes(template.name)) {
                                    result.push(template.name);
                                }
                            }
                        }
                        this._panel.webview.postMessage({ command: 'TemplateName', results: result });
                    }
                    catch (e) {
                        vscode.window.showErrorMessage(e.message);
                    }
                    break;
                }
            }
        }, null, this._disposables);
    }
    /**
     * Command lgLivingTest.start runner
     *
     * @static
     * @returns
     * @memberof LGDebugPanel
     */
    static createOrShow(extensionPath) {
        // If already have a panel, show it.
        if (LGDebugPanel.currentPanel) {
            LGDebugPanel.currentPanel._panel.reveal(vscode.ViewColumn.Beside);
            return;
        }
        // If not, create one.
        const panel = vscode.window.createWebviewPanel(LGDebugPanel.viewType, "LG debug", vscode.ViewColumn.Beside, {
            enableScripts: true
        });
        LGDebugPanel.currentPanel = new LGDebugPanel(panel, extensionPath);
    }
    dispose() {
        LGDebugPanel.currentPanel = undefined;
        this._panel.dispose();
        while (this._disposables.length) {
            const x = this._disposables.pop();
            if (x) {
                x.dispose();
            }
        }
    }
    _update() {
        this._panel.webview.html = this._getHtmlForWebview();
        this._panel.title = 'Language Generation Tester';
    }
    _getHtmlForWebview() {
        const htmlFilePath = path.join(this._extensionPath, 'resources', 'lgPreviewTemplate.html');
        const htmlContent = fs.readFileSync(htmlFilePath, 'utf-8');
        return htmlContent;
    }
}
exports.LGDebugPanel = LGDebugPanel;
LGDebugPanel.viewType = 'lgDebug';
//# sourceMappingURL=debugPanel.js.map
"use strict";
/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
Object.defineProperty(exports, "__esModule", { value: true });
exports.LGDebugPanel = exports.activate = void 0;
const vscode = require("vscode");
const path = require("path");
const fs = require("fs");
const templatesStatus_1 = require("../templatesStatus");
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
            switch (message.command) {
                case 'evaluate': {
                    try {
                        const scope = JSON.parse(message.scopeValue);
                        const iterations = message.iterations;
                        let results = [];
                        if (message.templateName) {
                            // evaluate template
                            const templateName = message.templateName;
                            const engineEntity = templatesStatus_1.TemplatesStatus.templatesMap.get(vscode.window.visibleTextEditors[0].document.uri.fsPath);
                            if (engineEntity === undefined || engineEntity.templates === undefined) {
                                vscode.window.showErrorMessage("Sorry, something is wrong.");
                            }
                            else {
                                const templates = engineEntity.templates;
                                const evaledResults = templates.expandTemplate(templateName, scope);
                                if (iterations >= evaledResults.length) {
                                    results = evaledResults;
                                }
                                else {
                                    results = evaledResults.slice(0, iterations);
                                }
                                this._panel.webview.postMessage({ command: 'evaluateResults', results: results });
                            }
                        }
                        else if (message.freetext) {
                            // evaluate inline free text
                            let inlineStr = message.freetext;
                            const engineEntity = templatesStatus_1.TemplatesStatus.templatesMap.get(vscode.window.visibleTextEditors[0].document.uri.fsPath);
                            if (engineEntity === undefined || engineEntity.templates === undefined) {
                                vscode.window.showErrorMessage("Sorry, something is wrong.");
                            }
                            else {
                                const templates = engineEntity.templates;
                                const fakeTemplateId = '__temp__';
                                const multiLineMark = '```';
                                inlineStr = !(inlineStr.trim().startsWith(multiLineMark) && inlineStr.includes('\n'))
                                    ? `- ${multiLineMark}${inlineStr}${multiLineMark}` : `- ${inlineStr}`;
                                const newTemplates = templates.addTemplate(fakeTemplateId, undefined, inlineStr);
                                const evaledResults = newTemplates.expandTemplate(fakeTemplateId, scope);
                                if (iterations >= evaledResults.length) {
                                    results = evaledResults;
                                }
                                else {
                                    results = evaledResults.slice(0, iterations);
                                }
                                templates.deleteTemplate(fakeTemplateId);
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
                        const result = [];
                        const allTemplates = util.getTemplatesFromCurrentLGFile(vscode.window.visibleTextEditors[0].document.uri).allTemplates;
                        if (allTemplates.length === 0) {
                            vscode.window.showErrorMessage("Sorry, something is wrong.");
                        }
                        else {
                            for (const template of allTemplates) {
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
        const htmlFilePath = path.join(this._extensionPath, 'resources', 'lg', 'lgPreviewTemplate.html');
        const htmlContent = fs.readFileSync(htmlFilePath, 'utf-8');
        return htmlContent;
    }
}
exports.LGDebugPanel = LGDebugPanel;
LGDebugPanel.viewType = 'lgDebug';
//# sourceMappingURL=debugPanel.js.map
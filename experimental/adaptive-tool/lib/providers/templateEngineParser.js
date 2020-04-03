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
function activate(context) {
    if (vscode.window.activeTextEditor) {
        triggerLGFileFinder();
    }
    // each 3 second, would re-parse current file
    setInterval(() => {
        const editer = vscode.window.activeTextEditor;
        if (editer !== undefined && util.isLgFile(editer.document.fileName)) {
            updateTemplateEngine(editer.document.uri);
        }
    }, 3000);
    // if the current file is saved, re-parse file
    context.subscriptions.push(vscode.workspace.onDidSaveTextDocument(e => {
        if (util.isLgFile(e.fileName)) {
            updateTemplateEngine(e.uri);
        }
    }));
    // if the file is opened, parse-it
    context.subscriptions.push(vscode.workspace.onDidOpenTextDocument(e => {
        if (util.isLgFile(e.fileName)) {
            updateTemplateEngine(e.uri);
        }
    }));
}
exports.activate = activate;
function triggerLGFileFinder() {
    vscode.workspace.findFiles('**/*.lg').then(uris => {
        dataStorage_1.DataStorage.templatesMap.clear();
        uris.forEach(uri => {
            updateTemplateEngine(uri);
        });
    });
}
function updateTemplateEngine(uri) {
    let engine = botbuilder_lg_1.Templates.parseFile(uri.fsPath);
    dataStorage_1.DataStorage.templatesMap.set(uri.fsPath, new dataStorage_1.TemplatesEntity(uri, engine));
}
//# sourceMappingURL=templateEngineParser.js.map
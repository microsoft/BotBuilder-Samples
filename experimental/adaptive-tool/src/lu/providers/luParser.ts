/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

import * as vscode from 'vscode';
import * as util from '../util';
import { DataStorage, LUResource } from '../dataStorage';

const fileHelper = require('@microsoft/bf-lu/lib/utils/fileHelper');
const LUParser = require('@microsoft/bf-lu/lib/parser/lufile/luPaser').LUParser;

export async function activate(context: vscode.ExtensionContext) {
    if (vscode.window.activeTextEditor) {
        await triggerLUFileFinder();
    }

    // each 3 second, would re-parse current file
    setInterval(async () => {
        const editer = vscode.window.activeTextEditor;
        if (editer !== undefined && util.isLuFile(editer.document.fileName)) {
            await updateSectionEngine(editer.document.uri);
         }
    }, 3000);

    // if the current file is saved, re-parse file
    context.subscriptions.push(vscode.workspace.onDidSaveTextDocument(async e => {
        if (util.isLuFile(e.fileName))
        {
            await updateSectionEngine(e.uri);
        }
    }));

    // if the file is opened, parse-it
    context.subscriptions.push(vscode.workspace.onDidOpenTextDocument(async e => {
        if (util.isLuFile(e.fileName))
        {
            await updateSectionEngine(e.uri);
        }
    }));
}

async function triggerLUFileFinder() {
    vscode.workspace.findFiles('**/*.lu').then(uris => {
        DataStorage.LuResourceMap.clear();
        uris.forEach(async uri => {
            await updateSectionEngine(uri);
        });
    });
}

async function updateSectionEngine(uri: vscode.Uri) {
    const content = await fileHelper.getContentFromFile(uri.fsPath);
    let engine: any = await LUParser.parse(content);
    DataStorage.LuResourceMap.set(uri.fsPath, new LUResource(uri, engine));
}
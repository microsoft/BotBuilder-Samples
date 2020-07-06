"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.provideKeyBinding = void 0;
/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
const vscode_languageserver_1 = require("vscode-languageserver");
const util = require("../util");
function provideKeyBinding(params, documents, connection) {
    const commandName = params.command;
    const args = params.arguments;
    if (commandName == 'lg.extension.onEnterKey' && args != undefined) {
        onEnterKey(args, documents, connection);
    }
}
exports.provideKeyBinding = provideKeyBinding;
function onEnterKey(args, documents, connection) {
    const document = documents.get(args[0]);
    // const editor = window.document.documentURI;//.activeTextEditor;
    const cursorPos = args[1];
    const textBeforeCursor = document === null || document === void 0 ? void 0 : document.getText({
        start: {
            line: cursorPos.line,
            character: 0
        },
        end: cursorPos
    });
    const lineBreakPos = cursorPos;
    let matches;
    if ((matches = /^(\s*-)\s?(IF|ELSE|SWITCH|CASE|DEFAULT|(ELSE\\s*IF))\s*:.*/i.exec(textBeforeCursor)) !== null && !util.isInFencedCodeBlock(document, cursorPos)) {
        // -IF: new line would indent 
        const emptyNumber = matches;
        const dashIndex = '\n    ' + emptyNumber[1];
        connection.workspace.applyEdit({
            documentChanges: [
                vscode_languageserver_1.TextDocumentEdit.create({ uri: document.uri, version: document.version }, [
                    vscode_languageserver_1.TextEdit.insert(lineBreakPos, dashIndex)
                ])
            ]
        });
    }
    else if ((matches = /^(\s*[#-]).*\S+.*/.exec(textBeforeCursor)) !== null && !util.isInFencedCodeBlock(document, cursorPos)) {
        // in '- ' or '# ' line
        const replacedStr = `\n${matches[1].replace('#', '-')} `;
        connection.workspace.applyEdit({
            documentChanges: [
                vscode_languageserver_1.TextDocumentEdit.create({ uri: document.uri, version: document.version }, [
                    vscode_languageserver_1.TextEdit.insert(lineBreakPos, replacedStr)
                ])
            ]
        });
    }
    else if ((matches = /^(\s*-)\s*/.exec(textBeforeCursor)) !== null && !util.isInFencedCodeBlock(document, cursorPos)) {
        // in '-' empty line, enter would delete the head dash
        const range = { start: { line: lineBreakPos.line, character: 0 }, end: { line: lineBreakPos.line, character: lineBreakPos.character } };
        connection.workspace.applyEdit({
            documentChanges: [
                vscode_languageserver_1.TextDocumentEdit.create({ uri: document.uri, version: document.version }, [
                    vscode_languageserver_1.TextEdit.del(range)
                ])
            ]
        });
    }
    else {
        connection.workspace.applyEdit({
            documentChanges: [
                vscode_languageserver_1.TextDocumentEdit.create({ uri: document.uri, version: document.version }, [
                    vscode_languageserver_1.TextEdit.insert(lineBreakPos, '\n')
                ])
            ]
        });
    }
}
//# sourceMappingURL=keyBinding.js.map
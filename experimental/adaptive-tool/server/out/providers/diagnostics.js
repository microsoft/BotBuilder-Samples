"use strict";
/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.updateDiagnostics = void 0;
const vscode_languageserver_1 = require("vscode-languageserver");
const templatesStatus_1 = require("../templatesStatus");
const util = require("../util");
const path = require("path");
const botbuilder_lg_1 = require("botbuilder-lg");
function updateDiagnostics(document, connection) {
    return __awaiter(this, void 0, void 0, function* () {
        if (!util.isLgFile(path.basename(document.uri)))
            return;
        const confDiagLevel = yield connection.workspace.getConfiguration({
            scopeUri: document.uri,
            section: 'LG.Expression.ignoreUnknownFunction'
        }); //.then(((value: string) => value));
        const confCustomFuncListSetting = yield connection.workspace.getConfiguration({
            scopeUri: document.uri,
            section: 'LG.Expression.customFunctionList'
        }).then(((value) => value == null ? '' : value));
        const engine = botbuilder_lg_1.Templates.parseText(document.getText(), vscode_languageserver_1.Files.uriToFilePath(document.uri));
        const diagnostics = engine.diagnostics;
        templatesStatus_1.TemplatesStatus.templatesMap.set(vscode_languageserver_1.Files.uriToFilePath(document.uri), new templatesStatus_1.TemplatesEntity(document.uri, engine));
        const lspDiagnostics = [];
        let customFunctionList = [];
        if (confCustomFuncListSetting.length >= 1) {
            customFunctionList = confCustomFuncListSetting.split(",").map(u => u.trim());
        }
        diagnostics.forEach(u => {
            const isUnkownFuncDiag = u.message.includes("it's not a built-in function or a custom function");
            let severity;
            switch (u.severity) {
                case 0:
                    severity = vscode_languageserver_1.DiagnosticSeverity.Error;
                    break;
                case 1:
                    severity = vscode_languageserver_1.DiagnosticSeverity.Warning;
                    break;
                case 2:
                    severity = vscode_languageserver_1.DiagnosticSeverity.Information;
                    break;
                case 3:
                    severity = vscode_languageserver_1.DiagnosticSeverity.Hint;
                    break;
                default:
                    severity = vscode_languageserver_1.DiagnosticSeverity.Error;
                    break;
            }
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
                                u.severity = vscode_languageserver_1.DiagnosticSeverity.Warning;
                            }
                            break;
                        default:
                            break;
                    }
                }
                if (ignored === false) {
                    const diagItem = vscode_languageserver_1.Diagnostic.create({
                        start: {
                            line: u.range.start.line - 1 < 0 ? 0 : u.range.start.line - 1,
                            character: u.range.start.character
                        },
                        end: {
                            line: u.range.end.line - 1 < 0 ? 0 : u.range.end.line - 1,
                            character: u.range.end.character
                        },
                    }, u.message, severity);
                    lspDiagnostics.push(diagItem);
                }
            }
            else {
                const diagItem = vscode_languageserver_1.Diagnostic.create({
                    start: {
                        line: u.range.start.line - 1 < 0 ? 0 : u.range.start.line - 1,
                        character: u.range.start.character
                    },
                    end: {
                        line: u.range.end.line - 1 < 0 ? 0 : u.range.end.line - 1,
                        character: u.range.end.character
                    }
                }, u.message, severity);
                lspDiagnostics.push(diagItem);
            }
        });
        // Send the computed diagnostics to VSCode.
        connection.sendDiagnostics({ uri: document.uri, diagnostics: lspDiagnostics });
    });
}
exports.updateDiagnostics = updateDiagnostics;
function extractFuncName(errorMessage) {
    const message = errorMessage.match(/'\.\s([\w][\w0\-.9_]*)\s+does\snot\shave/)[1];
    return message;
}
//# sourceMappingURL=diagnostics.js.map
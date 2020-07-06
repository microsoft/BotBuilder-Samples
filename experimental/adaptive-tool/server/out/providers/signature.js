"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.provideSignatureHelp = void 0;
/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
const vscode_languageserver_1 = require("vscode-languageserver");
const util = require("../util");
const path = require("path");
function provideSignatureHelp(params, documents) {
    const document = documents.get(params.textDocument.uri);
    const position = params.position;
    if (!util.isLgFile(path.basename(document.uri))) {
        return;
    }
    const { functionName, paramIndex } = parseFunction(document, position);
    const functionEntity = util.getFunctionEntity(document.uri, functionName);
    if (functionEntity === undefined) {
        return;
    }
    const paramInfoList = [];
    functionEntity.params.forEach(u => paramInfoList.push(vscode_languageserver_1.ParameterInformation.create(u)));
    const returnType = util.getreturnTypeStrFromReturnType(functionEntity.returntype);
    const sigLabel = `${functionName}(${functionEntity.params.join(", ")}): ${returnType}`;
    const sigInfo = vscode_languageserver_1.SignatureInformation.create(sigLabel);
    sigInfo.parameters = paramInfoList;
    const signatureHelp = {
        activeParameter: paramIndex,
        activeSignature: 0,
        signatures: [sigInfo]
    };
    return signatureHelp;
}
exports.provideSignatureHelp = provideSignatureHelp;
function parseFunction(document, position) {
    let functionName = "";
    let paramIndex = 0;
    const range = {
        start: {
            line: position.line,
            character: 0
        },
        end: position
    };
    const text = document.getText(range);
    let bracketsDiffNumber = 0; // right bracket number - left bracket number, if bracketsDiffNumber === 0, present that is out of param inner scope
    for (let i = text.length - 1; i >= 0; i--) {
        const currentChar = text.charAt(i);
        if (currentChar === ',' && bracketsDiffNumber === 0) {
            paramIndex++;
        }
        else if (currentChar === '(' && bracketsDiffNumber === 0 && i > 0) {
            const wordRange = util.getWordRangeAtPosition(document, { line: position.line, character: i - 1 });
            if (wordRange !== undefined) {
                functionName = document.getText(wordRange);
                break;
            }
        }
        else if (currentChar === ')') {
            bracketsDiffNumber++;
        }
        else if (currentChar === '(') {
            bracketsDiffNumber--;
        }
    }
    return { functionName, paramIndex };
}
//# sourceMappingURL=signature.js.map
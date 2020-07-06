"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.provideHover = void 0;
/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
const vscode_languageserver_1 = require("vscode-languageserver");
const util = require("../util");
const path = require("path");
function provideHover(params, documents) {
    const document = documents.get(params.textDocument.uri);
    const position = params.position;
    if (!util.isLgFile(path.basename(document.uri))) {
        return;
    }
    const wordRange = util.getWordRangeAtPosition(document, position);
    if (!wordRange) {
        return undefined;
    }
    const wordName = document.getText(wordRange);
    const functionEntity = util.getFunctionEntity(document.uri, wordName);
    if (functionEntity !== undefined) {
        const returnType = util.getreturnTypeStrFromReturnType(functionEntity.returntype);
        const functionIntroduction = `${wordName}(${functionEntity.params.join(", ")}): ${returnType}`;
        const contents = [];
        contents.push(vscode_languageserver_1.MarkedString.fromPlainText(functionIntroduction));
        contents.push(vscode_languageserver_1.MarkedString.fromPlainText(functionEntity.introduction));
        return { contents, wordRange };
    }
}
exports.provideHover = provideHover;
//# sourceMappingURL=hover.js.map
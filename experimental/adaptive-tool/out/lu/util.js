"use strict";
/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
Object.defineProperty(exports, "__esModule", { value: true });
exports.isInFencedCodeBlock = exports.isLuFile = void 0;
const vscode_1 = require("vscode");
function isLuFile(fileName) {
    if (fileName === undefined || !fileName.toLowerCase().endsWith('.lu')) {
        return false;
    }
    return true;
}
exports.isLuFile = isLuFile;
function isInFencedCodeBlock(doc, position) {
    const textBefore = doc.getText(new vscode_1.Range(new vscode_1.Position(0, 0), position));
    const matches = textBefore.match(/```[\w ]*$/gm);
    if (matches == null) {
        return false;
    }
    else {
        return matches.length % 2 != 0;
    }
}
exports.isInFencedCodeBlock = isInFencedCodeBlock;
//# sourceMappingURL=util.js.map
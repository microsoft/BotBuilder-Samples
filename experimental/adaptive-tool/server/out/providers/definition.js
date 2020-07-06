"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.provideDefinition = void 0;
/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
const vscode_languageserver_1 = require("vscode-languageserver");
const templatesStatus_1 = require("../templatesStatus");
const util = require("../util");
const path = require("path");
function provideDefinition(params, documents) {
    const document = documents.get(params.textDocument.uri);
    const position = params.position;
    if (!util.isLgFile(path.basename(document.uri))) {
        return undefined;
    }
    try {
        const wordRange = util.getWordRangeAtPosition(document, position);
        if (!wordRange) {
            return undefined;
        }
        let templateName = document.getText(wordRange);
        if (templateName.startsWith('lg.')) {
            templateName = templateName.substr('lg.'.length);
        }
        const templates = util.getTemplatesFromCurrentLGFile(document.uri);
        const template = templates.allTemplates.find(u => u.name === templateName);
        if (template === undefined)
            return undefined;
        const lineNumber = template.sourceRange.range.start.line - 1;
        const columnNumber = template.sourceRange.range.start.character;
        const definitionPosition = { line: lineNumber, character: columnNumber };
        let definitionUri = undefined;
        templatesStatus_1.TemplatesStatus.templatesMap.forEach((value, key) => {
            if (template.sourceRange.source === key) {
                definitionUri = value.uri;
            }
        });
        if (definitionUri === undefined) {
            return undefined;
        }
        // return new Location(definitionUri, definitionPosition);
        return vscode_languageserver_1.Location.create(definitionUri, { start: definitionPosition, end: definitionPosition });
    }
    catch (e) {
        return undefined;
    }
}
exports.provideDefinition = provideDefinition;
//# sourceMappingURL=definition.js.map
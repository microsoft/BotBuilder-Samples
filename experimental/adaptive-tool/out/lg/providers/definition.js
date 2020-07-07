"use strict";
/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
Object.defineProperty(exports, "__esModule", { value: true });
exports.activate = void 0;
const vscode = require("vscode");
const util = require("../util");
const templatesStatus_1 = require("../templatesStatus");
function activate(context) {
    context.subscriptions.push(vscode.languages.registerDefinitionProvider('*', new LGDefinitionProvider()));
}
exports.activate = activate;
/**
 * Allow the user to see the definition of variables/functions/methods right where the variables/functions/methods are being used.
 * @see https://code.visualstudio.com/api/language-extensions/programmatic-language-features#show-definitions-of-a-symbol
 * @export
 * @class LGDefinitionProvider
 * @implements {vscode.DefinitionProvider}
 */
class LGDefinitionProvider {
    provideDefinition(document, position, token) {
        if (!util.isLgFile(document.fileName)) {
            return;
        }
        try {
            const wordRange = document.getWordRangeAtPosition(position, /[a-zA-Z0-9_.]+/);
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
            const definitionPosition = new vscode.Position(lineNumber, columnNumber);
            let definitionUri = undefined;
            templatesStatus_1.TemplatesStatus.templatesMap.forEach((value, key) => {
                if (template.sourceRange.source === key) {
                    definitionUri = value.uri;
                }
            });
            if (definitionUri === undefined) {
                return undefined;
            }
            return new vscode.Location(definitionUri, definitionPosition);
        }
        catch (e) {
            return undefined;
        }
    }
}
//# sourceMappingURL=definition.js.map
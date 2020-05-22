/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

import * as vscode from 'vscode';
import { Templates, Template} from 'botbuilder-lg';
import * as util from '../util';
import { TemplatesStatus } from '../templatesStatus';


export function activate(context: vscode.ExtensionContext) {
    context.subscriptions.push(vscode.languages.registerDefinitionProvider('*', new LGDefinitionProvider()));
}

/**
 * Allow the user to see the definition of variables/functions/methods right where the variables/functions/methods are being used.
 * @see https://code.visualstudio.com/api/language-extensions/programmatic-language-features#show-definitions-of-a-symbol
 * @export
 * @class LGDefinitionProvider
 * @implements {vscode.DefinitionProvider}
 */
class LGDefinitionProvider implements vscode.DefinitionProvider{
    provideDefinition(document: vscode.TextDocument, position: vscode.Position, token: vscode.CancellationToken): vscode.ProviderResult<vscode.Definition> {
        if (!util.isLgFile(document.fileName)) {
            return;
        }

        try {
            const wordRange = document.getWordRangeAtPosition(position, /[a-zA-Z0-9_\.]+/);
            if (!wordRange) {
                return undefined;
            }
            let templateName = document.getText(wordRange);
            if (templateName.startsWith('lg.')) {
                templateName = templateName.substr('lg.'.length);
            }

            const templates: Templates = util.getTemplatesFromCurrentLGFile(document.uri);
            let template: Template = templates.allTemplates.find(u=>u.name === templateName);
            if (template === undefined)
                return undefined;

            const lineNumber: number = template.sourceRange.range.start.line - 1;
            const columnNumber: number = template.sourceRange.range.start.character;
            const definitionPosition: vscode.Position = new vscode.Position(lineNumber, columnNumber);

            let definitionUri: vscode.Uri = undefined;
            TemplatesStatus.templatesMap.forEach((value, key) => {
                if (template.sourceRange.source === key) {
                    definitionUri = value.uri;
                }
            });

            if (definitionUri === undefined) {
                return undefined;
            }

            return new vscode.Location(definitionUri, definitionPosition);
        } catch(e){
            return undefined;
       }
    }
}

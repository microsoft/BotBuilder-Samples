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
exports.activate = void 0;
const luFilesStatus_1 = require("../luFilesStatus");
const entityEnum_1 = require("../entityEnum");
const vscode = require("vscode");
const util = require("../util");
const path = require("path");
const matchingPattern = require("../matchingPattern");
// eslint-disable-next-line @typescript-eslint/no-var-requires
const parseFile = require('@microsoft/bf-lu/lib/parser/lufile/parseFileContents.js').parseFile;
/**
 * Code completions provide context sensitive suggestions to the user.
 * @see https://code.visualstudio.com/api/language-extensions/programmatic-language-features#show-code-completion-proposals
 * @export
 * @class LGCompletionItemProvider
 * @implements [CompletionItemProvider](#vscode.CompletionItemProvider)
 */
function activate(context) {
    context.subscriptions.push(vscode.languages.registerCompletionItemProvider('*', new LUCompletionItemProvider(), '@', ' ', '{', ':', '[', '('));
}
exports.activate = activate;
class LUCompletionItemProvider {
    provideCompletionItems(document, position, token, context) {
        if (!util.isLuFile(document.fileName)) {
            return;
        }
        const fullContent = document.getText();
        const curLineContent = document.lineAt(position.line).text.substring(0, position.character);
        const lines = fullContent.split('\n');
        const textExceptCurLine = lines
            .slice(0, position.line)
            .concat(lines.slice(position.line + 1))
            .join('\n');
        const completionList = [];
        if (/\[[^\]]*\]\([^)]*$/.test(curLineContent)) {
            // []() import suggestion
            const paths = Array.from(new Set(luFilesStatus_1.LuFilesStatus.luFilesOfWorkspace));
            return paths.filter(u => u !== document.uri.fsPath).reduce((prev, curr) => {
                const relativePath = path.relative(path.dirname(document.uri.fsPath), curr);
                const item = new vscode.CompletionItem(relativePath, vscode.CompletionItemKind.Reference);
                item.detail = curr;
                prev.push(item);
                return prev;
            }, []);
        }
        if (matchingPattern.isEntityType(curLineContent)) {
            const entityTypes = entityEnum_1.EntityTypesObj.EntityType;
            entityTypes.forEach(entity => {
                const item = {
                    label: entity,
                    kind: vscode.CompletionItemKind.Keyword,
                    insertText: `${entity}`,
                    documentation: `Enitity type: ${entity}`,
                };
                completionList.push(item);
            });
        }
        if (matchingPattern.isPrebuiltEntity(curLineContent)) {
            const prebuiltTypes = entityEnum_1.EntityTypesObj.Prebuilt;
            prebuiltTypes.forEach(entity => {
                const item = {
                    label: entity,
                    kind: vscode.CompletionItemKind.Keyword,
                    insertText: `${entity}`,
                    documentation: `Prebuilt enitity: ${entity}`,
                };
                completionList.push(item);
            });
        }
        if (matchingPattern.isRegexEntity(curLineContent)) {
            const item = {
                label: 'RegExp Entity',
                kind: vscode.CompletionItemKind.Keyword,
                insertText: `//`,
                documentation: `regex enitity`,
            };
            completionList.push(item);
        }
        if (matchingPattern.isEntityName(curLineContent)) {
            const item = {
                label: 'hasRoles?',
                kind: vscode.CompletionItemKind.Keyword,
                insertText: `hasRoles`,
                documentation: `Entity name hasRole?`,
            };
            completionList.push(item);
            const item2 = {
                label: 'usesFeature?',
                kind: vscode.CompletionItemKind.Keyword,
                insertText: `usesFeature`,
                documentation: `Entity name usesFeature?`,
            };
            completionList.push(item2);
        }
        if (matchingPattern.isPhraseListEntity(curLineContent)) {
            const item = {
                label: 'interchangeable synonyms?',
                kind: vscode.CompletionItemKind.Keyword,
                insertText: `interchangeable`,
                documentation: `interchangeable synonyms as part of the entity definition`,
            };
            completionList.push(item);
        }
        // completion for entities and patterns, use the text without current line due to usually it will cause parser errors, the luisjson will be undefined
        if (completionList.length === 0) {
            return this.extractLUISContent(fullContent).then(luisJson => {
                if (!luisJson) {
                    this.extractLUISContent(textExceptCurLine).then(newluisJson => {
                        return this.processSuggestions(newluisJson, curLineContent, fullContent);
                    });
                }
                else {
                    return this.processSuggestions(luisJson, curLineContent, fullContent);
                }
            });
        }
        return completionList;
    }
    processSuggestions(luisJson, curLineContent, fullContent) {
        const suggestionEntityList = matchingPattern.getSuggestionEntities(luisJson, matchingPattern.suggestionAllEntityTypes);
        const regexEntityList = matchingPattern.getRegexEntities(luisJson);
        const completionList = [];
        //suggest a regex pattern for seperated line definition
        if (matchingPattern.isSeperatedEntityDef(curLineContent)) {
            const seperatedEntityDef = /^\s*@\s*([\w._]+|"[\w._\s]+")+\s*=\s*$/;
            let entityName = '';
            const matchGroup = curLineContent.match(seperatedEntityDef);
            if (matchGroup && matchGroup.length >= 2) {
                entityName = matchGroup[1].trim();
            }
            if (regexEntityList.includes(entityName)) {
                const item = {
                    label: 'RegExp Entity',
                    kind: vscode.CompletionItemKind.Keyword,
                    insertText: `//`,
                    documentation: `regex enitity`,
                };
                completionList.push(item);
            }
        }
        // auto suggest pattern
        if (matchingPattern.matchedEnterPattern(curLineContent)) {
            suggestionEntityList.forEach(name => {
                const item = {
                    label: `Entity: ${name}`,
                    kind: vscode.CompletionItemKind.Property,
                    insertText: `${name}`,
                    documentation: `pattern suggestion for entity: ${name}`,
                };
                completionList.push(item);
            });
        }
        // suggestions for entities in a seperated line
        if (matchingPattern.isEntityType(curLineContent)) {
            suggestionEntityList.forEach(entity => {
                const item = {
                    label: entity,
                    kind: vscode.CompletionItemKind.Property,
                    insertText: `${entity}`,
                    documentation: `Enitity type: ${entity}`,
                };
                completionList.push(item);
            });
        }
        if (matchingPattern.isCompositeEntity(curLineContent)) {
            matchingPattern.getSuggestionEntities(luisJson, matchingPattern.suggestionNoCompositeEntityTypes).forEach(entity => {
                const item = {
                    label: entity,
                    kind: vscode.CompletionItemKind.Property,
                    insertText: `${entity}`,
                    documentation: `Enitity type: ${entity}`,
                };
                completionList.push(item);
            });
        }
        const suggestionRolesList = matchingPattern.getSuggestionRoles(luisJson, matchingPattern.suggestionAllEntityTypes);
        // auto suggest roles
        if (matchingPattern.matchedRolesPattern(curLineContent)) {
            suggestionRolesList.forEach(name => {
                const item = {
                    label: `Role: ${name}`,
                    kind: vscode.CompletionItemKind.Property,
                    insertText: `${name}`,
                    documentation: `roles suggestion for entity name: ${name}`,
                };
                completionList.push(item);
            });
        }
        if (matchingPattern.matchedEntityPattern(curLineContent)) {
            suggestionEntityList.forEach(name => {
                const item = {
                    label: `Entity: ${name}`,
                    kind: vscode.CompletionItemKind.Property,
                    insertText: ` ${name}`,
                    documentation: `pattern suggestion for entity: ${name}`,
                };
                completionList.push(item);
            });
        }
        if (matchingPattern.matchedEntityCanUsesFeature(curLineContent, fullContent, luisJson)) {
            const enitityName = matchingPattern.extractEntityNameInUseFeature(curLineContent);
            const suggestionFeatureList = matchingPattern.getSuggestionEntities(luisJson, matchingPattern.suggestionNoPatternAnyEntityTypes);
            suggestionFeatureList.forEach(name => {
                if (name !== enitityName) {
                    const item = {
                        label: `Entity: ${name}`,
                        kind: vscode.CompletionItemKind.Method,
                        insertText: `${name}`,
                        documentation: `Feature suggestion for current entity: ${name}`,
                    };
                    completionList.push(item);
                }
            });
        }
        if (matchingPattern.matchIntentInEntityDef(curLineContent)) {
            const item = {
                label: 'usesFeature?',
                kind: vscode.CompletionItemKind.Keyword,
                insertText: `usesFeature`,
                documentation: `Does this intent usesFeature?`,
            };
            completionList.push(item);
        }
        if (matchingPattern.matchIntentUsesFeatures(curLineContent)) {
            const suggestionFeatureList = matchingPattern.getSuggestionEntities(luisJson, matchingPattern.suggestionNoPatternAnyEntityTypes);
            suggestionFeatureList.forEach(name => {
                const item = {
                    label: `Entity: ${name}`,
                    kind: vscode.CompletionItemKind.Method,
                    insertText: `${name}`,
                    documentation: `Feature suggestion for current entity: ${name}`,
                };
                completionList.push(item);
            });
        }
        return completionList;
    }
    extractLUISContent(text) {
        return __awaiter(this, void 0, void 0, function* () {
            let parsedContent;
            const log = false;
            const locale = 'en-us';
            try {
                parsedContent = yield parseFile(text, log, locale);
            }
            catch (e) {
                // nothing to do in catch block
            }
            if (parsedContent !== undefined) {
                return parsedContent.LUISJsonStructure;
            }
            else {
                return undefined;
            }
        });
    }
}
//# sourceMappingURL=completion.js.map
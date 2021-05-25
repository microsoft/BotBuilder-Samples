/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

/* eslint-disable @typescript-eslint/no-explicit-any */

import { LuFilesStatus } from '../luFilesStatus';
import { EntityTypesObj } from '../entityEnum';
import * as util from '../util';
import * as path from 'path';
import * as matchingPattern from '../matchingPattern';

import {
	CompletionItem, CompletionItemKind, TextDocumentPositionParams,	Files, TextDocuments
} from 'vscode-languageserver';

import {
	TextDocument
} from 'vscode-languageserver-textdocument';

// eslint-disable-next-line @typescript-eslint/no-var-requires
const parseFile = require('@microsoft/bf-lu/lib/parser/lufile/parseFileContents.js').parseFile;

/**
 * Code completions provide context sensitive suggestions to the user.
 * @see https://code.visualstudio.com/api/language-extensions/programmatic-language-features#show-code-completion-proposals
 * @export
 * @class LGCompletionItemProvider
 * @implements [CompletionItemProvider](#vscode.CompletionItemProvider)
 */
export function provideCompletionItems(_textDocumentPosition: TextDocumentPositionParams, documents: TextDocuments<TextDocument>) {
	const document = documents.get(_textDocumentPosition.textDocument.uri)!;
	const fspath = Files.uriToFilePath(document.uri);
	const position = _textDocumentPosition.position;
	const curLineContent = document?.getText({
		start: {
			line: position.line,
			character: 0
		},
		end: position
	}).toString();

    if(!util.isLuFile(path.basename(document.uri)) ) 
		return;

    const fullContent = document.getText();
    const lines = fullContent.split('\n');
	const textExceptCurLine = lines
		.slice(0, position.line)
		.concat(lines.slice(position.line + 1))
		.join('\n');

    const completionList: CompletionItem[] = [];

    if (/\[[^\]]*\]\([^)]*$/.test(curLineContent)) {
		// []() import suggestion
		const paths = Array.from(new Set(LuFilesStatus.luFilesOfWorkspace));

		return paths.filter(u => u !== fspath).reduce((prev : any[], curr: string) => {
			const relativePath = path.relative(path.dirname(fspath!), curr);
			const item = {
				label: relativePath, 
				kind: CompletionItemKind.Reference,
				detail: curr
			};
			prev.push(item);
			return prev;
		}, []);
	}

	// if (matchingPattern.isEntityType(curLineContent)) {
	// 	const entityTypes: string[] = EntityTypesObj.EntityType;
	// 	entityTypes.forEach(entity => {
	// 		const item = {
	// 			label: entity,
	// 			kind: CompletionItemKind.Keyword,
	// 			insertText: `${entity}`,
	// 			documentation: `Enitity type: ${entity}`,
	// 		};
	// 		completionList.push(item);
	// 	});
	// }

    if (matchingPattern.isPrebuiltEntity(curLineContent)) {
		const prebuiltTypes: string[] = EntityTypesObj.Prebuilt;
		prebuiltTypes.forEach(entity => {
			const item = {
				label: entity,
				kind: CompletionItemKind.Keyword,
				insertText: `${entity}`,
				documentation: `Prebuilt enitity: ${entity}`,
			};
			completionList.push(item);
		});
	}

    if (matchingPattern.isRegexEntity(curLineContent)) {
		const item = {
			label: 'RegExp Entity',
			kind: CompletionItemKind.Keyword,
			insertText: `//`,
			documentation: `regex enitity`,
		};
		completionList.push(item);
	}

    if (matchingPattern.isEntityName(curLineContent)) {
		const item = {
			label: 'hasRoles?',
			kind: CompletionItemKind.Keyword,
			insertText: `hasRoles`,
			documentation: `Entity name hasRole?`,
		};
		completionList.push(item);
		const item2 = {
			label: 'usesFeature?',
			kind: CompletionItemKind.Keyword,
			insertText: `usesFeature`,
			documentation: `Entity name usesFeature?`,
		};
		completionList.push(item2);
	}

    if (matchingPattern.isPhraseListEntity(curLineContent)) {
		const item = {
			label: 'interchangeable synonyms?',
			kind: CompletionItemKind.Keyword,
			insertText: `interchangeable`,
			documentation: `interchangeable synonyms as part of the entity definition`,
		};
		completionList.push(item);
	}

    if (matchingPattern.isEntityType(curLineContent)) {
		// @ can be followed by entity like ml, regex or entity
		const entityTypes: string[] = EntityTypesObj.EntityType;
		entityTypes.forEach(entity => {
			const item = {
				label: entity,
				kind: CompletionItemKind.Keyword,
				insertText: `${entity}`,
				documentation: `Enitity type: ${entity}`,
			};
			completionList.push(item);
		});
		return extractLUISContent(fullContent).then(
			luisJson => {
				if (!luisJson) {
					return extractLUISContent(textExceptCurLine).then(
						newluisJson => {
							return addEntities(newluisJson, completionList);}
					);
				} else {
					return addEntities(luisJson, completionList);
				}
			}
		);
    }
    // completion for entities and patterns, use the text without current line due to usually it will cause parser errors, the luisjson will be undefined
    if (completionList.length === 0){
		
		return extractLUISContent(fullContent).then(
			luisJson => {
				if (!luisJson) {
					extractLUISContent(textExceptCurLine).then(
						newluisJson => {
							return processSuggestions(newluisJson, curLineContent, fullContent);}
					)
				} else {
					return processSuggestions(luisJson, curLineContent, fullContent)
				}
			}
		);
	}
	
	return completionList;
}
    

function processSuggestions(luisJson: any, curLineContent: string, fullContent: string) {
	const suggestionEntityList = matchingPattern.getSuggestionEntities(luisJson, matchingPattern.suggestionAllEntityTypes);
	const regexEntityList = matchingPattern.getRegexEntities(luisJson);
	const completionList: CompletionItem[] = []
	
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
				kind: CompletionItemKind.Keyword,
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
				kind: CompletionItemKind.Property,
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
				kind: CompletionItemKind.Property,
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
				kind: CompletionItemKind.Property,
				insertText: `${entity}`,
				documentation: `Enitity type: ${entity}`,
			};
			completionList.push(item);
		});
	}
	
	const suggestionRolesList = matchingPattern.getSuggestionRoles(luisJson, matchingPattern.suggestionAllEntityTypes);
	// auto suggest roles
	if (matchingPattern.matchedRolesPattern(curLineContent) || matchingPattern.matchedEntityPattern(curLineContent)) {
		// {@ } or {entity: }
		suggestionRolesList.forEach(name => {
			const item = {
				label: `Role: ${name}`,
				kind: CompletionItemKind.Property,
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
				kind: CompletionItemKind.Property,
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
					kind: CompletionItemKind.Method,
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
			kind: CompletionItemKind.Keyword,
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
				kind: CompletionItemKind.Method,
				insertText: `${name}`,
				documentation: `Feature suggestion for current entity: ${name}`,
			};
			completionList.push(item);
		});
    }

	return completionList;
}

async function extractLUISContent(text: string): Promise<any> {
    let parsedContent: any;
    const log = false;
    const locale = 'en-us';
    try {
		parsedContent = await parseFile(text, log, locale);
    } catch (e) {
		//nothing to do in catch block
    }

    if (parsedContent !== undefined) {
		return parsedContent.LUISJsonStructure;
    } else {
		return undefined;
    }
}

function addEntities(luisJson: any, completionList: CompletionItem[]) : CompletionItem[] {
	matchingPattern.getSuggestionEntities(luisJson, matchingPattern.suggestionAllEntityTypes).forEach(name => {
		const item = {
			label: `Entity: ${name}`,
			kind: CompletionItemKind.Property,
			insertText: `${name}`,
			documentation: `pattern suggestion for entity: ${name}`,
		};
		completionList.push(item);
	});
	return completionList;
}

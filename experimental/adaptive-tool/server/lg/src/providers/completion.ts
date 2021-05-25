/* eslint-disable @typescript-eslint/no-non-null-assertion */
/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

import {
	CompletionItem, CompletionItemKind, TextDocumentPositionParams, TextDocuments
} from 'vscode-languageserver';

import {
	TextDocument, Position
} from 'vscode-languageserver-textdocument';
import { URI } from 'vscode-uri'
import * as util from '../util';
import { TemplatesStatus } from '../templatesStatus';
import * as path from 'path';

export function provideCompletionItems(_textDocumentPosition: TextDocumentPositionParams, documents: TextDocuments<TextDocument>): CompletionItem[] {
	const document = documents.get(_textDocumentPosition.textDocument.uri)!;
	const fspath = URI.parse(document.uri).fsPath;
	const position = _textDocumentPosition.position;
	const lineTextBefore = document?.getText({
		start: {
			line: position.line,
			character: 0
		},
		end: position
	}).toString();

	if (!util.isLgFile(path.basename(document.uri))) {
		return [];
	}

	if (/\[[^\]]*\]\([^)]*$/.test(lineTextBefore!) && !util.isInFencedCodeBlock(document!, position) && isExpression(lineTextBefore!)) {
		// []() import suggestion
		const paths = Array.from(new Set(TemplatesStatus.lgFilesOfWorkspace));

		return paths.filter(u => fspath !== u).reduce((prev : CompletionItem[], curr: string) => {
			const relativePath = path.relative(path.dirname(fspath!), curr);
			const item = {
				label: relativePath, 
				kind: CompletionItemKind.Reference,
				detail: curr
			};
			prev.push(item);
			return prev;
		}, []);
	} else if (/\$\{[^}]*$/.test(lineTextBefore!) && isExpression(lineTextBefore!)) {
		// buildin function prompt in expression
		const items: CompletionItem[] = [];
		const functions = util.getAllFunctions(document!.uri);
		functions.forEach((value, key) => {
			const completionItem = CompletionItem.create(key);
			const returnType = util.getreturnTypeStrFromReturnType(value.returntype);
			completionItem.detail = `${key}(${value.params.join(", ")}): ${returnType}`;
			completionItem.documentation = value.introduction;
			completionItem.insertText = `${key}(${value.params.map(u => u.split(':')[0].trim()).join(", ")})`;
			items.push(completionItem);
		});

		return items;
	} else if (isStartStructure(document!, position)) {
		// structure name and key suggestion
		const items: CompletionItem[] = [];
		util.cardTypes.forEach(value => {
			const completionItem = CompletionItem.create(value);
			completionItem.detail = `create ${value} structure`;
			let insertTextArray = util.cardPropDict.Others;
			// if (value === 'CardAction' || value === 'Suggestions' || value === 'Attachment' || value === 'Activity') {
			if (value === 'CardAction' || value === 'Attachment' || value === 'Activity') {
				insertTextArray = util.cardPropDict[value];
			} else if (value.endsWith('Card')){
				insertTextArray = util.cardPropDict.Cards;
			}
			completionItem.insertText = value + '\r\n' + insertTextArray.map(u => `\t${u.name} = ${u.placeHolder}`).join('\r\n') + '\r\n';
			items.push(completionItem);
		});

		return items;
	} else if (isInStructure(document!, position).isInStruct && /^\s*$/.test(lineTextBefore!)) {
		const structureName = isInStructure(document!, position).struType;
		const items: CompletionItem[] = []; 

		const nameToPropertiesMapping = Object.entries(util.cardPropDictFull);
		const propertiesMapping = nameToPropertiesMapping.find(u => u[0].toLowerCase() === structureName!.toLowerCase());
		if (propertiesMapping !== undefined) {
			const properties = propertiesMapping[1];
			properties.forEach(propertyItem => {
				const completionItem = CompletionItem.create(propertyItem.name);
				completionItem.detail = `create property ${propertyItem.name}`;
				const placeHolder = 'placeHolder' in propertyItem ? propertyItem['placeHolder'] : `${propertyItem.name}`;
				completionItem.insertText = propertyItem.name + ' = ' + placeHolder;
				items.push(completionItem);
			});
			return items;
		}
	}  else if (/^>\s!#/.test(lineTextBefore!)) {
		// options suggestion following "> !#"
		const items: CompletionItem[] = [];
		const optionDefinitionRegex = />\s!#\s*$/;
		if (optionDefinitionRegex.test(lineTextBefore!)) {
			AddToCompletion(util.optionsMap.options, items);
		} else {
			const strictRegex = />\s!#\s*@strict\s*=$/;
			const replaceNullRegex = />\s!#\s*@replaceNull\s*=$/;
			const lineBreakStyleRegex = />\s!#\s*@lineBreakStyle\s*=$/;
			const exportsFirstRegex = />\s!#\s*@Exports\s*=$/;
			const exportsMidRegex = />\s!#\s*@Exports\s*=/;
			if (strictRegex.test(lineTextBefore!)) {
				AddToCompletion(util.optionsMap.strictOptions, items);
			} else if (replaceNullRegex.test(lineTextBefore!)) {
				AddToCompletion(util.optionsMap.replaceNullOptions, items);
			} else if (lineBreakStyleRegex.test(lineTextBefore!)) {
				AddToCompletion(util.optionsMap.lineBreakStyleOptions, items);
			} else if (exportsFirstRegex.test(lineTextBefore!) || (exportsMidRegex.test(lineTextBefore!) && /,\s*$/.test(lineTextBefore!))) {
				const templatesOptions = TemplatesStatus.templatesMap.get(fspath!)!.templates.toArray();
				// const templatesOptions = Templates.parseFile(fspath!).toArray();
				for (let i=0;i<templatesOptions!.length;++i) {
					const templateName = templatesOptions![i].name;
					const completionItem = CompletionItem.create(" " + templateName);
					completionItem.detail = " " + templateName;
					completionItem.insertText = " " + templateName;
					items.push(completionItem);
				}
			}
		}
		return items;
	}
	return [];
}

function isExpression(lineTextBefore: string) {
    if (!lineTextBefore.trim().startsWith('-')) {
        return false;
	}
	const state : string[] = [];
	state.push("ROOT");
    let i = 0;
    while (i < lineTextBefore.length) {
		const char = lineTextBefore.charAt(i);
		if (char === `'`) {
			if (state[state.length - 1] === "EXPRESSION" || state[state.length - 1] === "DOUBLE") {
				state.push("SINGLE");
			} else {
				state.pop();
			}
		}
		if (char === `"`) {
			if (state[state.length - 1] === "EXPRESSION" || state[state.length - 1] === "SINGLE") {
				state.push("DOUBLE");
			} else {
				state.pop();
			}
		}
		i++;
	}
	if (state.pop() == "ROOT") {
		return true;
	} else {
		return false;
	}
}

function isStartStructure(document: TextDocument,
	position: Position) {
		const lineTextBefore = document?.getText({
			start: {
				line: position.line,
				character: 0
			},
			end: position
		});
		if (!lineTextBefore) {
			return false;
		}
		if (util.isInFencedCodeBlock(document, position)
				|| !(/^\s*\[[^\]]*$/.test(lineTextBefore))
				|| position.line <= 0)
			return false;
		
		let previourLine = position.line - 1;
		const currentOffset = document.offsetAt({
			line: position.line,
			character: 0
		});
		while(previourLine >= 0) {
			const previousLineText = document?.getText({
				start: {
					line: previourLine,
					character: 0
				},
				end: document.positionAt(currentOffset - 1) 
			}).toString().trim();
			if (previousLineText === '') {
				previourLine--;
				continue;
			} else if (previousLineText.startsWith('#')){
				return true;
			} else {
				return false;
			}
		}
		return false;
}

function isInStructure(document: TextDocument,
	position: Position): { isInStruct:boolean; struType:string|undefined } {
		if (util.isInFencedCodeBlock(document, position)
				|| position.line <= 0)
			return {isInStruct:false, struType:undefined};

		let previourLine = position.line - 1;
		const currentOffset = document.offsetAt({
			line: position.line,
			character: 0
		});
		while(previourLine >= 0) {
			const previousLineText = document?.getText({
				start: {
					line: previourLine,
					character: 0
				},
				end: document.positionAt(currentOffset - 1) 
			}).toString().trim();			
			if (previousLineText.startsWith('#')
			|| previousLineText === ']'
			|| previousLineText.startsWith('-')
			|| previousLineText.endsWith('```')) {
				return {isInStruct:false, struType:undefined};
			} else if (previousLineText.startsWith('[')){
				const structureType = previousLineText.substr(1).trim();
				return {isInStruct:true, struType:structureType};
			}
			previourLine--;
		}
		return {isInStruct:false, struType:undefined};
}

function AddToCompletion(options: any, items: CompletionItem[]) {
	for (const option in options) {
		const completionItem = CompletionItem.create(" " + option);
		completionItem.detail = options[option].detail;
		completionItem.documentation = options[option].documentation;
		completionItem.insertText = options[option].insertText;
		items.push(completionItem);
	}
}

/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

import {
	CompletionItem, CompletionItemKind, TextDocumentPositionParams,	Files, TextDocuments
} from 'vscode-languageserver';

import {
	TextDocument, Position
} from 'vscode-languageserver-textdocument';
import * as util from '../util';
import { TemplatesStatus } from '../templatesStatus';
import * as path from 'path';

export function provideCompletionItems(_textDocumentPosition: TextDocumentPositionParams, documents: TextDocuments<TextDocument>){
	const document = documents.get(_textDocumentPosition.textDocument.uri);
	const fspath = Files.uriToFilePath(document!.uri);
	const position = _textDocumentPosition.position;
	const lineTextBefore = document?.getText({
		start: {
			line: position.line,
			character: 0
		},
		end: position
	}).toString();

	if (/\[[^\]]*\]\([^)]*$/.test(lineTextBefore!) && !util.isInFencedCodeBlock(document!, position)) {
		// []() import suggestion
		const paths = Array.from(new Set(TemplatesStatus.lgFilesOfWorkspace));

		return paths.filter(u => fspath !== u).reduce((prev : any[], curr: string) => {
			const relativePath = path.relative(path.dirname(fspath!), curr);
			const item = {
				label: relativePath, 
				kind: CompletionItemKind.Reference,
				detail: curr
			};
			item.detail = curr;
			prev.push(item);
			return prev;
		}, []);
	} else if (/\$\{[^}]*$/.test(lineTextBefore!)) {
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

			completionItem.insertText = value + '\r\n' + insertTextArray.map(u => `\t${u.name}=${u.placeHolder}`).join('\r\n') + '\r\n';
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
				const placeHolder = 'placeHolder' in propertyItem ? propertyItem['placeHolder'] : `{${propertyItem.name}}`;
				completionItem.insertText = propertyItem.name + '=' + placeHolder;
				items.push(completionItem);
			});
			return items;
		}
	}  else if (/^>\s!#/.test(lineTextBefore!)) {
		// options suggestion following "> !#"
		const items: CompletionItem[] = [];
		if (/>\s!#$/.test(lineTextBefore!)) {
			AddToCompletion(optionsMap.options, items);
		} else {
			if (/>\s!#\s*@strict\s*=$/.test(lineTextBefore!)) {
				AddToCompletion(optionsMap.strictOptions, items);
			} else if (/>\s!#\s*@replaceNull\s*=$/.test(lineTextBefore!)) {
				AddToCompletion(optionsMap.replaceNullOptions, items);
			} else if (/>\s!#\s*@lineBreakStyle\s*=$/.test(lineTextBefore!)) {
				AddToCompletion(optionsMap.lineBreakStyleOptions, items);
			} else if (/>\s!#\s*@Exports\s*=$/.test(lineTextBefore!) || (/>\s!#\s*@Exports\s*=/.test(lineTextBefore!) && /,\s*$/.test(lineTextBefore!))) {
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

function isStartStructure(document: TextDocument,
	position: Position) {
		const lineTextBefore = document?.getText({
			start: {
				line: position.line,
				character: 0
			},
			end: position
		}).toString();
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

const optionsMap = {
    options: { 
        '@strict': {
            detail: ' @strict = true',
            documentation: 'Developers who do not want to allow a null evaluated result can implement the strict option.', 
            insertText: ' @strict'},
        '@replaceNull': {
            detail: ' @replaceNull = ${path} is undefined',
            documentation: 'Developers can create delegates to replace null values in evaluated expressions by using the replaceNull option.',
            insertText: ' @replaceNull'},
        '@lineBreakStyle': {
            detail: ' @lineBreakStyle = markdown',
            documentation: 'Developers can set options for how the LG system renders line breaks using the lineBreakStyle option.',
            insertText: ' @lineBreakStyle'},
        '@Namespace': {
            detail: ' @Namespace = foo',
            documentation: 'You can register a namespace for the LG templates you want to export.',
            insertText: ' @Namespace'},
        '@Exports': {
            detail: ' @Exports = template1, template2',
            documentation: 'You can specify a list of LG templates to export.',
            insertText: ' @Exports'}
    },
    strictOptions : {
        'true': {
            detail: ' true',
            documentation: 'Null error will throw a friendly message.',
            insertText: ' true'
        },
        'false': {
            detail: ' false',
            documentation: 'A compatible result will be given.',
            insertText: ' false'
        }
    },
    replaceNullOptions: {
        '${path} is undefined':{
            detail: 'The null input in the path variable would be replaced with ${path} is undefined.',
            documentation: null,
            insertText: ' ${path} is undefined'
        }    
    },
    lineBreakStyleOptions: {
        'default': {
            detail: ' default',
            documentation: 'Line breaks in multiline text create normal line breaks.',
            insertText: ' default'
        },
        'markdown': {
            detail: ' markdown',
            documentation: 'Line breaks in multiline text will be automatically converted to two lines to create a newline.',
            insertText: ' markdown'
        }
    } 
};

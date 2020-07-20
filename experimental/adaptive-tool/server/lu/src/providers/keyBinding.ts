/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
/* eslint-disable @typescript-eslint/no-explicit-any */
/* eslint-disable @typescript-eslint/no-non-null-assertion */
import {
	TextDocuments, ExecuteCommandParams, TextEdit, TextDocumentEdit, Connection
} from 'vscode-languageserver';

import {
	TextDocument, Position
} from 'vscode-languageserver-textdocument';

// eslint-disable-next-line @typescript-eslint/no-var-requires
const parseFile = require('@microsoft/bf-lu/lib/parser/lufile/parseFileContents.js').parseFile;

import * as util from '../util';

export function provideKeyBinding(params: ExecuteCommandParams, documents: TextDocuments<TextDocument>, connection: Connection) : void {
	const commandName = params.command;
	const args = params.arguments;

	if (commandName == 'lu.extension.labelingExperienceRequest' && args != undefined) {
		onLabelingExperienceRequest(args, documents, connection);
	} else if (commandName == 'lu.extension.onEnterKey' && args != undefined) {
		onEnterKey(args, documents, connection);
	} 
}

function onEnterKey(args: any[], documents: TextDocuments<TextDocument>, connection: Connection) {
	
	const document = documents.get(args[0]);
	const cursorPos: Position = args[1];
	
	const textBeforeCursor = document?.getText({
		start: {
			line: cursorPos.line,
			character: 0
		},
		end: cursorPos
	});

    const lineBreakPos = cursorPos;

	let matches: RegExpExecArray | { replace: (arg0: string, arg1: string) => void; }[];

	const match = textBeforeCursor?.match(/^\s*@\s*(\w+)\s*=\s*$/);
	if (match !== null) {
		const fullContent = document?.getText();
		const luContent = extractLUISContent(fullContent!);
		luContent.then(luisJson => {
			if (luisJson != null) {
				luisJson["entities"].forEach((entity : any) => {
					if (entity.name == match![1]) {
						const replacedStr = `\n\t- @ `;
						connection.workspace.applyEdit({
							documentChanges: [
								TextDocumentEdit.create({ uri: document!.uri, version: document!.version }, [
									TextEdit.insert(lineBreakPos, replacedStr)
								])
							]
						});
					}
				});
			} else {
				// it's not a ml entity, just add \n
				connection.workspace.applyEdit({
					documentChanges: [
						TextDocumentEdit.create({ uri: document!.uri, version: document!.version }, [
							TextEdit.insert(lineBreakPos, '\n')
						])
					]
				});
			}
		});
	} else if ((matches = /^(\s*)@\s*list\s*\S+\s*=\s*/.exec(textBeforeCursor!)!) !== null) {
		// in '@ list' line
		const replacedStr = `\n${matches[1]}\t- `;
		connection.workspace.applyEdit({
			documentChanges: [
				TextDocumentEdit.create({ uri: document!.uri, version: document!.version }, [
					TextEdit.insert(lineBreakPos, replacedStr)
				])
			]
		});
	} else if ((matches = /^(\s*-\s*@).*\S+.*/.exec(textBeforeCursor!)!) !== null) {
		// in '- @' line
		const replacedStr = `\n${matches[1]} `;
		connection.workspace.applyEdit({
			documentChanges: [
				TextDocumentEdit.create({ uri: document!.uri, version: document!.version }, [
					TextEdit.insert(lineBreakPos, replacedStr)
				])
			]
		});
	} else if ((matches = /^(\s*-\s*@)\s*$/.exec(textBeforeCursor!)!) !== null) {
        // in '- @' empty line, enter would delete the head dash
		const range = {start: {line: lineBreakPos.line, character: 0}, end: {line: lineBreakPos.line, character: lineBreakPos.character}};
		connection.workspace.applyEdit({
			documentChanges: [
				TextDocumentEdit.create({ uri: document!.uri, version: document!.version }, [
					TextEdit.del(range)
				])
			]
		});
	} else if ((matches = /^(\s*[-#]).*\S+.*/.exec(textBeforeCursor!)!) !== null && /^\s*-\s*@\s*$/.exec(textBeforeCursor!) == null) {
		// in '- ' line
		let replacedStr;
		if (/:\s*$/.exec(textBeforeCursor!) !== null) {
			replacedStr = `\n\t${matches[1].replace('#', '-')} `;
		} else {
			replacedStr = `\n${matches[1].replace('#', '-')} `;
		}
		connection.workspace.applyEdit({
			documentChanges: [
				TextDocumentEdit.create({ uri: document!.uri, version: document!.version }, [
					TextEdit.insert(lineBreakPos, replacedStr)
				])
			]
		});
	} else if ((matches = /^(\s*-)\s*$/.exec(textBeforeCursor!)!) !== null) {
        // in '-' empty line, enter would delete the head dash
		const range = {start: {line: lineBreakPos.line, character: 0}, end: {line: lineBreakPos.line, character: lineBreakPos.character}};
		connection.workspace.applyEdit({
			documentChanges: [
				TextDocumentEdit.create({ uri: document!.uri, version: document!.version }, [
					TextEdit.del(range)
				])
			]
		});
    } else {
		connection.workspace.applyEdit({
			documentChanges: [
				TextDocumentEdit.create({ uri: document!.uri, version: document!.version }, [
					TextEdit.insert(lineBreakPos, '\n')
				])
			]
		});
    }
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


function onLabelingExperienceRequest(args: any[], documents: TextDocuments<TextDocument>, connection: Connection) {
	
	const document = documents.get(args[0]);
	const endPos : Position = args[2];
	const lineBreakPos : Position = args[1];
	
	const curLineContent = document?.getText({
		start: {
			line: endPos.line,
			character: 0
		}, 
		end: endPos
	});

	const labeledUtterRegex = /\s*-([^{}]*\s*\{[\w.@:\s]+\s*=\s*[\w.\s]+\}[^{}]*)+$/;
	if (labeledUtterRegex.test(curLineContent!)) {
		const newText = util.replaceText(curLineContent!);
		const newPosition1 = {line: endPos.line, character: 0};
		const newPosition2 : Position = {
			line: newPosition1.line,
			character: endPos.character + 1
		};
		connection.workspace.applyEdit({
			documentChanges: [
				TextDocumentEdit.create({ uri: document!.uri, version: document!.version }, [
					TextEdit.insert(newPosition1, newText + '\n')
				]),
				TextDocumentEdit.create({ uri: document!.uri, version: document!.version }, [
					TextEdit.insert(newPosition2, '\n' + newText)
				]),
			]
		});
	} else {
		connection.workspace.applyEdit({
			documentChanges: [
				TextDocumentEdit.create({ uri: document!.uri, version: document!.version }, [
					TextEdit.insert(lineBreakPos, '\n')
				])
			]
		});
	}
}

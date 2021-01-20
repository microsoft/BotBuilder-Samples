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

export function provideKeyBinding(params: ExecuteCommandParams, documents: TextDocuments<TextDocument>, connection: Connection) : void {
	const commandName = params.command;
	const args = params.arguments;

	if (commandName == 'qna.extension.onEnterKey' && args != undefined) {
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

	if (/^(\s*)\*\*\s*(Filters|Prompts):\s*\*\*\s*/i.exec(textBeforeCursor!) !== null) {
		// in 'Filters' or 'Prompts' line
		const replacedStr = `\n- `;
		connection.workspace.applyEdit({
			documentChanges: [
				TextDocumentEdit.create({ uri: document!.uri, version: document!.version }, [
					TextEdit.insert(lineBreakPos, replacedStr)
				])
			]
		});
	} else if (/^(\s*[-#]).*\S+.*/.exec(textBeforeCursor!) !== null) {
		// in '# or - ' line		
		let	replacedStr = `\n- `;
		connection.workspace.applyEdit({
			documentChanges: [
				TextDocumentEdit.create({ uri: document!.uri, version: document!.version }, [
					TextEdit.insert(lineBreakPos, replacedStr)
				])
			]
		});
	} else if (/^(\s*-)\s*$/.exec(textBeforeCursor!) !== null) {
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

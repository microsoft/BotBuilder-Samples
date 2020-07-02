/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
import {
	TextDocuments, ExecuteCommandParams, TextEdit, TextDocumentEdit, Connection
} from 'vscode-languageserver';

import * as util from '../util';

import {
	TextDocument, Position
} from 'vscode-languageserver-textdocument';

export function provideKeyBinding(params: ExecuteCommandParams, documents: TextDocuments<TextDocument>, connection: Connection) {
	const commandName = params.command;
	const args = params.arguments;

	if(commandName == 'lg.extension.onEnterKey' && args != undefined) {
		onEnterKey(args, documents, connection);
	}
}

function onEnterKey(args: any[], documents: TextDocuments<TextDocument>, connection: Connection) {
	
	const document = documents.get(args[0]);
    // const editor = window.document.documentURI;//.activeTextEditor;
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
    if ((matches = /^(\s*-)\s?(IF|ELSE|SWITCH|CASE|DEFAULT|(ELSE\\s*IF))\s*:.*/i.exec(textBeforeCursor!)!) !== null && !util.isInFencedCodeBlock(document!, cursorPos)) {
		// -IF: new line would indent 
		const emptyNumber  = matches as RegExpExecArray;
		const dashIndex = '\n    ' + emptyNumber[1];
		connection.workspace.applyEdit({
			documentChanges: [
				TextDocumentEdit.create({ uri: document!.uri, version: document!.version }, [
					TextEdit.insert(lineBreakPos, dashIndex)
				])
			]
		});
		
    } else if ((matches = /^(\s*[#-]).*\S+.*/.exec(textBeforeCursor!)!) !== null && !util.isInFencedCodeBlock(document!, cursorPos)) {
		// in '- ' or '# ' line
		const replacedStr = `\n${matches[1].replace('#', '-')} `;
		connection.workspace.applyEdit({
			documentChanges: [
				TextDocumentEdit.create({ uri: document!.uri, version: document!.version }, [
					TextEdit.insert(lineBreakPos, replacedStr)
				])
			]
		});
	} else if ((matches = /^(\s*-)\s*/.exec(textBeforeCursor!)!) !== null && !util.isInFencedCodeBlock(document!, cursorPos)) {
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

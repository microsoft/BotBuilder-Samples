/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

/* eslint-disable @typescript-eslint/no-explicit-any */

import { QnaFilesStatus } from '../qnaFilesStatus';
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

    if(!util.isQnaFile(path.basename(document.uri))) 
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
		const paths = Array.from(new Set(QnaFilesStatus.qnaFilesOfWorkspace));

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
	
	// TODO: add auto completion for multiturn references

	return completionList;
}

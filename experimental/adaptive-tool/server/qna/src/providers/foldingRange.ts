import { FoldingRangeParams, FoldingRange, TextDocuments } from 'vscode-languageserver';
import { TextDocument } from 'vscode-languageserver-textdocument';
import * as path from 'path';
import * as util from '../util';

const parseFile = require('@microsoft/bf-lu/lib/parser/lufile/luParser');

export function foldingRange(params: FoldingRangeParams, documents: TextDocuments<TextDocument>) {
	const document = documents.get(params.textDocument.uri)!;
	const item: FoldingRange[] = [];
	if (!util.isQnaFile(path.basename(document.uri))) {
		return item;
	}

	const lineCount = document.lineCount;
	for (let i = 0; i < lineCount; i++) {
		const currLine = getCurrLine(document, lineCount, i);
		if (currLine?.startsWith('>>')) {
			for (let j = i + 1; j < lineCount; j++) {
				if (getCurrLine(document, lineCount, j)?.startsWith('>>')) {
					item.push(FoldingRange.create(i, j - 1));
					i = j;
					break;
				}
			}
		}
	}

	const qnaResource = parseFile.parse(document.getText());
	const sections = qnaResource.Sections;
	for (let section in qnaResource.Sections) {
		const start = sections[section].Range.Start.Line - 1;
		const end = sections[section].Range.End.Line - 1;
		item.push(FoldingRange.create(start, end));
	}

	return item;
}

function getCurrLine(document: TextDocument, lineCount: number, line: number) {
	if (line == lineCount) return null;
	const startPosition = { line: line, character: 0 };
	var endPosition;
	if (line == lineCount - 1) {
		endPosition = document.positionAt(document.getText().length - 1);
	} else {
		endPosition = document.positionAt(document.offsetAt({ line: line + 1, character: 0 }) - 1);
	}

	return document.getText({
		start: startPosition,
		end: endPosition
	});
}
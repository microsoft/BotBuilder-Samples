import { FoldingRangeParams, FoldingRange, TextDocuments } from 'vscode-languageserver';
import { TextDocument } from 'vscode-languageserver-textdocument';
import * as path from 'path';
import * as util from '../util';

export function foldingRange(params: FoldingRangeParams, documents: TextDocuments<TextDocument>) {
	const document = documents.get(params.textDocument.uri)!;
	const item: FoldingRange[] = [];
	if (!util.isLgFile(path.basename(document.uri))) {
		return item;
	}
	const lineCount = document.lineCount;
	for (let i = 0;i < lineCount;i++) {
		const currLine = getCurrLine(document, lineCount, i);
		if (currLine?.startsWith('>>')) {
			for (let j = i + 1;j < lineCount;j++) {
				if (getCurrLine(document, lineCount, j)?.startsWith('>>')) {
                    item.push(FoldingRange.create(i, j - 1));
                    i = j;
					break;
				}
			}
        }
     }
     for (let i = 0;i < lineCount;i++) {
        const currLine = getCurrLine(document, lineCount, i);
        let j = 0;
        if (currLine?.startsWith('#')) {
			for (j = i + 1;j < lineCount;j++) {
				const secLine = getCurrLine(document, lineCount, j);
				if (secLine?.startsWith('>>') || secLine?.startsWith('#')) {
                    item.push(FoldingRange.create(i, j - 1));
                    i = j - 1;
					break;
				}
            }
            if (i != j - 1) {
                item.push(FoldingRange.create(i, j - 1));
                i = j - 2;
            }
		}
	}
	return item;
}

function getCurrLine(document: TextDocument, lineCount: number, line: number) {
	if (line == lineCount) return null;
	const startPosition = {line: line, character: 0};
	let endPosition;
	if (line == lineCount - 1) {
		endPosition = document.positionAt(document.getText().length - 1);
	} else {
		endPosition = document.positionAt(document.offsetAt({line: line + 1, character: 0}) - 1);
	}
	return document.getText({
		start: startPosition,
		end: endPosition
	});
}
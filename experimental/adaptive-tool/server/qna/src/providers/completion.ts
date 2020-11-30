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

    if (matchingPattern.isImport(curLineContent)) {
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
	
	if (matchingPattern.isHash(curLineContent)) {
		const item = {
			label: `#?`,
			kind: CompletionItemKind.Keyword,
			insertText: `? `,
			documentation: '',
		};
		completionList.push(item);
	}

	if (matchingPattern.isFiltersOrPrompts(curLineContent)) {
		const item1 = {
			label: `**Prompts:**`,
			kind: CompletionItemKind.Property,
			insertText: `Prompts:**`,
			documentation: 'add prompts',
		};

		completionList.push(item1);

		const item2 = {
			label: `**Filters:**`,
			kind: CompletionItemKind.Property,
			insertText: `Filters:**`,
			documentation: 'add filters',
		};

		completionList.push(item2);
	}

	if (matchingPattern.isId(curLineContent)) {
		const item = {
			label: `<a id = ""></a>`,
			kind: CompletionItemKind.Keyword,
			insertText: `a id = ""></a>`,
			documentation: 'add id for QA pair',
		};
		completionList.push(item);
	}

	if (matchingPattern.isAnswer(curLineContent)) {
		const item = {
			label: `answer placeholder`,
			kind: CompletionItemKind.Keyword,
			insertText: `\n\`\`\``,
			documentation: 'answer placeholder',
		};
		completionList.push(item);
	}

	if (matchingPattern.isQASourceOrKBName(curLineContent)) {
		const item1 = {
			label: `@qna.pair.source = `,
			kind: CompletionItemKind.Property,
			insertText: `.pair.source = `,
			documentation: 'add QA pair source',
		};

		completionList.push(item1);

		const item2 = {
			label: `@kb.name = `,
			kind: CompletionItemKind.Property,
			insertText: `.name = `,
			documentation: 'add knowledge base name',
		};

		completionList.push(item2);
	}

	if (matchingPattern.isContextOnly(curLineContent)) {
		const item = {
			label: `\`context-only\``,
			kind: CompletionItemKind.Keyword,
			insertText: `\`context-only\``,
			documentation: 'context-only mark',
		};
		completionList.push(item);
	}

	if (matchingPattern.isMultiturnReference(curLineContent)) {
		return extractQnAContent(fullContent).then(
			qnaJson => {
				if (!qnaJson) {
					return extractQnAContent(textExceptCurLine).then(
						newQnaJson => {
							return curLineContent.endsWith('?') ? addQuestions(newQnaJson, completionList) : addIds(newQnaJson, completionList);
						});
				} else {
					return curLineContent.endsWith('?') ? addQuestions(qnaJson, completionList) : addIds(qnaJson, completionList);
				}
			}
		);
    }

	return completionList;
}

async function extractQnAContent(text: string): Promise<any> {
    let parsedContent: any;
    const log = false;
    const locale = 'en-us';
    try {
		parsedContent = await parseFile(text, log, locale);
    } catch (e) {
		//nothing to do in catch block
    }

    if (parsedContent !== undefined) {
		return parsedContent.qnaJsonStructure;
    } else {
		return undefined;
    }
}

function addQuestions(qnaJson: any, completionList: CompletionItem[]) : CompletionItem[] {
	getSuggestionQuestions(qnaJson).forEach(question => {
		const item = {
			label: `Question: ${question}`,
			kind: CompletionItemKind.Property,
			insertText: `${question.replace(/\s+/g, '-')}`,
			documentation: `question-answer pair reference suggestion`,
		};
		completionList.push(item);
	});
	return completionList;
}

function getSuggestionQuestions(qnaJson: any): string[] {
	const suggestionQuestionList: string[] = [];
	if (qnaJson !== undefined) {
		qnaJson.qnaList.forEach((qaPair: any) => {
			if (qaPair.questions && qaPair.questions.length > 0) {
				suggestionQuestionList.push(qaPair.questions[0])
			}
		});
	}

	return suggestionQuestionList;
}

function addIds(qnaJson: any, completionList: CompletionItem[]) : CompletionItem[] {
	getSuggestionIds(qnaJson).forEach(id => {
		const item = {
			label: `Question ID: ${id}`,
			kind: CompletionItemKind.Property,
			insertText: `${id}`,
			documentation: `question-answer pair reference suggestion`,
		};
		completionList.push(item);
	});
	return completionList;
}

function getSuggestionIds(qnaJson: any): string[] {
	const suggestionIdList: string[] = [];
	if (qnaJson !== undefined) {
		qnaJson.qnaList.forEach((qaPair: any) => {
			if (qaPair.id && (qaPair.id > 0 || qaPair.id > '0')) {
				suggestionIdList.push(qaPair.id)
			}
		});
	}

	return suggestionIdList;
}

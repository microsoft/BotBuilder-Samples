/* eslint-disable @typescript-eslint/no-non-null-assertion */
/* eslint-disable @typescript-eslint/explicit-module-boundary-types */

/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
import {
	TextDocuments, MarkedString, HoverParams
} from 'vscode-languageserver';

import * as util from '../util';
import * as path from 'path';

import { TextDocument } from 'vscode-languageserver-textdocument';

export function provideHover(params: HoverParams, documents: TextDocuments<TextDocument>) {

	const document = documents.get(params.textDocument.uri)!;
	const position = params.position;

	if (!util.isLgFile(path.basename(document.uri))) {
        return;
    }

	
	const wordRange = util.getWordRangeAtPosition(document, position);

	if (!wordRange) {
		return undefined;
	}

	const wordName = document.getText(wordRange);
	const functionEntity = util.getFunctionEntity(document.uri, wordName);

	if (functionEntity !== undefined) {
		const returnType = util.getreturnTypeStrFromReturnType(functionEntity.returntype);
		const functionIntroduction = `${wordName}(${functionEntity.params.join(", ")}): ${returnType}`;

		const contents = [];
		contents.push(MarkedString.fromPlainText(functionIntroduction));
		contents.push(MarkedString.fromPlainText(functionEntity.introduction));
		
		return {contents, wordRange};
	}
	return undefined;
}
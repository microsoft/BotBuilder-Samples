/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
import {
	TextDocuments, SignatureHelpParams, SignatureHelp, ParameterInformation, SignatureInformation
} from 'vscode-languageserver';

import * as util from '../util';
import * as path from 'path';

import {
	TextDocument, Position, Range
} from 'vscode-languageserver-textdocument';

export function provideSignatureHelp(params: SignatureHelpParams, documents: TextDocuments<TextDocument>) {

	const document = documents.get(params.textDocument.uri)!;
	const position = params.position;

	if (!util.isLgFile(path.basename(document.uri))) {
		return;
	}

	const {functionName, paramIndex} = parseFunction(document, position);
	const functionEntity = util.getFunctionEntity(document.uri, functionName);

	if (functionEntity === undefined) {
		return;
	}

	const paramInfoList: ParameterInformation[] = [];
	functionEntity.params.forEach(u => paramInfoList.push(ParameterInformation.create(u)));
	
	const returnType = util.getreturnTypeStrFromReturnType(functionEntity.returntype);
	const sigLabel = `${functionName}(${functionEntity.params.join(", ")}): ${returnType}`;
	const sigInfo = SignatureInformation.create(sigLabel);
	sigInfo.parameters = paramInfoList;
	
	const signatureHelp : SignatureHelp = {
		activeParameter: paramIndex,
		activeSignature: 0,
		signatures: [sigInfo]
	};

	return signatureHelp;
}

function parseFunction(document: TextDocument, position: Position) : {functionName:string, paramIndex: number}
{
	let functionName = "";
	let paramIndex = 0;
	const range: Range = {
		start: {
			line: position.line, 
			character: 0
		}, 
		end: position
	};
	const text: string = document.getText(range);
	let bracketsDiffNumber = 0;  // right bracket number - left bracket number, if bracketsDiffNumber === 0, present that is out of param inner scope
	for (let i = text.length - 1; i >= 0; i--) {
		const currentChar: string = text.charAt(i);
		if (currentChar === ',' && bracketsDiffNumber === 0) {
			paramIndex++;
		} else if (currentChar === '(' && bracketsDiffNumber === 0 && i > 0) {
			const wordRange = util.getWordRangeAtPosition(document, {line: position.line, character: i - 1});
			if (wordRange !== undefined) {
				functionName = document.getText(wordRange!);
				break;
			}
		} else if (currentChar === ')') {
			bracketsDiffNumber++;
		} else if (currentChar === '(') {
			bracketsDiffNumber--;
		}
	}

	return { functionName, paramIndex };
}

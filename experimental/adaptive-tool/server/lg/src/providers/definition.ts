/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
import {
	TextDocuments, Location, DefinitionParams
} from 'vscode-languageserver';

<<<<<<< HEAD
<<<<<<< HEAD
import { TemplatesStatus, TemplatesEntity } from '../templatesStatus';
=======
import { TemplatesStatus } from '../templatesStatus';
>>>>>>> 8f776b02b8003c84c4a871704de82c96b05e98b3
=======
import { TemplatesStatus } from '../templatesStatus';
>>>>>>> 94336950cf32e49b58ee0e5b488913f397b40137
import * as util from '../util';
import * as path from 'path';

import {
<<<<<<< HEAD
<<<<<<< HEAD
	TextDocument, Position, Range, DocumentUri
=======
	TextDocument, Position, DocumentUri
>>>>>>> 8f776b02b8003c84c4a871704de82c96b05e98b3
=======
	TextDocument, Position, DocumentUri
>>>>>>> 94336950cf32e49b58ee0e5b488913f397b40137
} from 'vscode-languageserver-textdocument';
import { Templates, Template } from 'botbuilder-lg';

export function provideDefinition(params: DefinitionParams, documents: TextDocuments<TextDocument>): null|undefined|Location {
	
	const document = documents.get(params.textDocument.uri)!;
	const position = params.position;
		
	if (!util.isLgFile(path.basename(document.uri))) {
		return undefined;
	}

	try {
		const wordRange = util.getWordRangeAtPosition(document, position);
		if (!wordRange) {
			return undefined;
		}
		let templateName = document.getText(wordRange);
		if (templateName.startsWith('lg.')) {
			templateName = templateName.substr('lg.'.length);
		}

		const templates: Templates = util.getTemplatesFromCurrentLGFile(document.uri);
		const template: Template|undefined = templates.allTemplates.find(u=>u.name === templateName);
		if (template === undefined)
			return undefined;

		const lineNumber: number = template.sourceRange.range.start.line - 1;
		const columnNumber: number = template.sourceRange.range.start.character;
		const definitionPosition: Position = {line: lineNumber, character: columnNumber};

		let definitionUri: DocumentUri|undefined = undefined;
		TemplatesStatus.templatesMap.forEach((value, key) => {
			if (template.sourceRange.source === key) {
				definitionUri = value.uri;
			}
		});

		if (definitionUri === undefined) {
			return undefined;
		}

		// return new Location(definitionUri, definitionPosition);
		return Location.create(definitionUri, {start: definitionPosition, end: definitionPosition});
	} catch(e){
		return undefined;
   }
}

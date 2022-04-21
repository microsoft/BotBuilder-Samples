// Type definitions for Visual Studio Code 1.46
// Project: https://github.com/microsoft/vscode
// Definitions by: Visual Studio Code Team, Microsoft <https://github.com/Microsoft>
// Definitions: https://github.com/DefinitelyTyped/DefinitelyTyped

/* --------------------------------------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License. See License.txt in the project root for license information.
 * ------------------------------------------------------------------------------------------ */

/* eslint-disable @typescript-eslint/no-non-null-assertion */

import {
	createConnection,
	TextDocuments,
	ProposedFeatures,
	InitializeParams,
	DidChangeConfigurationNotification,
	TextDocumentPositionParams,
	TextDocumentSyncKind,
	InitializeResult,
	Files,
	WorkspaceFolder,
	DidChangeWatchedFilesNotification,
	DidChangeWatchedFilesRegistrationOptions,
	ExecuteCommandParams,
	FoldingRangeParams
} from 'vscode-languageserver';

import * as completion from './providers/completion';
import * as diagnostics from './providers/diagnostics';
import * as keyBinding from './providers/keyBinding';
import * as foldingRange from './providers/foldingRange';

import * as util from './util';
import { LuFilesStatus } from './luFilesStatus';
import { TextDocument } from 'vscode-languageserver-textdocument';

// Create a connection for the server, using Node's IPC as a transport.
// Also include all preview / proposed LSP features.
const connection = createConnection(ProposedFeatures.all);

// Create a simple text document manager. 
const documents: TextDocuments<TextDocument> = new TextDocuments(TextDocument);
let workspaceFolders: WorkspaceFolder[] | null | undefined;

let hasConfigurationCapability = false;
let hasWorkspaceFolderCapability = false;
let hasDiagnosticRelatedInformationCapability = false;
let hasDidChangeWatchedFilesCapability = false;


connection.onInitialize((params: InitializeParams) => {
	const capabilities = params.capabilities;
	workspaceFolders = params.workspaceFolders;
	util.triggerLGFileFinder(workspaceFolders!);

	// Does the client support the `workspace/configuration` request?
	// If not, we fall back using global settings.
	hasConfigurationCapability = !!(
		capabilities.workspace && !!capabilities.workspace.configuration
	);
	hasWorkspaceFolderCapability = !!(
		capabilities.workspace && !!capabilities.workspace.workspaceFolders
	);
	hasDiagnosticRelatedInformationCapability = !!(
		capabilities.textDocument &&
		capabilities.textDocument.publishDiagnostics &&
		capabilities.textDocument.publishDiagnostics.relatedInformation
	);
	hasDidChangeWatchedFilesCapability = !!(
		capabilities.workspace && !! capabilities.workspace.didChangeWatchedFiles?.dynamicRegistration
	);

	const result: InitializeResult = {
		capabilities: {
			textDocumentSync: TextDocumentSyncKind.Incremental,
			// Tell the client that this server supports code completion.
			completionProvider: {
				// resolveProvider: true,
				triggerCharacters: [ '@', ' ', '{', ':', '[', '(' ]
			},
			executeCommandProvider: {
				commands: ['lu.extension.onEnterKey', 'lu.extension.labelingExperienceRequest']
			},
			foldingRangeProvider: true,
			workspace: {
				workspaceFolders: {
					supported: true
				}
			}
		}
	};
	if (hasWorkspaceFolderCapability) {
		result.capabilities.workspace = {
			workspaceFolders: {
				supported: true
			}
		};
	}
	return result;
});

connection.onInitialized(() => {
	if (hasConfigurationCapability) {
		// Register for all configuration changes.
		connection.client.register(DidChangeConfigurationNotification.type, undefined);
	}

	if(hasDidChangeWatchedFilesCapability) {	
		const option : DidChangeWatchedFilesRegistrationOptions = {watchers: [{globPattern: '**/*.lu'}]};
		connection.client.register(DidChangeWatchedFilesNotification.type, option);
	}

	if (hasWorkspaceFolderCapability) {
		connection.workspace.onDidChangeWorkspaceFolders(_event => {
			workspaceFolders = workspaceFolders?.filter(workspaceFolder => !_event.removed.includes(workspaceFolder));
			_event.added.forEach(folderAdded => workspaceFolders?.push(folderAdded));
			util.triggerLGFileFinder(workspaceFolders!);
			connection.console.log('Workspace folder change event received.');
		});
	}
});

// The example settings
interface LuSettings {
	maxNumberOfProblems: number;
}

// The global settings, used when the `workspace/configuration` request is not supported by the client.
// Please note that this is not the case when using this server with the client provided in this example
// but could happen with other clients.
const defaultSettings: LuSettings = { maxNumberOfProblems: 1000 };
let globalSettings: LuSettings = defaultSettings;

// Cache the settings of all open documents
const documentSettings: Map<string, Thenable<LuSettings>> = new Map();


connection.onDidChangeConfiguration(change => {
	if (hasConfigurationCapability) {
		// Reset all cached document settings
		documentSettings.clear();
	} else {
		globalSettings = <LuSettings>(
			(change.settings.languageServerExample || defaultSettings)
		);
	}

	// To update templatestatus with only open text documents
	util.triggerLGFileFinder(workspaceFolders!); 

	// Revalidate all open text documents
	documents.all().forEach(document => diagnostics.updateDiagnostics(document, connection));
});

// This handler provides the initial list of the completion items.
connection.onCompletion((_textDocumentPosition: TextDocumentPositionParams) => {
	return completion.provideCompletionItems(_textDocumentPosition, documents);
});

connection.onExecuteCommand((params: ExecuteCommandParams) =>{
	keyBinding.provideKeyBinding(params, documents, connection);
});

connection.onFoldingRanges((params: FoldingRangeParams) => {
	return foldingRange.foldingRange(params, documents);
})

documents.onDidOpen(e => {
	const filePath = Files.uriToFilePath(e.document.uri)!;
	if(!LuFilesStatus.luFilesOfWorkspace.includes(filePath)) {
		LuFilesStatus.luFilesOfWorkspace.push(filePath);
	}
});

// Only keep settings for open documents
documents.onDidClose(e => {
	documentSettings.delete(e.document.uri);
});

// The content of a text document has changed. This event is emitted
// when the text document first opened or when its content has changed.
documents.onDidChangeContent(change => {
	diagnostics.updateDiagnostics(change.document, connection);
});



connection.onDidChangeWatchedFiles(_change => {
	// // Monitored files have change in VSCode
	util.triggerLGFileFinder(workspaceFolders!);
	connection.console.log('We received an file change event');
});

// Make the text document manager listen on the connection
// for open, change and close text document events
documents.listen(connection);

// Listen on the connection
connection.listen();

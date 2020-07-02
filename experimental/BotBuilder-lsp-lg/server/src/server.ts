// Type definitions for Visual Studio Code 1.46
// Project: https://github.com/microsoft/vscode
// Definitions by: Visual Studio Code Team, Microsoft <https://github.com/Microsoft>
// Definitions: https://github.com/DefinitelyTyped/DefinitelyTyped

/* --------------------------------------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License. See License.txt in the project root for license information.
 * ------------------------------------------------------------------------------------------ */
import {
	createConnection,
	TextDocuments,
	ProposedFeatures,
	InitializeParams,
	DidChangeConfigurationNotification,
	CompletionItem,
	TextDocumentPositionParams,
	TextDocumentSyncKind,
	InitializeResult,
	Files,
	HoverParams,
	DefinitionParams,
	ExecuteCommandParams,
	SignatureHelpParams,
} from 'vscode-languageserver';

import * as completion from './providers/completion';
import * as diagnostics from './providers/diagnostics';
import * as definition from './providers/definition';
import * as hover from './providers/hover';
import * as keyBinding from './providers/keyBinding';
import * as signature from './providers/signature';

import { TemplatesStatus } from './templatesStatus';


import { TextDocument } from 'vscode-languageserver-textdocument';

// Create a connection for the server, using Node's IPC as a transport.
// Also include all preview / proposed LSP features.
const connection = createConnection(ProposedFeatures.all);

// Create a simple text document manager. 
const documents: TextDocuments<TextDocument> = new TextDocuments(TextDocument);

let hasConfigurationCapability = false;
let hasWorkspaceFolderCapability = false;
let hasDiagnosticRelatedInformationCapability = false;


connection.onInitialize((params: InitializeParams) => {
	const capabilities = params.capabilities;

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

	const result: InitializeResult = {
		capabilities: {
			textDocumentSync: TextDocumentSyncKind.Incremental,
			// Tell the client that this server supports code completion.
			completionProvider: {
				// resolveProvider: true,
				triggerCharacters: [ '{', '(', '[', '.', '\n', '#', '=', ',' ]
			},
			hoverProvider: true,
			definitionProvider: true,
			executeCommandProvider: {
				commands: ['lg.extension.onEnterKey']
			},
			signatureHelpProvider: {
				triggerCharacters: ['(', ',']
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

	if (hasWorkspaceFolderCapability) {
		connection.workspace.onDidChangeWorkspaceFolders(_event => {
			// _event.added.forEach(e => path..uri)
			connection.console.log('Workspace folder change event received.');
		});
	}
});

// The example settings
interface ExampleSettings {
	maxNumberOfProblems: number;
}

// The global settings, used when the `workspace/configuration` request is not supported by the client.
// Please note that this is not the case when using this server with the client provided in this example
// but could happen with other clients.
const defaultSettings: ExampleSettings = { maxNumberOfProblems: 1000 };
let globalSettings: ExampleSettings = defaultSettings;

// Cache the settings of all open documents
const documentSettings: Map<string, Thenable<ExampleSettings>> = new Map();


connection.onDidChangeConfiguration(change => {
	if (hasConfigurationCapability) {
		// Reset all cached document settings
		documentSettings.clear();
	} else {
		globalSettings = <ExampleSettings>(
			(change.settings.languageServerExample || defaultSettings)
		);
	}

	// To update templatestatus with only open text documents
	triggerLGFileFinder(); 

	// Revalidate all open text documents
	documents.all().forEach(document => diagnostics.updateDiagnostics(document, connection));
});

// This handler provides the initial list of the completion items.
connection.onCompletion(
	(_textDocumentPosition: TextDocumentPositionParams): CompletionItem[] => {
		return completion.provideCompletionItems(_textDocumentPosition, documents);
	}        
);

connection.onHover((params: HoverParams) => {
	return hover.provideHover(params, documents);
});

connection.onExecuteCommand((params: ExecuteCommandParams) =>{
	keyBinding.provideKeyBinding(params, documents, connection);
});

connection.onSignatureHelp((params: SignatureHelpParams) => {
	return signature.provideSignatureHelp(params, documents);
});


connection.onDefinition((params: DefinitionParams) => {
	return definition.provideDefinition(params, documents);
});

function triggerLGFileFinder() {
    TemplatesStatus.lgFilesOfWorkspace = [];
    documents.all().forEach(textDocument => {
        TemplatesStatus.lgFilesOfWorkspace.push(Files.uriToFilePath(textDocument.uri)!);
    });
}

function getDocumentSettings(resource: string): Thenable<ExampleSettings> {
	if (!hasConfigurationCapability) {
		return Promise.resolve(globalSettings);
	}
	triggerLGFileFinder();
	let result = documentSettings.get(resource);
	if (!result) {
		result = connection.workspace.getConfiguration({
			scopeUri: resource,
			section: 'languageServerExample'
		});
		documentSettings.set(resource, result);
	}
	return result;
}

// Only keep settings for open documents
documents.onDidClose(e => {
	documentSettings.delete(e.document.uri);
	// delete templates from map
	triggerLGFileFinder();
	if(TemplatesStatus.templatesMap.has(Files.uriToFilePath(e.document.uri)!)) {
		TemplatesStatus.templatesMap.delete(e.document.uri);
	}
});

// The content of a text document has changed. This event is emitted
// when the text document first opened or when its content has changed.
documents.onDidChangeContent(change => {
	triggerLGFileFinder();
	diagnostics.updateDiagnostics(change.document, connection);
});


connection.onDidChangeWatchedFiles(_change => {
	// Monitored files have change in VSCode
	connection.console.log('We received an file change event');
});

// Make the text document manager listen on the connection
// for open, change and close text document events
documents.listen(connection);

// Listen on the connection
connection.listen();

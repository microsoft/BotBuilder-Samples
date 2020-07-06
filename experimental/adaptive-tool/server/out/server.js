"use strict";
// Type definitions for Visual Studio Code 1.46
// Project: https://github.com/microsoft/vscode
// Definitions by: Visual Studio Code Team, Microsoft <https://github.com/Microsoft>
// Definitions: https://github.com/DefinitelyTyped/DefinitelyTyped
Object.defineProperty(exports, "__esModule", { value: true });
/* --------------------------------------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License. See License.txt in the project root for license information.
 * ------------------------------------------------------------------------------------------ */
const vscode_languageserver_1 = require("vscode-languageserver");
const completion = require("./providers/completion");
const diagnostics = require("./providers/diagnostics");
const definition = require("./providers/definition");
const hover = require("./providers/hover");
const keyBinding = require("./providers/keyBinding");
const signature = require("./providers/signature");
const util = require("./util");
const templatesStatus_1 = require("./templatesStatus");
const vscode_languageserver_textdocument_1 = require("vscode-languageserver-textdocument");
// Create a connection for the server, using Node's IPC as a transport.
// Also include all preview / proposed LSP features.
const connection = vscode_languageserver_1.createConnection(vscode_languageserver_1.ProposedFeatures.all);
// Create a simple text document manager. 
const documents = new vscode_languageserver_1.TextDocuments(vscode_languageserver_textdocument_1.TextDocument);
let workspaceFolders;
let hasConfigurationCapability = false;
let hasWorkspaceFolderCapability = false;
let hasDiagnosticRelatedInformationCapability = false;
let hasDidChangeWatchedFilesCapability = false;
connection.onInitialize((params) => {
    var _a;
    const capabilities = params.capabilities;
    workspaceFolders = params.workspaceFolders;
    util.triggerLGFileFinder(workspaceFolders);
    // Does the client support the `workspace/configuration` request?
    // If not, we fall back using global settings.
    hasConfigurationCapability = !!(capabilities.workspace && !!capabilities.workspace.configuration);
    hasWorkspaceFolderCapability = !!(capabilities.workspace && !!capabilities.workspace.workspaceFolders);
    hasDiagnosticRelatedInformationCapability = !!(capabilities.textDocument &&
        capabilities.textDocument.publishDiagnostics &&
        capabilities.textDocument.publishDiagnostics.relatedInformation);
    hasDidChangeWatchedFilesCapability = !!(capabilities.workspace && !!((_a = capabilities.workspace.didChangeWatchedFiles) === null || _a === void 0 ? void 0 : _a.dynamicRegistration));
    const result = {
        capabilities: {
            textDocumentSync: vscode_languageserver_1.TextDocumentSyncKind.Incremental,
            // Tell the client that this server supports code completion.
            completionProvider: {
                // resolveProvider: true,
                triggerCharacters: ['{', '(', '[', '.', '\n', '#', '=', ',', ' ']
            },
            hoverProvider: true,
            definitionProvider: true,
            executeCommandProvider: {
                commands: ['lg.extension.onEnterKey']
            },
            signatureHelpProvider: {
                triggerCharacters: ['(', ',']
            },
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
        connection.client.register(vscode_languageserver_1.DidChangeConfigurationNotification.type, undefined);
    }
    if (hasDidChangeWatchedFilesCapability) {
        const option = { watchers: [{ globPattern: '**/*.lg' }] };
        connection.client.register(vscode_languageserver_1.DidChangeWatchedFilesNotification.type, option);
    }
    if (hasWorkspaceFolderCapability) {
        connection.workspace.onDidChangeWorkspaceFolders(_event => {
            workspaceFolders = workspaceFolders === null || workspaceFolders === void 0 ? void 0 : workspaceFolders.filter(workspaceFolder => !_event.removed.includes(workspaceFolder));
            _event.added.forEach(folderAdded => workspaceFolders === null || workspaceFolders === void 0 ? void 0 : workspaceFolders.push(folderAdded));
            util.triggerLGFileFinder(workspaceFolders);
            connection.console.log('Workspace folder change event received.');
        });
    }
});
// The global settings, used when the `workspace/configuration` request is not supported by the client.
// Please note that this is not the case when using this server with the client provided in this example
// but could happen with other clients.
const defaultSettings = { maxNumberOfProblems: 1000 };
let globalSettings = defaultSettings;
// Cache the settings of all open documents
const documentSettings = new Map();
connection.onDidChangeConfiguration(change => {
    if (hasConfigurationCapability) {
        // Reset all cached document settings
        documentSettings.clear();
    }
    else {
        globalSettings = ((change.settings.languageServerExample || defaultSettings));
    }
    // To update templatestatus with only open text documents
    util.triggerLGFileFinder(workspaceFolders);
    // Revalidate all open text documents
    documents.all().forEach(document => diagnostics.updateDiagnostics(document, connection));
});
// This handler provides the initial list of the completion items.
connection.onCompletion((_textDocumentPosition) => {
    return completion.provideCompletionItems(_textDocumentPosition, documents);
});
connection.onHover((params) => {
    return hover.provideHover(params, documents);
});
connection.onExecuteCommand((params) => {
    keyBinding.provideKeyBinding(params, documents, connection);
});
connection.onSignatureHelp((params) => {
    return signature.provideSignatureHelp(params, documents);
});
connection.onDefinition((params) => {
    return definition.provideDefinition(params, documents);
});
function getDocumentSettings(resource) {
    if (!hasConfigurationCapability) {
        return Promise.resolve(globalSettings);
    }
    util.triggerLGFileFinder(workspaceFolders);
    let result = documentSettings.get(resource);
    if (!result) {
        result = connection.workspace.getConfiguration({
            scopeUri: resource,
            section: 'lg'
        });
        documentSettings.set(resource, result);
    }
    return result;
}
documents.onDidOpen(e => {
    const filePath = vscode_languageserver_1.Files.uriToFilePath(e.document.uri);
    if (!templatesStatus_1.TemplatesStatus.lgFilesOfWorkspace.includes(filePath)) {
        templatesStatus_1.TemplatesStatus.lgFilesOfWorkspace.push(filePath);
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
    // Monitored files have change in VSCode
    _change.changes.forEach(e => {
        if (e.type == vscode_languageserver_1.FileChangeType.Created) {
            util.triggerLGFileFinder(workspaceFolders);
        }
        else if (e.type == vscode_languageserver_1.FileChangeType.Deleted) {
            util.triggerLGFileFinder(workspaceFolders);
            if (templatesStatus_1.TemplatesStatus.templatesMap.has(vscode_languageserver_1.Files.uriToFilePath(e.uri))) {
                templatesStatus_1.TemplatesStatus.templatesMap.delete(vscode_languageserver_1.Files.uriToFilePath(e.uri));
            }
        }
    });
    connection.console.log('We received an file change event');
});
// Make the text document manager listen on the connection
// for open, change and close text document events
documents.listen(connection);
// Listen on the connection
connection.listen();
//# sourceMappingURL=server.js.map
"use strict";
/* --------------------------------------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License. See License.txt in the project root for license information.
 * ------------------------------------------------------------------------------------------ */
Object.defineProperty(exports, "__esModule", { value: true });
exports.deactivate = exports.activate = void 0;
const path = require("path");
const vscode_1 = require("vscode");
const vscode_languageclient_1 = require("vscode-languageclient");
let client;
function activate(context) {
    // The server is implemented in node
    const serverModule = context.asAbsolutePath(path.join('server', 'out', 'server.js'));
    // The debug options for the server
    // --inspect=6009: runs the server in Node's Inspector mode so VS Code can attach to the server for debugging
    const debugOptions = { execArgv: ['--nolazy', '--inspect=6010'] };
    // If the extension is launched in debug mode then the debug server options are used
    // Otherwise the run options are used
    const serverOptions = {
        run: { module: serverModule, transport: vscode_languageclient_1.TransportKind.ipc },
        debug: {
            module: serverModule,
            transport: vscode_languageclient_1.TransportKind.ipc,
            options: debugOptions
        }
    };
    // Options to control the language client
    const clientOptions = {
        // Register the server for .lg documents
        documentSelector: [{ scheme: 'file', language: 'lg' }],
        synchronize: {
            // Notify the server about file changes to '.clientrc files contained in the workspace
            fileEvents: vscode_1.workspace.createFileSystemWatcher('**/.clientrc')
        },
        middleware: {
            executeCommand: async (command, args, next) => {
                const editor = vscode_1.window.activeTextEditor;
                const cursorPos = editor.selection.active;
                const uri = editor.document.uri.toString();
                args.push(uri);
                args.push(cursorPos);
                next(command, args);
            },
            workspace: {
                didChangeWorkspaceFolders: ((data, next) => {
                    next(data);
                })
            }
        },
        diagnosticCollectionName: 'lg'
    };
    // Create the language client and start the client.
    client = new vscode_languageclient_1.LanguageClient('lg', 'Language Server For LG', serverOptions, clientOptions);
    // Start the client. This will also launch the server
    client.start();
    const collection = client.diagnostics;
    context.subscriptions.push(vscode_1.workspace.onDidCloseTextDocument(doc => {
        if (doc && isLgFile(doc.fileName)) {
            collection.delete(doc.uri);
        }
    }));
}
exports.activate = activate;
function deactivate() {
    if (!client) {
        return undefined;
    }
    return client.stop();
}
exports.deactivate = deactivate;
function isLgFile(fileName) {
    if (fileName === undefined || !fileName.toLowerCase().endsWith('.lg')) {
        return false;
    }
    return true;
}
//# sourceMappingURL=extension.js.map
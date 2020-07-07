"use strict";
/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.activate = void 0;
const vscode = require("vscode");
const cp = require("child_process");
const readline = require("readline");
const DebugType = 'bot';
function activate(context) {
    context.subscriptions.push(vscode.debug.registerDebugConfigurationProvider(DebugType, new DialogConfigurationProvider()));
    const factory = new DialogDebugAdapterDescriptorFactory();
    context.subscriptions.push(vscode.debug.registerDebugAdapterDescriptorFactory(DebugType, factory));
    context.subscriptions.push(factory);
}
exports.activate = activate;
class DialogConfigurationProvider {
    resolveDebugConfiguration(folder, config, token) {
        // TODO: any validation or fixes or UI we want to do around configurations
        // see https://github.com/Microsoft/vscode-mock-debug/blob/master/src/extension.ts
        return config;
    }
}
class DialogDebugAdapterDescriptorFactory {
    appendLine(line) {
        if (this.output !== undefined) {
            this.output.appendLine(line);
        }
        else {
            console.log(line);
        }
    }
    launch(session, executable, configuration) {
        return new Promise((resolve, reject) => {
            const options = {};
            const { workspaceFolder } = session;
            if (workspaceFolder !== undefined) {
                options.cwd = workspaceFolder.uri.fsPath;
            }
            const { command, args } = configuration;
            this.handle = cp.spawn(command, args, options);
            const onReject = (target, event) => {
                target.on(event, (...items) => {
                    const message = `createDebugAdapterDescriptor: ${event}: ${items.join(',')}`;
                    this.appendLine(message);
                    reject(message);
                });
            };
            onReject(this.handle, 'error');
            onReject(this.handle, 'close');
            onReject(this.handle, 'exit');
            const stdout = readline.createInterface(this.handle.stdout);
            const stderr = readline.createInterface(this.handle.stderr);
            stdout.on('line', line => this.appendLine(line));
            stderr.on('line', line => this.appendLine(line));
            stdout.on('line', line => {
                const match = /^DebugTransport\t([^\t]+)\t(\d+)$/.exec(line);
                if (match !== null) {
                    const host = match[1];
                    const port = Number.parseInt(match[2], 10);
                    resolve(new vscode.DebugAdapterServer(port, host));
                }
            });
        });
    }
    attach(session, executable, configuration) {
        // note: we don't see attach here is debugServer is set
        return new Promise((resolve, reject) => {
            resolve(new vscode.DebugAdapterServer(4712));
        });
    }
    createDebugAdapterDescriptor(session, executable) {
        return __awaiter(this, void 0, void 0, function* () {
            this.output = vscode.window.createOutputChannel('Bot Debugger');
            this.output.clear();
            this.output.show();
            const configuration = session.configuration;
            switch (configuration.request) {
                case 'attach': return yield this.attach(session, executable, configuration);
                case 'launch': return yield this.launch(session, executable, configuration);
                default: throw new Error();
            }
        });
    }
    dispose() {
        if (this.handle !== undefined) {
            this.handle.kill();
        }
        if (this.output !== undefined) {
            this.output.dispose();
        }
    }
}
//# sourceMappingURL=dialogDebugAdapter.js.map
/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

import * as vscode from 'vscode';
import * as cp from 'child_process';
import * as readline from 'readline';

const DebugType = 'bot';

export function activate(context: vscode.ExtensionContext) {
    context.subscriptions.push(vscode.debug.registerDebugConfigurationProvider(DebugType, new DialogConfigurationProvider()));

    const factory = new DialogDebugAdapterDescriptorFactory();
    context.subscriptions.push(vscode.debug.registerDebugAdapterDescriptorFactory(DebugType, factory));
    context.subscriptions.push(factory);
}

// make sure these are kept in sync with contributes.debuggers from the extension's package.json.
export interface AttachConfiguration extends vscode.DebugConfiguration {
    request: 'attach';
}

export interface LaunchConfiguration extends vscode.DebugConfiguration {
    request: 'launch';
    command: string;
    args: Array<string>;
}

export type Configuration = AttachConfiguration | LaunchConfiguration;

class DialogConfigurationProvider implements vscode.DebugConfigurationProvider {
    resolveDebugConfiguration(folder: vscode.WorkspaceFolder | undefined, config: Configuration, token?: vscode.CancellationToken): vscode.ProviderResult<Configuration> {
        // TODO: any validation or fixes or UI we want to do around configurations
        // see https://github.com/Microsoft/vscode-mock-debug/blob/master/src/extension.ts
        return config;
    }
}

class DialogDebugAdapterDescriptorFactory implements vscode.DebugAdapterDescriptorFactory {

    private handle?: cp.ChildProcessWithoutNullStreams;
    private output?: vscode.OutputChannel;

    private appendLine(line: string): void {
        if (this.output !== undefined) {
            this.output.appendLine(line);
        }
        else {
            console.log(line);
        }
    }

    private launch(session: vscode.DebugSession, executable: vscode.DebugAdapterExecutable | undefined, configuration: LaunchConfiguration): Promise<vscode.DebugAdapterDescriptor> {
        return new Promise<vscode.DebugAdapterDescriptor>((resolve, reject) => {
            let options: cp.SpawnOptionsWithoutStdio = {};

            const { workspaceFolder } = session;

            if (workspaceFolder !== undefined) {
                options.cwd = workspaceFolder.uri.fsPath;
            }

            const { command, args } = configuration;
            this.handle = cp.spawn(command, args, options);

            type Listener = (...args: any[]) => void;
            type Target = { on: (event: string, Listener: Listener) => void };
            const onReject = (target: Target, event: string) => {
                target.on(event, (...items) => {
                    const message = `createDebugAdapterDescriptor: ${event}: ${items.join(',')}`;
                    this.appendLine(message);
                    reject(message);
                })
            }

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

    private attach(session: vscode.DebugSession, executable: vscode.DebugAdapterExecutable | undefined, configuration: AttachConfiguration): Promise<vscode.DebugAdapterDescriptor> {
        // note: we don't see attach here is debugServer is set
        return new Promise<vscode.DebugAdapterDescriptor>((resolve, reject) => {
            resolve(new vscode.DebugAdapterServer(4712));
        });
    }

    async createDebugAdapterDescriptor(session: vscode.DebugSession, executable: vscode.DebugAdapterExecutable | undefined): Promise<vscode.DebugAdapterDescriptor> {
        this.output = vscode.window.createOutputChannel('Bot Debugger');
        this.output.clear();
        this.output.show();

        const configuration = session.configuration as Configuration;
        switch (configuration.request) {
            case 'attach': return await this.attach(session, executable, configuration);
            case 'launch': return await this.launch(session, executable, configuration);
            default: throw new Error();
        }
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

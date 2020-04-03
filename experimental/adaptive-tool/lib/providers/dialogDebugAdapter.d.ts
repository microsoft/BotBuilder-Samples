/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
import * as vscode from 'vscode';
export declare function activate(context: vscode.ExtensionContext): void;
export interface AttachConfiguration extends vscode.DebugConfiguration {
    request: 'attach';
}
export interface LaunchConfiguration extends vscode.DebugConfiguration {
    request: 'launch';
    command: string;
    args: Array<string>;
}
export declare type Configuration = AttachConfiguration | LaunchConfiguration;
//# sourceMappingURL=dialogDebugAdapter.d.ts.map
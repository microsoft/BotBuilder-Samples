/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

import * as vscode from 'vscode';
import * as completion from './providers/completion';
import * as diagnostics from './providers/diagnostics';
import * as luParser from './providers/luParser';

/**
 * Main vs code Extension code part
 *
 * @export
 * @param {vscode.ExtensionContext} context
 */
export async function activate(context: vscode.ExtensionContext) {
    await luParser.activate(context);
    completion.activate(context);
    diagnostics.activate(context);
}
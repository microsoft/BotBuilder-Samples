/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

import * as vscode from 'vscode';
import * as completion from './providers/completion';
import * as diagnostics from './providers/diagnostics';
import * as eventsTrigger from './providers/eventsTrigger';

/**
 * Main vs code Extension code part
 *
 * @export
 * @param {vscode.ExtensionContext} context
 */
export function activate(context: vscode.ExtensionContext) {
    completion.activate(context);
    diagnostics.activate(context);
    eventsTrigger.activate(context);
}

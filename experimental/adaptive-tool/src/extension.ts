/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

import * as vscode from 'vscode';
import * as lgExtension from './lg/lgExtension';
import * as luExtension from './lu/luExtension';
import * as debuggerExtension from './debugger/debuggerExtension';

/**
 * Main vs code Extension code part
 *
 * @export
 * @param {vscode.ExtensionContext} context
 */
export function activate(context: vscode.ExtensionContext) {
    lgExtension.activate(context);
    luExtension.activate(context);
    debuggerExtension.activate(context);
}

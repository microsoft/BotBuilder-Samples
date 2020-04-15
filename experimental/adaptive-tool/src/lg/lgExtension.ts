/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

import * as vscode from 'vscode';
import * as keyBinding from './providers/keyBinding';
import * as completion from './providers/completion';
import * as diagnostics from './providers/diagnostics';
import * as templatesParser from './providers/templatesParser';
import * as definition from './providers/definition';
import * as hover from './providers/hover';
import * as signature from './providers/signature';
import * as debugPanel from './providers/debugPanel';

/**
 * Main vs code Extension code part
 *
 * @export
 * @param {vscode.ExtensionContext} context
 */
export function activate(context: vscode.ExtensionContext) {
    keyBinding.activate(context);
    completion.activate(context);
    diagnostics.activate(context);
    templatesParser.activate(context);
    definition.activate(context);
    hover.activate(context);
    signature.activate(context);
    debugPanel.activate(context);
}
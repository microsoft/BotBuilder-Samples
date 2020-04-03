"use strict";
/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
Object.defineProperty(exports, "__esModule", { value: true });
const keyBinding = require("./providers/keyBinding");
const completion = require("./providers/completion");
const diagnostics = require("./providers/diagnostics");
const templateEngineParser = require("./providers/templateEngineParser");
const definition = require("./providers/definition");
const hover = require("./providers/hover");
const signature = require("./providers/signature");
const debugPanel = require("./providers/debugPanel");
const dialogDebugAdapter = require("./providers/dialogDebugAdapter");
/**
 * Main vs code Extension code part
 *
 * @export
 * @param {vscode.ExtensionContext} context
 */
function activate(context) {
    keyBinding.activate(context);
    completion.activate(context);
    diagnostics.activate(context);
    templateEngineParser.activate(context);
    definition.activate(context);
    hover.activate(context);
    signature.activate(context);
    debugPanel.activate(context);
    dialogDebugAdapter.activate(context);
}
exports.activate = activate;
//# sourceMappingURL=extension.js.map
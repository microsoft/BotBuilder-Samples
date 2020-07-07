"use strict";
/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
Object.defineProperty(exports, "__esModule", { value: true });
exports.activate = void 0;
const keyBinding = require("./providers/keyBinding");
const completion = require("./providers/completion");
const diagnostics = require("./providers/diagnostics");
const eventsTrigger = require("./providers/eventsTrigger");
const definition = require("./providers/definition");
const hover = require("./providers/hover");
const debugPanel = require("./providers/debugPanel");
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
    eventsTrigger.activate(context);
    definition.activate(context);
    hover.activate(context);
    //signature.activate(context);
    debugPanel.activate(context);
}
exports.activate = activate;
//# sourceMappingURL=lgExtension.js.map
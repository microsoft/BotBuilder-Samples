"use strict";
/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
Object.defineProperty(exports, "__esModule", { value: true });
exports.activate = void 0;
const completion = require("./providers/completion");
const diagnostics = require("./providers/diagnostics");
const eventsTrigger = require("./providers/eventsTrigger");
/**
 * Main vs code Extension code part
 *
 * @export
 * @param {vscode.ExtensionContext} context
 */
function activate(context) {
    completion.activate(context);
    diagnostics.activate(context);
    eventsTrigger.activate(context);
}
exports.activate = activate;
//# sourceMappingURL=luExtension.js.map
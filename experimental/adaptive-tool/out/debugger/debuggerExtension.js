"use strict";
/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
Object.defineProperty(exports, "__esModule", { value: true });
exports.activate = void 0;
const dialogDebugAdapter = require("./dialogDebugAdapter");
/**
 * Main vs code Extension code part
 *
 * @export
 * @param {vscode.ExtensionContext} context
 */
function activate(context) {
    dialogDebugAdapter.activate(context);
}
exports.activate = activate;
//# sourceMappingURL=debuggerExtension.js.map
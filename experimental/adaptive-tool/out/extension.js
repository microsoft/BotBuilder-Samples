"use strict";
/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
Object.defineProperty(exports, "__esModule", { value: true });
exports.activate = void 0;
const lgExtension = require("./lg/lgExtension");
const luExtension = require("./lu/luExtension");
const debuggerExtension = require("./debugger/debuggerExtension");
/**
 * Main vs code Extension code part
 *
 * @export
 * @param {vscode.ExtensionContext} context
 */
function activate(context) {
    lgExtension.activate(context);
    luExtension.activate(context);
    debuggerExtension.activate(context);
}
exports.activate = activate;
//# sourceMappingURL=extension.js.map
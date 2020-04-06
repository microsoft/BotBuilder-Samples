"use strict";
/*!
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
Object.defineProperty(exports, "__esModule", { value: true });
const bf_cli_command_1 = require("@microsoft/bf-cli-command");
class Index extends bf_cli_command_1.Command {
    async run() {
        this._help();
    }
}
exports.default = Index;
Index.description = 'The dialog commands allow you to work with dialog schema.';
Index.flags = {
    help: bf_cli_command_1.flags.help({ char: 'h' }),
};
//# sourceMappingURL=index.js.map
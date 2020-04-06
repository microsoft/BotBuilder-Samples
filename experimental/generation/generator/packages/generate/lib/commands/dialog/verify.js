"use strict";
/*!
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
Object.defineProperty(exports, "__esModule", { value: true });
const bf_cli_command_1 = require("@microsoft/bf-cli-command");
const chalk = require("chalk");
const dialogTracker_1 = require("../../library/dialogTracker");
// import * as process from 'process';
class DialogVerify extends bf_cli_command_1.Command {
    constructor() {
        super(...arguments);
        this.currentFile = '';
        this.files = 0;
        this.errors = 0;
        this.warnings = 0;
    }
    async run() {
        const { argv, flags } = this.parse(DialogVerify);
        await this.execute(argv, flags.verbose);
    }
    async execute(dialogFiles, verbose) {
        const schema = new dialogTracker_1.SchemaTracker();
        const tracker = new dialogTracker_1.DialogTracker(schema);
        await tracker.addDialogFiles(dialogFiles);
        if (tracker.dialogs.length === 0) {
            this.error('No  dialogs found!');
        }
        else {
            for (let dialog of tracker.dialogs) {
                this.files++;
                this.currentFile = dialog.file;
                if (dialog.errors.length === 0) {
                    if (verbose) {
                        this.consoleLog(`${dialog}`);
                    }
                }
                else {
                    for (let error of dialog.errors) {
                        this.consoleError(`${error.message.trim()}`, 'DLG001');
                    }
                }
            }
            for (let defs of tracker.multipleDefinitions()) {
                let def = defs[0];
                this.consoleError(`Multiple definitions for ${def} ${def.usedByString()}`, 'DLG002');
                for (let def of defs) {
                    this.consoleError(`  ${def.pathString()}`, 'DLG002');
                }
            }
            for (let def of tracker.missingDefinitions()) {
                this.consoleError(`Missing definition for ${def} ${def.usedByString()}`, 'DLG003');
            }
            for (let def of tracker.missingTypes) {
                this.consoleError(`Missing $kind for ${def}`, 'DLG004');
            }
            for (let def of tracker.unusedIDs()) {
                this.consoleWarn(`Unused id ${def}`, 'DLG005');
            }
            if (verbose) {
                for (let [type, definitions] of tracker.typeToDef) {
                    this.consoleMsg(`Instances of ${type}`);
                    for (let def of definitions) {
                        this.consoleMsg(`  ${def.locatorString()}`);
                    }
                }
            }
            this.log(`${this.files} files processed.`);
            this.error(`${this.warnings} found.`);
            if (this.errors > 0) {
                this.error(`Error: ${this.errors} found.`);
            }
        }
    }
    consoleMsg(msg) {
        this.log(chalk.default(msg));
    }
    consoleLog(msg) {
        this.log(chalk.default.gray(msg));
    }
    consoleWarn(msg, code) {
        this.warnings++;
        this.warn(`${this.currentFile} - warning ${code || ''}: ${msg}`);
    }
    consoleError(msg, code) {
        this.errors++;
        this.error(`${this.currentFile} - error ${code || ''}: ${msg}`);
    }
}
exports.default = DialogVerify;
DialogVerify.args = [
    { name: 'glob1', required: true },
    { name: 'glob2', required: false },
    { name: 'glob3', required: false },
    { name: 'glob4', required: false },
    { name: 'glob5', required: false },
    { name: 'glob6', required: false },
    { name: 'glob7', required: false },
    { name: 'glob8', required: false },
    { name: 'glob9', required: false },
];
DialogVerify.flags = {
    help: bf_cli_command_1.flags.help({ char: 'h' }),
    verbose: bf_cli_command_1.flags.boolean({ description: 'Show verbose output', default: false }),
};
//# sourceMappingURL=verify.js.map
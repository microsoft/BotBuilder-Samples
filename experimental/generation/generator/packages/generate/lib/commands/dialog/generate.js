"use strict";
/*!
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
Object.defineProperty(exports, "__esModule", { value: true });
const bf_cli_command_1 = require("@microsoft/bf-cli-command");
const gen = require("../../library/dialogGenerator");
class GenerateDialog extends bf_cli_command_1.Command {
    async run() {
        const { args, flags } = this.parse(GenerateDialog);
        try {
            await gen.generate(args.schema, flags.prefix, flags.output, flags.schema, flags.locale, flags.templates, flags.force, (type, msg) => {
                if (type === gen.FeedbackType.message
                    || (type === gen.FeedbackType.info && flags.verbose)) {
                    this.info(msg);
                }
                else if (type === gen.FeedbackType.warning) {
                    this.warning(msg);
                }
                else if (type === gen.FeedbackType.error) {
                    this.errorMsg(msg);
                }
            });
            return true;
        }
        catch (e) {
            this.thrownError(e);
        }
    }
    thrownError(err) {
        this.error(err.message);
    }
    info(msg) {
        console.error(msg);
    }
    warning(msg) {
        this.warn(msg);
    }
    errorMsg(msg) {
        this.error(msg);
    }
}
exports.default = GenerateDialog;
GenerateDialog.description = '[PREVIEW] Generate localized .lu, .lg, .qna and .dialog assets to define a bot based on a schema using templates.';
GenerateDialog.examples = [`
      $ bf dialog:generate sandwich.schema --output c:/tmp
    `];
GenerateDialog.args = [
    { name: 'schema', required: true, description: 'JSON Schema .schema file used to drive generation.' }
];
GenerateDialog.flags = {
    force: bf_cli_command_1.flags.boolean({ char: 'f', description: 'Force overwriting generated files.' }),
    help: bf_cli_command_1.flags.help({ char: 'h' }),
    locale: bf_cli_command_1.flags.string({ char: 'l', description: 'Locales to generate. [default: en-us]', multiple: true }),
    output: bf_cli_command_1.flags.string({ char: 'o', description: 'Output path for where to put generated .lu, .lg, .qna and .dialog files. [default: .]', default: '.', required: false }),
    prefix: bf_cli_command_1.flags.string({ char: 'p', description: 'Prefix to use for generated files. [default: schema name]' }),
    schema: bf_cli_command_1.flags.string({ char: 's', description: 'Path to your app.schema file.', required: false }),
    templates: bf_cli_command_1.flags.string({ char: 't', description: 'Directory with templates to use for generating assets.  With multiple directories, the first definition found wins.  To include the standard templates, just use "standard" as a template directory name.', multiple: true }),
    verbose: bf_cli_command_1.flags.boolean({ description: 'Output verbose logging of files as they are processed', default: false }),
};
//# sourceMappingURL=generate.js.map
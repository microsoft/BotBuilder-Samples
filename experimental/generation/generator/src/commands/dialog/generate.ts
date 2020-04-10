/*!
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

import { Command, flags } from '@microsoft/bf-cli-command';
import * as gen from '../../library/dialogGenerator'
import * as integ from '../../library/integration'
import * as fs from 'fs-extra';
import * as os from 'os';

export default class GenerateDialog extends Command {
    static description = '[PREVIEW] Generate localized .lu, .lg, .qna and .dialog assets to define a bot based on a schema using templates.'

    static examples = [`
      $ bf dialog:generate sandwich.schema --output c:/tmp
    `]

    static args = [
        { name: 'schema', required: true, description: 'JSON Schema .schema file used to drive generation.' }
    ]

    static flags: flags.Input<any> = {
        force: flags.boolean({ char: 'f', description: 'Force overwriting generated files.' }),
        help: flags.help({ char: 'h' }),
        locale: flags.string({ char: 'l', description: 'Locales to generate. [default: en-us]', multiple: true }),
        output: flags.string({ char: 'o', description: 'Output path for where to put generated .lu, .lg, .qna and .dialog files.', default: '.', required: false }),
        prefix: flags.string({ char: 'p', description: 'Prefix to use for generated files. [default: schema name]' }),
        schema: flags.string({ char: 's', description: 'Path to your app.schema file.', required: false }),
        templates: flags.string({ char: 't', description: 'Directory with templates to use for generating assets.  With multiple directories, the first definition found wins.  To include the standard templates, just use "standard" as a template directory name.', multiple: true }),
        verbose: flags.boolean({ description: 'Output verbose logging of files as they are processed', default: false }),
        integrate: flags.boolean({ description: 'If merge two assets or not', default: false })
    }

    async run() {
        const { args, flags } = this.parse(GenerateDialog)
        if (flags.integrate && fs.pathExists(flags.output)) {
            let start = args.schema.lastIndexOf('/')
            let end = args.schema.lastIndexOf('.')
            let schemaName = args.schema.slice(start + 1, end)
            let tempNewPath = `${os.tmpdir}/tempNew/`
            let tempOldPath = `${os.tmpdir}/tempOld/`
            try {
                fs.emptyDirSync(tempNewPath)
                fs.emptyDirSync(tempOldPath)
                fs.copySync(flags.output, tempOldPath)
                fs.emptyDirSync(flags.output)
                await gen.generate(args.schema, flags.prefix, tempNewPath, flags.schema, flags.locale, flags.templates, flags.force,
                    (type, msg) => {
                        if (type === gen.FeedbackType.message
                            || (type === gen.FeedbackType.info && flags.verbose)) {
                            this.info(msg)
                        } else if (type === gen.FeedbackType.warning) {
                            this.warning(msg)
                        } else if (type === gen.FeedbackType.error) {
                            this.errorMsg(msg)
                        }
                    })
            } catch (e) {
                this.thrownError(e)
            }

            try {
                await integ.integrateAssets(schemaName, tempOldPath, tempNewPath, flags.output, flags.locale, (type, msg) => {
                    if (type === integ.FeedbackType.message
                        || (type === integ.FeedbackType.info && flags.verbose)) {
                        this.info(msg)
                    } else if (type === integ.FeedbackType.warning) {
                        this.warning(msg)
                    } else if (type === integ.FeedbackType.error) {
                        this.errorMsg(msg)
                    }
                })
                return true;
            } catch (e) {
                this.thrownError(e)
            }

        } else {
            try {
                await gen.generate(args.schema, flags.prefix, flags.output, flags.schema, flags.locale, flags.templates, flags.force,
                    (type, msg) => {
                        if (type === gen.FeedbackType.message
                            || (type === gen.FeedbackType.info && flags.verbose)) {
                            this.info(msg)
                        } else if (type === gen.FeedbackType.warning) {
                            this.warning(msg)
                        } else if (type === gen.FeedbackType.error) {
                            this.errorMsg(msg)
                        }
                    })
                return true;
            } catch (e) {
                this.thrownError(e)
            }
        }
    }

    thrownError(err: Error): void {
        this.error(err.message)
    }

    info(msg: string): void {
        console.error(msg)
    }

    warning(msg: string): void {
        this.warn(msg)
    }

    errorMsg(msg: string): void {
        this.error(msg)
    }
}

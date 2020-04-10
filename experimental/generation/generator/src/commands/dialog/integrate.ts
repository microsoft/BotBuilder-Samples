/*!
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

import { Command, flags } from '@microsoft/bf-cli-command';
import { integrateAssets } from '../../library/integration'
import { FeedbackType } from '../../library/dialogGenerator'

export default class DialogIntegrate extends Command {

    static args = [
        { name: 'schema', required: true, description: 'JSON Schema .schema file used to drive generation.' }
    ]

    static flags: flags.Input<any> = {
        help: flags.help({ char: 'h' }),
        oldPath: flags.string({ char: 'o', description: 'path of old assets ', required: true }),
        newPath: flags.string({ char: 'n', description: 'path of new assets ', required: true }),
        mergedPath: flags.string({ char: 'm', description: 'path of merged assets ', required: true }),
        locale: flags.string({ char: 'l', description: 'locale', required: true }),
        verbose: flags.boolean({ description: 'output verbose logging of files as they are processed', default: false }),
    }

    static examples = [
        '$ bf dialog:integrate aaa -o /bbb -n /ccc -m /ddd -l en-us'
    ]

    async run() {
        const { args, flags } = this.parse(DialogIntegrate)
        try {
            await integrateAssets(args.schema, flags.oldPath, flags.newPath, flags.mergedPath, flags.locale, (type, msg) => {
                if (type === FeedbackType.message
                    || (type === FeedbackType.info && flags.verbose)) {
                    this.info(msg)
                } else if (type === FeedbackType.warning) {
                    this.warning(msg)
                } else if (type === FeedbackType.error) {
                    this.errorMsg(msg)
                }
            })
            return true;
        } catch (e) {
            this.thrownError(e)
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

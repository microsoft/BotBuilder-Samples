/*!
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

import { Command, flags } from '@microsoft/bf-cli-command';

export default class Index extends Command {
    static description = 'The dialog commands allow you to work with dialog schema.'

    static flags: flags.Input<any> = {
        help: flags.help({ char: 'h' }),
    }

    async run() {
        this._help()
    }
}

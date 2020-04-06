/*!
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
import { Command, flags } from '@microsoft/bf-cli-command';
export default class Index extends Command {
    static description: string;
    static flags: flags.Input<any>;
    run(): Promise<void>;
}

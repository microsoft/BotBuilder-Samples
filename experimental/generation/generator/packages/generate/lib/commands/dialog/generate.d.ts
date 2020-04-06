/*!
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
import { Command, flags } from '@microsoft/bf-cli-command';
export default class GenerateDialog extends Command {
    static description: string;
    static examples: string[];
    static args: {
        name: string;
        required: boolean;
        description: string;
    }[];
    static flags: flags.Input<any>;
    run(): Promise<true | undefined>;
    thrownError(err: Error): void;
    info(msg: string): void;
    warning(msg: string): void;
    errorMsg(msg: string): void;
}

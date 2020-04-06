/*!
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
import { Command, flags } from '@microsoft/bf-cli-command';
export default class DialogVerify extends Command {
    static args: {
        name: string;
        required: boolean;
    }[];
    static flags: flags.Input<any>;
    private currentFile;
    private files;
    private errors;
    private warnings;
    run(): Promise<void>;
    execute(dialogFiles: string[], verbose?: boolean): Promise<void>;
    consoleMsg(msg: string): void;
    consoleLog(msg: string): void;
    consoleWarn(msg: string, code: string): void;
    consoleError(msg: string, code: string): void;
}

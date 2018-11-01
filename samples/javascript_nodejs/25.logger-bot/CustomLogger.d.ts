// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { TranscriptLogger } from 'botframework-core';

/*
* CustomLogger, imports the TranscriptLogger interface and exports the module.
*/
export declare class CustomLogger implements TranscriptLogger {
    /**
     * Log an activity to the transcript.
     * @param activity Activity being logged.
     */
    logActivity(activity: Activity): void | Promise<void>;
}
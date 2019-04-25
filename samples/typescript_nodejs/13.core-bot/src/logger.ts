// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

export interface Logger {
    error(data: any, ...args: any): void;
    log(data: any, ...args: any): void;
    warn(data: any, ...args: any): void;
}

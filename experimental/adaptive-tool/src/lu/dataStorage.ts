/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

import * as vscode from 'vscode';

export class DataStorage {
    public static LuResourceMap: Map<string, LUResource> = new Map<string, LUResource>();
}

export class LUResource {
    public constructor(uri: vscode.Uri, luResource: any) {
        this.luResource = luResource;
        this.uri = uri;
    }
    public uri: vscode.Uri;
    public luResource: any;
}
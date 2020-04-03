/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

import { Templates } from 'botbuilder-lg';
import * as vscode from 'vscode';

export class DataStorage {
    public static templatesMap: Map<string, TemplatesEntity> = new Map<string, TemplatesEntity>(); // file path -> templates
}

export class TemplatesEntity {
    public constructor(uri: vscode.Uri, templates: Templates) {
        this.templates = templates;
        this.uri = uri;
    }
    public uri: vscode.Uri;
    public templates: Templates;
}
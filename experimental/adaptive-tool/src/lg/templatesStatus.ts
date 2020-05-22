/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

import { Templates } from 'botbuilder-lg';
import * as vscode from 'vscode';

export class TemplatesStatus {
    public static templatesMap: Map<string, TemplatesEntity> = new Map<string, TemplatesEntity>();
    public static lgFilesOfWorkspace: string[] = [];
}

export class TemplatesEntity {
    public constructor(uri: vscode.Uri, templates: Templates) {
        this.templates = templates;
        this.uri = uri;
    }
    public uri: vscode.Uri;
    public templates: Templates;
}

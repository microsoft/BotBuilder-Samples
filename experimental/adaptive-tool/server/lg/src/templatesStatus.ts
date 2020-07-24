/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

import { Templates } from 'botbuilder-lg';
import { DocumentUri } from 'vscode-languageserver';

export class TemplatesStatus {
    public static templatesMap: Map<string, TemplatesEntity> = new Map<string, TemplatesEntity>();
    public static lgFilesOfWorkspace: string[] = [];
}

export class TemplatesEntity {
    public constructor(uri: DocumentUri, templates: Templates) {
        this.templates = templates;
        this.uri = uri;
    }
    public uri: DocumentUri;
    public templates: Templates;
}
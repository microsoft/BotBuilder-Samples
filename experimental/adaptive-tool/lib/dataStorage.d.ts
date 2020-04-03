/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
import { Templates } from 'botbuilder-lg';
import * as vscode from 'vscode';
export declare class DataStorage {
    static templatesMap: Map<string, TemplatesEntity>;
}
export declare class TemplatesEntity {
    constructor(uri: vscode.Uri, templates: Templates);
    uri: vscode.Uri;
    templates: Templates;
}
//# sourceMappingURL=dataStorage.d.ts.map
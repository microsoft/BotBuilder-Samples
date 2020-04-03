"use strict";
/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
Object.defineProperty(exports, "__esModule", { value: true });
class DataStorage {
}
exports.DataStorage = DataStorage;
DataStorage.templatesMap = new Map(); // file path -> templates
class TemplatesEntity {
    constructor(uri, templates) {
        this.templates = templates;
        this.uri = uri;
    }
}
exports.TemplatesEntity = TemplatesEntity;
//# sourceMappingURL=dataStorage.js.map
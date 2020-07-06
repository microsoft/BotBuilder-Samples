"use strict";
/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
Object.defineProperty(exports, "__esModule", { value: true });
exports.TemplatesEntity = exports.TemplatesStatus = void 0;
class TemplatesStatus {
}
exports.TemplatesStatus = TemplatesStatus;
TemplatesStatus.templatesMap = new Map();
TemplatesStatus.lgFilesOfWorkspace = [];
class TemplatesEntity {
    constructor(uri, templates) {
        this.templates = templates;
        this.uri = uri;
    }
}
exports.TemplatesEntity = TemplatesEntity;
//# sourceMappingURL=templatesStatus.js.map
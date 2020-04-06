#!/usr/bin/env node
/*!
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
import * as ajv from 'ajv';
export declare class SchemaTracker {
    typeToType: Map<string, Type>;
    private readonly validator;
    constructor();
    getValidator(schemaPath: string): Promise<[ajv.ValidateFunction, boolean]>;
    private getURL;
}
export declare class Type {
    name: string;
    lgProperties: string[];
    implementations: Type[];
    interfaces: Type[];
    constructor(name: string, schema?: any);
    addImplementation(type: Type): void;
    toString(): string;
    private walkProps;
}

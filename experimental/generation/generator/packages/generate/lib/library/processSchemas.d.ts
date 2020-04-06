#!/usr/bin/env node
/*!
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
import * as fg from './dialogGenerator';
export declare function typeName(property: any): string;
export declare function processSchemas(schemaPath: string, templateDirs: string[], feedback: fg.Feedback): Promise<any>;

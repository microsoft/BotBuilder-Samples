#!/usr/bin/env node
/*!
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
export * from './dialogGenerator';
export declare enum FeedbackType {
    message = 0,
    info = 1,
    warning = 2,
    error = 3
}
export declare type Feedback = (type: FeedbackType, message: string) => void;
export declare function isUnchanged(path: string): Promise<boolean>;
/**
 * Iterate through the locale templates and generate per property/locale files.
 * Each template file will map to <filename>_<property>.<ext>.
 * @param schemaPath Path to JSON Schema to use for generation.
 * @param prefix Prefix to use for generated files.
 * @param outDir Where to put generated files.
 * @param metaSchema Schema to use when generating .dialog files
 * @param allLocales Locales to generate.
 * @param templateDirs Where templates are found.
 * @param force True to force overwriting existing files.
 * @param feedback Callback function for progress and errors.
 */
export declare function generate(schemaPath: string, prefix?: string, outDir?: string, metaSchema?: string, allLocales?: string[], templateDirs?: string[], force?: boolean, feedback?: Feedback): Promise<void>;

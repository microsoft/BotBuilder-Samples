/*!
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
import { Command, flags } from '@microsoft/bf-cli-command';
import * as Validator from 'ajv';
export default class DialogMerge extends Command {
    static args: {
        name: string;
        required: boolean;
    }[];
    static flags: flags.Input<any>;
    static examples: string[];
    private verbose?;
    private failed;
    private missingKinds;
    private currentFile;
    private readonly jsonOptions;
    run(): Promise<void>;
    /**
     * Merge together .schema files to make a custom schema.
     * @param patterns Glob patterns for the .schema files to combine.
     * @param output The output file to create.  app.schema by default.
     * @param branch Branch to use for where to find component.schema.
     * @param update True to update .schema files to point to branch component.schema files.
     */
    mergeSchemas(patterns: string[], output?: string, branch?: string, update?: boolean, verbose?: boolean): Promise<boolean>;
    expandPackages(paths: string[]): AsyncIterable<string>;
    prettyPath(path: string): string;
    findGlobalNuget(): Promise<string>;
    xmlToJSON(path: string): Promise<string>;
    findParentDirectory(path: string, dir: string): Promise<string>;
    updateMetaSchema(branch: string): Promise<void>;
    processRoles(definitions: any): void;
    processRole(role: string, elt: any, kind: string, definitions: any, key?: string): void;
    addKindTitles(definitions: any): void;
    fixDefinitionReferences(definitions: any): void;
    expandKinds(definitions: any): void;
    addStandardProperties(definitions: any, dialogSchema: any): void;
    sortImplementations(definitions: any): void;
    walkJSON(elt: any, fun: (val: any, obj?: any, key?: string) => boolean, obj?: any, key?: any): boolean;
    getURL(url: string): Promise<any>;
    isInterface(schema: any): boolean;
    missing(kind: string): void;
    schemaError(err: Validator.ErrorObject): void;
    thrownError(err: Error): void;
    errorMsg(kind: string, message: string): void;
}

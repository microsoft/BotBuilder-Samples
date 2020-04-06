#!/usr/bin/env node
/*!
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
export * from './schemaTracker';
import * as st from './schemaTracker';
export declare class Dialog {
    file: string;
    body?: any;
    errors: Error[];
    save: boolean;
    constructor(file: string, body?: object);
    id(): string;
    schema(): string;
    toString(): string;
}
export declare class Definition {
    type?: st.Type;
    dialog?: Dialog;
    path?: string;
    id?: string;
    usedBy: Definition[];
    copy?: string;
    /**
     * Construct a component definition.
     * @param type The $kind of the component.
     * @param id The $id of the component if present.
     * @param dialog The dialog that contains the definition. (undefined for forward reference.)
     * @param path The path within the file to the component.
     * @param copy The id of the copied definition.
     */
    constructor(type?: st.Type, id?: string, dialog?: Dialog, path?: string, copy?: string);
    compare(definition: Definition): number;
    usedByString(): string;
    toString(): string;
    locatorString(): string;
    pathString(): string;
}
export declare class DialogTracker {
    root: string;
    schema: st.SchemaTracker;
    /**
     *
     * Map from $id to the definition.
     * If there are more than one, then it is multiply defined.
     * If any of them are missing dialog, then there is a $copy, but no definition.
     */
    idToDef: Map<string, Definition[]>;
    typeToDef: Map<string, Definition[]>;
    missingTypes: Definition[];
    dialogs: Dialog[];
    constructor(schema: st.SchemaTracker, root?: string);
    addDialog(dialog: Dialog): Promise<void>;
    addDialogFile(file: string): Promise<Dialog>;
    addDialogFiles(patterns: string[]): Promise<void>;
    removeDialog(dialog: Dialog): void;
    findDialog(id: string): undefined | Dialog;
    findDialogFile(file: string): undefined | Dialog;
    cloneDialog(file: string): undefined | Dialog;
    updateDialog(dialog: Dialog): Promise<void>;
    /**
     * Write out dialog files with save true and reset the flag.
     * @param root If present this is the new root and paths below will be relative to process.cwd.
     */
    writeDialogs(root?: string): Promise<void>;
    allDefinitions(): Iterable<Definition>;
    multipleDefinitions(): Iterable<Definition[]>;
    missingDefinitions(): Iterable<Definition>;
    unusedIDs(): Iterable<Definition>;
    /**
     * Add a new definition to the tracker.
     * The definition might be a forward reference.
     */
    private addDefinition;
    /**
     * Add reference to a $id.
     * @param ref Reference found in $copy.
     * @param source Definition that contains $copy.
     */
    private addReference;
    private removeDefinition;
    private expandRef;
}

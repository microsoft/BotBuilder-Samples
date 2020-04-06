#!/usr/bin/env node
"use strict";
/*!
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
Object.defineProperty(exports, "__esModule", { value: true });
const tslib_1 = require("tslib");
tslib_1.__exportStar(require("./schemaTracker"), exports);
const fs = require("fs-extra");
const glob = require("globby");
const path = require("path");
let clone = require('clone');
// Top-level dialog definition that would be found in a file.
class Dialog {
    constructor(file, body) {
        this.file = file;
        if (!path.isAbsolute) {
            throw new Error(`${file} must be an absolute path.`);
        }
        this.body = body;
        this.errors = [];
        this.save = true;
    }
    // Return the id of the dialog, i.e. the base filename.
    id() {
        return path.basename(this.file, '.dialog');
    }
    // Base schema for dialog.
    schema() {
        return this.body.$schema;
    }
    toString() {
        return path.relative(process.cwd(), this.file);
    }
}
exports.Dialog = Dialog;
// Definition of a Bot Framework component.
class Definition {
    /**
     * Construct a component definition.
     * @param type The $kind of the component.
     * @param id The $id of the component if present.
     * @param dialog The dialog that contains the definition. (undefined for forward reference.)
     * @param path The path within the file to the component.
     * @param copy The id of the copied definition.
     */
    constructor(type, id, dialog, path, copy) {
        this.type = type;
        this.id = id;
        this.dialog = dialog;
        this.path = path;
        this.copy = copy;
        this.usedBy = [];
    }
    // Compare definitions and return -1 for less than, 0 for equals and +1 for greater than.
    compare(definition) {
        let result;
        if (this.dialog !== undefined && this.path !== undefined
            && definition.dialog !== undefined && definition.path !== undefined) { // Actual definitions
            if (this.dialog === definition.dialog) {
                if (this.path === definition.path) {
                    result = 0;
                }
                else {
                    result = this.path.localeCompare(definition.path);
                }
            }
            else {
                result = this.dialog.file.localeCompare(definition.dialog.file);
            }
        }
        else if (this.dialog !== undefined && this.path !== undefined) {
            result = +1;
        }
        else if (definition.dialog !== undefined && definition.path !== undefined) {
            result = -1;
        }
        else if (this.id !== undefined && this.type !== undefined
            && definition.id !== undefined && definition.type !== undefined) {
            if (this.id === definition.id) {
                if (this.type === definition.type) {
                    result = 0;
                }
                else {
                    result = this.type.name.localeCompare(definition.type.name);
                }
            }
            else {
                result = this.id.localeCompare(definition.id);
            }
        }
        else {
            if (this.id !== undefined && this.type !== undefined) {
                result = -1;
            }
            else if (definition.id !== undefined && definition.type !== undefined) {
                result = +1;
            }
            else {
                result = -1;
            }
        }
        return result;
    }
    usedByString() {
        let result = '';
        if (this.usedBy.length > 0) {
            result = 'used by';
            for (let user of this.usedBy) {
                result += ' ' + user.locatorString();
            }
        }
        return result;
    }
    toString() {
        return `${this.type}${this.locatorString()}`;
    }
    locatorString() {
        if (this.id) {
            return `[${this.id}]`;
        }
        else {
            return this.pathString();
        }
    }
    pathString() {
        let id = this.dialog ? this.dialog.id() : 'undefined';
        return `(${id}#${this.path})`;
    }
}
exports.Definition = Definition;
// Tracks cogs and the definitions they contain.
class DialogTracker {
    constructor(schema, root) {
        this.schema = schema;
        this.root = root || process.cwd();
        this.idToDef = new Map();
        this.typeToDef = new Map();
        this.missingTypes = [];
        this.dialogs = [];
    }
    // Add a new Dialog file to the tracker.
    async addDialog(dialog) {
        try {
            const schemaFile = dialog.body.$schema;
            if (schemaFile) {
                let schemaPath = path.join(path.dirname(dialog.file), schemaFile);
                let [validator] = await this.schema.getValidator(schemaPath);
                let validation = validator(dialog.body, dialog.file);
                if (!validation && validator.errors) {
                    for (let err of validator.errors) {
                        let path = err.dataPath;
                        if (path.startsWith(dialog.file)) {
                            path = path.substring(dialog.file.length);
                        }
                        dialog.errors.push(new Error(`${path} ${err.message}`));
                    }
                }
            }
            else {
                dialog.errors.push(new Error(`${dialog} does not have a $schema.`));
            }
            if (dialog.body.$id) {
                dialog.errors.push(new Error('dialog cannot have $id at the root because it is defined by the filename.'));
            }
            // Expand $id to include root dialog
            walkJSON(dialog.body, '', elt => {
                if (elt.$id) {
                    elt.$id = dialog.id() + '#' + elt.$id;
                }
                return false;
            });
            dialog.body.$id = dialog.id();
            walkJSON(dialog.body, '', (elt, path) => {
                if (elt.$kind) {
                    let def = new Definition(this.schema.typeToType.get(elt.$kind), elt.$id, dialog, path, elt.$copy);
                    this.addDefinition(def);
                    if (elt.$copy) {
                        this.addReference(elt.$copy, def);
                    }
                }
                else if (elt.$id || elt.$copy) { // Missing type
                    this.addDefinition(new Definition(undefined, elt.$id, dialog, path, elt.$copy));
                }
                return false;
            });
            // Assume we will save it and reset this when coming from file
            dialog.save = true;
        }
        catch (e) {
            dialog.errors.push(e);
        }
        this.dialogs.push(dialog);
    }
    // Read a dialog file and add it to the tracker.
    async addDialogFile(file) {
        let dialog;
        let rel = path.relative(this.root, file);
        try {
            dialog = new Dialog(rel, await fs.readJSON(rel));
            await this.addDialog(dialog);
        }
        catch (e) {
            // File is not valid JSON
            dialog = new Dialog(rel);
            dialog.errors.push(e);
            this.dialogs.push(dialog);
        }
        dialog.save = false;
        return dialog;
    }
    // Add dialog files that match patterns to tracker.
    async addDialogFiles(patterns) {
        let filePaths = await glob(patterns);
        for (let filePath of filePaths) {
            await this.addDialogFile(filePath);
        }
    }
    // Remove dialog from tracker.
    removeDialog(dialog) {
        this.dialogs = this.dialogs.filter(c => c !== dialog);
        for (let definition of this.allDefinitions()) {
            if (definition.dialog === dialog) {
                this.removeDefinition(definition);
            }
        }
    }
    // Find an existing dialog or return undefined.
    findDialog(id) {
        let result;
        for (let dialog of this.dialogs) {
            if (dialog.id() === id) {
                result = dialog;
                break;
            }
        }
        return result;
    }
    // Find the dialog corresponding to a file path.
    findDialogFile(file) {
        return this.findDialog(path.basename(file, '.dialog'));
    }
    // Clone an existing dialog so you can modify it and then call updateDialog.
    cloneDialog(file) {
        let dialog = this.findDialog(file);
        return dialog ? clone(dialog, false) : undefined;
    }
    // Update or add a dialog.
    async updateDialog(dialog) {
        let oldDialog = this.findDialog(dialog.id());
        if (oldDialog) {
            this.removeDialog(oldDialog);
        }
        await this.addDialog(dialog);
    }
    /**
     * Write out dialog files with save true and reset the flag.
     * @param root If present this is the new root and paths below will be relative to process.cwd.
     */
    async writeDialogs(root) {
        for (let dialog of this.dialogs) {
            if (dialog.save) {
                let filePath = root ? path.join(path.resolve(root), path.relative(process.cwd(), dialog.file)) : dialog.file;
                let dir = path.dirname(filePath);
                await fs.mkdirp(dir);
                let oldID = dialog.id();
                delete dialog.body.$id;
                await fs.writeJSON(filePath, dialog.body, { spaces: 4 });
                dialog.file = path.relative(process.cwd(), filePath);
                dialog.body.$id = oldID;
                dialog.save = false;
            }
        }
    }
    // All definitions.
    *allDefinitions() {
        for (let defs of this.typeToDef.values()) {
            for (let def of defs) {
                yield def;
            }
        }
        for (let def of this.missingTypes) {
            yield def;
        }
    }
    // Definitions that try to define the same $id.
    *multipleDefinitions() {
        for (let def of this.idToDef.values()) {
            if (def.length > 1) {
                let type = def[0].type;
                if (!def.find(d => d.type !== type)) {
                    yield def;
                }
            }
        }
    }
    // Definitions that are referred to through $copy, but are not defined.
    *missingDefinitions() {
        for (let defs of this.idToDef.values()) {
            for (let def of defs) {
                if (!def.dialog) {
                    yield def;
                }
            }
        }
    }
    // Definitions with ids that are unused.
    *unusedIDs() {
        for (let defs of this.idToDef.values()) {
            for (let def of defs) {
                if (def.usedBy.length === 0) {
                    yield def;
                }
            }
        }
    }
    /**
     * Add a new definition to the tracker.
     * The definition might be a forward reference.
     */
    addDefinition(definition) {
        if (definition.type && !this.typeToDef.has(definition.type.name)) {
            this.typeToDef.set(definition.type.name, []);
        }
        if (definition.id) {
            let add = true;
            if (this.idToDef.has(definition.id)) {
                // Reference already existed, check for consistency
                // Merge if possible, otherwise add
                for (let old of this.idToDef.get(definition.id)) {
                    if (!old.dialog && !old.path && old.type === definition.type) {
                        add = false;
                        old.dialog = definition.dialog;
                        old.path = definition.path;
                        break;
                    }
                }
            }
            else {
                this.idToDef.set(definition.id, []);
            }
            if (add) {
                this.idToDef.get(definition.id).push(definition);
                if (definition.type) {
                    this.typeToDef.get(definition.type.name).push(definition);
                }
                else {
                    this.missingTypes.push(definition);
                }
            }
        }
        else {
            if (definition.type) {
                this.typeToDef.get(definition.type.name).push(definition);
            }
            else {
                this.missingTypes.push(definition);
            }
        }
    }
    /**
     * Add reference to a $id.
     * @param ref Reference found in $copy.
     * @param source Definition that contains $copy.
     */
    addReference(ref, source) {
        let fullRef = this.expandRef(ref, source.dialog);
        if (!this.idToDef.has(fullRef)) {
            // ID does not exist so add place holder
            let definition = new Definition(source.type, fullRef);
            this.addDefinition(definition);
            this.idToDef.set(fullRef, [definition]);
        }
        let copyDef = this.idToDef.get(fullRef);
        for (let idDef of copyDef) {
            idDef.usedBy.push(source);
        }
    }
    // Remove definition from tracker.
    removeDefinition(definition) {
        let found = false;
        if (definition.id && this.idToDef.has(definition.id)) {
            // Remove from ids
            const defs = this.idToDef.get(definition.id);
            const newDefs = defs.filter(d => d.compare(definition) !== 0);
            if (newDefs.length === 0) {
                this.idToDef.delete(definition.id);
            }
            else {
                this.idToDef.set(definition.id, newDefs);
            }
            found = newDefs.length !== defs.length;
        }
        if (definition.type && this.typeToDef.has(definition.type.name)) {
            const defs = this.typeToDef.get(definition.type.name);
            const newDefs = defs.filter(d => d.compare(definition) !== 0);
            if (newDefs.length === 0) {
                this.typeToDef.delete(definition.type.name);
            }
            else {
                this.typeToDef.set(definition.type.name, newDefs);
            }
            found = found || newDefs.length !== defs.length;
        }
        else {
            // Remove from missing types
            let newDefs = this.missingTypes.filter(d => d.compare(definition) !== 0);
            found = found || newDefs.length !== this.missingTypes.length;
            this.missingTypes = newDefs;
        }
        // Remove from all usedBy.
        for (let def of this.allDefinitions()) {
            def.usedBy = def.usedBy.filter(d => d.compare(definition) !== 0);
        }
        return found;
    }
    expandRef(ref, dialog) {
        return ref.startsWith('#') ? `${dialog.id()}${ref}` : ref;
    }
}
exports.DialogTracker = DialogTracker;
function walkJSON(json, path, fun) {
    let done = fun(json, path);
    if (!done) {
        if (Array.isArray(json)) {
            let i = 0;
            for (let val of json) {
                done = walkJSON(val, `${path}[${i}]`, fun);
                if (done)
                    break;
                ++i;
            }
        }
        else if (typeof json === 'object') {
            for (let val in json) {
                done = walkJSON(json[val], `${path}/${val}`, fun);
                if (done)
                    break;
            }
        }
    }
    return done;
}
//# sourceMappingURL=dialogTracker.js.map
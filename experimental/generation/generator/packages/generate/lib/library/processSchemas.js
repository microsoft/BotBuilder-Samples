#!/usr/bin/env node
"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
/*!
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
const fg = require("./dialogGenerator");
const glob = require("globby");
const ppath = require("path");
const s = require("./schema");
let allof = require('json-schema-merge-allof');
let clone = require('clone');
let parser = require('json-schema-ref-parser');
// All .schema files found in template directories
async function templateSchemas(templateDirs, feedback) {
    let map = {};
    for (let dir of templateDirs) {
        for (let file of await glob(ppath.join(dir, '**/*.schema'))) {
            let schema = await getSchema(file, feedback);
            let id = schema.$id || ppath.basename(file);
            if (!map[id]) {
                // First definition found wins
                map[id] = schema;
            }
        }
    }
    return map;
}
// Find recursive requires
async function findRequires(schema, map, found, resolver, feedback) {
    let addRequired = async (required) => {
        if (!found[required]) {
            let schema = map[required] || await getSchema(required, feedback, resolver);
            if (!schema) {
                feedback(fg.FeedbackType.error, `Schema ${required} cannot be found`);
            }
            else {
                found[required] = schema;
            }
        }
    };
    if (typeof schema === 'object') {
        for (let [child, val] of Object.entries(schema)) {
            if (child === '$requires') {
                for (let required of val) {
                    await addRequired(required);
                }
            }
            else {
                await findRequires(val, map, found, resolver, feedback);
            }
        }
    }
}
// Get a schema after following all references and removing allOf
async function getSchema(path, feedback, resolver) {
    let schema;
    try {
        let noref = await parser.dereference(path, { resolve: { template: resolver } });
        schema = allof(noref);
    }
    catch (err) {
        feedback(fg.FeedbackType.error, err);
    }
    return schema;
}
// Merge together multiple schemas
function mergeSchemas(allSchema, schemas) {
    for (let schema of schemas) {
        allSchema.properties = Object.assign(Object.assign({}, allSchema.properties), schema.properties);
        allSchema.definitions = Object.assign(Object.assign({}, allSchema.definitions), schema.definitions);
        if (schema.required)
            allSchema.required = allSchema.required.concat(schema.required);
        if (schema.$expectedOnly)
            allSchema.$expectedOnly = allSchema.$expectedOnly.concat(schema.$expectedOnly);
        if (schema.$templates)
            allSchema.$templates = allSchema.$templates.concat(schema.$templates);
        if (schema.$operations)
            allSchema.$operations = allSchema.$operations.concat(schema.$operations);
        // Last definition wins
        if (schema.$defaultOperation)
            allSchema.$defaultOperation = schema.$defaultOperation;
        if (schema.$public)
            allSchema.$public = allSchema.$public.concat(schema.$public);
    }
}
function typeName(property) {
    let type = property.type;
    let isArray = false;
    if (type === 'array') {
        property = property.items;
        type = property.type;
        isArray = true;
    }
    if (property.enum) {
        type = 'enum';
    }
    if (isArray) {
        type = type + 'Array';
    }
    return type;
}
exports.typeName = typeName;
function addMissingEntities(property, path) {
    let entities = property.$entities;
    if (!entities) {
        let type = typeName(property);
        if (type === 'number') {
            entities = [`number:${path}`, 'number'];
        }
        else if (type === 'string') {
            entities = [path + 'Entity', 'utterance'];
        }
        else if (type === 'object') {
            // For objects go to leaves
            for (let childPath of Object.keys(property.properties)) {
                let child = property.properties[childPath];
                addMissingEntities(child, path + '.' + child);
            }
        }
        else {
            entities = [path + 'Entity'];
        }
        if (!entities) {
            entities = [];
        }
        property.$entities = entities;
    }
}
// Fill in $entities if missing
function addMissing(schema) {
    for (let path of Object.keys(schema.properties)) {
        let property = schema.properties[path];
        addMissingEntities(property, path);
    }
}
// Process the root schema to generate all schemas
// 1) A property can $ref to a property definition to reuse a type like address. 
//    Ref resolver includes template: for referring to template files.
// 2) $requires:[] can be in a property or at the top.  
//    This is handled by finding all of the referenced schemas and then merging.  
async function processSchemas(schemaPath, templateDirs, feedback) {
    let allRequired = await templateSchemas(templateDirs, feedback);
    let resolver = {
        canRead: /template:/,
        read(file) {
            let base = file.url.substring(file.url.indexOf(':') + 1);
            return allRequired[base];
        }
    };
    let formSchema = await getSchema(schemaPath, feedback, resolver);
    let required = {};
    await findRequires(formSchema, allRequired, required, resolver, feedback);
    let allSchema = clone(formSchema);
    addMissing(allSchema);
    if (!allSchema.required)
        allSchema.required = [];
    if (!allSchema.$expectedOnly)
        allSchema.$expectedOnly = [];
    if (!allSchema.$templates)
        allSchema.$templates = [];
    if (!allSchema.$operations)
        allSchema.$operations = [];
    if (formSchema.$public) {
        allSchema.$public = formSchema.$public;
    }
    else {
        // Default to properties in root schema
        allSchema.$public = Object.keys(formSchema.properties);
    }
    mergeSchemas(allSchema, Object.values(required));
    return new s.Schema(schemaPath, allSchema);
}
exports.processSchemas = processSchemas;
//# sourceMappingURL=processSchemas.js.map
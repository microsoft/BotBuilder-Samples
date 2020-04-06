#!/usr/bin/env node
"use strict";
/*!
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
Object.defineProperty(exports, "__esModule", { value: true });
const tslib_1 = require("tslib");
/* tslint:disable:no-unused */
tslib_1.__exportStar(require("./schema"), exports);
const Validator = require("ajv");
const os = require("os");
const ppath = require("path");
const ps = require("./processSchemas");
let allof = require('json-schema-merge-allof');
let parser = require('json-schema-ref-parser');
var EntityType;
(function (EntityType) {
    EntityType[EntityType["simple"] = 0] = "simple";
    EntityType[EntityType["list"] = 1] = "list";
    EntityType[EntityType["prebuilt"] = 2] = "prebuilt";
})(EntityType = exports.EntityType || (exports.EntityType = {}));
class Entity {
    constructor(name, property) {
        this.name = name;
        this.property = property;
    }
}
exports.Entity = Entity;
/**
 * Extra properties:
 * $entities: [entity] defaults based on type, string -> [property], numbers -> [property, number]
 * $templates: Template basenames to specialize for this property.
 */
class Schema {
    constructor(source, schema, path) {
        this.source = source;
        this.path = path || '';
        this.schema = schema;
    }
    /**
     * Read and validate schema from a path.
     * @param schemaPath URL for schema.
     */
    static async readSchema(schemaPath) {
        let noref = await parser.dereference(schemaPath);
        let schema = allof(noref);
        let validator = new Validator();
        if (!validator.validateSchema(schema)) {
            let message = '';
            for (let error in validator.errors) {
                message = message + error + os.EOL;
            }
            throw new Error(message);
        }
        if (schema.type !== 'object') {
            throw new Error('Root schema must be of type object.');
        }
        return new Schema(schemaPath, schema);
    }
    static basename(loc) {
        let name = ppath.basename(loc);
        return name.substring(0, name.indexOf('.'));
    }
    name() {
        return Schema.basename(this.source);
    }
    *schemaProperties() {
        for (let prop in this.schema.properties) {
            let newPath = this.path + (this.path === '' ? '' : '.') + prop;
            yield new Schema(this.source, this.schema.properties[prop], newPath);
        }
    }
    typeName() {
        return ps.typeName(this.schema);
    }
    triggerIntent() {
        return this.schema.$triggerIntent || this.name();
    }
    /**
     * Return all entities found in schema.
     */
    allEntities() {
        let entities = [];
        this.addEntities(entities);
        return entities;
    }
    /**
     * Return all of the entity types in schema.
     */
    entityTypes() {
        let found = [];
        for (let entity of this.allEntities()) {
            let [entityName, _] = entity.name.split(':');
            if (!found.includes(entityName)) {
                found.push(entityName);
            }
        }
        return found;
    }
    addEntities(entities) {
        if (this.schema.$entities) {
            for (let entity of this.schema.$entities) {
                // Do not include entities generated from property
                if (!entities.hasOwnProperty(entity)) {
                    let entityWrapper = new Entity(entity, this.path);
                    entities.push(entityWrapper);
                }
            }
        }
        else {
            // Don't explore properties below an explicit mapping
            for (let prop of this.schemaProperties()) {
                prop.addEntities(entities);
            }
        }
    }
}
exports.Schema = Schema;
//# sourceMappingURL=schema.js.map
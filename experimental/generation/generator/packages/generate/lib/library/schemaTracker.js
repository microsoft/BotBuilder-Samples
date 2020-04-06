#!/usr/bin/env node
"use strict";
/*!
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
Object.defineProperty(exports, "__esModule", { value: true });
const ajv = require("ajv");
const fs = require("fs-extra");
const path = require("path");
const http = require('http');
const https = require('https');
class SchemaTracker {
    constructor() {
        this.typeToType = new Map();
        this.validator = new ajv();
    }
    async getValidator(schemaPath) {
        let validator = this.validator.getSchema(schemaPath);
        let added = false;
        if (!validator) {
            let schemaObject = await fs.readJSON(schemaPath);
            added = true;
            if (schemaObject.oneOf) {
                const defRef = '#/definitions/';
                const implementsRole = 'implements(';
                let processRole = (role, type) => {
                    if (role.startsWith(implementsRole)) {
                        role = role.substring(implementsRole.length, role.length - 1);
                        let interfaceDefinition = this.typeToType.get(role);
                        if (!interfaceDefinition) {
                            interfaceDefinition = new Type(role);
                            this.typeToType.set(role, interfaceDefinition);
                        }
                        interfaceDefinition.addImplementation(type);
                    }
                };
                for (let one of schemaObject.oneOf) {
                    let ref = one.$ref;
                    // NOTE: Assuming schema file format is from httpSchema or we will ignore.
                    // Assumption is that a given type name is the same across different schemas.
                    // All .dialog in one app should use the same app.schema, but if you are using
                    // a .dialog from another app then it will use its own schema which if it follows the rules
                    // should have globally unique type names.
                    if (ref.startsWith(defRef)) {
                        ref = ref.substring(defRef.length);
                        if (!this.typeToType.get(ref)) {
                            let def = schemaObject.definitions[ref];
                            if (def) {
                                let type = new Type(ref, def);
                                this.typeToType.set(ref, type);
                                if (def.$role) {
                                    if (typeof def.$role === 'string') {
                                        processRole(def.$role, type);
                                    }
                                    else {
                                        for (let role of def.$role) {
                                            processRole(role, type);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            let metaSchemaName = schemaObject.$schema;
            let metaSchemaCache = path.join(__dirname, path.basename(metaSchemaName));
            let metaSchema;
            if (!await fs.pathExists(metaSchemaCache)) {
                try {
                    let metaSchemaDefinition = await this.getURL(metaSchemaName);
                    metaSchema = JSON.parse(metaSchemaDefinition);
                }
                catch (_a) {
                    throw new Error(`Could not parse ${metaSchemaName}`);
                }
                await fs.writeJSON(metaSchemaCache, metaSchema, { spaces: 4 });
            }
            else {
                metaSchema = await fs.readJSON(metaSchemaCache);
            }
            if (!this.validator.getSchema(metaSchemaName)) {
                this.validator.addSchema(metaSchema, metaSchemaName);
            }
            this.validator.addSchema(schemaObject, schemaPath);
            validator = this.validator.getSchema(schemaPath);
        }
        if (!validator) {
            throw new Error('Could not find schema validator.');
        }
        return [validator, added];
    }
    async getURL(url) {
        return new Promise((resolve, reject) => {
            let client = http;
            if (url.toString().indexOf('https') === 0) {
                client = https;
            }
            client.get(url, (resp) => {
                let data = '';
                // A chunk of data has been received.
                resp.on('data', (chunk) => {
                    data += chunk;
                });
                // The whole response has been received.
                resp.on('end', () => {
                    resolve(data);
                });
            }).on('error', (err) => {
                reject(err);
            });
        });
    }
}
exports.SchemaTracker = SchemaTracker;
// Information about a type.
class Type {
    constructor(name, schema) {
        this.name = name;
        this.lgProperties = [];
        this.implementations = [];
        this.interfaces = [];
        if (schema) {
            this.walkProps(schema, name);
        }
    }
    addImplementation(type) {
        this.implementations.push(type);
        type.interfaces.push(this);
    }
    toString() {
        return this.name;
    }
    walkProps(val, path) {
        if (val.properties) {
            for (let propName in val.properties) {
                let prop = val.properties[propName];
                let newPath = `${path}/${propName}`;
                if (prop.$role === 'lg') {
                    this.lgProperties.push(newPath);
                }
                else if (prop.properties) {
                    this.walkProps(prop, newPath);
                }
            }
        }
    }
}
exports.Type = Type;
//# sourceMappingURL=schemaTracker.js.map
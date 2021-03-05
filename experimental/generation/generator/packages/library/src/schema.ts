#!/usr/bin/env node
/*!
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

/* tslint:disable:no-unused */
export * from './schema'
import * as Validator from 'ajv'
import * as os from 'os'
import * as ppath from 'path'
import * as ps from './processSchemas'
const allof: any = require('json-schema-merge-allof')
const parser: any = require('json-schema-ref-parser')

// Map from entity name to property paths where it is used
export type EntityToProperties = Record<string, string[]>

/**
 * Extra properties:
 * $entities: List of entities to use for property.
 * $templates: Template basenames to specialize for this property.
 */
export class Schema {
    /**
     * Read and validate schema from a path.
     * @param schemaPath URL for schema.
     */
    static async readSchema(schemaPath: string): Promise<Schema> {
        const noref = await parser.dereference(schemaPath)
        const schema = allof(noref)
        const validator = new Validator()
        if (!validator.validateSchema(schema)) {
            let message = ''
            for (const error in validator.errors) {
                message = message + error + os.EOL
            }
            throw new Error(message)
        }
        if (schema.type !== 'object') {
            throw new Error('Root schema must be of type object.')
        }
        return new Schema(schemaPath, schema)
    }

    static basename(loc: string): string {
        const name = ppath.basename(loc)
        return name.substring(0, name.indexOf('.'))
    }

    /** 
     * Path to this schema definition. 
     */
    path: string

    /** 
     * Schema definition.  This is the content of a properties JSON Schema object. 
     */
    schema: any

    /**
     * Source of schema
     */
    source: string

    constructor(source: string, schema: any, path?: string) {
        this.source = source
        this.path = path || ''
        this.schema = schema
    }

    name(): string {
        return Schema.basename(this.source)
    }

    * schemaProperties(): Iterable<Schema> {
        for (const prop in this.schema.properties) {
            const newPath = this.path + (this.path === '' ? '' : '.') + prop
            yield new Schema(this.source, this.schema.properties[prop], newPath)
        }
    }

    typeName(): string {
        return ps.typeName(this.schema)
    }

    triggerIntent(): string {
        return this.schema.$triggerIntent || this.name();
    }

    /**
     * Return map from entity to properties that use it.
     */
    entityToProperties(): EntityToProperties {
        const entities: EntityToProperties = {}
        this.addEntities(entities)
        return entities
    }

    private addEntities(entities: EntityToProperties) {
        if (this.schema.$entities) {
            // Don't explore properties below an explicit mapping
            for (const entity of this.schema.$entities) {
                let list = entities[entity]
                if (!list) {
                    entities[entity] = list = []
                }
                list.push(this.path)
            }
        } else {
            // Add child properties
            for (const prop of this.schemaProperties()) {
                prop.addEntities(entities)
            }
        }
    }
}

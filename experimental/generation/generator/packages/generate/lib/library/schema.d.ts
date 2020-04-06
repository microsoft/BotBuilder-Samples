#!/usr/bin/env node
/*!
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
export * from './schema';
export declare enum EntityType {
    simple = 0,
    list = 1,
    prebuilt = 2
}
export interface ListEntry {
    value: string;
    synonyms: string;
}
export declare class Entity {
    name: string;
    property: string;
    values?: ListEntry[];
    constructor(name: string, property: string);
}
/**
 * Extra properties:
 * $entities: [entity] defaults based on type, string -> [property], numbers -> [property, number]
 * $templates: Template basenames to specialize for this property.
 */
export declare class Schema {
    /**
     * Read and validate schema from a path.
     * @param schemaPath URL for schema.
     */
    static readSchema(schemaPath: string): Promise<Schema>;
    static basename(loc: string): string;
    /**
     * Path to this schema definition.
     */
    path: string;
    /**
     * Schema definition.  This is the content of a properties JSON Schema object.
     */
    schema: any;
    /**
     * Source of schema
     */
    source: string;
    constructor(source: string, schema: any, path?: string);
    name(): string;
    schemaProperties(): Iterable<Schema>;
    typeName(): string;
    triggerIntent(): string;
    /**
     * Return all entities found in schema.
     */
    allEntities(): Entity[];
    /**
     * Return all of the entity types in schema.
     */
    entityTypes(): string[];
    private addEntities;
}

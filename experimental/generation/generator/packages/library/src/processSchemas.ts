#!/usr/bin/env node
/*!
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
import * as fg from './dialogGenerator'
import * as glob from 'globby'
import * as ppath from 'path'
import * as s from './schema'
let allof: any = require('json-schema-merge-allof')
let clone = require('clone')
let parser: any = require('json-schema-ref-parser')

/** Mapping from schema name to schema definition. */
export type idToSchema = {[id: string]: any}

/** 
 * Find all schemas in template directories.
 * @param templateDirs User supplied ordered list of template directories.
 * @returns Object mapping from schema name to schema definition.
 */
export async function schemas(templateDirs?: string[]): Promise<idToSchema> {
    templateDirs = templateDirs || []
    let templates = await fg.templateDirectories(templateDirs)
    let schemas = await templateSchemas(templates, feedbackException)
    return schemas
}

/** Check to see if schema is global. */
export function isGlobalSchema(schema: any) {
    return schema.$global
}

export function feedbackException(type: fg.FeedbackType, message: string) {
    if (type === fg.FeedbackType.error) {
        throw new Error(message)
    }
}

// All .schema files found in template directories
async function templateSchemas(templateDirs: string[], feedback: fg.Feedback): Promise<idToSchema> {
    let map: idToSchema = {}
    for (let dir of templateDirs) {
        for (let file of await glob(ppath.posix.join(dir.replace(/\\/g, '/'), '**/*.schema'))) {
            let schema = await getSchema(file, feedback)
            let id: string = schema.$id || ppath.basename(file)
            if (!map[id]) {
                // First definition found wins
                map[id] = schema
                if (!schema.$templateDirs) {
                    schema.$templateDirs = [ppath.dirname(file)]
                }
            }
        }
    }
    return map
}

// Find recursive requires
async function findRequires(schema: any, map: idToSchema, found: idToSchema, resolver: any, feedback: fg.Feedback): Promise<void> {
    let addRequired = async (required: string) => {
        if (!found[required]) {
            let schema = map[required] || await getSchema(required, feedback, resolver)
            if (!schema) {
                feedback(fg.FeedbackType.error, `Schema ${required} cannot be found`)
            } else {
                found[required] = schema
            }
        }
    }
    if (typeof schema === 'object') {
        for (let [child, val] of Object.entries(schema)) {
            if (child === '$requires') {
                for (let required of val as string[]) {
                    await addRequired(required)
                }
            } else {
                await findRequires(val, map, found, resolver, feedback)
            }
        }
    }
}

// Get a schema after following all references and removing allOf
async function getSchema(path: string, feedback: fg.Feedback, resolver?: any): Promise<any> {
    let schema
    try {
        let noref = await parser.dereference(path, {resolve: {template: resolver}})
        schema = allof(noref)
    } catch (err) {
        feedback(fg.FeedbackType.error, err)
    }
    return schema
}

// Merge together multiple schemas
function mergeSchemas(allSchema: any, schemas: any[]) {
    for (let schema of schemas) {
        // Merge definitions
        allSchema.properties = {...allSchema.properties, ...schema.properties}
        allSchema.definitions = {...allSchema.definitions, ...schema.definitions}
        if (schema.required) allSchema.required = allSchema.required.concat(schema.required)
        if (schema.$defaultOperation) allSchema.$defaultOperation = allSchema.$defaultOperation.concat(schema.$defaultOperation)
        if (schema.$examples) allSchema.$examples = {...allSchema.$examples, ...schema.$examples}
        if (schema.$parameters) allSchema.$parameters = {...allSchema.$parameters, ...schema.$parameters}
        if (schema.$expectedOnly) allSchema.$expectedOnly = allSchema.$expectedOnly.concat(schema.$expectedOnly)
        if (schema.$operations) allSchema.$operations = allSchema.$operations.concat(schema.$operations)
        if (schema.$public) allSchema.$public = allSchema.$public.concat(schema.$public)
        if (schema.$templateDirs) allSchema.$templateDirs = allSchema.$templateDirs.concat(schema.$templateDirs)
        if (schema.$templates) allSchema.$templates = allSchema.$templates.concat(schema.$templates)
    }
}

export function typeName(property: any): string {
    let type = property.type
    let isArray = false
    if (type === 'array') {
        property = property.items
        type = property.type
        isArray = true
    }
    if (property.enum) {
        type = 'enum'
    }
    if (isArray) {
        type = type + 'Array'
    }
    return type
}

async function templateResolver(templateDirs: string[], feedback: fg.Feedback): Promise<{allRequired: any, resolver: any}> {
    let allRequired = await templateSchemas(templateDirs, feedback)
    return {
        allRequired,
        resolver: {
            canRead: /template:/,
            read(file: any): any {
                let base = file.url.substring(file.url.indexOf(':') + 1)
                return allRequired[base]
            }
        }
    }
}

/** 
 * Expand JSON schema property definition by resolving $ref including template: and removing allOf.
 * @param property JSON Schema type definition.
 * @param templateDirs Optional set of template directories.
 * @returns Expanded schema definition including $templateDirs if not present.
 */
export async function expandPropertyDefinition(property: any, templateDirs: string[]): Promise<any> {
    let {allRequired, resolver} = await templateResolver(templateDirs, feedbackException)
    let schema = await parser.dereference(property, {resolve: {template: resolver}})
    schema = allof(schema)
    if (!schema.$templateDirs) {
        schema.$templateDirs = templateDirs
    }
    return schema
}

/**
 * Process the root schema to generate a single merged schema.
 * Involves resolving $ref including template:, removing allOf and combining with $requires.
 * @param schemaPath Path to schema to process.
 * @param templateDirs Template directories to use when resolving template: and $requires.
 * @param feedback Feedback channel
 */
export async function processSchemas(schemaPath: string, templateDirs: string[], feedback: fg.Feedback)
    : Promise<any> {
    let {allRequired, resolver} = await templateResolver(templateDirs, feedback)
    let formSchema = await getSchema(schemaPath, feedback, resolver)
    let required = {}
    if (!formSchema.$requires) {
        // Default to standard schema
        formSchema.$requires = ['standard.schema']
    }
    if (!formSchema.$templateDirs) {
        // Default to including schema directory
        formSchema.$templateDirs = [ppath.resolve(ppath.dirname(schemaPath))]
    }
    await findRequires(formSchema, allRequired, required, resolver, feedback)
    let allSchema = clone(formSchema)
    if (!allSchema.required) allSchema.required = []
    if (!allSchema.$expectedOnly) allSchema.$expectedOnly = []
    if (!allSchema.$templates) allSchema.$templates = []
    if (!allSchema.$operations) allSchema.$operations = []
    if (!allSchema.$defaultOperation) allSchema.$defaultOperation = []
    if (!allSchema.$examples) allSchema.$examples = []
    if (!allSchema.$parameters) allSchema.$parameters = []
    if (formSchema.$public) {
        allSchema.$public = formSchema.$public
    } else {
        // Default to properties in root schema
        allSchema.$public = Object.keys(formSchema.properties)
    }
    mergeSchemas(allSchema, Object.values(required))

    return new s.Schema(schemaPath, allSchema)
}

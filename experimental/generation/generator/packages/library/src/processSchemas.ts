#!/usr/bin/env node
/*!
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
import * as fg from './dialogGenerator'
import * as glob from 'globby'
import * as ppath from 'path'
import * as s from './schema'
const allof: any = require('json-schema-merge-allof')
const clone = require('clone')
const parser: any = require('json-schema-ref-parser')

/** Mapping from schema name to schema definition. */
export type IdToSchema = { [id: string]: any }

const skipSchemas: string[] = ["boolean", "date-time", "date", "email", "enum", "integer", "iri", "number", "string", "time", "uri"]

/** 
 * Find all schemas in template directories.
 * @param templateDirs User supplied ordered list of template directories.
 * @returns Object mapping from schema name to schema definition.
 */
export async function schemas(templateDirs?: string[]): Promise<IdToSchema> {
    templateDirs = templateDirs || []
    const templates = await fg.templateDirectories(templateDirs)
    const schemas = await templateSchemas(templates, feedbackException)
    // TODO: Temporarily filter out built-in types until composer can be updated
    const filteredSchemas = {}
    for (const [name, definition] of Object.entries(schemas)) {
        if (!skipSchemas.includes(name)) {
            filteredSchemas[name] = definition
        }
    }
    return filteredSchemas
}

/** Check to see if schema is global. */
export function isGlobalSchema(schema: any): boolean {
    return schema.$global
}

export function feedbackException(type: fg.FeedbackType, message: string) {
    if (type === fg.FeedbackType.error) {
        throw new Error(message)
    }
}

// Cache of all schemas
const SchemaCache: { [path: string]: any } = {}

// All .schema files found in template directories
async function templateSchemas(templateDirs: string[], feedback: fg.Feedback): Promise<IdToSchema> {
    const map: IdToSchema = {}
    for (const dir of templateDirs) {
        for (const file of await glob(ppath.posix.join(dir.replace(/\\/g, '/'), '**/*.schema'))) {
            let schema = SchemaCache[file]
            if (!schema) {
                schema = await getSchema(file, feedback)
                SchemaCache[file] = schema
            }
            const id = schema.$id || ppath.basename(file, '.schema').toLowerCase()
            if (!map[id]) {
                // First definition found wins
                map[id] = schema
                if (!schema.$templateDirs) {
                    // We pick up template directories by where schemas are found
                    schema.$templateDirs = [ppath.dirname(file)]
                }
            }
        }
    }
    return map
}

// Find recursive requires
async function findRequires(schema: any, map: IdToSchema, found: IdToSchema, resolver: any, feedback: fg.Feedback): Promise<void> {
    const addRequired = async (required: string) => {
        if (!found[required]) {
            const schema = map[required] || await getSchema(required, feedback, resolver)
            if (!schema) {
                feedback(fg.FeedbackType.error, `Schema ${required} cannot be found`)
            } else {
                found[required] = schema
            }
        }
    }
    if (typeof schema === 'object') {
        for (const [child, val] of Object.entries(schema)) {
            if (child === '$requires') {
                for (const required of val as string[]) {
                    await addRequired(ppath.basename(required, '.schema'))
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
        const noref = await parser.dereference(path, { resolve: { template: resolver } })
        schema = allof(noref)
        if (schema.$generator) {
            schema.$generator = allof(schema.$generator)
        }
    } catch (err) {
        feedback(fg.FeedbackType.error, err)
    }
    return schema
}

// Merge together multiple schemas
function mergeSchemas(allSchema: any, schemas: any[]) {
    for (const schema of schemas) {
        // Merge definitions
        allSchema.properties = { ...allSchema.properties, ...schema.properties }
        allSchema.definitions = { ...allSchema.definitions, ...schema.definitions }
        if (schema.required) allSchema.required = allSchema.required.concat(schema.required)
        if (schema.$defaultOperation) allSchema.$defaultOperation = allSchema.$defaultOperation.concat(schema.$defaultOperation)
        if (schema.$requiresValue) allSchema.$requiresValue = allSchema.$requiresValue.concat(schema.$requiresValue)
        if (schema.$examples) allSchema.$examples = { ...allSchema.$examples, ...schema.$examples }
        if (schema.$parameters) allSchema.$parameters = { ...allSchema.$parameters, ...schema.$parameters }
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

    if (property.format) {
        type = property.format
    }

    if (isArray) {
        type = type + 'Array'
    }
    return type
}

async function templateResolver(templateDirs: string[], feedback: fg.Feedback): Promise<{ allRequired: any, resolver: any }> {
    const allRequired = await templateSchemas(templateDirs, feedback)
    return {
        allRequired,
        resolver: {
            canRead: /template:/,
            read(file: any): any {
                const base = ppath.basename(file.url.substring(file.url.indexOf(':') + 1), '.schema')
                const schema = allRequired[base.toLowerCase()]
                if (!schema) {
                    throw new Error(`Could not find ${file.url}`)
                }
                return schema
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
    const { allRequired, resolver } = await templateResolver(templateDirs, feedbackException)
    let schema = await parser.dereference(property, { resolve: { template: resolver } })
    schema = allof(schema)
    return schema
}

// Items keywords that are constraints and can be promoted to the property level
const ContraintKeywords = ['minimum', 'maximum', 'exclusiveMinimum', 'exclusiveMaximum', 'pattern']

/**
 * Promote any $properties or constraints in items to the parent property unless already specified.
 * This makes it easier to use a child schema in items.
 * @param schema Schema fragment to promote.
 */
function promoteItems(schema: any): void {
    if (Array.isArray(schema)) {
        for (var child in schema) {
            promoteItems(child)
        }
    } else if (typeof schema === 'object' && schema !== null) {
        for (var [prop, val] of Object.entries(schema)) {
            if (prop === 'items') {
                if (val && typeof val === 'object' && !Array.isArray(val)) {
                    for (var [prop, itemVal] of Object.entries(val as object)) {
                        if (((prop.startsWith('$') && prop !== '$schema') || ContraintKeywords.includes(prop)) && !schema[prop]) {
                            schema[prop] = itemVal
                        }
                    }
                }
            } else {
                promoteItems(val)
            }
        }
    }
}

// Return the template name from a property definition
// Either $template or inferred from primitive types or array items
function propertyTemplate(property: any): string {
    let template = property.$template
    if (!template) {
        template = property.type
        switch (template) {
            case 'array':
                if (typeof property.items === 'object' && !Array.isArray(property.items)) {
                    template = propertyTemplate(property.items)
                }
                break
            case 'string':
                if (property.format) {
                    template = property.format
                } else if (property.enum) {
                    template = 'enum'
                }
                break
        }
    }
    return template
}

// Ensure each property definition has a $template
function ensureTemplate(schema: any): void {
    for (const def of Object.values(schema.properties)) {
        const definition = def as any
        const template = propertyTemplate(definition)
        definition.$template = template
    }
}

/**
 * Process the root schema to generate a single merged schema.
 * Involves resolving $ref including template:, removing allOf and combining with $requires.
 * @param schemaPath Path to schema to process.
 * @param templateDirs Template directories to use when resolving template: and $requires.
 * @param feedback Feedback channel
 */
export async function processSchemas(schemaPath: string, templateDirs: string[], feedback: fg.Feedback)
    : Promise<s.Schema> {
    const { allRequired, resolver } = await templateResolver(templateDirs, feedback)
    const formSchema = await getSchema(schemaPath, feedback, resolver)
    const required = {}
    if (!formSchema.$requires) {
        // Default to standard schema
        formSchema.$requires = ['standard.schema']
    }
    if (!formSchema.$templateDirs) {
        // Default to including schema directory
        formSchema.$templateDirs = [ppath.resolve(ppath.dirname(schemaPath))]
    }
    await findRequires(formSchema, allRequired, required, resolver, feedback)
    const allSchema = clone(formSchema)
    if (!allSchema.required) allSchema.required = []
    if (!allSchema.$expectedOnly) allSchema.$expectedOnly = []
    if (!allSchema.$templates) allSchema.$templates = []
    if (!allSchema.$operations) allSchema.$operations = []
    if (!allSchema.$defaultOperation) allSchema.$defaultOperation = []
    if (!allSchema.$requiresValue) allSchema.$requiresValue = []
    if (!allSchema.$examples) allSchema.$examples = []
    if (!allSchema.$parameters) allSchema.$parameters = []
    if (formSchema.$public) {
        allSchema.$public = formSchema.$public
    } else {
        // Default to properties in root schema
        allSchema.$public = Object.keys(formSchema.properties)
    }
    mergeSchemas(allSchema, Object.values(required))
    promoteItems(allSchema)
    ensureTemplate(allSchema)
    return new s.Schema(schemaPath, allSchema)
}

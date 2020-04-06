#!/usr/bin/env node
"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const tslib_1 = require("tslib");
/*!
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
tslib_1.__exportStar(require("./dialogGenerator"), exports);
const crypto = require("crypto");
const expressions = require("adaptive-expressions");
const fs = require("fs-extra");
const lg = require("botbuilder-lg");
const os = require("os");
const ppath = require("path");
const ph = require("./generatePhrases");
const substitutions_1 = require("./substitutions");
const processSchemas_1 = require("./processSchemas");
var FeedbackType;
(function (FeedbackType) {
    FeedbackType[FeedbackType["message"] = 0] = "message";
    FeedbackType[FeedbackType["info"] = 1] = "info";
    FeedbackType[FeedbackType["warning"] = 2] = "warning";
    FeedbackType[FeedbackType["error"] = 3] = "error";
})(FeedbackType = exports.FeedbackType || (exports.FeedbackType = {}));
function templatePath(name, dir) {
    return ppath.join(dir, name);
}
function computeHash(val) {
    return crypto.createHash('md5').update(val).digest('hex');
}
function computeJSONHash(json) {
    return computeHash(JSON.stringify(json, null, 4));
}
const commentHash = ['.lg', '.lu', '.qna'];
const jsonHash = ['.dialog'];
function addHash(path, val) {
    let ext = ppath.extname(path);
    if (commentHash.includes(ext)) {
        if (!val.endsWith(os.EOL)) {
            val += os.EOL;
        }
        val += `${os.EOL}> Generator: ${computeHash(val)}`;
    }
    else if (jsonHash.includes(ext)) {
        let json = JSON.parse(val);
        json.$Generator = computeJSONHash(json);
        val = JSON.stringify(json, null, 4);
    }
    return val;
}
const GeneratorPattern = /\r?\n> Generator: (.*)/m;
async function isUnchanged(path) {
    let result = false;
    let ext = ppath.extname(path);
    let file = await fs.readFile(path, 'utf8');
    if (commentHash.includes(ext)) {
        let match = file.match(GeneratorPattern);
        if (match) {
            let oldHash = match[1];
            file = file.substring(0, match.index);
            let hash = computeHash(file);
            result = oldHash === hash;
        }
    }
    else if (jsonHash.includes(ext)) {
        let json = JSON.parse(file);
        let oldHash = json.$Generator;
        if (oldHash) {
            delete json.$Generator;
            let hash = computeJSONHash(json);
            result = oldHash === hash;
        }
    }
    return result;
}
exports.isUnchanged = isUnchanged;
async function writeFile(path, val, feedback) {
    try {
        let dir = ppath.dirname(path);
        await fs.ensureDir(dir);
        val = addHash(path, val);
        await fs.writeFile(path, val);
    }
    catch (e) {
        let match = /position ([0-9]+)/.exec(e.message);
        if (match) {
            let offset = Number(match[1]);
            val = `${val.substring(0, offset)}^^^${val.substring(offset)}`;
        }
        feedback(FeedbackType.error, `${e.message}${os.EOL}${val}`);
    }
}
async function generateFile(path, val, force, feedback) {
    if (force || !await fs.pathExists(path)) {
        feedback(FeedbackType.info, `Generating ${path}`);
        await writeFile(path, val, feedback);
    }
    else {
        feedback(FeedbackType.warning, `Skipping already existing ${path}`);
    }
}
const expressionEngine = new expressions.ExpressionParser((func) => {
    switch (func) {
        case 'phrase': return ph.PhraseEvaluator;
        case 'phrases': return ph.PhrasesEvaluator;
        case 'substitutions': return substitutions_1.SubstitutionsEvaluator;
        default:
            return expressions.ExpressionFunctions.standardFunctions.get(func);
    }
});
async function findTemplate(name, templateDirs) {
    let template;
    for (let dir of templateDirs) {
        let loc = templatePath(name, dir);
        if (await fs.pathExists(loc)) {
            // Direct file
            template = await fs.readFile(loc, 'utf8');
        }
        else {
            // LG file
            loc = templatePath(name + '.lg', dir);
            if (await fs.pathExists(loc)) {
                template = lg.Templates.parseFile(loc, undefined, expressionEngine);
            }
        }
    }
    return template;
}
// Add prefix to [] imports in constant .lg files
const RefPattern = /^[ \t]*\[[^\]\n]*\][ \t]*$/gm;
function addPrefixToImports(template, scope) {
    return template.replace(RefPattern, (match) => {
        let ref = match.substring(match.indexOf('[') + 1, match.indexOf(']'));
        return `[${scope.prefix}-${ref}](${scope.prefix}-${ref})${os.EOL}`;
    });
}
function addPrefix(prefix, name) {
    return `${prefix}-${name}`;
}
function addEntry(fullPath, outDir, tracker) {
    let ref;
    let basename = ppath.basename(fullPath, '.dialog');
    let ext = ppath.extname(fullPath).substring(1);
    let arr = tracker[ext];
    if (!arr.find(ref => ref.name === basename)) {
        ref = {
            name: basename,
            fallbackName: basename.replace(/\.[^.]+\.lg/, '.lg'),
            fullName: ppath.basename(fullPath),
            relative: ppath.relative(outDir, fullPath)
        };
    }
    return ref;
}
function existingRef(name, tracker) {
    let ext = ppath.extname(name).substring(1);
    let arr = tracker[ext];
    if (!arr) {
        arr = [];
        tracker[ext] = arr;
    }
    return arr.find(ref => ref.fullName === name);
}
async function processTemplate(templateName, templateDirs, outDir, scope, force, feedback, ignorable) {
    let outPath = '';
    let oldDir = process.cwd();
    try {
        let ref = existingRef(templateName, scope.templates);
        if (ref) {
            // Simple file already existed
            outPath = ppath.join(outDir, ref.relative);
        }
        else {
            let template = await findTemplate(templateName, templateDirs);
            if (template !== undefined) {
                // Ignore templates that are defined, but are empty
                if (template) {
                    if (typeof template !== 'object' || template.allTemplates.some(f => f.name === 'template')) {
                        // Constant file or .lg template so output
                        let filename = addPrefix(scope.prefix, templateName);
                        if (typeof template === 'object' && template.allTemplates.some(f => f.name === 'filename')) {
                            try {
                                filename = template.evaluate('filename', scope);
                            }
                            catch (e) {
                                throw new Error(`${templateName}: ${e.message}`);
                            }
                        }
                        else if (filename.includes(scope.locale)) {
                            // Move constant files into locale specific directories
                            filename = `${scope.locale}/${filename}`;
                        }
                        // Add prefix to constant imports
                        if (typeof template !== 'object') {
                            template = addPrefixToImports(template, scope);
                        }
                        outPath = ppath.join(outDir, filename);
                        let ref = addEntry(outPath, outDir, scope.templates);
                        if (ref) {
                            // This is a new file
                            if (force || !await fs.pathExists(outPath)) {
                                feedback(FeedbackType.info, `Generating ${outPath}`);
                                let result = template;
                                if (typeof template === 'object') {
                                    process.chdir(ppath.dirname(template.allTemplates[0].source));
                                    result = template.evaluate('template', scope);
                                    if (Array.isArray(result)) {
                                        result = result.join('\n');
                                    }
                                }
                                // See if generated file has been overridden in templates
                                let existing = await findTemplate(filename, templateDirs);
                                if (existing) {
                                    result = existing;
                                }
                                await writeFile(outPath, result, feedback);
                                scope.templates[ppath.extname(outPath).substring(1)].push(ref);
                            }
                            else {
                                feedback(FeedbackType.warning, `Skipping already existing ${outPath}`);
                            }
                        }
                    }
                    if (typeof template === 'object') {
                        if (template.allTemplates.some(f => f.name === 'entities') && !scope.schema.properties[scope.property].$entities) {
                            let entities = template.evaluate('entities', scope);
                            if (entities) {
                                scope.schema.properties[scope.property].$entities = entities;
                            }
                        }
                        if (template.allTemplates.some(f => f.name === 'templates')) {
                            let generated = template.evaluate('templates', scope);
                            if (!Array.isArray(generated)) {
                                generated = [generated];
                            }
                            for (let generate of generated) {
                                await processTemplate(generate, templateDirs, outDir, scope, force, feedback, false);
                            }
                        }
                    }
                }
            }
            else if (!ignorable) {
                feedback(FeedbackType.error, `Missing template ${templateName}`);
            }
        }
    }
    catch (e) {
        feedback(FeedbackType.error, e.message);
    }
    finally {
        process.chdir(oldDir);
    }
    return outPath;
}
async function processTemplates(schema, templateDirs, locales, outDir, scope, force, feedback) {
    scope.templates = {};
    for (let locale of locales) {
        scope.locale = locale;
        for (let property of schema.schemaProperties()) {
            scope.property = property.path;
            scope.type = property.typeName();
            let templates = property.schema.$templates;
            if (!templates) {
                templates = [scope.type];
            }
            for (let template of templates) {
                await processTemplate(template, templateDirs, outDir, scope, force, feedback, false);
            }
            let entities = property.schema.$entities;
            if (!entities) {
                feedback(FeedbackType.error, `${property.path} does not have $entities defined in schema or template.`);
            }
            else if (!property.schema.$templates) {
                for (let entity of entities) {
                    let [entityName, role] = entity.split(':');
                    scope.entity = entityName;
                    scope.role = role;
                    if (entityName === `${scope.property}Entity`) {
                        entityName = `${scope.type}`;
                    }
                    await processTemplate(`${entityName}Entity-${scope.type}`, templateDirs, outDir, scope, force, feedback, false);
                }
            }
        }
        // Process templates found at the top
        if (schema.schema.$templates) {
            scope.entities = schema.entityTypes();
            for (let templateName of schema.schema.$templates) {
                await processTemplate(templateName, templateDirs, outDir, scope, force, feedback, false);
            }
        }
    }
}
// Expand strings with ${} expression in them by evaluating and then interpreting as JSON.
function expandSchema(schema, scope, path, inProperties, missingIsError, feedback) {
    let newSchema = schema;
    if (Array.isArray(schema)) {
        newSchema = [];
        for (let val of schema) {
            let newVal = expandSchema(val, scope, path, false, missingIsError, feedback);
            newSchema.push(newVal);
        }
    }
    else if (typeof schema === 'object') {
        newSchema = {};
        for (let [key, val] of Object.entries(schema)) {
            let newPath = path;
            if (inProperties) {
                newPath += newPath === '' ? key : '.' + key;
            }
            let newVal = expandSchema(val, Object.assign(Object.assign({}, scope), { property: newPath }), newPath, key === 'properties', missingIsError, feedback);
            newSchema[key] = newVal;
        }
    }
    else if (typeof schema === 'string' && schema.startsWith('${')) {
        let expr = schema.substring(2, schema.length - 1);
        try {
            let { value, error } = expressionEngine.parse(expr).tryEvaluate(scope);
            if (!error && value) {
                newSchema = value;
            }
            else {
                if (missingIsError) {
                    feedback(FeedbackType.error, `${expr}: ${error}`);
                }
            }
        }
        catch (e) {
            feedback(FeedbackType.error, `${expr}: ${e.message}`);
        }
    }
    return newSchema;
}
function expandStandard(dirs) {
    let expanded = [];
    for (let dir of dirs) {
        if (dir === 'standard') {
            dir = ppath.join(__dirname, '../../templates');
        }
        else {
            dir = ppath.resolve(dir);
        }
        expanded.push(dir);
    }
    return expanded;
}
/**
 * Iterate through the locale templates and generate per property/locale files.
 * Each template file will map to <filename>_<property>.<ext>.
 * @param schemaPath Path to JSON Schema to use for generation.
 * @param prefix Prefix to use for generated files.
 * @param outDir Where to put generated files.
 * @param metaSchema Schema to use when generating .dialog files
 * @param allLocales Locales to generate.
 * @param templateDirs Where templates are found.
 * @param force True to force overwriting existing files.
 * @param feedback Callback function for progress and errors.
 */
async function generate(schemaPath, prefix, outDir, metaSchema, allLocales, templateDirs, force, feedback) {
    if (!feedback) {
        feedback = (_info, _message) => true;
    }
    if (!prefix) {
        prefix = ppath.basename(schemaPath, '.schema');
    }
    if (!outDir) {
        outDir = ppath.join(prefix + '-resources');
    }
    if (!metaSchema) {
        metaSchema = 'https://raw.githubusercontent.com/microsoft/botbuilder-dotnet/master/schemas/sdk.schema';
    }
    else if (!metaSchema.startsWith('http')) {
        // Adjust relative to outDir
        metaSchema = ppath.relative(outDir, metaSchema);
    }
    if (!allLocales) {
        allLocales = ['en-us'];
    }
    if (!templateDirs) {
        templateDirs = ['standard'];
    }
    let op = 'Regenerating';
    if (!force) {
        force = false;
        op = 'Generating';
    }
    feedback(FeedbackType.message, `${op} resources for ${ppath.basename(schemaPath, '.schema')} in ${outDir}`);
    feedback(FeedbackType.message, `Locales: ${JSON.stringify(allLocales)} `);
    feedback(FeedbackType.message, `Templates: ${JSON.stringify(templateDirs)} `);
    feedback(FeedbackType.message, `App.schema: ${metaSchema} `);
    try {
        templateDirs = expandStandard(templateDirs);
        await fs.ensureDir(outDir);
        let schema = await processSchemas_1.processSchemas(schemaPath, templateDirs, feedback);
        schema.schema = expandSchema(schema.schema, {}, '', false, false, feedback);
        // Process templates
        let scope = {
            locales: allLocales,
            prefix: prefix || schema.name(),
            schema: schema.schema,
            properties: schema.schema.$public,
            triggerIntent: schema.triggerIntent(),
            appSchema: metaSchema
        };
        await processTemplates(schema, templateDirs, allLocales, outDir, scope, force, feedback);
        // Expand schema expressions
        let expanded = expandSchema(schema.schema, scope, '', false, true, feedback);
        // Write final schema
        let body = JSON.stringify(expanded, (key, val) => (key === '$templates' || key === '$requires') ? undefined : val, 4);
        await generateFile(ppath.join(outDir, `${prefix}.schema.dialog`), body, force, feedback);
    }
    catch (e) {
        feedback(FeedbackType.error, e.message);
    }
}
exports.generate = generate;
//# sourceMappingURL=dialogGenerator.js.map
"use strict";
/*!
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
Object.defineProperty(exports, "__esModule", { value: true });
const tslib_1 = require("tslib");
const bf_cli_command_1 = require("@microsoft/bf-cli-command");
const Validator = require("ajv");
const fs = require("fs-extra");
const glob = require("globby");
const os = require("os");
const ppath = require("path");
const semver = require("semver");
const xp = require("xml2js");
let allof = require('json-schema-merge-allof');
let clone = require('clone');
let getJson = require('get-json');
let parser = require('json-schema-ref-parser');
let util = require('util');
let exec = util.promisify(require('child_process').exec);
class DialogMerge extends bf_cli_command_1.Command {
    constructor() {
        super(...arguments);
        this.verbose = false;
        this.failed = false;
        this.missingKinds = new Set();
        this.currentFile = '';
        this.jsonOptions = { spaces: 4, EOL: os.EOL };
    }
    async run() {
        const { argv, flags } = this.parse(DialogMerge);
        await this.mergeSchemas(argv, flags.output, flags.branch, flags.update, flags.verbose);
    }
    /**
     * Merge together .schema files to make a custom schema.
     * @param patterns Glob patterns for the .schema files to combine.
     * @param output The output file to create.  app.schema by default.
     * @param branch Branch to use for where to find component.schema.
     * @param update True to update .schema files to point to branch component.schema files.
     */
    async mergeSchemas(patterns, output, branch, update, verbose) {
        var e_1, _a;
        this.verbose = verbose;
        this.failed = false;
        this.missingKinds = new Set();
        try {
            let schemaPaths = [];
            if (update) {
                if (!branch) {
                    this.error(`${this.currentFile}: error: Must specify -branch <branch> in order to use -update`);
                    return false;
                }
                await this.updateMetaSchema(branch);
                if (verbose) {
                    this.log(`Updating component.schema references to branch ${branch}`);
                }
            }
            if (!output) {
                output = 'app.schema';
            }
            // delete output so we don't attempt to process it
            if (fs.existsSync(output)) {
                fs.unlinkSync(output);
            }
            try {
                for (var _b = tslib_1.__asyncValues(this.expandPackages(await glob(patterns))), _c; _c = await _b.next(), !_c.done;) {
                    const path = _c.value;
                    schemaPaths.push(path);
                }
            }
            catch (e_1_1) { e_1 = { error: e_1_1 }; }
            finally {
                try {
                    if (_c && !_c.done && (_a = _b.return)) await _a.call(_b);
                }
                finally { if (e_1) throw e_1.error; }
            }
            if (schemaPaths.length === 0) {
                return false;
            }
            else {
                let metaSchema;
                let definitions = {};
                let validator = new Validator();
                if (!metaSchema && branch) {
                    // Find branch specific schema
                    let path = `https://raw.githubusercontent.com/Microsoft/botbuilder-dotnet/${branch}/schemas/component.schema`;
                    metaSchema = await getJson(path);
                }
                if (metaSchema) {
                    validator.addSchema(metaSchema, 'componentSchema');
                }
                for (let schemaPath of schemaPaths) {
                    this.currentFile = schemaPath;
                    if (verbose) {
                        this.log(`Parsing ${schemaPath}`);
                    }
                    if (update) {
                        let schema = await fs.readJSON(schemaPath);
                        if (!schema.$id) {
                            schema.$schema = schema.$schema.replace(/botbuilder-dotnet\/[^/]*\//, `botbuilder-dotnet/${branch}/`);
                            await fs.writeJSON(schemaPath, schema, this.jsonOptions);
                        }
                    }
                    let noref = await parser.dereference(schemaPath);
                    if (noref.$id) {
                        this.error(`${this.currentFile}: warning: Skipping because of top-level $id:${noref.$id}.`);
                    }
                    else {
                        let schema = allof(noref);
                        // Pick up meta-schema from first .dialog file
                        if (!metaSchema) {
                            metaSchema = JSON.parse(await this.getURL(schema.$schema));
                            validator.addSchema(metaSchema, 'componentSchema');
                            if (verbose) {
                                this.log(`  Using component.schema ${metaSchema.$id}`);
                            }
                        }
                        else if (schema.$schema !== metaSchema.$id) {
                            this.error(`${this.currentFile}: error:${this.currentFile}: error:${schema.$schema} does not match component.schema ${metaSchema.$id}`);
                        }
                        delete schema.$schema;
                        if (!validator.validate('componentSchema', schema)) {
                            for (let error of validator.errors) {
                                this.schemaError(error);
                            }
                        }
                        let filename = schemaPath.split(/[\\\/]/).pop();
                        let kind = filename.substr(0, filename.lastIndexOf('.'));
                        if (!schema.type && !this.isInterface(schema)) {
                            schema.type = 'object';
                        }
                        definitions[kind] = schema;
                    }
                }
                this.fixDefinitionReferences(definitions);
                this.processRoles(definitions);
                this.addKindTitles(definitions);
                this.expandKinds(definitions);
                this.addStandardProperties(definitions, metaSchema);
                this.sortImplementations(definitions);
                let finalDefinitions = {};
                for (let key of Object.keys(definitions).sort()) {
                    finalDefinitions[key] = definitions[key];
                }
                let finalSchema = {
                    $schema: metaSchema.$id,
                    $id: ppath.basename(output),
                    type: 'object',
                    title: 'Component kinds',
                    description: 'These are all of the kinds that can be created by the loader.',
                    oneOf: Object.keys(definitions)
                        .filter(schemaName => !this.isInterface(definitions[schemaName]))
                        .sort()
                        .map(schemaName => {
                        return {
                            title: schemaName,
                            description: definitions[schemaName].description || '',
                            $ref: '#/definitions/' + schemaName
                        };
                    }),
                    definitions: finalDefinitions
                };
                if (!this.failed) {
                    this.log(`Writing ${output}`);
                    await fs.writeJSON(output, finalSchema, this.jsonOptions);
                }
                else {
                    this.error(`${this.currentFile}: error: Could not merge schemas`);
                }
            }
        }
        catch (e) {
            this.thrownError(e);
        }
        return true;
    }
    // Expand package.json, package.config or *.csproj to look for .schema below referenced packages.
    expandPackages(paths) {
        return tslib_1.__asyncGenerator(this, arguments, function* expandPackages_1() {
            for (let path of paths) {
                if (path.endsWith('.schema')) {
                    yield yield tslib_1.__await(this.prettyPath(path));
                }
                else {
                    let references = [];
                    let name = ppath.basename(path);
                    if (this.verbose) {
                        this.log(`Following ${path}`);
                    }
                    if (name.endsWith('.csproj')) {
                        references.push(ppath.join(ppath.dirname(path), '/**/*.schema'));
                        let json = yield tslib_1.__await(this.xmlToJSON(path));
                        let packages = yield tslib_1.__await(this.findGlobalNuget());
                        if (packages) {
                            this.walkJSON(json, elt => {
                                let done = false;
                                if (elt.PackageReference) {
                                    for (let pkgRef of elt.PackageReference) {
                                        let pkg = pkgRef.$;
                                        let pkgName = pkg.Include.toLowerCase();
                                        let pkgPath = ppath.join(packages, pkgName);
                                        let versions = [];
                                        for (let version of fs.readdirSync(pkgPath)) {
                                            versions.push(version.toLowerCase());
                                        }
                                        let baseVersion = pkg.Version || '0.0.0';
                                        let version = semver.minSatisfying(versions, `>=${baseVersion.toLowerCase()}`);
                                        references.push(ppath.join(packages, pkgName, version || '', '/**/*.schema'));
                                    }
                                    done = true;
                                }
                                return done;
                            });
                        }
                    }
                    else if (name === 'packages.config') {
                        let json = yield tslib_1.__await(this.xmlToJSON(path));
                        let packages = yield tslib_1.__await(this.findParentDirectory(ppath.dirname(path), 'packages'));
                        if (packages) {
                            this.walkJSON(json, elt => {
                                let done = false;
                                if (elt.package) {
                                    for (let info of elt.package) {
                                        let id = `${info.$.id}.${info.$.version}`;
                                        references.push(ppath.join(packages, `${id}/**/*.schema`));
                                    }
                                    done = true;
                                }
                                return done;
                            });
                        }
                    }
                    else if (name === 'package.json') {
                        let json = yield tslib_1.__await(fs.readJSON(path));
                        for (let pkg in json.dependencies) {
                            references.push(ppath.join(ppath.dirname(path), `node_modules/${pkg}/**/*.schema`));
                        }
                    }
                    else {
                        throw new Error(`Unknown package type ${path}`);
                    }
                    for (let ref of references) {
                        for (let expandedRef of yield tslib_1.__await(glob(ref))) {
                            yield yield tslib_1.__await(this.prettyPath(expandedRef));
                        }
                    }
                }
            }
            return yield tslib_1.__await([]);
        });
    }
    prettyPath(path) {
        let newPath = ppath.relative(process.cwd(), path);
        if (newPath.startsWith('..')) {
            newPath = path;
        }
        return newPath;
    }
    async findGlobalNuget() {
        let result = '';
        try {
            const { stdout } = await exec('dotnet nuget locals global-packages --list');
            const name = 'global-packages:';
            let start = stdout.indexOf(name);
            if (start > -1) {
                result = stdout.substring(start + name.length).trim();
            }
        }
        catch (err) {
            this.warn(`${this.currentFile}: warning: Cannot find global nuget packages so skipping .csproj\n${err}`);
        }
        return result;
    }
    async xmlToJSON(path) {
        let xml = (await fs.readFile(path)).toString();
        return new Promise((resolve, reject) => xp.parseString(xml, (err, result) => {
            if (err) {
                reject(err);
            }
            else {
                resolve(result);
            }
        }));
    }
    async findParentDirectory(path, dir) {
        path = ppath.resolve(path);
        let result = '';
        if (path) {
            result = ppath.join(path, dir);
            if (!await fs.pathExists(result)) {
                result = await this.findParentDirectory(ppath.dirname(path), dir);
            }
        }
        return result;
    }
    // Update component.schema to a specific branch version
    async updateMetaSchema(branch) {
        if (fs.existsSync('baseComponent.schema')) {
            if (this.verbose) {
                this.log(`Generating component.schema for branch ${branch}`);
            }
            let schema = await fs.readJSON('baseComponent.schema');
            let metaSchemaName = schema.$schema;
            // tslint:disable-next-line: no-http-string
            if (metaSchemaName === 'http://json-schema.org/draft-07/schema#') {
                // This is because http is referral, but redirects
                metaSchemaName = 'https://json-schema.org/draft-07/schema#';
            }
            let metaSchemaDef = await this.getURL(metaSchemaName);
            let metaSchema = JSON.parse(metaSchemaDef);
            for (let prop in schema) {
                let propDef = schema[prop];
                if (typeof propDef === 'string') {
                    metaSchema[prop] = propDef;
                }
                else {
                    for (let subProp in propDef) {
                        metaSchema[prop][subProp] = propDef[subProp];
                    }
                }
            }
            metaSchema.$id = metaSchema.$id.replace('{branch}', branch);
            metaSchema.$comment = `File generated by: 'bf dialog:merge -u -b ${branch}'.`;
            await fs.writeJSON('component.schema', metaSchema, this.jsonOptions);
        }
    }
    processRoles(definitions) {
        for (let kind in definitions) {
            this.walkJSON(definitions[kind], (val, _obj, key) => {
                if (val.$role) {
                    if (typeof val.$role === 'string') {
                        this.processRole(val.$role, val, kind, definitions, key);
                    }
                    else {
                        for (let role of val.$role) {
                            this.processRole(role, val, kind, definitions, key);
                        }
                    }
                }
                return false;
            });
        }
    }
    processRole(role, elt, kind, definitions, key) {
        const prefix = 'implements(';
        if (role === 'expression') {
            const reserved = ['title', 'description', 'examples', '$role'];
            if (elt.kind) {
                this.error(`${this.currentFile}:error: $role ${role} must not have a kind.`);
            }
            if (elt.type) {
                if (Array.isArray(elt.type)) {
                    if (!elt.type.includes('string')) {
                        elt.type.push('string');
                    }
                }
                else if (elt.type !== 'string') {
                    let props = {};
                    let desc = `Expression evaluating to ${elt.type}.`;
                    for (let key of Object.keys(elt)) {
                        if (!reserved.includes(key)) {
                            let value = elt[key];
                            delete elt[key];
                            props[key] = value;
                        }
                    }
                    elt.oneOf = [props, {
                            type: 'string',
                            title: 'Expression',
                            description: desc
                        }];
                }
            }
            else if (elt.oneOf || elt.anyOf || elt.allOf) {
                let choices = elt.oneOf || elt.anyOf || elt.allOf;
                let found = false;
                for (let choice of choices) {
                    if (choice.type === 'string') {
                        found = true;
                        break;
                    }
                }
                if (!found) {
                    choices.push({ type: 'string', title: 'Expression' });
                }
            }
        }
        else if (role === 'interface') {
            if (key) {
                this.error(`${this.currentFile}:error: interface $role can only be defined at the top of the schema definition.`);
            }
        }
        else if (role.startsWith(prefix) && role.endsWith(')')) {
            let interfaceName = role.substring(prefix.length, role.length - 1);
            if (!definitions[interfaceName]) {
                this.error(`${this.currentFile}:error: interface ${interfaceName} is not defined.`);
            }
            else if (!this.isInterface(definitions[interfaceName])) {
                this.error(`${this.currentFile}:error: is missing $role of interface.`);
            }
            else {
                let definition = definitions[kind];
                let interfaceDefinition = definitions[interfaceName];
                if (!interfaceDefinition.oneOf) {
                    interfaceDefinition.oneOf = [
                        {
                            type: 'string',
                            title: `Reference to ${interfaceName}`,
                            description: `Reference to ${interfaceName} .dialog file.`
                        }
                    ];
                }
                interfaceDefinition.oneOf.push({
                    title: kind,
                    description: definition.description || '',
                    $ref: `#/definitions/${kind}`
                });
            }
        }
        else {
            this.error(`${this.currentFile}:error: Unknown $role ${role}`);
        }
    }
    addKindTitles(definitions) {
        this.walkJSON(definitions, val => {
            if (val.oneOf) {
                this.walkJSON(val.oneOf, def => {
                    if (def.type && !def.title) {
                        // NOTE: For simple types, not kinds promote into title to prevent collisions
                        def.title = def.type;
                    }
                    return false;
                });
            }
            return false;
        });
    }
    fixDefinitionReferences(definitions) {
        for (let kind in definitions) {
            this.walkJSON(definitions[kind], (val) => {
                if (val.$ref && typeof val.$ref === 'string') {
                    let ref = val.$ref;
                    if (ref.startsWith('#/definitions/')) {
                        val.$ref = '#/definitions/' + kind + '/definitions' + ref.substr(ref.indexOf('/'));
                    }
                }
                return false;
            });
        }
    }
    expandKinds(definitions) {
        this.walkJSON(definitions, val => {
            if (val.$kind) {
                if (definitions.hasOwnProperty(val.$kind)) {
                    val.$ref = '#/definitions/' + val.$kind;
                }
                else {
                    this.missing(val.$kind);
                }
            }
            return false;
        });
    }
    addStandardProperties(definitions, dialogSchema) {
        for (let kind in definitions) {
            let definition = definitions[kind];
            if (!this.isInterface(definition)) {
                // Reorder properties to put $ first.
                let props = {
                    $kind: clone(dialogSchema.definitions.kind),
                    $copy: dialogSchema.definitions.copy,
                    $id: dialogSchema.definitions.id,
                    $designer: dialogSchema.definitions.designer
                };
                props.$kind.const = kind;
                if (definition.properties) {
                    for (let prop in definition.properties) {
                        props[prop] = definition.properties[prop];
                    }
                }
                definition.properties = props;
                definition.additionalProperties = false;
                definition.patternProperties = { '^\\$': { type: 'string' } };
                let required = definition.required;
                if (required) {
                    required.push('$kind');
                }
                else {
                    required = ['$kind'];
                }
                delete definition.required;
                definition.anyOf = [
                    {
                        title: 'Reference',
                        required: ['$copy']
                    },
                    {
                        title: 'Type',
                        required
                    }
                ];
            }
        }
    }
    sortImplementations(definitions) {
        for (let key in definitions) {
            let definition = definitions[key];
            if (this.isInterface(definition) && definition.oneOf) {
                definition.oneOf = definition.oneOf.sort((a, b) => a.title.localeCompare(b.title));
            }
        }
    }
    walkJSON(elt, fun, obj, key) {
        let done = fun(elt, obj, key);
        if (!done) {
            if (Array.isArray(elt)) {
                for (let val of elt) {
                    done = this.walkJSON(val, fun);
                    if (done)
                        break;
                }
            }
            else if (typeof elt === 'object') {
                for (let val in elt) {
                    done = this.walkJSON(elt[val], fun, elt, val);
                    if (done)
                        break;
                }
            }
        }
        return done;
    }
    async getURL(url) {
        return new Promise((resolve, reject) => {
            const http = require('http');
            const https = require('https');
            let client = http;
            if (url.toString().indexOf('https') === 0) {
                client = https;
            }
            client.get(url, (resp) => {
                let data = '';
                // A chunk of data has been recieved.
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
    isInterface(schema) {
        return schema.$role === 'interface';
    }
    missing(kind) {
        if (!this.missingKinds.has(kind)) {
            this.error(`${this.currentFile}: error: Missing ${kind} schema file from merge.`);
            this.missingKinds.add(kind);
            this.failed = true;
        }
    }
    schemaError(err) {
        this.error(`${this.currentFile}: error:${err.dataPath} ${err.message}`);
        this.failed = true;
    }
    thrownError(err) {
        this.error(`${this.currentFile}: error:${err.message}`);
        this.failed = true;
    }
    errorMsg(kind, message) {
        this.error(`${this.currentFile}: error:${kind}: ${message}`);
        this.failed = true;
    }
}
exports.default = DialogMerge;
DialogMerge.args = [
    { name: 'glob1', required: true },
    { name: 'glob2', required: false },
    { name: 'glob3', required: false },
    { name: 'glob4', required: false },
    { name: 'glob5', required: false },
    { name: 'glob6', required: false },
    { name: 'glob7', required: false },
    { name: 'glob8', required: false },
    { name: 'glob9', required: false },
];
DialogMerge.flags = {
    help: bf_cli_command_1.flags.help({ char: 'h' }),
    output: bf_cli_command_1.flags.string({ char: 'o', description: 'Output path and filename for merged schema. [default: app.schema]', default: 'app.schema', required: false }),
    branch: bf_cli_command_1.flags.string({ char: 'b', description: 'The branch to use for the meta-schema component.schema.', default: 'master', required: false }),
    update: bf_cli_command_1.flags.boolean({ char: 'u', description: 'Update .schema files to point the <branch> component.schema and regenerate component.schema if baseComponent.schema is present.', default: false, required: false }),
    verbose: bf_cli_command_1.flags.boolean({ description: 'output verbose logging of files as they are processed', default: false }),
};
DialogMerge.examples = [
    '$ bf dialog:merge *.csporj',
    '$ bf dialog:merge libraries/*.schema -o app.schema'
];
//# sourceMappingURL=merge.js.map
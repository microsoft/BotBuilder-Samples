#!/usr/bin/env node
"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
/*!
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
const expr = require("adaptive-expressions");
const fs = require("fs-extra");
const os = require("os");
const random = require("seedrandom");
/**
 * Return the result of replicating lines from a source file and substituting random values
 * from bindings into ${variable} placeholders.
 * @param path Path to file with lines.
 * @param bindings Object with binding names and an array of choices.
 * @param copies Optional number of times to copy each line, default is 1.
 * @param seed Optional seed string for random number generator, default is "0".
 */
function substitutions(path, bindings, copies, seed) {
    if (!copies)
        copies = 1;
    if (!seed)
        seed = '0';
    for (let binding of Object.keys(bindings)) {
        bindings[binding] = bindings[binding].flat();
    }
    let result = [];
    let rand = random(seed);
    let lines = fs.readFileSync(path).toString().split(os.EOL);
    for (let line of lines) {
        if (line.startsWith('>') || line.trim() === '') { // Copy comments
            result.push(line);
        }
        else {
            for (let i = 0; i < copies; ++i) {
                let newline = line.replace(/\${([^}]*)\}/g, (_, key) => {
                    let choice = '**MISSING**';
                    let choices = bindings[key];
                    if (choices) {
                        choice = choices[Math.abs(rand.int32()) % choices.length];
                    }
                    return choice;
                });
                result.push(newline);
            }
        }
    }
    return result.join('\n');
}
exports.SubstitutionsEvaluator = new expr.ExpressionEvaluator('substitutions', expr.ExpressionFunctions.apply(args => {
    let path = args[0];
    let bindings = args[1];
    let replications = args.length > 2 ? args[2] : undefined;
    let seed = args.length > 3 ? args[3] : undefined;
    return substitutions(path, bindings, replications, seed);
}, (val, expr, pos) => {
    let error;
    switch (pos) {
        case 0:
            if (typeof val !== 'string')
                error = `${expr} does not have a path.`;
            break;
        case 1:
            if (typeof val !== 'object')
                error = `${expr} does not have bindings.`;
            break;
        case 2:
            if (typeof val !== 'number')
                error = `${expr} does not have a numeric replication count.`;
            break;
        case 3:
            if (typeof val !== 'string')
                error = `${expr} does not have a string random seed.`;
    }
    return error;
}), expr.ReturnType.String, e => expr.ExpressionFunctions.validateOrder(e, [expr.ReturnType.Number, expr.ReturnType.String], expr.ReturnType.String, expr.ReturnType.Object));
//# sourceMappingURL=substitutions.js.map
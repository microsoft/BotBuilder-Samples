#!/usr/bin/env node
/*!
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
import * as expr from 'adaptive-expressions'
import * as fs from 'fs-extra'
import * as os from 'os'
import * as random from 'seedrandom'

/**
 * Return the result of replicating lines from a source file and substituting random values 
 * from bindings into ${variable} placeholders.
 * @param path Path to file with lines.
 * @param bindings Object with binding names and an array of choices.
 * @param copies Optional number of times to copy each line, default is 1.
 * @param seed Optional seed string for random number generator, default is "0".
 */
function substitutions(path: string, bindings: any, copies?: number, seed?: string): string {
    if (!copies) copies = 1
    if (!seed) seed = '0'
    for (let binding of Object.keys(bindings)) {
        bindings[binding] = bindings[binding].flat()
    }
    let result: string[] = []
    let rand = random(seed)
    let file = fs.readFileSync(path, 'utf8')
    let lines = file.split(os.EOL)
    if (lines.length < 2) {
        // Windows uses CRLF and that is how it is checked-in, but when an npm
        // package is built it switches to just LF.
        lines = file.split('\n')
    }
    for (let line of lines) {
        if (line.startsWith('>') || line.trim() === '') { // Copy comments
            result.push(line)
        } else {
            for (let i = 0; i < copies; ++i) {
                let newline = line.replace(/\${([^}]*)\}/g,
                    (_, key) => {
                        let choice = '**MISSING**'
                        let choices = bindings[key]
                        if (choices) {
                            choice = choices[Math.abs(rand.int32()) % choices.length]
                        }
                        return choice
                    })
                result.push(newline)
            }
        }
    }
    return result.join(os.EOL)
}

export let SubstitutionsEvaluator = new expr.ExpressionEvaluator('substitutions',
    expr.ExpressionFunctions.apply(
        args => {
            let path = args[0]
            let bindings = args[1]
            let replications = args.length > 2 ? args[2] : undefined
            let seed = args.length > 3 ? args[3] : undefined
            return substitutions(path, bindings, replications, seed)
        },
        (val, expr, pos) => {
            let error
            switch (pos) {
                case 0:
                    if (typeof val !== 'string') error = `${expr} does not have a path.`
                    break
                case 1:
                    if (typeof val !== 'object') error = `${expr} does not have bindings.`
                    break;
                case 2:
                    if (typeof val !== 'number') error = `${expr} does not have a numeric replication count.`
                    break;
                case 3:
                    if (typeof val !== 'string') error = `${expr} does not have a string random seed.`
            }
            return error
        }),
    expr.ReturnType.String,
    e => expr.ExpressionFunctions.validateOrder(e,
        [expr.ReturnType.Number, expr.ReturnType.String],
        expr.ReturnType.String,
        expr.ReturnType.Object))

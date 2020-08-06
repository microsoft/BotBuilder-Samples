#!/usr/bin/env node
/*!
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
import * as expr from 'adaptive-expressions'
import * as fs from 'fs-extra'
import * as os from 'os'
import * as random from 'seedrandom'

type Variable = {index: number, values: any}
type State = Map<string, Variable>

// Increment all fixed ones and return true if we hit the end
function increment(bindings: State): boolean {
    let fixed = 0
    let overflow = 0
    bindings.forEach(binding => binding.index < 0 || ++fixed);
    if (fixed > 0) {
        for (let variable of bindings.values()) {
            if (variable.index >= 0) {
                ++variable.index
                if (variable.index < variable.values.length) {
                    break;
                } else {
                    variable.index = 0
                    ++overflow
                }
            }
        }
    }
    return fixed === overflow
}

// Given a key either return the current value or a random one
function binding(key: string, bindings: State, rand: random.prng): string {
    let variable = bindings.get(key)
    let value: any
    if (variable) {
        if (variable.values.length > 0) {
            if (variable.index < 0) {
                value = variable.values[Math.abs(rand.int32()) % variable.values.length]
            } else {
                value = variable.values[variable.index]
            }
        }
    } else {
        value = `**Missing ${key}**`
    }
    return value
}

/**
 * Return the result of replicating lines from a source file and substituting random values 
 * from bindings into ${variable} placeholders for random choice or ${variable*} for all choices.
 * You can also use ${variable?} to conditionally test for the presence of a variable.
 * Lines with a missing 
 * @param path Path to file with lines.
 * @param bindings Object with binding names and an array of choices that will be flattened.
 * @param copies Optional number of times to copy each line with random values, default is 1.
 * @param seed Optional seed string for random number generator, default is "0".
 */
function substitutions(path: string, bindings: any, copies?: number, seed?: string): string {
    if (!copies) copies = 1
    if (!seed) seed = '0'

    // Normalize bindings into a simple array and setup variables
    let state = new Map<string, Variable>();
    for (let binding of Object.keys(bindings)) {
        let value = bindings[binding]
        if (Array.isArray(value)) {
            state.set(binding, {index: -1, values: (value as any).flat()})
        } else {
            state.set(binding, {index: -1, values: [value]})
        }
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
        let lineCopies = copies
        if (line.startsWith('>') || line.trim() === '') {
            // Copy comments unless filtered by test
            lineCopies = 1
        }
        let generated = new Set<string>()
        let all = line.match(/\${[^}*]+\*}/g) || []
        for (let [key, variable] of state) {
            variable.index = all.includes(`\${${key}*}`) ? 0 : -1
        }
        do {
            let missing = false
            for (let i = 0; i < lineCopies; ++i) {
                // Number of times to try for a unique result
                let tries = 3
                do {
                    let newline = line.replace(/\${([^}*?]+)[*?]?\}/g,
                        (match, key) => {
                            let val = binding(key, state, rand)
                            if (!val) {
                                missing = true
                            }
                            // For conditional tests value is empty but we set missing
                            return match.endsWith('?}') ? '' : val
                        })
                    if (missing) {
                        // If missing a value, drop the line
                        tries = 0
                        i = lineCopies
                        break
                    }
                    tries = tries - 1
                    if (!generated.has(newline)) {
                        generated.add(newline)
                        result.push(newline)
                        tries = 0
                    }
                } while (tries > 0)
            }
        } while (!increment(state))
    }
    return result.join(os.EOL)
}

export let SubstitutionsEvaluator = new expr.ExpressionEvaluator('substitutions',
    expr.FunctionUtils.apply(
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
    e => expr.FunctionUtils.validateOrder(e,
        [expr.ReturnType.Number, expr.ReturnType.String],
        expr.ReturnType.String,
        expr.ReturnType.Object))

#!/usr/bin/env node
"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
/*!
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
const expr = require("adaptive-expressions");
function generateWords(name, locale) {
    let words = [];
    switch (locale) {
        default:
            let current = 'lower';
            let start = 0;
            let i = 0;
            // Skip first character and treat as lower
            while (++i < name.length) {
                let ch = name.charAt(i);
                let split = false;
                let end = i;
                if (ch === ' ' || ch === '_' || ch === '-') {
                    split = true;
                    while (i + 1 < name.length) {
                        ch = name.charAt(i);
                        if (ch === ' ' || ch === '_' || ch === '-') {
                            ++i;
                        }
                        else {
                            break;
                        }
                    }
                }
                if (ch === ch.toLowerCase()) {
                    split = split || current === 'upper';
                    current = 'lower';
                }
                else if (ch === ch.toUpperCase()) {
                    split = split || current === 'lower';
                    current = 'upper';
                }
                // Word is either all same case or initial case is different
                if (split && end - start > 1) {
                    words.push(name.substring(start, end).toLowerCase());
                    start = i;
                }
            }
            if (start < name.length) {
                words.push(name.substring(start).toLowerCase());
            }
    }
    return words;
}
function* generatePhrases(name, locale, minLen, maxLen) {
    if (name) {
        let words = generateWords(name, locale);
        minLen = minLen || 1;
        maxLen = maxLen || words.length;
        for (let len = minLen; len <= maxLen; ++len) {
            for (let start = 0; start <= words.length - len; ++start) {
                yield words.slice(start, start + len).join(' ');
            }
        }
    }
}
exports.generatePhrases = generatePhrases;
exports.PhraseEvaluator = new expr.ExpressionEvaluator('phrase', expr.ExpressionFunctions.apply(args => {
    let name = args[0];
    let locale = args.length > 1 ? args[1] : 'en-us';
    return generateWords(name, locale).join(' ');
}, (val, expr, pos) => {
    let error;
    switch (pos) {
        case 0:
            if (typeof val !== 'string')
                error = `${expr} does not have a name string.`;
            break;
        case 1:
            if (typeof val !== 'string')
                error = `${expr} does not have a locale string.`;
    }
    return error;
}), expr.ReturnType.String, e => expr.ExpressionFunctions.validateOrder(e, [expr.ReturnType.String], expr.ReturnType.String));
exports.PhrasesEvaluator = new expr.ExpressionEvaluator('phrases', expr.ExpressionFunctions.apply(args => {
    let name = args[0];
    let locale = args.length > 1 ? args[1] : 'en-us';
    let min = args.length > 2 ? args[2] : undefined;
    let max = args.length > 3 ? args[3] : undefined;
    return Array.from(generatePhrases(name, locale, min, max));
}, (val, expr, pos) => {
    let error;
    switch (pos) {
        case 0:
            if (typeof val !== 'string')
                error = `${expr} does not have a name string.`;
            break;
        case 1:
            if (typeof val !== 'string')
                error = `${expr} does not have a locale string.`;
            break;
        case 2:
            if (typeof val !== 'number')
                error = `${expr} does not have a numeric minimum number of words.`;
            break;
        case 3:
            if (typeof val !== 'number')
                error = `${expr} does not have a numeric maximum number of words.`;
    }
    return error;
}), expr.ReturnType.String, e => expr.ExpressionFunctions.validateOrder(e, [expr.ReturnType.String, expr.ReturnType.Number, expr.ReturnType.Number], expr.ReturnType.String));
//# sourceMappingURL=generatePhrases.js.map
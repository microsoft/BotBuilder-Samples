#!/usr/bin/env node
/*!
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
import * as expr from 'adaptive-expressions';
export declare function generatePhrases(name?: string, locale?: string, minLen?: number, maxLen?: number): IterableIterator<string>;
export declare let PhraseEvaluator: expr.ExpressionEvaluator;
export declare let PhrasesEvaluator: expr.ExpressionEvaluator;

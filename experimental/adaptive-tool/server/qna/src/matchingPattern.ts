/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

export function isImport(content: string): boolean {
    const regex = /^\s*\[[^\]]*\]\([^)]*$/;
    return regex.test(content);
}

export function isHash(content: string): boolean {
    const regex = /^\s*#\s*$/;
    return regex.test(content);
}

export function isFiltersOrPrompts(content: string): boolean {
    const regex = /^\s*\*\*\s*$/i;
    return regex.test(content);
}

export function isId(content: string): boolean {
    const regex = /^\s*<a\s*$/i;
    return regex.test(content);
}

export function isAnswer(content: string): boolean {
    const regex = /^\s*```\s*$/;
    return regex.test(content);
}

export function isQASourceOrKBName(content: string): boolean {
    const regex = /^\s*>\s*!#\s*@\s*$/;
    return regex.test(content);
}

export function isContextOnly(content: string): boolean {
    const regex = /^\s*-\s*\[[^\]]*\]\([^)]*\)\s*$/;
    return regex.test(content);
}

export function isMultiturnReference(content: string): boolean {
    const regex = /^\s*-\s*\[[^\]]*\]\(#\??\s*$/;
    return regex.test(content);
}

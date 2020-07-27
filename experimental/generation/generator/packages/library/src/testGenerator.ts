#!/usr/bin/env node
/*!
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
import * as fs from 'fs-extra'
import * as ppath from 'path'

export async function generateTest(path: string, dialog: string, output: string, schema?: string): Promise<string> {
    let transcript = JSON.parse(await fs.readFile(path, 'utf8'))
    await fs.ensureDir(output)
    let outputPath = ppath.resolve(ppath.join(output, ppath.basename(path, ppath.extname(path)) + '.dialog'))
    let test: any = {}
    if (schema) {
        test.$schema = ppath.relative(ppath.resolve(schema), ppath.resolve(outputPath))
    }
    test.$kind = 'Microsoft.Test.Script'
    test.dialog = dialog
    let script: object[] = []
    test.script = script

    for (let record of transcript) {
        if (isBot(record)) {
            if (record.text) {
                script.push({$kind: 'Microsoft.Test.AssertReply', text: record.text})
            } else if (record.attachments) {
                let assertions: any[] = []
                script.push({$kind: 'Microsoft.Test.AssertReplyActivity', assertions})
                assertions.push(`type == 'message'`)
                objectAssertions(record.attachments, assertions, 'attachments')
            }
        } else if (isUser(record)) {
            script.push({$kind: 'Microsoft.Test.UserSays', 'text': record.text})
        }
    }
    await fs.writeJSON(outputPath, test, {spaces: 2})
    return outputPath
}

function isBot(record: any): Boolean {
    return record.type === 'message' && record.from.role === 'bot'
}

function isUser(record: any): Boolean {
    return record.type === 'message' && record.from.role === 'user' && record.text
}

function objectAssertions(object: any, assertions: any[], path: string) {
    if (Array.isArray(object)) {
        assertions.push(`count(${path}) == ${object.length}`)
        for (let i = 0; i < object.length; ++i) {
            let elt = object[i]
            objectAssertions(elt, assertions, path + `[${i}]`)
        }
    } else if (typeof object === 'object') {
        for (let [key, value] of Object.entries(object)) {
            objectAssertions(value, assertions, path + `.${key}`)
        }
    } else if (typeof object === 'string') {
        assertions.push(`${path} === '${object}'`)
    } else {
        assertions.push(`${path} === ${object}`)
    }
}
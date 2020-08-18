#!/usr/bin/env node
/*!
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
import * as fs from 'fs-extra'
import * as ppath from 'path'

/**
 * Given a transcript and dialog generate a test script.
 * @param path Path to transcript file.
 * @param dialog Name of dialog to test.
 * @param output Directory to put test into.
 * @param mock Whether to mock http requests or not.
 * @param schema Schema to use for test scripts.
 */
export async function generateTest(path: string, dialog: string, output: string, mock: boolean, schema?: string): Promise<string> {
    let transcript = JSON.parse(await fs.readFile(path, 'utf8'))
    await fs.ensureDir(output)
    let outputPath = ppath.resolve(ppath.join(output, ppath.basename(path, ppath.extname(path)) + '.test.dialog'))
    let test: any = {}
    if (schema) {
        test.$schema = ppath.relative(ppath.resolve(schema), ppath.resolve(outputPath))
    }
    test.$kind = 'Microsoft.Test.Script'
    test.dialog = dialog
    let script: object[] = []
    test.script = script
    let mocks: any[] = []
    test.httpRequestMocks = mocks

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
        } else if (isUserActivity(record)) {
            // TODO: There may be other fields that need to be set In other scenarios.
            script.push({$kind: 'Microsoft.Test.UserActivity','activity': {'type': record.type, 'value': record.value}})
        } else if (isConversationUpdate(record)) {
            let membersAdded: string[] = []
            let membersRemoved: string[] = []
            for (let member of record.membersAdded) {
                membersAdded.push(member.name)
            }
            for (let member of record.membersRemoved) {
                membersRemoved.push(member.name)
            }
            script.push({$kind: 'Microsoft.Test.UserConversationUpdate', membersAdded, membersRemoved})
        } else if (mock && isHttpRequest(record)) {
            let request = record.value.request
            let mock = mocks.find(m => m.url === request.url && m.method === request.method)
            if (!mock) {
                mock = { url: request.url, method: request.method }
                mocks.push(mock)
            }
            mock.responses.push({
                // TODO: Responses really should include a status code as well to model errors.
                contentType: 'String',
                content: record.value.response
            })
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

function isUserActivity(recode: any): Boolean {
    return recode.type === 'message' && recode.from.role === 'user';
}

function isConversationUpdate(record: any): Boolean {
    return record.type === 'conversationUpdate'
}

function isHttpRequest(record: any): Boolean {
    return record.type === 'trace' && record.valueType === 'Microsoft.HttpRequest'
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
        assertions.push(`${path} == '${object}'`)
    } else {
        assertions.push(`${path} == ${object}`)
    }
}
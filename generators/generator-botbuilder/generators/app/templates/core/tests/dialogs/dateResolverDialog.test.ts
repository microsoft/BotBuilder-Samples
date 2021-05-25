/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
import { DialogTestClient, DialogTestLogger } from 'botbuilder-testing';
import { DateResolverDialog } from '../../dialogs/dateResolverDialog';
const assert = require('assert');

describe('DateResolverDialog', () => {
    const testCases = require('./testData/dateResolverTestCases');
    const sut = new DateResolverDialog('dateResolver');

    testCases.map((testData) => {
        it(testData.name, async () => {
            const client = new DialogTestClient('test', sut, testData.initialData, [new DialogTestLogger()]);

            // Execute the test case
            console.log(`Test Case: ${ testData.name }`);
            console.log(`Dialog Input ${ JSON.stringify(testData.initialData) }`);
            for (const step of testData.steps) {
                const reply = await client.sendActivity(step[0]);
                assert.strictEqual((reply ? reply.text : null), step[1], `${ reply ? reply.text : null } != ${ step[1] }`);
            }
            console.log(`Dialog result: ${ client.dialogTurnResult.result }`);
            assert.strictEqual(client.dialogTurnResult.result, testData.expectedResult, `${ testData.expectedResult } != ${ client.dialogTurnResult.result }`);
        });
    });
});

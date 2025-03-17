// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// @ts-check

const fetch = require('cross-fetch').default;
const path = require('path');
const dotenv = require('dotenv');

const ENV_FILE = path.join(__dirname, '.env');
dotenv.config({ path: ENV_FILE });

module.exports = async function(
    token,
    { domain = process.env.WEBSITE_HOSTNAME } = {}
) {
    console.log(`Renewing Direct Line token using token "${ token.substring(0, 3) }...${ token.substring(-3) }"`);

    const tokenRes = await fetch(`${ domain }/v3/directline/tokens/refresh`, {
        headers: {
            authorization: `Bearer ${ token }`,
            'Content-Type': 'application/json'
        },
        method: 'POST'
    });

    if (tokenRes.status === 200) {
        const json = await tokenRes.json();

        if (json.error) {
            throw new Error(`Direct Line service responded ${ JSON.stringify(json.error) } while renewing token`);
        } else {
            return json;
        }
    } else {
        throw new Error(`Direct Line service returned ${ tokenRes.status } while renewing token`);
    }
};

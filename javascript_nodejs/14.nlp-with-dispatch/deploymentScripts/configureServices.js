// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { spawnSync } = require('child_process');
const { path } = require('path');
// Import required bot confuguration.
const { BotConfiguration } = require('botframework-config');

const CONFIG_ERROR = 1;

// Read botFilePath and botFileSecret from .env file
// Note: Ensure you have a .env file and include botFilePath and botFileSecret.
const ENV_FILE = path.join(__dirname, '.env');
const env = require('dotenv').config({path: ENV_FILE});

const BOT_FILE = path.join(__dirname, (process.env.botFilePath || ''));

// read bot configuration from .bot file. 
let botConfig;
try {
    botConfig = BotConfiguration.loadSync(BOT_FILE, process.env.botFileSecret);
} catch (err) {
    console.log(`Error reading bot file. Please ensure you have valid botFilePath and botFileSecret set for your environment`);
    process.exit(CONFIG_ERROR);
}

// install required tools
const TOOLS_REQUIRED = 'msbot luis-apis qnamaker ludown botdispatch';

const toolsInstall = spawnSync(`npm i -g ${TOOLS_REQUIRED}`);

console.log(toolsInstall);
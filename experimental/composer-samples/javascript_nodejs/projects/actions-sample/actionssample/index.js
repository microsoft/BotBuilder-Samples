// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

const { start } = require('botbuilder-dialogs-adaptive-runtime-integration-express');

(async function () {
  try {
    await start(process.cwd(), "settings");
  } catch (err) {
    console.error(err);
    process.exit(1);
  }
})();


// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const random = require('math-random');

module.exports = function createUserID() {
    return `dl_${ random().toString(36).substring(2) }`;
};

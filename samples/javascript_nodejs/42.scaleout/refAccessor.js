// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

class RefAccessor {

    constructor(dialogState) {
        this.dialogState = dialogState;
    }

    get value() {
        return this.dialogState;
    }

    async get(turnContext, defaultValue) {
        if (this.dialogState === undefined) {
            this.dialogState = JSON.parse(JSON.stringify(defaultValue));
        }
        return this.dialogState;
    }
}

module.exports.RefAccessor = RefAccessor;

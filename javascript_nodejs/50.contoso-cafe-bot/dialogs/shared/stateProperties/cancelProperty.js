// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

class CancelProperty {
    constructor(activeDialog) {
        this.retryCounter = 0;
        this.activeDialog = activeDialog ? activeDialog : '';
    }
};

CancelProperty.resetTurnCounter = function() {
    this.retryCounter = 0;
}

module.exports = CancelProperty;
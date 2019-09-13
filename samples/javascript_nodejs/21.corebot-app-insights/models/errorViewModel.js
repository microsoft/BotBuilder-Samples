// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

class ErrorViewModel {
    constructor(requestId = null) {
        this.requestId = requestId;
    }

    showRequestId() {
        return Boolean(this.requestId);
    }
}

module.exports.ErrorViewModel = ErrorViewModel;

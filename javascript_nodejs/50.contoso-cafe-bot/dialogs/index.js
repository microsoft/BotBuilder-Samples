// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

module.exports = {
    WhoAreYouDialog: require('./whoAreYou').WhoAreYouDialog,
    WhatCanYouDoDialog: require('./whatCanYouDo').WhatCanYouDoDialog,
    QnADialog: require('./qna').QnADialog,
    HelpDialog: require('./help'),
    FindCafeLocationsDialog: require('./findCafeLocations').FindCafeLocationsDialog,
    ChitChatDialog: require('./chitChat').ChitChatDialog,
    CancelDialog: require('./cancel').CancelDialog,
    BookTableDialog: require('./bookTable').BookTableDialog
};
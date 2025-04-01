// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// @ts-check

class UserProfile {
    constructor(transport, name, age, picture) {
        this.transport = transport;
        this.name = name;
        this.age = age;
        this.picture = picture;
    }
}

module.exports.UserProfile = UserProfile;

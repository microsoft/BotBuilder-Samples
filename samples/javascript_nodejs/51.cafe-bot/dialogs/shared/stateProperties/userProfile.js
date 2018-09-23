// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

class UserProfile {
    /**
     * User Profile Property constructor.
     *
     * @param {String} name user name
     * @param {String} location user location
     */
    constructor(name, location) {
        this.userName = name;
        this.location = location || '';
    }
};

module.exports = UserProfile;

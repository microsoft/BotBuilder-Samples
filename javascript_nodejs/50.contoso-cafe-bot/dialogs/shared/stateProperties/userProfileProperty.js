// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

class UserProfileProperty {
    /**
     * User Profile Property constructor.
     * 
     * @param {String} name user name
     * @param {String} location user location
     */
    constructor(name, location) {
        if(!name) throw ('Need name to create user profile');
        this.userName = name;
        this.location = location ? location : '';
    }
};

module.exports = UserProfileProperty;
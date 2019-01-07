// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

/**
 * Simple object used by the user state property accessor.
 * Used to store the user state.
 */
class GreetingState {
<<<<<<< HEAD
  constructor(name, city) {
    this.name = name ? name : undefined;
    this.city = city ? city : undefined;
  }
=======
    constructor(name, city) {
        this.name = name;
        this.city = city;
    }
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
}

module.exports.GreetingState = GreetingState;

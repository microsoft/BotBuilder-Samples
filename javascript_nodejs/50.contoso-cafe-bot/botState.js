// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const BotStateExports = {
    BotState: class {

    },
    UserProfile: class {

    },
    Reservations: class {

    },
    Reservation: class {

    },
    Query: class {
        constructor() {
            this.constraints = new Array(new this.Constraints());
        }
    },
    Constraint: class {
        constructor() {
            this.location = '';
            this.date = '';
            this.time = '';
            
        }
    },
    Constraints: class {
        constructor() {
            this.constraint = new this.Constraint();
        }
    }
}

module.exports = BotStateExports;
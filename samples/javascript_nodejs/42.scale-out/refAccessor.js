// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

/*
This is an accessor for any object. By definition objects (as opposed to values)
    are returned by reference in the GetAsync call on the accessor. As such the SetAsync
    call is never used. The actual act of saving any state to an external store therefore
    cannot be encapsulated in the Accessor implementation itself. And so to facilitate this
    the state itself is available as a public property on this class. The reason its here is
    because the caller of the constructor could pass in null for the state, in which case
    the factory provided on the GetAsync call will be used.
*/
class RefAccessor {
    constructor(value) {
        this.Value = value;
    }

    get Name() {
        return typeof (T);
    }

    async getAsync(TurnContext, defaultValueFactory = null) {
        if (this.Value == null) {
            if (defaultValueFactory == null) {
                throw new Error('KeyNotFoundException');
            }
            this.Value = defaultValueFactory();
        }

        return this.Value;
    }

    // Not Implemented
    async deleteAsync(TurnContext) {
        throw new Error('NotImplementedException');
    }

    async setAsync(TurnContext, value) {
        throw new Error('NotImplementedException');
    }
}

module.exports.RefAccessor = { RefAccessor };

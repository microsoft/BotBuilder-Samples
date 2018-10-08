// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
const HOME_AUTOMATION_STATE_PROPERTY = 'homeAutomation.state';

class DeviceState {
    /**
     *
     * @param {String} Device Name
     * @param {String} Room the device is in
     * @param {String} Device properties - e.g. temperature=72'F, brightnessLevel=20%
     * @param {String} State of the device - on/ off/ standby/ pause etc.
     */
    constructor(device, room, deviceProperty, deviceState) {
        this.deviceName = device || '';
        this.room = room || '';
        this.deviceProperty = deviceProperty || '';
        this.deviceState = deviceState || '';
    }
};

class HomeAutomationState {
    /**
     *
     * @param {ConversationState} convoState Conversation state
     * @param {UserState} userState User state
     */
    constructor(conversationState, userState) {
        if (!conversationState) throw new Error('Invalid conversation state provided.');
        if (!userState) throw new Error('Invalid user state provided.');

        // Device property accessor for home automation scenario.
        this.deviceProperty = conversationState.createProperty(HOME_AUTOMATION_STATE_PROPERTY);
    }

    /**
     *
     * @param {String} device
     * @param {String} room
     * @param {String} deviceState
     * @param {String} deviceProperty
     * @param {TurnContext} context object
     */
    async setDevice(device, room, deviceState, deviceProperty, context) {
        // Get devices from state.
        let operations = await this.deviceProperty.get(context);
        if (operations === undefined) {
            operations = new Array(new DeviceState(device, room, deviceProperty, deviceState));
        } else {
            // add this operation
            operations.push(new DeviceState(device, room, deviceProperty, deviceState));
        }
        return this.deviceProperty.set(context, operations);
    }

    /**
     *
     * @param {TurnContext} context object
     * @returns {String} text readout of state operations
     */
    async getDevices(context) {
        let returnText = 'No operations found';
        // read out of current devices from state
        const operations = await this.deviceProperty.get(context);
        if (operations === undefined) {
            return returnText;
        }
        returnText = '';
        operations.forEach((device, idx) => {
            returnText += '\n[' + idx + ']. ' +
                          (device.deviceName ? device.deviceName : 'Unknown device') +
                          (device.room ? ' in room = ' + device.room : '') +
                          (device.deviceState ? ' is ' + device.deviceState : '') +
                          (device.deviceProperty ? ' device property = ' + device.deviceProperty : '');
        });
        return returnText;
    }
}

module.exports.HomeAutomationState = HomeAutomationState;

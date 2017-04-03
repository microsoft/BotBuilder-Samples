var util = require('util');
var LuisActions = require('../core');

var TimeInPlaceAction = {
    intentName: 'TimeInPlace',
    friendlyName: 'Find out the time for a city',
    schema: {
        Place: {
            type: 'string',
            message: 'Please provide a location',
            builtInType: LuisActions.BuiltInTypes.Geography.City
        }
    },
    fulfill: function (parameters, callback) {
        callback(util.format('The time in %s is %s', parameters.Place, '00:00'));
    }
};

module.exports = TimeInPlaceAction;
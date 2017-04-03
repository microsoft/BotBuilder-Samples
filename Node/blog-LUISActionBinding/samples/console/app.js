require('dotenv-extended').load({
    path: '../.env'
});

var _ = require('lodash');
var util = require('util');
var readline = require('readline');

var LuisActions = require('../../core');

var SampleActions = require('../all');
var SampleActionsModelUrl = process.env.LUIS_MODEL_URL;

process.stdout.write("Query samples:\n");
process.stdout.write("- What is the time in Miami?\n");
process.stdout.write("- Search for 5 stars hotels in Barcelona\n");
process.stdout.write("- Tell me the weather in Buenos Aires\n");
process.stdout.write("- Find airport with code PMV\n");

var DefaultPrompt = 'Your query?\n>';

(function waitAndEvaluateInput(prompt, currentActionModel) {
    waitForInput(prompt, function (input) {

        // switching context response?
        if (currentActionModel && currentActionModel.status === LuisActions.Status.ContextSwitch) {
            var confirmed = input.toLowerCase() === 'yes' || input.toLowerCase() === 'y';
            currentActionModel.confirmSwitch = confirmed;
            input = ''; // no need for input
        }

        LuisActions.evaluate(SampleActionsModelUrl, SampleActions, currentActionModel, input.trim(), onContextCreation)
            .then(actionModel => {

                // completed sub context?
                if (actionModel.subcontextResult) {
                    process.stdout.write(actionModel.subcontextResult.toString());
                    process.stdout.write('\n');
                }

                switch (actionModel.status) {
                    case LuisActions.Status.NoActionRecognized:
                        // No action identified, restart
                        return waitAndEvaluateInput('Could not understand the input.\n' + DefaultPrompt);

                    case LuisActions.Status.Fulfilled:
                        // Action fulfilled
                        process.stdout.write(util.format('"%s" fulfilled:', actionModel.intentName));
                        process.stdout.write('\n\t' + actionModel.result.toString());
                        process.stdout.write('\n');

                        // restart
                        return waitAndEvaluateInput(DefaultPrompt);

                    case LuisActions.Status.MissingParameters:
                        // Prompt for first missing parameter
                        var errors = actionModel.parameterErrors;
                        var fieldError = _.first(errors);

                        // set current parameter name to help recognizer which parameter to match
                        actionModel.currentParameter = fieldError.parameterName;
                        return waitAndEvaluateInput(
                            util.format('(%s) %s:\n>', fieldError.parameterName, fieldError.message),
                            actionModel);

                    case LuisActions.Status.ContextSwitch:
                        return waitAndEvaluateInput(
                            (actionModel.contextSwitchPrompt || 'Are you sure you want to switch?') + ' (Yes or No)\n>',
                            actionModel);
                }
            }).catch(err => {
                process.stderr.write('Error with LuisAction: ' + err.toString());
                console.error('Error with LuisAction', err);
                waitAndEvaluateInput(DefaultPrompt);
            });
    });
})(DefaultPrompt);


function waitForInput(prompt, func) {
    // Read from input and trigger query
    var rl = readline.createInterface({
        input: process.stdin,
        output: process.stdout
    });

    rl.question(prompt, function (input) {
        rl.close();
        func(input);
    });
}

function onContextCreation(action, actionModel, next) {

    // Here you can implement a callback to hydrate the actionModel as per request

    // For example:
    // If your action is related with a 'Booking' intent, then you could do something like:
    // BookingSystem.Hydrate(action) - hydrate action context already stored within some repository
    // (ex. using a booking ref that you can get from the context somehow)

    // To simply showcase the idea, here we are setting the checkin/checkout dates for 1 night
    // when the user starts a contextual intent related with the 'FindHotelsAction'

    // So if you simply write 'Change location to Madrid' the main action will have required parameters already set up
    // and you can get the user information for any purpose

    // NOTE: Remember to call next() to continue executing the action binding's logic

    if(action.intentName === 'FindHotels') {
        if(!actionModel.parameters.Checkin) {
            actionModel.parameters.Checkin = new Date();
        }

        if(!actionModel.parameters.Checkout) {
            actionModel.parameters.Checkout = new Date();
            actionModel.parameters.Checkout.setDate(actionModel.parameters.Checkout.getDate() + 1);
        }
    }

    next();
}
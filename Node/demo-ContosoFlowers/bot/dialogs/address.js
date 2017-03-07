var builder = require('botbuilder');
var locationDialog = require('botbuilder-location');

var lib = new builder.Library('address');

// Register BotBuilder-Location dialog
lib.library(locationDialog.createLibrary(process.env.BING_MAPS_KEY));

// Main request address dialog, invokes BotBuilder-Location
lib.dialog('/', [
    function (session, args) {
        // Ask for address
        args = args || {};
        var promptMessage = args.promptMessage || 'default_address_prompt';
        session.dialogData.promptMessage = promptMessage;

        // Use botbuilder-location dialog for address request
        var options = {
            prompt: promptMessage,
            useNativeControl: true,
            reverseGeocode: true,
            skipConfirmationAsk: true,
            requiredFields:
                locationDialog.LocationRequiredFields.streetAddress |
                locationDialog.LocationRequiredFields.locality |
                locationDialog.LocationRequiredFields.country
        };

        locationDialog.getLocation(session, options);
    },
    function (session, results) {
        if (results.response) {
            // Return selected address to previous dialog in stack
            var place = results.response;
            var address = locationDialog.getFormattedAddressFromPlace(place, ', ');
            session.endDialogWithResult({
                address: address
            });
        } else {
            // No address resolved, restart
            session.replaceDialog('/', { promptMessage: session.dialogData.promptMessage });
        }
    }]);

// Request Billing Address
// Prompt/Save selected address. Uses previous dialog to request and validate address. 
var UseSavedInfoChoices = {
    Home: 'home_address',
    Work: 'work_address',
    NotThisTime: 'not_this_time'
};

lib.dialog('billing', [
    function (session, args, next) {
        var selection = session.message.text;
        var saved = session.userData.billingAddresses = session.userData.billingAddresses || {};
        if (hasAddresses(saved)) {
            // Saved data found, check for selection
            if (selection && saved[selection]) {
                // Retrieve selection
                var savedAddress = saved[selection];
                session.dialogData.billingAddress = savedAddress;
                next();
            } else if (selection.toLowerCase() === session.gettext(UseSavedInfoChoices.NotThisTime).toLowerCase()) {
                // Ask for data
                next();
            } else {
                // No selection, prompt which saved address to use
                session.send('select_billing_address');

                var message = new builder.Message(session)
                    .attachmentLayout(builder.AttachmentLayout.carousel);
                var homeAddress = saved[session.gettext(UseSavedInfoChoices.Home)];
                var workAddress = saved[session.gettext(UseSavedInfoChoices.Work)];
                if (homeAddress) message.addAttachment(createAddressCard(session, session.gettext(UseSavedInfoChoices.Home), homeAddress));
                if (workAddress) message.addAttachment(createAddressCard(session, session.gettext(UseSavedInfoChoices.Work), workAddress));
                message.addAttachment(createAddressCard(session, session.gettext(UseSavedInfoChoices.NotThisTime), 'add_new_address'));
                session.send(message);
            }
        } else {
            // No saved addresses data
            next();
        }
    },
    function (session, args, next) {
        if (session.dialogData.billingAddress) {
            // Address selected in previous step, skip
            return next();
        }

        // Ask for address
        session.beginDialog('/',
            {
                promptMessage: session.gettext('ask_billing_address')
            });
    },
    function (session, args, next) {
        if (session.dialogData.billingAddress) {
            return next();
        }

        // Retrieve address from previous dialog
        session.dialogData.billingAddress = args.address;

        // Ask to save address
        builder.Prompts.choice(session, 'ask_save_address', [
            session.gettext(UseSavedInfoChoices.Home),
            session.gettext(UseSavedInfoChoices.Work),
            session.gettext(UseSavedInfoChoices.NotThisTime)
        ]);
    },
    function (session, args, next) {
        var billingAddress = session.dialogData.billingAddress;

        if (args.response && args.response.entity !== session.gettext(UseSavedInfoChoices.NotThisTime)) {
            // Save address
            session.userData.billingAddresses = session.userData.billingAddresses || {};
            session.userData.billingAddresses[args.response.entity] = billingAddress;
        }

        // Return address 
        session.endDialogWithResult({ billingAddress: billingAddress });
    }
]);


// Helpers
function hasAddresses(addresses) {
    return addresses[UseSavedInfoChoices.Home] || addresses[UseSavedInfoChoices.Work];
}

function createAddressCard(session, buttonTitle, address) {
    return new builder.HeroCard(session)
        .title(buttonTitle)
        .subtitle(address)
        .buttons([
            builder.CardAction.imBack(session, buttonTitle, buttonTitle)
        ]);
}

module.exports.UseSavedInfoChoices = UseSavedInfoChoices;

// Export createLibrary() function
module.exports.createLibrary = function () {
    return lib.clone();
};
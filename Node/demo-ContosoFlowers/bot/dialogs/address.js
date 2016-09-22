var builder = require('botbuilder');
var locationService = require('../../services/location');

const library = new builder.Library('address');

// Address constants
const InvalidAddress = 'Sorry, I could not understand that address. Can you try again? (Number, street, city, state, and ZIP)';
const ConfirmChoice = 'Use this address';
const EditChoice = 'Edit';
library.dialog('/', [
    function (session, args) {
        // Ask for address
        args = args || {};
        var promptMessage = args.promptMessage || 'Address?';
        session.dialogData.promptMessage = promptMessage;
        if (args.reprompt) {
            // re-routed from invalid result
            promptMessage = InvalidAddress;
        }

        builder.Prompts.text(session, promptMessage);
    },
    function (session, args, next) {
        // Validate address
        var address = args.response;
        locationService.parseAddress(address)
            .then((addresses) => {
                if (addresses.length === 0) {
                    // Could not resolve address, retry dialog
                    session.replaceDialog('/', { reprompt: true, promptMessage: session.dialogData.promptMessage });
                } else if (addresses.length === 1) {
                    // Valid address, continue
                    next({ response: addresses[0] });
                } else {
                    session.beginDialog('/choose', { addresses });
                }
            }).catch((err) => {
                // Validation error, retry dialog
                console.error('Address.Validation.Error!', err);
                session.send('There was an error validating your address');
                session.replaceDialog('/', { reprompt: true, promptMessage: session.dialogData.promptMessage });
            });
    },
    function (session, args) {
        // Confirm address
        var address = args.response;
        session.dialogData.address = address;
        builder.Prompts.choice(session, address, [ConfirmChoice, EditChoice]);
    },
    function (session, args) {
        if (args.response.entity === ConfirmChoice) {
            // Confirmed, end dialog with address
            session.endDialogWithResult({
                address: session.dialogData.address
            });
        } else {
            // Edit, restart dialog
            session.replaceDialog('/', { promptMessage: session.dialogData.promptMessage });
        }
    }
]);

// Select address from list
library.dialog('/choose',
    function (session, args) {
        args = args || {};
        var addresses = args.addresses;
        if (addresses) {
            // display options
            session.dialogData.addresses = addresses;
            var message = new builder.Message(session)
                .attachmentLayout(builder.AttachmentLayout.carousel)
                .attachments(addresses.map((addr) =>
                    new builder.HeroCard(session)
                        .title('Did you mean?')
                        .subtitle(addr)
                        .buttons([builder.CardAction.imBack(session, addr, 'Use this address')])));
            session.send(message);
        } else {
            // process selected option
            var address = session.message.text;
            addresses = session.dialogData.addresses;
            if (addresses.indexOf(address) === -1) {
                // not a valid selection
                session.replaceDialog('/choose', { addresses });
            } else {
                // return
                session.endDialogWithResult({ response: address });
            }
        }
    });

// Request Billing Address
// Prompt/Save selected address. Uses previous dialog to request and validate address. 
const UseSavedInfoChoices = {
    Home: 'Home address',
    Work: 'Work address',
    NotThisTime: 'No, thanks!'
};
library.dialog('/billing', [
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
            } else if (selection === UseSavedInfoChoices.NotThisTime) {
                // Ask for data
                next();
            } else {
                // No selection, prompt which saved address to use
                session.send('Please select your billing address');

                var message = new builder.Message(session)
                    .attachmentLayout(builder.AttachmentLayout.carousel);
                var homeAddress = saved[UseSavedInfoChoices.Home];
                var workAddress = saved[UseSavedInfoChoices.Work];
                if (homeAddress) message.addAttachment(createAddressCard(session, UseSavedInfoChoices.Home, homeAddress));
                if (workAddress) message.addAttachment(createAddressCard(session, UseSavedInfoChoices.Work, workAddress));
                message.addAttachment(createAddressCard(session, UseSavedInfoChoices.NotThisTime, 'Add a new address'));
                session.send(message);
            }
        } else {
            // No data
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
                promptMessage: 'What\'s your billing address? Include apartment # if needed.'
            });
    },
    function (session, args, next) {
        if (session.dialogData.billingAddress) {
            return next();
        }

        // Retrieve address from previous dialog
        session.dialogData.billingAddress = args.address;

        // Ask to save address
        var options = [UseSavedInfoChoices.Home, UseSavedInfoChoices.Work, UseSavedInfoChoices.NotThisTime];
        builder.Prompts.choice(session, 'Would you like to save this address?', options);
    },
    function (session, args, next) {
        var billingAddress = session.dialogData.billingAddress;

        if (args.response && args.response.entity !== UseSavedInfoChoices.NotThisTime) {
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
    return !!addresses[UseSavedInfoChoices.Home] || !!addresses[UseSavedInfoChoices.Work];
}

function createAddressCard(session, buttonTitle, address) {
    return new builder.HeroCard(session)
        .title(buttonTitle)
        .subtitle(address)
        .buttons([
            builder.CardAction.imBack(session, buttonTitle, buttonTitle)
        ]);
}

module.exports = library;
module.exports.UseSavedInfoChoices = UseSavedInfoChoices;
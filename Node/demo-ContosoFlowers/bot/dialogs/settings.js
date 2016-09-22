var util = require('util');
var builder = require('botbuilder');
var validators = require('../validators');
var addressLibrary = require('./address');

const SettingChoice = {
    Email: 'Edit your email',
    Phone: 'Edit phone number',
    Addresses: 'Edit your addresses',
    Cancel: 'Go back'
};

var library = new builder.Library('settings');
library.dialog('/', [
    // Display options
    function (session) {
        builder.Prompts.choice(
            session,
            'Want to make changes to your personal info or addresses? You\'re in the right place.',
            [SettingChoice.Email, SettingChoice.Phone, SettingChoice.Addresses, SettingChoice.Cancel]);
    },
    // Trigger option edit
    function (session, args, next) {
        args = args || {};
        var response = args.response || {};
        var option = response.entity;
        switch (option) {
            case SettingChoice.Email:
                var promptMessage = 'Type your email or use (B)ack to return to the menu.';
                if (session.userData.sender && session.userData.sender.email) {
                    promptMessage = 'This is your current email: ' + session.userData.sender.email + '.\n\nType a new email if you need to update, or use (B)ack to return to the menu.';
                }
                session.send(promptMessage);
                return session.beginDialog('/email');

            case SettingChoice.Phone:
                var promptMessage = 'Type your phone number or use (B)ack to return to the menu.';
                if (session.userData.sender && session.userData.sender.phoneNumber) {
                    promptMessage = 'This is your current phone number: ' + session.userData.sender.phoneNumber + '.\n\nType a new number if you need to update, or use (B)ack to return to the menu.';
                }
                session.send(promptMessage);
                return session.beginDialog('/phone');

            case SettingChoice.Addresses:
                return session.beginDialog('/addresses');

            case SettingChoice.Cancel:
                return session.endDialog();
        }
    },
    // Setting updated/cancelled
    function (session, args) {
        args = args || {};
        var text = !!args.updated ? 'Thanks! Your setting was updated!' : 'No setting was updated.';
        session.send(text); 
        session.replaceDialog('/');
    }
]).reloadAction('restart', null, { matches: /^back|b/i });                               // restart menu options when 'B' or 'Back' is received
  


// Email edit
library.dialog('/email', editOptionDialog(
    (input) => validators.EmailRegex.test(input),
    'Something is wrong with that email address. Please try again.',
    (session, email) => saveSenderSetting(session, 'email', email)));

// Phone Number edit
library.dialog('/phone', editOptionDialog(
    (input) => validators.PhoneRegex.test(input),
    'Oops, that doesn\'t look like a valid number. Try again.',
    (session, phone) => saveSenderSetting(session, 'phoneNumber', phone)));

// Addresses
const UseSavedInfoChoices = addressLibrary.UseSavedInfoChoices;
library.dialog('/addresses', [
    function(session, args, next) {
        
        // Check if an option was selected
        var selection = session.message.text;
        if(selection === UseSavedInfoChoices.Home || selection === UseSavedInfoChoices.Work) {
            session.dialogData.selection = selection;
            return next();
        }

        // Show saved addresses
        session.send('Which address do you wish to update?');
        var saved = session.userData.billingAddresses = session.userData.billingAddresses || {};
        var message = new builder.Message(session)
            .attachmentLayout(builder.AttachmentLayout.carousel);
        var homeAddress = saved[UseSavedInfoChoices.Home];
        var workAddress = saved[UseSavedInfoChoices.Work];
        message.addAttachment(createAddressCard(session, UseSavedInfoChoices.Home, homeAddress || 'Not set'));
        message.addAttachment(createAddressCard(session, UseSavedInfoChoices.Work, workAddress || 'Not set'));
        message.addAttachment(new builder.HeroCard(session)
            .title('Not this time')
            .subtitle('Do not change any addresses')
            .buttons([
                builder.CardAction.imBack(session, 'Back', '(B)ack')
            ]));
        session.send(message);
    },
    function (session, args, next) {
        // Trigger address request dialog
        session.beginDialog('address:/', { promptMessage: util.format('Please specify your new %s.', session.dialogData.selection) });
    },
    function (session, args, next) {
        // Save new address
        var selection = session.dialogData.selection;
        var newAddress = args.address;
        session.userData.billingAddresses = session.userData.billingAddresses || {};
        session.userData.billingAddresses[selection] = newAddress;
        session.endDialogWithResult({ updated: true });
    }
]);

function saveSenderSetting(session, name, value) {
    session.userData.sender = session.userData.sender || {};
    session.userData.sender[name] = value;
}

function editOptionDialog(validationFunc, invalidMessage, saveFunc) {
    return new builder.SimpleDialog(function (session, args, next) {
        // check dialog was just forwarded
        if (!session.dialogData.loop) {
            session.dialogData.loop = true;
            session.sendBatch();
            return;
        }

        if(!validationFunc(session.message.text)) {
            // invalid
            session.send(invalidMessage);
        } else {
            // save
            saveFunc(session, session.message.text);
            session.endDialogWithResult({ updated: true });
        }
    });
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

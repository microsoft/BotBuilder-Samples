var util = require('util');
var builder = require('botbuilder');

const library = new builder.Library('details');

// Recipient & Sender details
library.dialog('/', [
    function (session) {
        builder.Prompts.text(session, 'What\'s the recipient\'s first name?');
    },
    function (session, args) {
        session.dialogData.recipientFirstName = args.response;
        builder.Prompts.text(session, 'What\'s the recipient\'s last name?');
    },
    function (session, args) {
        session.dialogData.recipientLastName = args.response;
        session.beginDialog('validators:/phonenumber', {
            prompt: 'What\'s the recipient\'s phone number?',
            retryPrompt: 'Oops, that doesn\'t look like a valid number. Try again.',
            maxRetries: Number.MAX_VALUE
        });
    },
    function (session, args) {
        session.dialogData.recipientPhoneNumber = args.response;
        session.beginDialog('validators:/notes', {
            prompt: 'What do you want the note to say? (in 200 characters)',
            retryPrompt: 'Oops, the note is max 200 characters. Try again.',
            maxRetries: Number.MAX_VALUE
        });
    },
    function (session, args) {
        session.dialogData.note = args.response;
        session.beginDialog('/sender');
    },
    function (session, args) {
        session.dialogData.sender = args.sender;
        var details = {
            recipient: {
                firstName: session.dialogData.recipientFirstName,
                lastName: session.dialogData.recipientLastName,
                phoneNumber: session.dialogData.recipientPhoneNumber
            },
            note: session.dialogData.note,
            sender: session.dialogData.sender
        };
        session.endDialogWithResult({ details: details });
    }
]);

// Sender details
const UseSavedInfoChoices = {
    Yes: 'Yes',
    No: 'Edit'
};

library.dialog('/sender', [
    function (session, args, next) {
        var sender = session.userData.sender;
        if (!!sender) {
            // sender data previously saved
            var promptMessage = util.format('Would you like to use this email %s and this phone number \'%s\' info?', sender.email, sender.phoneNumber);
            builder.Prompts.choice(session, promptMessage, [UseSavedInfoChoices.Yes, UseSavedInfoChoices.No]);
        } else {
            // no data
            next();
        }
    },
    function (session, args, next) {
        if (args.response && args.response.entity === UseSavedInfoChoices.Yes && session.userData.sender) {
            // Use previously saved data, store it in dialogData
            // Next steps will skip if present
            session.dialogData.useSaved = true;
            session.dialogData.email = session.userData.sender.email;
            session.dialogData.phoneNumber = session.userData.sender.phoneNumber;
        }
        next();
    },
    function (session, args, next) {
        if (session.dialogData.useSaved) {
            return next();
        }
        session.beginDialog('validators:/email', {
            prompt: 'What\'s your email?',
            retryPrompt: 'Something is wrong with that email address. Please try again.',
            maxRetries: Number.MAX_VALUE
        });
    },
    function (session, args, next) {
        if (session.dialogData.useSaved) {
            return next();
        }
        session.dialogData.email = args.response;
        session.beginDialog('validators:/phonenumber', {
            prompt: 'What\'s your phone number?',
            retryPrompt: 'Oops, that doesn\'t look like a valid number. Try again.',
            maxRetries: Number.MAX_VALUE
        });
    },
    function (session, args, next) {
        if (session.dialogData.useSaved) {
            return next();
        }
        session.dialogData.phoneNumber = args.response;
        builder.Prompts.confirm(session, 'Would you like to save your info?');
    },
    function (session, args) {
        var sender = {
            email: session.dialogData.email,
            phoneNumber: session.dialogData.phoneNumber
        };

        // Save data?
        var shouldSave = args.response;
        if (shouldSave) {
            session.userData.sender = sender;
        }

        // return sender information
        session.endDialogWithResult({ sender: sender });
    }
]);

module.exports = library;
var util = require('util');
var builder = require('botbuilder');

var lib = new builder.Library('details');

// Recipient & Sender details
lib.dialog('/', [
    function (session) {
        builder.Prompts.text(session, 'ask_recipient_first_name');
    },
    function (session, args) {
        session.dialogData.recipientFirstName = args.response;
        builder.Prompts.text(session, 'ask_recipient_last_name');
    },
    function (session, args) {
        session.dialogData.recipientLastName = args.response;
        session.beginDialog('validators:phonenumber', {
            prompt: session.gettext('ask_recipient_phone_number'),
            retryPrompt: session.gettext('invalid_phone_number')
        });
    },
    function (session, args) {
        session.dialogData.recipientPhoneNumber = args.response;
        session.beginDialog('validators:notes', {
            prompt: session.gettext('ask_note'),
            retryPrompt: session.gettext('invalid_note')
        });
    },
    function (session, args) {
        session.dialogData.note = args.response;
        session.beginDialog('sender');
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
var UseSavedInfoChoices = {
    Yes: 'yes',
    No: 'edit'
};

lib.dialog('sender', [
    function (session, args, next) {
        var sender = session.userData.sender;
        if (sender) {
            // sender data previously saved
            var promptMessage = session.gettext('use_this_email_and_phone_number', sender.email, sender.phoneNumber);
            builder.Prompts.choice(session, promptMessage, [
                session.gettext(UseSavedInfoChoices.Yes),
                session.gettext(UseSavedInfoChoices.No)
            ]);
        } else {
            // no data
            next();
        }
    },
    function (session, args, next) {
        if (args.response && args.response.entity === session.gettext(UseSavedInfoChoices.Yes) && session.userData.sender) {
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
        session.beginDialog('validators:email', {
            prompt: session.gettext('ask_email'),
            retryPrompt: session.gettext('invalid_email')
        });
    },
    function (session, args, next) {
        if (session.dialogData.useSaved) {
            return next();
        }
        session.dialogData.email = args.response;
        session.beginDialog('validators:phonenumber', {
            prompt: session.gettext('ask_phone_number'),
            retryPrompt: session.gettext('invalid_phone_number')
        });
    },
    function (session, args, next) {
        if (session.dialogData.useSaved) {
            return next();
        }
        session.dialogData.phoneNumber = args.response;
        builder.Prompts.confirm(session, 'ask_save_info');
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

// Export createLibrary() function
module.exports.createLibrary = function () {
    return lib.clone();
};
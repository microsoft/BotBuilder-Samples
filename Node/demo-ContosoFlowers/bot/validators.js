var builder = require('botbuilder');

var PhoneRegex = new RegExp(/^(\+\d{1,2}\s)?\(?\d{3}\)?[\s.-]?\d{3}[\s.-]?\d{4}$/);
var EmailRegex = new RegExp(/[a-z0-9!#$%&'*+\/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+\/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?/);

var lib = new builder.Library('validators');

lib.dialog('notes', basicPrompterWithExpression(function (input) {
    return input && input.length <= 200;
}));

lib.dialog('phonenumber', basicPrompterWithRegex(PhoneRegex));

lib.dialog('email', basicPrompterWithRegex(EmailRegex));

function basicPrompterWithRegex(regex) {
    return new builder.IntentDialog()
        .onBegin(function (session, args) {
            session.dialogData.retryPrompt = args.retryPrompt;
            session.send(args.prompt);
        }).matches(regex, function (session) {
            session.endDialogWithResult({ response: session.message.text });
        }).onDefault(function (session) {
            session.send(session.dialogData.retryPrompt);
        });
}

function basicPrompterWithExpression(expression) {
    return new builder.IntentDialog()
        .onBegin(function (session, args) {
            session.dialogData.retryPrompt = args.retryPrompt;
            session.send(args.prompt);
        }).onDefault(function (session) {
            var input = session.message.text;
            if (expression(input)) {
                session.endDialogWithResult({ response: input });
            } else {
                session.send(session.dialogData.retryPrompt);
            }
        });
}

// Export createLibrary() function
module.exports.createLibrary = function () {
    return lib.clone();
};

module.exports.PhoneRegex = PhoneRegex;
module.exports.EmailRegex = EmailRegex;
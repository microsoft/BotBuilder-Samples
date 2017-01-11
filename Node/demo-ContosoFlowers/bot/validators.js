var builder = require('botbuilder');

const PhoneRegex = new RegExp(/^(\+\d{1,2}\s)?\(?\d{3}\)?[\s.-]?\d{3}[\s.-]?\d{4}$/);
const EmailRegex = new RegExp(/[a-z0-9!#$%&'*+\/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+\/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?/);

const lib = new builder.Library('validators');

lib.dialog('notes',
    builder.DialogAction.validatedPrompt(builder.PromptType.text, (response) =>
        response && response.length <= 200));

lib.dialog('phonenumber',
    builder.DialogAction.validatedPrompt(builder.PromptType.text, (response) =>
        PhoneRegex.test(response)));

lib.dialog('email',
    builder.DialogAction.validatedPrompt(builder.PromptType.text, (response) =>
        EmailRegex.test(response)));

// Export createLibrary() function
module.exports.createLibrary = function () {
    return lib.clone();
};

module.exports.PhoneRegex = PhoneRegex;
module.exports.EmailRegex = EmailRegex;
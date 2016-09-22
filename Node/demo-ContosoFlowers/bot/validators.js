var builder = require('botbuilder');

const PhoneRegex = new RegExp(/^(\+\d{1,2}\s)?\(?\d{3}\)?[\s.-]?\d{3}[\s.-]?\d{4}$/);
const EmailRegex = new RegExp(/[a-z0-9!#$%&'*+\/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+\/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?/);

const library = new builder.Library('validators');

library.dialog('/notes',
    builder.DialogAction.validatedPrompt(builder.PromptType.text, (response) =>
        response && response.length <= 200));

library.dialog('/phonenumber',
    builder.DialogAction.validatedPrompt(builder.PromptType.text, (response) =>
        PhoneRegex.test(response)));

library.dialog('/email',
    builder.DialogAction.validatedPrompt(builder.PromptType.text, (response) =>
        EmailRegex.test(response)));

module.exports = library;
module.exports.PhoneRegex = PhoneRegex;
module.exports.EmailRegex = EmailRegex;
var builder = require('botbuilder');

const library = new builder.Library('help');
library.dialog('/', builder.DialogAction.endDialog('Support will contact you shortly. Have a nice day :)'));

module.exports = library;
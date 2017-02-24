var util = require('util');
var builder = require('botbuilder');

var lib = new builder.Library('shop');
lib.dialog('/', [
    function (session) {
        // Ask for delivery address using 'address' library
        session.beginDialog('address:/',
            {
                promptMessage: session.gettext('provide_delivery_address', session.message.user.name || session.gettext('default_user_name'))
            });
    },
    function (session, args) {
        // Retrieve address, continue to shop
        session.dialogData.recipientAddress = args.address;
        session.beginDialog('product-selection:/');
    },
    function (session, args) {
        // Retrieve selection, continue to delivery date
        session.dialogData.selection = args.selection;
        session.beginDialog('delivery:date');
    },
    function (session, args) {
        // Retrieve deliveryDate, continue to details
        session.dialogData.deliveryDate = args.deliveryDate;
        session.send('confirm_choice', session.dialogData.selection.name, session.dialogData.deliveryDate.toLocaleDateString());
        session.beginDialog('details:/');
    },
    function (session, args) {
        // Retrieve details, continue to billing address
        session.dialogData.details = args.details;
        session.beginDialog('address:billing');
    },
    function (session, args, next) {
        // Retrieve billing address
        session.dialogData.billingAddress = args.billingAddress;
        next();
    },
    function (session, args) {
        // Continue to checkout
        var order = {
            selection: session.dialogData.selection,
            delivery: {
                date: session.dialogData.deliveryDate,
                address: session.dialogData.recipientAddress
            },
            details: session.dialogData.details,
            billingAddress: session.dialogData.billingAddress
        };

        console.log('order', order);
        session.beginDialog('checkout:/', { order: order });
    }
]);

// Export createLibrary() function
module.exports.createLibrary = function () {
    return lib.clone();
};
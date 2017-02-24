var util = require('util');
var builder = require('botbuilder');
var botUtils = require('../utils');
var siteUrl = require('../site-url');
var orderService = require('../../services/orders');

// Checkout flow
var RestartMessage = 'restart';
var StartOver = 'start_over';
var KeepGoing = 'continue';
var Help = 'help';

var lib = new builder.Library('checkout');
lib.dialog('/', [
    function (session, args, next) {
        args = args || {};
        var order = args.order;

        if (!order) {
            // 'Changed my mind' was pressed, continue to next step and prompt for options
            return next();
        }

        // Serialize user address
        var addressSerialized = botUtils.serializeAddress(session.message.address);

        // Create order (with no payment - pending)
        orderService.placePendingOrder(order).then(function (order) {

            // Build Checkout url using previously stored Site url
            var checkoutUrl = util.format(
                '%s/checkout?orderId=%s&address=%s',
                siteUrl.retrieve(),
                encodeURIComponent(order.id),
                encodeURIComponent(addressSerialized));

            var messageText = session.gettext('final_price', order.selection.price);
            var card = new builder.HeroCard(session)
                .text(messageText)
                .buttons([
                    builder.CardAction.openUrl(session, checkoutUrl, 'add_credit_card'),
                    builder.CardAction.imBack(session, session.gettext(RestartMessage), RestartMessage)
                ]);

            session.send(new builder.Message(session)
                .addAttachment(card));
        });
    },
    function (session, args) {
        builder.Prompts.choice(session, 'select_how_to_continue', [
            session.gettext(StartOver),
            session.gettext(KeepGoing),
            session.gettext(Help)
        ]);
    },
    function (session, args) {
        switch (args.response.entity) {
            case KeepGoing:
                return session.reset();
            case StartOver:
                return session.reset('/');
            case Help:
                return session.beginDialog('help:/');
        }
    }
]);

// Checkout completed (initiated from web application. See /checkout.js in the root folder)
lib.dialog('completed', function (session, args, next) {
    args = args || {};
    var orderId = args.orderId;

    // Retrieve order and create ReceiptCard
    orderService.retrieveOrder(orderId).then(function (order) {
        if (!order) {
            throw new Error(session.gettext('order_not_found'));
        }

        var messageText = session.gettext(
            'order_details',
            order.id,
            order.selection.name,
            order.details.recipient.firstName,
            order.details.recipient.lastName,
            order.details.note);

        var receiptCard = new builder.ReceiptCard(session)
            .title(order.paymentDetails.creditcardHolder)
            .facts([
                builder.Fact.create(session, order.id, 'order_number'),
                builder.Fact.create(session, offuscateNumber(order.paymentDetails.creditcardNumber), 'payment_method')
            ])
            .items([
                builder.ReceiptItem.create(session, order.selection.price, order.selection.name)
                    .image(builder.CardImage.create(session, order.selection.imageUrl))
            ])
            .total(order.selection.price)
            .buttons([
                builder.CardAction.openUrl(session, 'https://dev.botframework.com/', 'more_information')
            ]);

        var message = new builder.Message(session)
            .text(messageText)
            .addAttachment(receiptCard);

        session.endDialog(message);
    }).catch(function (err) {
        session.endDialog(session.gettext('error_ocurred', err.message));
    });
});

// Helpers
function offuscateNumber(cardNumber) {
    return cardNumber.substring(0, 4) + ' ****';
}

// Export createLibrary() function
module.exports.createLibrary = function () {
    return lib.clone();
};
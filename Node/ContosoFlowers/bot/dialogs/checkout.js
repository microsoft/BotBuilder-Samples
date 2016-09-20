var util = require('util');
var builder = require('botbuilder');
var botUtils = require('../utils');
var siteUrl = require('../site-url');
var orderService = require('../../services/orders');

const library = new builder.Library('checkout');

// Checkout flow
const RestartMessage = 'Changed my mind'
const StartOver = 'Start over';
const KeepGoing = 'Keep going';
const Help = 'Talk to support';
library.dialog('/', [
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
        orderService.placePendingOrder(order).then((order) => {

            // Build Checkout url using previously stored Site url
            var checkoutUrl = util.format(
                '%s/checkout?orderId=%s&address=%s',
                siteUrl.retrieve(),
                encodeURIComponent(order.id),
                encodeURIComponent(addressSerialized));

            var messageText = util.format('The final price is $%d (including delivery). Pay securely using our payment provider.', order.selection.price);
            var card = new builder.HeroCard(session)
                .text(messageText)
                .buttons([
                    builder.CardAction.openUrl(session, checkoutUrl, 'Add credit card'),
                    builder.CardAction.imBack(session, RestartMessage, RestartMessage)
                ]);

            session.send(new builder.Message(session)
                .addAttachment(card));
        });
    },
    function (session, args) {
        builder.Prompts.choice(session, 'What are you looking to do?', [StartOver, KeepGoing, Help]);
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
library.dialog('/completed', function (session, args, next) {
    args = args || {};
    var orderId = args.orderId;

    // Retrieve order and create ReceiptCard
    orderService.retrieveOrder(orderId).then((order) => {
        if (!order) {
            throw new Error('Order Id not found');
        }

        var messageText = util.format(
            '**Your order %s has been processed!**\n\n'
            + 'The **%s** will be sent to **%s %s** with the following note:\n\n'
            + '**"%s"**\n\n'
            + 'Thank you for using Contoso Flowers.\n\n'
            + 'Here is your receipt:',
            order.id,
            order.selection.name,
            order.details.recipient.firstName,
            order.details.recipient.lastName,
            order.details.note);

        var receiptCard = new builder.ReceiptCard(session)
            .title(order.paymentDetails.creditcardHolder)
            .facts([
                builder.Fact.create(session, order.id, 'Order Number'),
                builder.Fact.create(session, offuscateNumber(order.paymentDetails.creditcardNumber), 'Payment Method'),
            ])
            .items([
                builder.ReceiptItem.create(session, order.selection.price, order.selection.name)
                    .image(builder.CardImage.create(session, order.selection.imageUrl)),
            ])
            .total(order.selection.price)
            .buttons([
                builder.CardAction.openUrl(session, 'https://dev.botframework.com/', 'More Information')
            ]);

        var message = new builder.Message(session)
            .text(messageText)
            .addAttachment(receiptCard)

        session.endDialog(message);
    }).catch((err) => {
        session.endDialog(util.format('An error has ocurred: %s', err.message))
    });
});

// Helpers
function offuscateNumber(cardNumber) {
    return cardNumber.substring(0, 4) + ' ****';
}

module.exports = library;

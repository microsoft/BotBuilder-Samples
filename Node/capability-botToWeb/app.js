'use strict';

// Node Requires
const url = require('url');

// Third party modules
const restify = require('restify');
const builder = require('botbuilder');
const paypal = require('paypal-rest-sdk');

// Configure the paypal module with a client id and client secret that you
// generate from https://developer.paypal.com/
paypal.configure({
    'mode': process.env.PAYPAL_CLIENT_MODE,
    'client_id': process.env.PAYPAL_CLIENT_ID,
    'client_secret': process.env.PAYPAL_CLIENT_SECRET
});

// A connector connects a bot on bot framework to various messaging services that a bot
// can talk to.
let connector = new builder.ChatConnector({
    appId: process.env.MICROSOFT_APP_ID,
    appPassword: process.env.MICROSOFT_APP_PASSWORD
});

// A bot listens and reacts to messages that the connector picks up on.
let bot = new builder.UniversalBot(connector, function (session, args) {
    session.beginDialog('listFines');
});

// Simple two step dialog to list 'fines' that a user has received, and allow
// a user to 'pay' them. 
bot.dialog('listFines', [
    function (session, args) {
        console.log('List Fines Dialog');
        session.send('You have 1 outstanding fine:');

        session.send('Parking Fine Violation');
        builder.Prompts.choice(session, "What would you like to do?", ["Pay fine", "Cancel"]);
    },
    function (session, results, next) {
        let choice = results.response;

        if (choice.entity === 'Cancel') {
            return;
        }

        // Starts the payment flow.
        createAndSendPayment(session);
    },
]);

// We're using restify here to set up an HTTP server, and create some callbacks that Paypal will hit.
let server = restify.createServer();
server.use(restify.queryParser());

server.listen(process.env.PORT || process.env.port || 3978, function () {
    console.log('%s listening to %s', server.name, server.url);
});

// This is a callback that Paypal hits when a user approves a transaction for completion.
server.get('approvalComplete', function (req, res, next) {
    console.log('User approved transaction');
    executePayment(req.params);
    res.end('<html><body>Executing your transaction - you may close this browser tab.</body></html>');
});

// This is a callback that Paypal hits when a user cancels a transaction for completion.
server.get('cancelPayment', function (req, res, next) {
    console.log('User cancelled transaction');
    cancelledPayment(req.params);
    res.end('<html><body>Canceling your transaction - you may close this browser tab.</body></html>');
});

// Messages are posted to this endpoint. We ask the connector to listen at this endpoint
// for new messages.
server.post('/api/messages', connector.listen());

/**
 * This function creates and returns an object that is passed through to the PayPal Node SDK 
 * to create a payment that a user must manually approve.
 * 
 * See https://developer.paypal.com/docs/api/payments/#payment_create_request for a description of * the fields.
 */
function createPaymentJson(returnUrl, cancelUrl) {
    return {
        "intent": "sale",
        "payer": {
            "payment_method": "paypal"
        },
        "redirect_urls": {
            "return_url": returnUrl,
            "cancel_url": cancelUrl
        },
        "transactions": [{
            "item_list": {
                "items": [{
                    "name": "Fine",
                    "sku": "ParkingFine",
                    "price": "1.00",
                    "currency": "USD",
                    "quantity": 1
                }]
            },
            "amount": {
                "currency": "USD",
                "total": "1.00"
            },
            "description": "This is your fine. Please pay it :3"
        }]
    };
}

/**
 * This function creates and returns an object that is passed through to the PayPal Node SDK
 * to execute an authorized payment.
 * 
 * See https://developer.paypal.com/docs/api/payments/#payment_execute_request for a description of * the fields.
 */
function executePaymentJson(payerId) {
    return {
        "payer_id": payerId,
        "transactions": [{
            "amount": {
                "currency": "USD",
                "total": "1.00"
            }
        }]
    };
}

/**
 * Generates a URL that Paypal will redirect to on approval or cancellation 
 * of the payment by the user.
 */
function createUrl(path, address) {
    console.log('Creating URL for path: ' + path);

    // The address passed in is an Object that defines the context
    // of the conversation - the user, the channel, the http endpoint the bot
    // exists on, and so on. We encode this information into the return URL
    // to be parsed out by our approval completion endpoint.
    let addressEncoded = encodeURIComponent(JSON.stringify(address));

    // This object encodes the endpoint that PayPal redirects to when
    // a user approves the transaction.
    let urlObject = {
        protocol: 'http',
        hostname: process.env.HOST || 'localhost',
        port: process.env.PORT || process.env.port || 3978,
        pathname: path,
        query: { addressEncoded }
    }

    return url.format(urlObject);
}

/**
 * Creates a payment on paypal that a user must approve.
 */
function createAndSendPayment(session) {
    console.log('Creating Payment');

    let returnUrl = createUrl('approvalComplete', session.message.address);
    let cancelUrl = createUrl('cancelPayment', session.message.address);
    let paymentJson = createPaymentJson(returnUrl, cancelUrl);

    paypal.payment.create(paymentJson, function (error, payment) {
        if (error) {
            console.log(error);
            throw error;
        } else {
            // The SDK returns a payment object when the payment is successfully created. 
            // This object has a few properties, described at length here: 
            // https://developer.paypal.com/docs/api/payments/#payment_create_response
            // We're looking for the 'approval_url' property, which the user must go to
            // to approve the transaction before we can actively execute the transaction.
            for (var index = 0; index < payment.links.length; index++) {
                if (payment.links[index].rel === 'approval_url') {
                    session.send("Please pay your fine: " + payment.links[index].href);
                }
            }
        }
    });
};

/**
 * When a payment is approved by the user, we can go ahead and execute it.
 */
function executePayment(parameters) {
    console.log('Executing an Approved Payment');

    // Appended to the URL by PayPal during the approval step.
    let paymentId = parameters.paymentId;
    let payerId = parameters.PayerID;

    // Generate the sample payment execution JSON that paypal requires:
    let paymentJson = executePaymentJson(payerId);

    // Grab the encoded address object, URL decode it, and parse it back into a JSON object.
    let addressEncoded = decodeURIComponent(parameters.addressEncoded);
    let address = JSON.parse(addressEncoded);

    // Finally, execute the payment, and tell the user that we got their payment.
    paypal.payment.execute(paymentId, paymentJson, function (error, payment) {
        if (error) {
            console.log(error.response);
            throw error;
        } else {
            console.log('Payment Executed Successfully');
            respondToUserSuccess(payment, address);
        }
    });
}

/**
 * This function completes the payment dialog by creating a message, binding an address to it, 
 * and sending it.
 */
function respondToUserSuccess(payment, address) {
    let message = new builder.Message().address(address).text('Thanks for your payment!');

    bot.send(message.toMessage());
}

/**
 * If a user chooses to cancel the payment (on the PayPal approval dialog), we should
 * back via the bot.
 */
function cancelledPayment(parameters) {
    console.log('Cancelled a payment');

    let addressEncoded = decodeURIComponent(parameters.addressEncoded);
    let address = JSON.parse(addressEncoded);
    let message = new builder.Message().address(address).text('Cancelled your payment.');

    bot.send(message.toMessage());
}
// This loads the environment variables from the .env file
require('dotenv-extended').load();

var util = require('util');
var builder = require('botbuilder');
var restify = require('restify');
var payments = require('./payments');
var checkout = require('./checkout');
var catalog = require('./services/catalog');

var connector = new builder.ChatConnector({
  appId: process.env.MICROSOFT_APP_ID,
  appPassword: process.env.MICROSOFT_APP_PASSWORD
});

var server = restify.createServer();
server.listen(process.env.port || process.env.PORT || 3978, function () {
  console.log('%s listening to %s', server.name, server.url);
});
server.post('/api/messages', connector.listen());

var CartIdKey = 'CardId';

var bot = new builder.UniversalBot(connector, (session) => {

  catalog.getPromotedItem().then(product => {

    // Store userId for later, when reading relatedTo to resume dialog with the receipt
    var cartId = product.id;
    session.conversationData[CartIdKey] = cartId;
    session.conversationData[cartId] = session.message.address.user.id;

    // Create PaymentRequest obj based on product information
    var paymentRequest = createPaymentRequest(cartId, product);

    var buyCard = new builder.HeroCard(session)
      .title(product.name)
      .subtitle(util.format('%s %s', product.currency, product.price))
      .text(product.description)
      .images([
        new builder.CardImage(session).url(product.imageUrl)
      ])
      .buttons([
        new builder.CardAction(session)
          .title('Buy')
          .type(payments.PaymentActionType)
          .value(paymentRequest)
      ]);

    session.send(new builder.Message(session)
      .addAttachment(buyCard));
  });
});

bot.set('persistConversationData', true);

connector.onInvoke((invoke, callback) => {
  console.log('onInvoke', invoke);

  // This is a temporary workaround for the issue that the channelId for "webchat" is mapped to "directline" in the incoming RelatesTo object
  invoke.relatesTo.channelId = invoke.relatesTo.channelId === 'directline' ? 'webchat' : invoke.relatesTo.channelId;

  var storageCtx = {
    address: invoke.relatesTo,
    persistConversationData: true,
    conversationId: invoke.relatesTo.conversation.id
  };

  connector.getData(storageCtx, (err, data) => {
    var cartId = data.conversationData[CartIdKey];
    if (!invoke.relatesTo.user && cartId) {
      // Bot keeps the userId in context.ConversationData[cartId]
      var userId = data.conversationData[cartId];
      invoke.relatesTo.useAuth = true;
      invoke.relatesTo.user = { id: userId };
    }

    // Continue based on PaymentRequest event
    var paymentRequest = null;
    switch (invoke.name) {
      case payments.Operations.UpdateShippingAddressOperation:
      case payments.Operations.UpdateShippingOptionOperation:
        paymentRequest = invoke.value;

        // Validate address AND shipping method (if selected)
        checkout
          .validateAndCalculateDetails(paymentRequest, paymentRequest.shippingAddress, paymentRequest.shippingOption)
          .then(updatedPaymentRequest => {
            // return new paymentRequest with updated details
            callback(null, updatedPaymentRequest, 200);
          }).catch(err => {
            // return error to onInvoke handler
            callback(err);
            // send error message back to user
            bot.beginDialog(invoke.relatesTo, 'checkout_failed', {
              errorMessage: err.message
            });
          });

        break;

      case payments.Operations.PaymentCompleteOperation:
        var paymentRequestComplete = invoke.value;
        paymentRequest = paymentRequestComplete.paymentRequest;
        var paymentResponse = paymentRequestComplete.paymentResponse;

        // Validate address AND shipping method
        checkout
          .validateAndCalculateDetails(paymentRequest, paymentResponse.shippingAddress, paymentResponse.shippingOption)
          .then(updatedPaymentRequest =>
            // Process Payment
            checkout
              .processPayment(updatedPaymentRequest, paymentResponse)
              .then(chargeResult => {
                // return success
                callback(null, { result: "success" }, 200);
                // send receipt to user
                bot.beginDialog(invoke.relatesTo, 'checkout_receipt', {
                  paymentRequest: updatedPaymentRequest,
                  chargeResult: chargeResult
                });
              })
          ).catch(err => {
            // return error to onInvoke handler
            callback(err);
            // send error message back to user
            bot.beginDialog(invoke.relatesTo, 'checkout_failed', {
              errorMessage: err.message
            });
          });

        break;
    }

  });
});

bot.dialog('checkout_receipt', function (session, args) {
  console.log('checkout_receipt', args);

  cleanupConversationData(session);

  var paymentRequest = args.paymentRequest;
  var chargeResult = args.chargeResult;
  var shippingAddress = chargeResult.shippingAddress;
  var shippingOption = chargeResult.shippingOption;
  var orderId = chargeResult.orderId;

  // send receipt card
  var items = paymentRequest.details.displayItems
    .map(o => builder.ReceiptItem.create(session, o.amount.currency + ' ' + o.amount.value, o.label));

  var receiptCard = new builder.ReceiptCard(session)
    .title('Contoso Order Receipt')
    .facts([
      builder.Fact.create(session, orderId, 'Order ID'),
      builder.Fact.create(session, chargeResult.methodName, 'Payment Method'),
      builder.Fact.create(session, [shippingAddress.addressLine, shippingAddress.city, shippingAddress.region, shippingAddress.country].join(', '), 'Shipping Address'),
      builder.Fact.create(session, shippingOption, 'Shipping Option')
    ])
    .items(items)
    .total(paymentRequest.details.total.amount.currency + ' ' + paymentRequest.details.total.amount.value);

  session.endDialog(
    new builder.Message(session)
      .addAttachment(receiptCard));
});

bot.dialog('checkout_failed', function (session, args) {
  cleanupConversationData(session);
  session.endDialog('Could not process your payment: %s', args.errorMessage);
});

// PaymentRequest with default options
function createPaymentRequest(cartId, product) {
  if (!cartId) {
    throw new Error('cartId is missing');
  }

  if (!product) {
    throw new Error('product is missing');
  }

  // PaymentMethodData[]
  var paymentMethods = [{
    supportedMethods: [payments.MicrosoftPayMethodName],
    data: {
      mode: process.env.PAYMENTS_LIVEMODE === 'true' ? null : 'TEST',
      merchantId: process.env.PAYMENTS_MERCHANT_ID,
      supportedNetworks: ['visa', 'mastercard'],
      supportedTypes: ['credit']
    }
  }];

  // PaymentDetails
  var paymentDetails = {
    total: {
      label: 'Total',
      amount: { currency: product.currency, value: product.price.toFixed(2) },
      pending: true
    },
    displayItems: [
      {
        label: product.name,
        amount: { currency: product.currency, value: product.price.toFixed(2) }
      }, {
        label: 'Shipping',
        amount: { currency: product.currency, value: '0.00' },
        pending: true
      }, {
        label: 'Sales Tax',
        amount: { currency: product.currency, value: '0.00' },
        pending: true
      }],
    // until a shipping address is selected, we can't offer shipping options or calculate taxes or shipping costs
    shippingOptions: []
  };

  // PaymentOptions
  var paymentOptions = {
    requestPayerName: true,
    requestPayerEmail: true,
    requestPayerPhone: true,
    requestShipping: true,
    shippingType: 'shipping'
  };

  // PaymentRequest
  return {
    id: cartId,
    expires: '1.00:00:00',          // 1 day
    methodData: paymentMethods,     // paymethodMethods: paymentMethods,
    details: paymentDetails,        // paymentDetails: paymentDetails,
    options: paymentOptions         // paymentOptions: paymentOptions
  };
}

function cleanupConversationData(session) {
  var cartId = session.conversationData[CartIdKey];
  delete session.conversationData[CartIdKey];
  delete session.conversationData[cartId];
}

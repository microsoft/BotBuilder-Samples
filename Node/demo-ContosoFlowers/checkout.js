var util = require('util');
var express = require('express');
var router = express.Router();

var orderService = require('./services/orders');
var bot = require('./bot');
var botUtils = require('./bot/utils');

/* GET Checkout */
router.get('/', function (req, res, next) {
  // orderId and user address
  var orderId = req.query.orderId;
  var address = botUtils.deserializeAddress(req.query.address);
  console.log('user address is', address);

  orderService.retrieveOrder(orderId).then(function (order) {
    // Check order exists
    if (!order) {
      throw new Error('Order ID not found');
    }

    // Check order if order is already processed
    if (order.payed) {
      // Dispatch completion dialog
      bot.beginDialog(address, 'checkout:completed', { orderId: orderId });

      // Show completion
      return res.render('checkout/completed', {
        title: 'Contoso Flowers - Order Processed',
        order: order
      });
    }

    // Payment form
    return res.render('checkout/index', {
      title: 'Contoso Flowers - Order Checkout',
      address: req.query.address,
      order: order
    });

  }).catch(function (err) {
    next(err);
  });

});

/* POST Checkout */
router.post('/', function (req, res, next) {
  // orderId and user address
  var orderId = req.body.orderId;
  var address = botUtils.deserializeAddress(req.body.address);
  console.log('user address is', address);

  // Payment information
  var paymentDetails = {
    creditcardNumber: req.body.creditcard,
    creditcardHolder: req.body.fullname
  };

  // Complete order
  orderService.confirmOrder(orderId, paymentDetails).then(function (processedOrder) {

    // Dispatch completion dialog
    bot.beginDialog(address, 'checkout:completed', { orderId: orderId });

    // Show completion
    return res.render('checkout/completed', {
      title: 'Contoso Flowers - Order processed',
      order: processedOrder
    });

  }).catch(function (err) {
    next(err);
  });
});

module.exports = router;

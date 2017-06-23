var _ = require('lodash');
var uuid = require('uuid');
var util = require('util');
var Promise = require('bluebird');
var Stripe = require('stripe')(process.env.PAYMENTS_STRIPE_API_KEY);
var PaymentToken = require('./payment-token');
var payments = require('./payments');
var catalog = require('./services/catalog');

function validateAndCalculateDetails(paymentRequest, shippingAddress, selectedShippingOption) {
    // selectedShippingOption may not be set the first time

    var productIds = [paymentRequest.id];

    // get product items
    return Promise
        .all(productIds.map(id => catalog.getItemById(id)))
        .then(products => {
            if (products.length === 0) {
                throw new Error('Product not available');
            }

            // get available shipping methods for address and validate
            var shippingMethods = getShippingMethodsForAddress(shippingAddress);
            var selectedShippingMethod = shippingMethods.find(o => o.id === selectedShippingOption);
            if (!selectedShippingMethod && shippingMethods.length > 0) {
                // preselect first option if none is found
                selectedShippingMethod = shippingMethods[0];
            }

            if (selectedShippingMethod) {
                selectedShippingMethod.selected = true;
            }

            // details
            var paymentDetails = {
                total: {
                    label: 'Total',
                    amount: { currency: 'USD', value: '0.00' }
                },
                displayItems: products.map(asDisplayItem)
                    .concat([
                        {
                            label: 'Shipping',
                            amount: { currency: 'USD', value: '0.00' },
                            pending: true
                        }, {
                            label: 'Sales Tax',
                            amount: { currency: 'USD', value: '0.00' },
                            pending: true
                        }]),
                shippingOptions: shippingMethods
            };

            // calculate tax, shipping cost and total
            recalculateTaxAndTotal(paymentDetails, shippingAddress, selectedShippingMethod);

            return _.assign({}, paymentRequest, { details: paymentDetails })
        });
}

function processPayment(paymentRequest, paymentResponse) {
    try {
        // sanity checks
        checkParam(paymentRequest, 'paymentRequest');
        checkParam(paymentResponse, 'paymentResponse');
        if (paymentResponse.methodName !== payments.MicrosoftPayMethodName) {
            throw new Error('Payment method is not supported.');
        }

        checkParam(paymentResponse.details, 'paymentResponse.details');
        checkParam(paymentResponse.details.paymentToken, 'paymentResponse.details.paymentToken');

        var paymentToken = PaymentToken.parse(paymentResponse.details.paymentToken);
        checkParam(paymentToken, 'parsed paymentToken');
        checkParam(paymentToken.source, 'Payment token source is empty.');
        if (paymentToken.header.format !== PaymentToken.tokenFormat.Stripe) {
            throw new Error('Payment token format is not Stripe.');
        }

        if (paymentToken.header.merchantId !== process.env.PAYMENTS_MERCHANT_ID) {
            throw new Error('MerchantId is not supported.');
        }

        if (paymentToken.header.amount.currency !== paymentRequest.details.total.amount.currency ||
            paymentToken.header.amount.value !== paymentRequest.details.total.amount.value) {
            throw new Error('Payment token amount currency/amount mismatch.');
        }

        var paymentRecord = {
            orderId: paymentRequest.id,
            transactionId: uuid.v1(),
            methodName: paymentResponse.methodName,
            paymentProcessor: paymentToken.header.Format,
            shippingAddress: paymentResponse.shippingAddress,
            shippingOption: paymentResponse.shippingOption,
            items: paymentRequest.details.displayItems,
            total: paymentRequest.details.total,
            liveMode: !paymentToken.isEmulated
        };

        // If the payment token is microsoft emulated do not charge (as it will fail)
        if (paymentToken.isEmulated) {
            return Promise.resolve(paymentRecord);
        } else {
            // Charge using Stripe
            var chargeOptions = {
                amount: Math.floor(parseFloat(paymentRequest.details.total.amount.value) * 100),            // Amount in cents
                currency: paymentRequest.details.total.amount.currency,
                description: paymentRequest.id,
                source: paymentToken.source
            };

            return Stripe.charges
                .create(chargeOptions)
                .then((charge) => {
                    console.log('Strige.charge.result', charge);
                    if (charge.status === 'succeeded' && charge.captured) {
                        // Charge succeeded, return payment paymentRecord
                        // Ideally, you should register the transaction using the paymentRecord and charge.id
                        return paymentRecord;
                    }

                    // Other statuses may include processing "pending" or "success" with non captured funds. It is up to the merchant how to handle these cases.
                    // If payment is captured but not charged this would be considered "unknown" (charge the captured amount after shipping scenario)
                    // Merchant might choose to handle "pending" and "failed" status or handle "success" status with funds captured null or false
                    // More information @ https://stripe.com/docs/api#charge_object-captured
                    throw new Error(util.format('Could not process charge using Stripe with charge.status: %s and charge.captured: %s', change.status, change.captured));
                });
        }

    } catch (err) {
        return Promise.reject(err);
    }
}

module.exports = {
    validateAndCalculateDetails: validateAndCalculateDetails,
    processPayment: processPayment
};

// Available Shipping Methods
function getShippingMethodsForAddress(shippingAddress) {
    console.log('getAvailableShippingMethods', shippingAddress.country);

    if (shippingAddress.country.toLowerCase() === 'zw') {
        throw new Error('ZW country not supported');
    }

    if (shippingAddress.country.toLowerCase() === 'us') {
        return [{
            id: 'STANDARD',
            label: 'Standard - (5-6 Business days)',
            amount: { currency: 'USD', value: '0.00' }
        }, {
            id: 'EXPEDITED',
            label: 'Expedited - (2 Business days)',
            amount: { currency: 'USD', value: '2.50' }
        }];
    } else {
        return [{
            id: 'INTERNATIONAL',
            label: 'International',
            amount: { currency: 'USD', value: '25.00' }
        }];
    }
}

function recalculateTaxAndTotal(details, shippingAddress, shippingOption) {
    console.log('updateTotalWithShipping', details);

    // shipping price
    if (shippingOption) {
        var shippingItem = details.displayItems.find(i => i.label === 'Shipping');
        shippingItem.amount = shippingOption.amount;
        shippingItem.pending = false;
    }

    // tax
    var subTotal = details.displayItems
        .filter(i => i.label !== 'Sales Tax')
        .reduce((a, b) => a + parseFloat(b.amount.value), 0);
    var taxItem = details.displayItems.find(i => i.label === 'Sales Tax');

    // only for US
    var tax = shippingAddress.country.toLowerCase() === 'us' ? subTotal * 0.085 : 0;
    taxItem.amount.value = tax.toFixed(2);
    taxItem.pending = false;

    // new total
    details.total.amount.value = details.displayItems
        .reduce((a, b) => a + parseFloat(b.amount.value), 0)
        .toFixed(2);
}

function asDisplayItem(product) {
    return {
        label: product.name,
        amount: {
            currency: product.currency,
            value: product.price.toFixed(2)
        }
    };
}

// helpers
function checkParam(param, name) {
    if (param === undefined) {
        throw new Error('Mising Parameter: \'' + name + '\'');
    }
}

function checkType(param, expectedType, name) {
    if (!(param instanceof expectedType)) {
        throw new Error('Expected type \'' + expectedType.name + '\' for parameter \'' + name + '\'');
    }
}

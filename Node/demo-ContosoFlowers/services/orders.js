var uuid = require('uuid');
var fs = require('fs');
var _ = require('lodash');
var Promise = require('bluebird');

var OrderService = {
    placePendingOrder: function (order) {
        order.id = uuid.v1();
        order.payed = false;

        var orders = this.load();
        orders.push(order);
        this.save(orders);

        return Promise.resolve(order);
    },
    retrieveOrder: function (orderId) {
        var orders = this.load();
        var order = _.find(orders, ['id', orderId]);

        return Promise.resolve(order);
    },
    confirmOrder: function (orderId, paymentDetails) {
        var orders = this.load();
        var order = _.find(orders, ['id', orderId]);
        if (!order) {
            return Promise.reject({ error: 'Order ID not found' });
        }

        if (order.payed) {
            return Promise.resolve(order);
        }

        order.payed = true;
        order.paymentDetails = paymentDetails;
        this.save(orders);

        return Promise.resolve(order);
    },

    // persistence
    load: function () {
        var json = fs.readFileSync('./data/orders.json', { encoding: 'utf8' });
        return JSON.parse(json);
    },
    save: function (orders) {
        var json = JSON.stringify(orders);
        fs.writeFileSync('./data/orders.json', json, { encoding: 'utf8' });
    }
};

module.exports = OrderService;
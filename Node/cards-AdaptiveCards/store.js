var Promise = require('bluebird');

module.exports = {
    searchHotels: function (destination, checkInDate, checkOutDate) {
        return new Promise(function (resolve) {

            // Filling the hotels results manually just for demo purposes
            var hotels = [];
            for (var i = 1; i <= 6; i++) {
                hotels.push({
                    name: 'Hotel ' + i,
                    location: destination,
                    rating: Math.ceil(Math.random() * 5),
                    numberOfReviews: Math.floor(Math.random() * 5000) + 1,
                    priceStarting: Math.floor(Math.random() * 450) + 80,
                    image: 'https://placeholdit.imgix.net/~text?txtsize=35&txt=Hotel+' + i + '&w=500&h=260',
                    moreImages: [
                        'https://placeholdit.imgix.net/~text?txtsize=65&txt=Pic+1&w=450&h=300',
                        'https://placeholdit.imgix.net/~text?txtsize=65&txt=Pic+2&w=450&h=300',
                        'https://placeholdit.imgix.net/~text?txtsize=65&txt=Pic+3&w=450&h=300',
                        'https://placeholdit.imgix.net/~text?txtsize=65&txt=Pic+4&w=450&h=300'
                    ]
                });
            }

            hotels.sort(function (a, b) { return a.priceStarting - b.priceStarting; });

            // complete promise with a timer to simulate async response
            setTimeout(function () { resolve(hotels); }, 1000);
        });
    }
};
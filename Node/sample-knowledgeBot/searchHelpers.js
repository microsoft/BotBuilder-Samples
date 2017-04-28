module.exports = function () {
    global.request = require('request');

    global.searchQueryStringBuilder = function (query) {
        return queryString + query;
    }

    global.performSearchQuery = function (queryString, callback) {
        request(queryString, function (error, response, body) {
            if (!error && response && response.statusCode == 200) {
                var result = JSON.parse(body);
                callback(null, result);
            } else {
                callback(error, null);
            }
        })
    }
}

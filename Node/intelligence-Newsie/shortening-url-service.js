var apiHandler = require('./api-handler-service');

const TINY_URL_API_URL = "http://tinyurl.com/api-create.php";

module.exports = {
    getShortenedUrl: (url) => {
        return apiHandler.getResponse(TINY_URL_API_URL, { "url": url }, {});
    }
}
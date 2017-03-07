var apiHandler = require('./api-handler-service');

const BING_SEARCH_API_URL = "https://api.cognitive.microsoft.com/bing/v5.0/search/",
    BING_SEARCH_API_KEY = process.env.BING_SEARCH_API_KEY;

var headers = { "Ocp-Apim-Subscription-Key": BING_SEARCH_API_KEY }

module.exports = {
    findArticles: (query) => {
        return apiHandler.getResponse(BING_SEARCH_API_URL, { "q": query + " site:wikipedia.org", "form": "BTCSWR" }, headers)
            .then(result => { return JSON.parse(result); }, err => { return err });
    }
}
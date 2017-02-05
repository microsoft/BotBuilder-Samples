var apiHandler = require('./api-handler-service');

const BING_NEWS_API_URL = "https://api.cognitive.microsoft.com/bing/v5.0/news/",
    BING_NEWS_API_KEY = process.env.BING_NEWS_API_KEY;

var headers = { "Ocp-Apim-Subscription-Key": BING_NEWS_API_KEY }

module.exports = {
    findNewsByQuery: (query) => {
        return apiHandler.getResponse(BING_NEWS_API_URL + "search", { "q": query }, headers)
            .then(result => { return JSON.parse(result); }, err => { return err });
    },
    findNewsByCategory: (categoryName) => {
        return apiHandler.getResponse(BING_NEWS_API_URL, { "category": categoryName }, headers)
            .then(result => { return JSON.parse(result); }, err => { return err });
    }
}
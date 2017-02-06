var apiHandler = require('./api-handler-service');

const BING_SUMMARIZER_API_URL = "http://bing-blink.azurewebsites.net/api/blink/summary",
    BING_SUMMARIZER_API_KEY = process.env.BING_SUMMARIZER_API_KEY;

module.exports = {
    getSummary: (url) => {
        return apiHandler.getResponse(BING_SUMMARIZER_API_URL, { "url": url }, { "Ocp-Apim-Subscription-Key": BING_SUMMARIZER_API_KEY })
            .then(result => { return JSON.parse(result); }, err => { return err });
    }
}
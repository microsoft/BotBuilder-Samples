var shorteningUrlService = require('./shortening-url-service.js');
var urlObj = require('url');

module.exports = {
    newsCategories: {
        NONE: { name: "", displayName: "", synonyms: [""] },
        BUSINESS: { name: "Business", displayName: "Business", queryName: "Business", synonyms: ["Business"] },
        HEALTH: { name: "Health", displayName: "Health", queryName: "Health", synonyms: ["Health"] },
        WORLD: { name: "World", displayName: "World", queryName: "World", synonyms: ["World"] },
        SPORTS: { name: "Sports", displayName: "Sports", queryName: "Sports", synonyms: ["Sport"] },
        ENTERTAINMENT: { name: "Entertainment", displayName: "Entertainment", queryName: "Entertainment", synonyms: ["Entertaining", "Entertain"] },
        POLITICS: { name: "Politics", displayName: "Politics", queryName: "Politics", synonyms: ["Politic"] },
        SCIENCEANDTECHNOLOGY: { name: "ScienceAndTechnology", displayName: "Science and Technology", queryName: "Science Technology", synonyms: ["Sci", "Tech", "Tech.", "Technology", "Science", "Science and Technology", "ScienceAndTechnology"] },
        parseNewsCategory: (input) => {
            input = input.toUpperCase();
            for (var newsCategory in module.exports.newsCategories) {
                if (module.exports.newsCategories.hasOwnProperty(newsCategory) && typeof module.exports.newsCategories[newsCategory] !== "function") {
                    var obj = module.exports.newsCategories[newsCategory];
                    if (obj.displayName.toUpperCase() == input.toUpperCase() ||
                        obj.synonyms.some((elem) => { return elem.toUpperCase().indexOf(input) != -1; })) {
                        return obj;
                    }
                }
            }
            return module.exports.newsCategories.NONE;
        },
        listDisplayNames: () => {
            var displayNames = [];
            for (var newsCategory in module.exports.newsCategories) {
                if (module.exports.newsCategories.hasOwnProperty(newsCategory) && typeof module.exports.newsCategories[newsCategory] !== "function") {
                    var obj = module.exports.newsCategories[newsCategory];
                    if (obj != module.exports.newsCategories.NONE)
                        displayNames.push(module.exports.newsCategories[newsCategory].displayName);
                }
            }
            return displayNames;
        }
    },
    prepareNewsieResult: (bingNews) => {
        var myUrl = urlObj.parse(bingNews.url, true);
        var url;

        if (myUrl.host == "www.bing.com" && myUrl.pathname == "/cr") {
            url = myUrl.query["r"];
        } else {
            url = bingNews.url;
        }

        return shorteningUrlService.getShortenedUrl(url)
            .then(shortenedUrl => {
                var newsieResult = {};

                newsieResult.shortenedUrl = shortenedUrl;

                module.exports.prepareNewsieResultHelper(bingNews, newsieResult);

                return newsieResult;
            });
    },
    prepareNewsieResultHelper: (bingNews, newsieResult) => {
        const newsMaxTitleChar = 200;
        const newsMaxDescriptionChar = 500;
        const newsMaxProviderChar = 100;

        if (bingNews.description.length > newsMaxDescriptionChar) {
            newsieResult.shortenedDescription = bingNews.description.substr(0, newsMaxDescriptionChar) + '...';
        } else {
            newsieResult.shortenedDescription = bingNews.description;
        }

        if (bingNews.name.length > newsMaxTitleChar) {
            newsieResult.name = bingNews.name.substr(0, newsMaxTitleChar) + '...';
        } else {
            newsieResult.name = bingNews.name;
        }

        if (bingNews.provider[0].name.length > newsMaxProviderChar) {
            newsieResult.providerShortenedName = bingNews.provider[0].name.substr(0, newsMaxProviderChar) + '...';
        } else {
            newsieResult.providerShortenedName = bingNews.provider[0].name;
        }

        newsieResult.datePublished = "";

        newsieResult.imageContentUrl = bingNews.image.thumbnail.contentUrl + "&w=" + bingNews.image.thumbnail.width + "&h=" + bingNews.image.thumbnail.height;
        newsieResult.url = bingNews.url;

        return newsieResult;
    }
}
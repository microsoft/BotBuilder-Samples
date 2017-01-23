// This loads the environment variables from the .env file
require('dotenv-extended').load();

var builder = require('botbuilder');
var restify = require('restify');
var memoryCache = require('memory-cache');
var util = require('util');
var fs = require('fs');
var newsieStrings = require('./newsie-strings.js');
var bingNewsService = require('./bing-news-service.js');
var bingSummarizerService = require('./bing-summarizer-service.js');
var newsieUtils = require('./newsie-utils.js');

// Setup Restify Server
var server = restify.createServer();
server.listen(process.env.port || process.env.PORT || 3978, () => {
    console.log('%s listening to %s', server.name, server.url);
});

server.get('/content/binglogo.jpg', (req, res) => {
    var img = fs.readFileSync('./content/binglogo.jpg');
    res.writeHead(200, { 'Content-Type': 'image/jpg' });
    res.end(img, 'binary');
});

// Create chat bot
var connector = new builder.ChatConnector({
    appId: process.env.MICROSOFT_APP_ID,
    appPassword: process.env.MICROSOFT_APP_PASSWORD
});
var bot = new builder.UniversalBot(connector);

var serverName;

server.post('/api/messages', (req, res) => {
    var listenerFunc = connector.listen();
    serverName = req.headers.host;
    listenerFunc(req, res);
});

// You can provide your own model by specifing the 'LUIS_MODEL_URL' environment variable
// This Url can be obtained by uploading or creating your model from the LUIS portal: https://www.luis.ai/
const LuisModelUrl = process.env.LUIS_MODEL_URL ||
    'https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/6ef53edb-ea47-48f1-92f9-7405e8a4dc46?subscription-key=a6d628faa2404cd799f2a291245eb135';

// Main dialog with LUIS
var recognizer = new builder.LuisRecognizer(LuisModelUrl);
var intents = new builder.IntentDialog({ recognizers: [recognizer] })
    .matches('Greeting', [
        function(session) {
            var message = new builder.Message()
                .attachments([
                    new builder.HeroCard().text(newsieStrings.GreetOnDemand),
                    new builder.HeroCard().buttons(function getCardActions() {
                        var cardActions = [];
                        newsieUtils.newsCategories.listDisplayNames().forEach((displayName) => {
                            cardActions.push(new builder.CardAction().title(displayName).value(displayName).type('imBack'));
                        });
                        return cardActions;
                    }()),
                    new builder.ThumbnailCard().buttons([new builder.CardAction().title(newsieStrings.BingForMore).type("openUrl").value("https://www.bing.com/news/search?q=bing+news")])
                ]);

            session.send(message).endDialog();
        }
    ])
    .matches(new RegExp('(^summary )(.*)', 'i'), [
        (session, args) => {
            var articleUrl = args.matched[2];
            if (args && args.matched && args.matched.length == 3) {
                bingSummarizerService.getSummary(articleUrl).then((bingSummary) => {
                    if (bingSummary && bingSummary.Data && bingSummary.Data.lenght != 0) {

                        var bingNews;
                        var message;

                        if ((bingNews = memoryCache.get(articleUrl))) {

                            var newsieResult = {};
                            newsieUtils.prepareNewsieResultHelper(bingNews, newsieResult);

                            var thumbNailCard = getNewsArticleHeadlineImageCard(newsieResult, session.message.address.channelId);

                            message = new builder.Message().attachments([thumbNailCard]);
                            session.send(message);
                        }

                        session.send(newsieStrings.SummaryString);
                        message = new builder.Message()
                            .attachments(bingSummary.Data.map((item) => {
                                return new builder.HeroCard().text(item.Text);
                            }));
                        session.send(message).endDialog();
                    } else {
                        session.send(newsieStrings.SummaryErrorMessage).endDialog();
                    }
                }).catch(() => { session.send(newsieStrings.SummaryErrorMessage).endDialog(); });
            } else {
                session.send(newsieStrings.SummaryErrorMessage).endDialog();
            }
        }
    ])
    .matches('News', [
        (session, args) => {
            var entityRecognized;
            var category = newsieUtils.newsCategories.NONE;
            var promise;

            if ((entityRecognized = builder.EntityRecognizer.findEntity(args.entities, 'NewsCategory')) && (category = newsieUtils.newsCategories.parseNewsCategory(entityRecognized.entity))) {
                promise = bingNewsService.findNewsByCategory(category.name).then((bingNews) => {
                    session.send(util.format(newsieStrings.NewsCategoryTypeMessage, category.name.toLowerCase()));
                    return createNewsMessage(bingNews, category, session.message.address.channelId);
                });
            }
            else if ((entityRecognized = builder.EntityRecognizer.findEntity(args.entities, 'NewsTopic'))) {
                promise = bingNewsService.findNewsByQuery(entityRecognized.entity).then((bingNews) => {
                    session.send(newsieStrings.NewsTopicTypeMessage);
                    return createNewsMessage(bingNews, category, session.message.address.channelId);
                });
            }
            else {
                promise = bingNewsService.findNewsByQuery(session.message.text).then((bingNews) => {
                    session.send(newsieStrings.NewsTopicTypeMessage);
                    return createNewsMessage(bingNews, category, session.message.address.channelId);
                    
                });
            }

            promise.then((message) => {
                session.send(message).endDialog();
            }).catch(() => {
                session.endDialog();
            });
        }
    ])
    .onDefault((session) => {
        session.send(newsieStrings.FallbackIntentMessage).endDialog();
    });

bot.dialog('/', intents);

function createNewsMessage(allBingNews, category, channelId) {
    const newsMaxResults = 5;
    const oneDayMiliSec = 86400000;

    var newsieResultsPromises = allBingNews.value.slice(0, newsMaxResults).map((bingNews) => {

        return newsieUtils.prepareNewsieResult(bingNews).then(newsieResult => {
            memoryCache.put(newsieResult.shortenedUrl, bingNews, oneDayMiliSec);
            return newsieResult;
        });
    });

    var attachPromise = Promise.all(newsieResultsPromises)
        .then(newsieResults => {
            return newsieResults.map((newsieResult) => {
                return getNewsArticleCard(newsieResult, channelId);
            });
        });

    return attachPromise.then(atts => {
        if (category != newsieUtils.newsCategories.NONE) {
            atts.push(new builder.HeroCard()
                .buttons([new builder.CardAction().title(newsieStrings.BingForMore).value(util.format("https://www.bing.com/news?q=%s+news", category.queryName)).type("openUrl")])
                .images([new builder.CardImage().url("http://" + serverName + "/content/binglogo.jpg")]));
        }

        var message = new builder.Message()
            .attachmentLayout(builder.AttachmentLayout.carousel)
            .attachments(atts);

        return message;
    });
}

// Helper functions NewsCardGenerators
function getNewsArticleCard(newsieResult, channelId) {
    var name = newsieResult.name;
    var publisher = newsieResult.providerShortenedName.toUpperCase() + "(" + newsieResult.datePublished + ")";
    var image = new builder.CardImage().url(newsieResult.imageContentUrl);
    var tap = new builder.CardAction().title(newsieStrings.ViewOnWeb).value(newsieResult.shortenedUrl).type('openUrl');

    var card = new builder.ThumbnailCard().text(newsieResult.shortenedDescription)
        .images([image])
        .buttons([
            new builder.CardAction().title(newsieStrings.ReadSummary).value("summary " + newsieResult.shortenedUrl).type("imBack")
        ]);

    switch (channelId) {
    case "slack":
        card.title(publisher).subtitle(name).tap(tap);
        break;
    default:
        card.title(name).subtitle(publisher);
    }


    return card;
}

function getNewsArticleHeadlineImageCard(newsieResult, channelId) {
    var name = newsieResult.name;
    var publisher = newsieResult.providerShortenedName.toUpperCase() + "(" + newsieResult.datePublished + ")";
    var image = new builder.CardImage().url(newsieResult.imageContentUrl);
    var action = new builder.CardAction().title(newsieStrings.FullStoryString).value(newsieResult.url).type('openUrl');

    var card = new builder.HeroCard().text(newsieResult.shortenedDescription).images([image]).buttons([action]);

    switch (channelId) {
        case "slack":
            card.title(publisher).subtitle(name);
            break;
        default:
            card.title(name).subtitle(publisher);
    }

    return card;
}

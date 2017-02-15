// This loads the environment variables from the .env file
require('dotenv-extended').load();

var builder = require('botbuilder');
var restify = require('restify');
var util = require('util');
var zummerStrings = require('./zummer-strings.js');
var bingSearchService = require('./bing-search-service.js');
var urlObj = require('url');

// Setup Restify Server
var server = restify.createServer();
server.listen(process.env.port || process.env.PORT || 3978, () => {
    console.log('%s listening to %s', server.name, server.url);
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
    'https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/b550e80a-74ec-4bb4-bcbc-fe35f5b1fce4?subscription-key=a6d628faa2404cd799f2a291245eb135';

// Main dialog with LUIS
var recognizer = new builder.LuisRecognizer(LuisModelUrl);
var intents = new builder.IntentDialog({ recognizers: [recognizer] })
    .matches('Greeting', [
        (session) => {
            session.send(zummerStrings.GreetOnDemand).endDialog();
        }
    ])
    .matches('Search', [
        (session, args) => {
            var entityRecognized;
            var query;

            if ((entityRecognized = builder.EntityRecognizer.findEntity(args.entities, 'ArticleTopic'))) {
                query = entityRecognized.entity;
            } else {
                query = session.message.text;
            }

            bingSearchService.findArticles(query).then((bingSearch) => {

                session.send(zummerStrings.SearchTopicTypeMessage);

                var zummerResult = prepareZummerResult(query, bingSearch.webPages.value[0]);

                var summaryText = util.format("### [%s](%s)\n%s\n\n", zummerResult.title, zummerResult.url, zummerResult.snippet);

                summaryText += util.format("*%s*", util.format(zummerStrings.PoweredBy, util.format("[Bing™](https://www.bing.com/search/?q=%s site:wikipedia.org)", zummerResult.query)));

                session.send(summaryText).endDialog();
            });
        }
    ])
    .onDefault((session) => {
        session.send(zummerStrings.FallbackIntentMessage).endDialog();
    });

bot.dialog('/', intents);


function prepareZummerResult(query, bingSearchResult) {
    var myUrl = urlObj.parse(bingSearchResult.url, true);
    var zummerResult = {};

    if (myUrl.host == "www.bing.com" && myUrl.pathname == "/cr") {
        zummerResult.url = myUrl.query["r"];
    } else {
        zummerResult.url = bingSearchResult.url;
    }

    zummerResult.title = bingSearchResult.name;
    zummerResult.query = query;
    zummerResult.snippet = bingSearchResult.snippet;

    return zummerResult;
}
// This loads the environment variables from the .env file
require('dotenv-extended').load();

var builder = require('botbuilder');
var restify = require('restify');
var Store = require('./store');
var spellService = require('./spell-service');

// Setup Restify Server
var server = restify.createServer();
server.listen(process.env.port || process.env.PORT || 3978, function () {
    console.log('%s listening to %s', server.name, server.url);
});

// Create chat bot
var connector = new builder.ChatConnector({
    appId: process.env.MICROSOFT_APP_ID,
    appPassword: process.env.MICROSOFT_APP_PASSWORD
});
var bot = new builder.UniversalBot(connector);
server.post('/api/messages', connector.listen());

// You can provide your own model by specifing the 'LUIS_MODEL_URL' environment variable
// This Url can be obtained by uploading or creating your model from the LUIS portal: https://www.luis.ai/
const LuisModelUrl = process.env.LUIS_MODEL_URL ||
    'https://api.projectoxford.ai/luis/v1/application?id=162bf6ee-379b-4ce4-a519-5f5af90086b5&subscription-key=11be6373fca44ded80fbe2afa8597c18';

// Main dialog with LUIS
var recognizer = new builder.LuisRecognizer(LuisModelUrl);
var intents = new builder.IntentDialog({ recognizers: [recognizer] })
    .matches('SearchHotels', [
        function (session, args, next) {
            session.send('Welcome to the Hotels finder! we are analyzing your message: \'%s\'', session.message.text);

            // try extracting entities
            var cityEntity = builder.EntityRecognizer.findEntity(args.entities, 'builtin.geography.city');
            var airportEntity = builder.EntityRecognizer.findEntity(args.entities, 'AirportCode');
            if (cityEntity) {
                // city entity detected, continue to next step
                session.dialogData.searchType = 'city';
                next({ response: cityEntity.entity });
            } else if (airportEntity) {
                // airport entity detected, continue to next step
                session.dialogData.searchType = 'airport';
                next({ response: airportEntity.entity });
            } else {
                // no entities detected, ask user for a destination
                builder.Prompts.text(session, 'Please enter your destination');
            }
        },
        function (session, results) {
            var destination = results.response;

            var message = 'Looking for hotels';
            if (session.dialogData.searchType === 'airport') {
                message += ' near %s airport...';
            } else {
                message += ' in %s...';
            }

            session.send(message, destination);

            // Async search
            Store
                .searchHotels(destination)
                .then((hotels) => {
                    // args
                    session.send('I found %d hotels:', hotels.length);

                    var message = new builder.Message()
                        .attachmentLayout(builder.AttachmentLayout.carousel)
                        .attachments(hotels.map(hotelAsAttachment));

                    session.send(message);

                    // End
                    session.endDialog();
                });
        }
    ])
    .matches('ShowHotelsReviews', (session, args) => {
        // retrieve hotel name from matched entities
        var hotelEntity = builder.EntityRecognizer.findEntity(args.entities, 'Hotel');
        if (hotelEntity) {
            session.send('Looking for reviews of \'%s\'...', hotelEntity.entity);
            Store.searchHotelReviews(hotelEntity.entity)
                .then((reviews) => {
                    var message = new builder.Message()
                        .attachmentLayout(builder.AttachmentLayout.carousel)
                        .attachments(reviews.map(reviewAsAttachment));
                    session.send(message)
                });
        }
    })
    .matches('Help', builder.DialogAction.send('Hi! Try asking me things like \'search hotels in Seattle\', \'search hotels near LAX airport\' or \'show me the reviews of The Bot Resort\''))
    .onDefault((session) => {
        session.send('Sorry, I did not understand \'%s\'. Type \'help\' if you need assistance.', session.message.text);
    });

if (process.env.IS_SPELL_CORRECTION_ENABLED == "true") {
    bot.use({
        botbuilder: function (session, next) {
            spellService
                .getCorrectedText(session.message.text)
                .then(text => {
                    session.message.text = text;
                    next();
                })
                .catch((error) => {
                    console.error(error);
                    next();
                });
        }
    })
}

bot.dialog('/', intents);

// Helpers
function hotelAsAttachment(hotel) {
    return new builder.HeroCard()
        .title(hotel.name)
        .subtitle('%d stars. %d reviews. From $%d per night.', hotel.rating, hotel.numberOfReviews, hotel.priceStarting)
        .images([new builder.CardImage().url(hotel.image)])
        .buttons([
            new builder.CardAction()
                .title('More details')
                .type('openUrl')
                .value('https://www.bing.com/search?q=hotels+in+' + encodeURIComponent(hotel.location))
        ]);
}

function reviewAsAttachment(review) {
    return new builder.ThumbnailCard()
        .title(review.title)
        .text(review.text)
        .images([new builder.CardImage().url(review.image)])
}
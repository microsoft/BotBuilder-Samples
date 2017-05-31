// This loads the environment variables from the .env file
require('dotenv-extended').load();

var util = require('util');
var builder = require('botbuilder');
var restify = require('restify');

// Setup Restify Server
var server = restify.createServer();
server.listen(process.env.port || process.env.PORT || 3978, function () {
    console.log('%s listening to %s', server.name, server.url);
});

// Create chat bot and listen to messages
var connector = new builder.ChatConnector({
    appId: process.env.MICROSOFT_APP_ID,
    appPassword: process.env.MICROSOFT_APP_PASSWORD
});
server.post('/api/messages', connector.listen());

var bot = new builder.UniversalBot(connector, function (session) {

    if (session.message && session.message.value) {
        // A Card's Submit Action obj was received
        processSubmitAction(session, session.message.value);
        return;
    }

    // Display Welcome card with Hotels and Flights search options
    var card = {
        'contentType': 'application/vnd.microsoft.card.adaptive',
        'content': {
            '$schema': 'http://adaptivecards.io/schemas/adaptive-card.json',
            'type': 'AdaptiveCard',
            'version': '1.0',
            'body': [
                {
                    'type': 'Container',
                    'speak': '<s>Hello!</s><s>Are you looking for a flight or a hotel?</s>',
                    'items': [
                        {
                            'type': 'ColumnSet',
                            'columns': [
                                {
                                    'type': 'Column',
                                    'size': 'auto',
                                    'items': [
                                        {
                                            'type': 'Image',
                                            'url': 'https://placeholdit.imgix.net/~text?txtsize=65&txt=Adaptive+Cards&w=300&h=300',
                                            'size': 'medium',
                                            'style': 'person'
                                        }
                                    ]
                                },
                                {
                                    'type': 'Column',
                                    'size': 'stretch',
                                    'items': [
                                        {
                                            'type': 'TextBlock',
                                            'text': 'Hello!',
                                            'weight': 'bolder',
                                            'isSubtle': true
                                        },
                                        {
                                            'type': 'TextBlock',
                                            'text': 'Are you looking for a flight or a hotel?',
                                            'wrap': true
                                        }
                                    ]
                                }
                            ]
                        }
                    ]
                }
            ],
            'actions': [
                // Hotels Search form
                {
                    'type': 'Action.ShowCard',
                    'title': 'Hotels',
                    'speak': '<s>Hotels</s>',
                    'card': {
                        'type': 'AdaptiveCard',
                        'body': [
                            {
                                'type': 'TextBlock',
                                'text': 'Welcome to the Hotels finder!',
                                'speak': '<s>Welcome to the Hotels finder!</s>',
                                'weight': 'bolder',
                                'size': 'large'
                            },
                            {
                                'type': 'TextBlock',
                                'text': 'Please enter your destination:'
                            },
                            {
                                'type': 'Input.Text',
                                'id': 'destination',
                                'speak': '<s>Please enter your destination</s>',
                                'placeholder': 'Miami, Florida',
                                'style': 'text'
                            },
                            {
                                'type': 'TextBlock',
                                'text': 'When do you want to check in?'
                            },
                            {
                                'type': 'Input.Date',
                                'id': 'checkin',
                                'speak': '<s>When do you want to check in?</s>'
                            },
                            {
                                'type': 'TextBlock',
                                'text': 'How many nights do you want to stay?'
                            },
                            {
                                'type': 'Input.Number',
                                'id': 'nights',
                                'min': 1,
                                'max': 60,
                                'speak': '<s>How many nights do you want to stay?</s>'
                            }
                        ],
                        'actions': [
                            {
                                'type': 'Action.Submit',
                                'title': 'Search',
                                'speak': '<s>Search</s>',
                                'data': {
                                    'type': 'hotelSearch'
                                }
                            }
                        ]
                    }
                },
                {
                    'type': 'Action.ShowCard',
                    'title': 'Flights',
                    'speak': '<s>Flights</s>',
                    'card': {
                        'type': 'AdaptiveCard',
                        'body': [
                            {
                                'type': 'TextBlock',
                                'text': 'Flights is not implemented =(',
                                'speak': '<s>Flights is not implemented</s>',
                                'weight': 'bolder'
                            }
                        ]
                    }
                }
            ]
        }
    };

    var msg = new builder.Message(session)
        .addAttachment(card);
    session.send(msg);
});

// Search Hotels
bot.dialog('hotels-search', require('./hotels-search'));

// Help
bot.dialog('support', require('./support'))
    .triggerAction({
        matches: [/help/i, /support/i, /problem/i]
    });

// log any bot errors into the console
bot.on('error', function (e) {
    console.log('And error ocurred', e);
});

function processSubmitAction(session, value) {
    var defaultErrorMessage = 'Please complete all the search parameters';
    switch (value.type) {
        case 'hotelSearch':
            // Search, validate parameters
            if (validateHotelSearch(value)) {
                // proceed to search
                session.beginDialog('hotels-search', value);
            } else {
                session.send(defaultErrorMessage);
            }
            break;

        case 'hotelSelection':
            // Hotel selection
            sendHotelSelection(session, value);
            break;

        default:
            // A form data was received, invalid or incomplete since the previous validation did not pass
            session.send(defaultErrorMessage);
    }
}

function validateHotelSearch(hotelSearch) {
    if (!hotelSearch) {
        return false;
    }

    // Destination
    var hasDestination = typeof hotelSearch.destination === 'string' && hotelSearch.destination.length > 3;

    // Checkin
    var checkin = Date.parse(hotelSearch.checkin);
    var hasCheckin = !isNaN(checkin);
    if (hasCheckin) {
        hotelSearch.checkin = new Date(checkin);
    }

    // Nights
    var nights = parseInt(hotelSearch.nights, 10);
    var hasNights = !isNaN(nights);
    if (hasNights) {
        hotelSearch.nights = nights;
    }

    return hasDestination && hasCheckin && hasNights;
}

function sendHotelSelection(session, hotel) {
    var description = util.format('%d stars with %d reviews. From $%d per night.', hotel.rating, hotel.numberOfReviews, hotel.priceStarting);
    var card = {
        'contentType': 'application/vnd.microsoft.card.adaptive',
        'content': {
            'type': 'AdaptiveCard',
            'body': [
                {
                    'type': 'Container',
                    'items': [
                        {
                            'type': 'TextBlock',
                            'text': hotel.name + ' in ' + hotel.location,
                            'weight': 'bolder',
                            'speak': '<s>' + hotel.name + '</s>'
                        },
                        {
                            'type': 'TextBlock',
                            'text': description,
                            'speak': '<s>' + description + '</s>'
                        },
                        {
                            'type': 'Image',
                            'size': 'auto',
                            'url': hotel.image
                        },
                        {
                            'type': 'ImageSet',
                            'imageSize': 'medium',
                            'separation': 'strong',
                            'images': hotel.moreImages.map((img) => ({
                                'type': 'Image',
                                'url': img
                            }))
                        }
                    ],
                    'selectAction': {
                        'type': 'Action.OpenUrl',
                        'url': 'https://dev.botframework.com/'
                    }
                }
            ]
        }
    };

    var msg = new builder.Message(session)
        .addAttachment(card);

    session.send(msg);
}
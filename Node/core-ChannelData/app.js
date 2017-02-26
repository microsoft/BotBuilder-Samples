// This loads the environment variables from the .env file
require('dotenv-extended').load();

var builder = require('botbuilder');
var restify = require('restify');

// Setup Restify Server
var server = restify.createServer();
server.listen(process.env.port || process.env.PORT || 3978, function () {
    console.log('%s listening to %s', server.name, server.url);
});

// Create connector and listen for messages
var connector = new builder.ChatConnector({
    appId: process.env.MICROSOFT_APP_ID,
    appPassword: process.env.MICROSOFT_APP_PASSWORD
});
server.post('/api/messages', connector.listen());

var FacebookDataModels = require('./facebook-channeldata');

var bot = new builder.UniversalBot(connector, function (session) {

    session.send('Looking into your upcoming flights to see if you can check-in on any of those...');

    var now = new Date();

    // Airline Checkin
    var checkin = new FacebookDataModels.AirlineCheckin(
        'Check-in is available now',
        'en_US',
        'ABCDED',
        'http://www.airline.com/check_in',
        [
            new FacebookDataModels.FlightInfo(
                'F001',
                new FacebookDataModels.Airport(
                    'SFO',
                    'San Francisco',
                    'T4',
                    'G8'
                ),
                new FacebookDataModels.Airport(
                    'EZE',
                    'Buenos Aires',
                    'C',
                    'A2'
                ),
                new FacebookDataModels.FlightSchedule(
                    now.addDays(1),
                    now.addDays(1).addHours(1.5),
                    now.addDays(2)
                ))
        ]);

    // The previous object construct will be serialized to JSON when passed as an attachment.
    // The same result can be achieved using the following in JSON notation:
    // var checkin = {
    //     "type": "template",
    //     "payload": {
    //         "template_type": "airline_checkin",
    //         "intro_message": "Check-in is available now",
    //         "locale": "en_US",
    //         "pnr_number": "ABCDED",
    //         "checkin_url": "http://www.airline.com/check_in",
    //         "flight_info": [
    //             {
    //                 "flight_number": "F001",
    //                 "departure_airport": {
    //                     "airport_code": "SFO",
    //                     "city": "San Francisco",
    //                     "terminal": "T4",
    //                     "gate": "G8"
    //                 },
    //                 "arrival_airport": {
    //                     "airport_code": "EZE",
    //                     "city": "Buenos Aires",
    //                     "terminal": "C",
    //                     "gate": "A2"
    //                 },
    //                 "flight_schedule": {
    //                     "boarding_time": formatDate(now.addDays(1)),
    //                     "departure_time": formatDate(now.addDays(1).addHours(1.5)),
    //                     "arrival_time": formatDate(now.addDays(2))
    //                 }
    //             }
    //         ]
    //     }
    // };

    var reply = new builder.Message(session);
    if (session.message.address.channelId === 'facebook') {
        reply.sourceEvent({
            facebook: {
                attachment: checkin
            }
        });
    } else {
        reply.text(checkin.toString());
    }

    session.send(reply);
});


// Helpers
Date.prototype.addDays = function (days) {
    var date = new Date(this.valueOf());
    date.setDate(date.getDate() + days);
    return date;
};

Date.prototype.addHours = function (hours) {
    var date = new Date(this.valueOf());
    date.setTime(date.getTime() + hours * 60 * 60 * 1000);
    return date;
};

function formatDate(date) {
    return date.toISOString().split('.')[0];
}
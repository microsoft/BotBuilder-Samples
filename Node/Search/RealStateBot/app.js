var util = require('util');
var _ = require('lodash');
var builder = require('botbuilder');
var restify = require('restify');

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

// Root dialog, triggers search and process its results
bot.dialog('/', [
    function (session) {
        // Trigger Search
        session.beginDialog('realstate:/');
    },
    function (session, args) {
        // Process selected search results
        session.send(
            'Done! For future reference, you selected these properties: %s',
            args.selection.map(i => i.key).join(', '));
    }
]);

// Azure Search provider
var AzureSearch = require('../SearchProviders/azure-search');
var azureSearchClient = AzureSearch.create('realestate', '82BCF03D2FC9AC7F4E9D7DE1DF3618A5', 'listings');

/// <reference path="../SearchDialogLibrary/index.d.ts" />
var SearchDialogLibrary = require('../SearchDialogLibrary');

// RealState Search
var realStateResultsMapper = SearchDialogLibrary.defaultResultsMapper(realstateToSearchHit);
var realstate = SearchDialogLibrary.create('realstate', {
    multipleSelection: true,
    search: (query) => azureSearchClient.search(query).then(realStateResultsMapper),
    refiners: ['region', 'city', 'type'],
    refineFormatter: (refiners) =>
        _.zipObject(
            refiners.map(r => 'By ' + _.capitalize(r)),
            refiners)
});

bot.library(realstate);

// Maps the AzureSearch RealState Document into a SearchHit that the Search Library can use
function realstateToSearchHit(realstate) {
    return {
        key: realstate.listingId,
        title: util.format('%d bedroom, %d bath in %s, $%s',
            realstate.beds, realstate.baths, realstate.city, realstate.price.toFixed(2)),
        description: realstate.description,
        imageUrl: realstate.thumbnail
    };
}
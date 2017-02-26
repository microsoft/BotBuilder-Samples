var util = require('util');
var _ = require('lodash');
var builder = require('botbuilder');
var restify = require('restify');

/// <reference path="../SearchDialogLibrary/index.d.ts" />
var SearchLibrary = require('../SearchDialogLibrary');
var AzureSearch = require('../SearchProviders/azure-search');

// Setup Restify Server
var server = restify.createServer();
server.listen(process.env.port || process.env.PORT || 3978, function () {
    console.log('%s listening to %s', server.name, server.url);
});

// Create chat bot and listen for messages
var connector = new builder.ChatConnector({
    appId: process.env.MICROSOFT_APP_ID,
    appPassword: process.env.MICROSOFT_APP_PASSWORD
});
server.post('/api/messages', connector.listen());

// Bot with main dialog that triggers search and display its results
var bot = new builder.UniversalBot(connector, [
    function (session) {
        // Trigger the refine dialog with 'business_title' facet.
        // Then continue the dialog waterfall with the selected refiner/filter
        SearchLibrary.refine(session,
            {
                refiner: 'business_title',
                prompt: 'Hi! To get started, what kind of position are you looking for?'
            });
    },
    function (session, args) {
        // trigger Search dialog root, using the generated Query created by /refine
        var query = args.query;
        SearchLibrary.begin(session, { query: query });
    },
    function (session, args) {
        // Process selected search results
        session.send(
            'Done! For future reference, you selected these job listings: %s',
            args.selection.map(function (i) { return i.key; }).join(', '));
    }
]);

// Azure Search provider
var azureSearchClient = AzureSearch.create('azs-playground', '512C4FBA9EED64A31A1052CFE3F7D3DB', 'nycjobs');
var jobsResultsMapper = SearchLibrary.defaultResultsMapper(jobToSearchHit);

// Register Search Dialogs Library with bot
bot.library(SearchLibrary.create({
    multipleSelection: true,
    search: function (query) { return azureSearchClient.search(query).then(jobsResultsMapper); },
    refiners: ['business_title', 'agency', 'work_location']
}));

// Maps the AzureSearch Job Document into a SearchHit that the Search Library can use
function jobToSearchHit(job) {
    return {
        key: job.id,
        title: util.format('%s at %s, %s to %s', job.business_title, job.agency, job.salary_range_from.toFixed(2), job.salary_range_to.toFixed(2)),
        description: job.job_description.substring(0, 512) + '...'
    };
}
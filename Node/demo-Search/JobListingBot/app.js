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
    function(session) {
        // Trigger the refine dialog with 'business_title' facet.
        // Then continue the dialog waterfall with the selected refiner/filter
        session.beginDialog('jobs:/refine', {
            refiner: 'business_title',
            prompt: 'Hi! To get started, what kind of position are you looking for?'
        });
    },
    function(session, args) {
        // trigger Search dialog root, using the generated Query created by /refine
        var query = args.query;
        session.beginDialog('jobs:/', { query });
    },
    function (session, args) {
        // Process selected search results
        session.send(
            'Done! For future reference, you selected these job listings: %s',
            args.selection.map(i => i.key).join(', '));
    }
]);

// Azure Search provider
var AzureSearch = require('../SearchProviders/azure-search');
var azureSearchClient = AzureSearch.create('azs-playground', '512C4FBA9EED64A31A1052CFE3F7D3DB', 'nycjobs');

/// <reference path="../SearchDialogLibrary/index.d.ts" />
var SearchDialogLibrary = require('../SearchDialogLibrary');

// Jobs Listing Search
var jobsResultsMapper = SearchDialogLibrary.defaultResultsMapper(jobToSearchHit);
var realstate = SearchDialogLibrary.create('jobs', {
    multipleSelection: true,
    search: (query) => azureSearchClient.search(query).then(jobsResultsMapper),
    refiners: ['business_title', 'agency', 'work_location']
});

bot.library(realstate);

// Maps the AzureSearch Job Document into a SearchHit that the Search Library can use
function jobToSearchHit(job) {
    return {
        key: job.id,
        title: util.format('%s at %s, %s to %s', job.business_title, job.agency, job.salary_range_from.toFixed(2), job.salary_range_to.toFixed(2)),
        description: job.job_description.substring(0, 512) + '...'
    };
}
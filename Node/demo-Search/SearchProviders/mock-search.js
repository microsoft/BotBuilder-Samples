var _ = require('lodash');
var uuid = require('uuid');
var loremIpsum = require('lorem-ipsum');

var Refiners = ['region', 'type'];

function refineFormatter(refiners) {
    return _.zipObject(
        refiners.map(function (r) { return 'By ' + _.capitalize(r); }),
        refiners);
}

function search(query) {
    console.log('mock-search.query:', query);

    // mock items facets
    var region = (_.find(query.filters, ['key', 'region']) || { value: undefined }).value;
    var type = (_.find(query.filters, ['key', 'type']) || { value: undefined }).value;

    var i = (query.pageNumber - 1) * 3;
    var result = {
        facets: [
            {
                key: 'region',
                options: [
                    { value: 'US', count: 10 },
                    { value: 'Canada', count: 10 }
                ]
            },
            {
                key: 'type',
                options: [
                    { value: 'Unknown', count: 6 },
                    { value: 'A', count: 2 },
                    { value: 'B', count: 3 }
                ]
            }
        ],
        results: [
            { key: uuid.v4(), title: 'Test ' + query.searchText + ' #' + (i + 1), description: loremIpsum(), region: region, type: type },
            { key: uuid.v4(), title: 'Test ' + query.searchText + ' #' + (i + 2), description: loremIpsum(), region: region, type: type },
            { key: uuid.v4(), title: 'Test ' + query.searchText + ' #' + (i + 3), description: loremIpsum(), region: region, type: type }
        ]
    };

    return Promise.resolve(result);
}

module.exports = {
    search: search,
    refiners: Refiners,
    refineFormatter: refineFormatter
};

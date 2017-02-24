var util = require('util');
var _ = require('lodash');
var Promise = require('bluebird');
var request = require('request');

function create(serviceName, serviceKey, index) {
    // base url
    // e.g.: https://realestate.search.windows.net/indexes('listings')/docs/search.post.search?api-version=2015-02-28-Preview 
    var url = "https://" + serviceName + ".search.windows.net" +
        "/indexes('" + index + "')" +
        "/docs/search.post.search?api-version=2015-02-28-Preview";

    return {
        search: function (query) {
            return new Promise(function (resolve, reject) {
                console.log('AzureSearch.inputQuery:', query);

                // create request & azure query
                var options = {
                    url: url,
                    headers: { 'api-key': serviceKey },
                    withCredentials: false,
                    json: true,
                    body: {
                        count: false,
                        facets: query.facets || [],
                        filter: createFilterParams(query.filters),
                        queryType: 'simple',
                        scoringParameters: [],
                        search: query.searchText,
                        searchMode: 'all',
                        skip: (query.pageNumber - 1) * query.pageSize,
                        top: query.pageSize
                    }
                };

                console.log('AzureSearch.query:', options.body);
                request.post(options, function (err, httpResponse, azureResponse) {
                    if (err) {
                        return reject(err);
                    }

                    resolve({
                        results: azureResponse.value,
                        facets: getFacets(azureResponse)
                    });
                });
            });
        }
    };
}

// Helpers
function getFacets(azureResponse) {
    var rawFacets = azureResponse['@search.facets'];
    if (!rawFacets) {
        return [];
    }

    var facets = _.toPairs(rawFacets)
        .filter(function (p) { return _.isArray(p[1]); })
        .map(function (p) { return { key: p[0], options: p[1] }; });

    return facets;
}

function createFilterParams(filters) {
    if (!filters || !filters.length) {
        return '';
    }

    return filters.map(function (f) {
        return util.format('%s eq \'%s\'', f.key, escapeFilterString(f.value));
    }).join(' and ');
}

function escapeFilterString(string) {
    return string.replace(/'/g, '\'\'');
}

// Exports
module.exports = {
    create: create
};
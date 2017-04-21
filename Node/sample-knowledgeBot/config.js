module.exports = function () {
    //process.env variables defined in Azure if deployed to a web app. For testing, place IDs and Keys inline
    global.searchName = process.env.AZURE_SEARCH_NAME ? process.env.AZURE_SEARCH_NAME : "<YourSearchName>";
    global.indexName = process.env.INDEX_NAME ? process.env.AZURE_SEARCH_NAME : "<YourIndexName>";
    global.searchKey = process.env.INDEX_NAME ? process.env.AZURE_SEARCH_KEY : "<YourSearchKey>";
    
    global. queryString = 'https://' + searchName + '.search.windows.net/indexes/' + indexName + '/docs?api-key=' + searchKey + '&api-version=2015-02-28&';
}
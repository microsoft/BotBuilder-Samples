// This module provides a singleton store for saving the Site's hosted url
var siteUrl = null;
module.exports = {
    save: function (url) { siteUrl = url; },
    retrieve: function () { return siteUrl; }
};
var _ = require('lodash');
var Promise = require('bluebird');
var geobing = require('geobing');

geobing.setKey(process.env.BING_MAPS_KEY);

function parseAddress(inputAddress) {
    return new Promise(function (resolve, reject) {

        geobing.geocode(inputAddress, (err, result) => {
            if (err) {
                return reject(err);
            }

            var entities = result.resourceSets[0] ? result.resourceSets[0].resources : [];
            var addressEntities = _.filter(entities, ['entityType', 'Address']);
            resolve(addressEntities.map(extractAddress));
        });
    });
}

function extractAddress(bingEntity) {
    return  bingEntity.address.formattedAddress;
}

module.exports = {
    parseAddress: parseAddress
};
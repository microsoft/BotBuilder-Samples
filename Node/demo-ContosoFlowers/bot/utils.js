var crypto = require('crypto');
var algo = 'aes-256-ctr';
var password = 'itsasecret!';

function serializeAddress(address) {
    return JSON.stringify(address);
}

function deserializeAddress(json) {
    return JSON.parse(json);
}

function encrypt(input) {
    var cipher = crypto.createCipher(algo, password);
    var encrypted = cipher.update(input, 'utf8', 'hex');
    encrypted += cipher.final('hex');
    return encrypted;
}

function descrypt(cryptedInput) {
    var decipher = crypto.createDecipher(algo, password);
    var decryted = decipher.update(cryptedInput, 'hex', 'utf8');
    decryted += decipher.final('utf8');
    return decryted;
}

module.exports = {
    serializeAddress: (address) => encrypt(serializeAddress(address)),
    deserializeAddress: (input) => deserializeAddress(descrypt(input))
};
// For information about Base 64 URL encoding, please refer to https://en.wikipedia.org/wiki/Base64#URL_applications and https://tools.ietf.org/html/rfc4648#section-5
module.exports = buffer => buffer.toString('base64').replace(/\+/g, '-').replace(/\//g, '_').replace(/=$/, '');

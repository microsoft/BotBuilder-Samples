var base64url = require('base64url'); 

var MsPayEmulatedStripeTokenSource = 'tok_18yWDMKVgMv7trmwyE21VqO';

function parse(tokenString) {
    if (!tokenString) {
        throw new Error('PaymentToken string expected');
    }

    var tokenParts = tokenString.split('.');
    if (tokenParts.length !== 3) {
        throw new Error('Invalid PaymentToken');
    }

    var header = parseHeader(tokenParts[0]);
    var source = parseSource(tokenParts[1]);
    var signature = parseSignature(tokenParts[2]);

    return {
        header,
        source,
        signature,
        isEmulated: MsPayEmulatedStripeTokenSource === source
    };
}

function parseHeader(headerString) {
    var json = base64url.decode(headerString);
    return JSON.parse(json);
}

function parseSource(sourceString) {
    return base64url.decode(sourceString);
}

function parseSignature(signatureString) {
    return base64url.toBuffer(signatureString);
}

module.exports = {
    parse: parse,
    tokenFormat: {
        Invalid: 0,
        Error: 1,
        Stripe: 2
    }
};
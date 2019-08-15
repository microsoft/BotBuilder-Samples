const { createHash } = require('crypto');
const encodeBase64URL = require('../../../encodeBase64URL');

const createPKCECodeVerifier = require('./createPKCECodeVerifier');

// Create a PKCE code challenge.
module.exports = seed => {
  const verifier = createPKCECodeVerifier(seed);
  const hash = createHash('sha256');

  hash.update(verifier);

  return encodeBase64URL(hash.digest());
};

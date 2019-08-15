const crypto = require('crypto');

// For additional security, GitHub recommends using a "state" parameter that is only known to the servers and never passed to the client.
// We are using a well-known salt to create the "state" parameter for distributed knowledge.
module.exports = function generateOAuthState(seed, salt) {
  const hash = crypto.createHash('sha384');

  hash.update(seed);
  hash.update(salt);

  return hash.digest('hex').substr(0, 10);
}

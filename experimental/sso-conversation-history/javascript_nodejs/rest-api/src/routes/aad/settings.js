const { AAD_OAUTH_CLIENT_ID } = process.env;

// GET /api/aad/settings
// Sends the OAuth configuration to browser
module.exports = (_, res) => {
  res.json({
    authorizeURL: '/api/aad/oauth/authorize',
    clientId: AAD_OAUTH_CLIENT_ID
  });
};

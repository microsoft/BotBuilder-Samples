const { GITHUB_OAUTH_CLIENT_ID } = process.env;

// GET /api/github/settings
// Sends the OAuth configuration to browser
module.exports = (_, res) => {
  res.json({
    authorizeURL: '/api/github/oauth/authorize',
    clientId: GITHUB_OAUTH_CLIENT_ID
  });
};

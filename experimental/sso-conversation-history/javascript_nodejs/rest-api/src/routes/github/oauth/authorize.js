const generateOAuthState = require('../../../utils/generateOAuthState');
const random = require('math-random');

const {
  GITHUB_OAUTH_AUTHORIZE_URL,
  GITHUB_OAUTH_CLIENT_ID,
  GITHUB_OAUTH_REDIRECT_URI,
  GITHUB_OAUTH_SCOPE,
  GITHUB_OAUTH_STATE_SALT
} = process.env;

// GET /api/github/oauth/authorize
// Redirects the user to GitHub OAuth authorize page at https://github.com/login/oauth/authorize
module.exports = (_, res) => {
  const seed = random().toString(36).substr(2, 10);
  const state = generateOAuthState(seed, GITHUB_OAUTH_STATE_SALT);
  const params = new URLSearchParams({
    client_id: GITHUB_OAUTH_CLIENT_ID,
    redirect_uri: `${ GITHUB_OAUTH_REDIRECT_URI }?${ new URLSearchParams({ seed }) }`,
    response_type: 'code',
    scope: GITHUB_OAUTH_SCOPE,
    state
  });

  res.setHeader('location', `${ GITHUB_OAUTH_AUTHORIZE_URL }?${ params }`);
  res.send(302);
};

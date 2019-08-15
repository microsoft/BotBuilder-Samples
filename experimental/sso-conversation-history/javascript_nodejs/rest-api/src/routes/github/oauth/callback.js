const createHTMLWithPostMessage = require('../../../utils/createHTMLWithPostMessage');
const exchangeAccessToken = require('../../../exchangeAccessToken');
const generateOAuthState = require('../../../utils/generateOAuthState');

const {
  GITHUB_OAUTH_ACCESS_TOKEN_URL,
  GITHUB_OAUTH_CLIENT_ID,
  GITHUB_OAUTH_CLIENT_SECRET,
  GITHUB_OAUTH_REDIRECT_URI,
  GITHUB_OAUTH_STATE_SALT
} = process.env;

// GET /api/github/oauth/callback
// When the OAuth Provider completed, regardless of positive or negative result,
// send the result back using window.opener.postMessage.
module.exports = async (req, res) => {
  let data;

  try {
    if ('error' in req.query) {
      console.warn(req.query);

      throw new Error(`OAuth: Failed to start authorization flow due to "${ req.query.error }"`);
    }

    const { code, seed } = req.query;
    const accessToken = await exchangeAccessToken(
      GITHUB_OAUTH_ACCESS_TOKEN_URL,
      GITHUB_OAUTH_CLIENT_ID,
      GITHUB_OAUTH_CLIENT_SECRET,
      code,
      GITHUB_OAUTH_REDIRECT_URI,
      generateOAuthState(seed, GITHUB_OAUTH_STATE_SALT)
    );

    data = { access_token: accessToken };
  } catch ({ message }) {
    data = { error: message };
  }

  res.end(createHTMLWithPostMessage(data, new URL(GITHUB_OAUTH_REDIRECT_URI).origin));
};

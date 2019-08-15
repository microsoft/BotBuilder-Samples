const createHTMLWithPostMessage = require('../../../utils/createHTMLWithPostMessage');
const createPKCECodeVerifier = require('./createPKCECodeVerifier');
const exchangeAccessToken = require('../../../exchangeAccessToken');

const {
  AAD_OAUTH_ACCESS_TOKEN_URL,
  AAD_OAUTH_CLIENT_ID,
  AAD_OAUTH_CLIENT_SECRET,
  AAD_OAUTH_REDIRECT_URI,
} = process.env;

// GET /api/aad/oauth/callback
// When the OAuth Provider completed, regardless of positive or negative result,
// send the result back using window.opener.postMessage.
module.exports = async (req, res) => {
  let data;

  try {
    if ('error' in req.query) {
      console.warn(req.query);

      throw new Error(`OAuth: Failed to start authorization flow due to "${ req.query.error }"`);
    }

    const { code, state } = req.query;
    const seed = Buffer.from(state, 'base64');
    const codeVerifier = createPKCECodeVerifier(seed);

    const accessToken = await exchangeAccessToken(
      AAD_OAUTH_ACCESS_TOKEN_URL,
      AAD_OAUTH_CLIENT_ID,
      AAD_OAUTH_CLIENT_SECRET,
      code,
      AAD_OAUTH_REDIRECT_URI,
      undefined,
      codeVerifier
    );

    data = { access_token: accessToken };
  } catch ({ message }) {
    data = { error: message };
  }

  res.end(createHTMLWithPostMessage(data, new URL(AAD_OAUTH_REDIRECT_URI).origin));
};

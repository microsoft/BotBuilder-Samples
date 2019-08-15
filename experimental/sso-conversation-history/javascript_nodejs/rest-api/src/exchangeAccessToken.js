const encodeBase64URL = require('./encodeBase64URL');
const fetch = require('node-fetch');

// Exchanges an access token from OAuth provider.
module.exports = async function exchangeAccessToken(tokenURL, clientID, clientSecret, code, redirectURI, state, codeVerifier) {
  const params = new URLSearchParams({
    client_id: clientID,
    ...clientSecret ? { client_secret: clientSecret } : {},
    code,
    code_verifier: codeVerifier,
    grant_type: 'authorization_code',
    redirect_uri: redirectURI,
    state
  });

  const accessTokenRes = await fetch(
    tokenURL,
    {
      body: params.toString(),
      headers: {
        accept: 'application/json',
        'content-type': 'application/x-www-form-urlencoded'
      },
      method: 'POST'
    });

  if (!accessTokenRes.ok) {
    console.error(await accessTokenRes.json());

    throw new Error(`OAuth: Failed to exchange access token`);
  }

  const { access_token: accessToken } = await accessTokenRes.json();

  return accessToken;
}

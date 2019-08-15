const fetchJSON = require('./utils/fetchJSON');

// Fetching the GitHub profile name and id by an access token
module.exports = async function fetchGitHubProfileName(accessToken) {
  const { name, id } = await fetchJSON(
    'https://api.github.com/user',
    {
      headers: {
        authorization: `Token ${ accessToken }`
      }
    }
  );

  return { name, id };
};

const fetchJSON = require('./utils/fetchJSON');

// Fetching the Microsoft Graph user display name and id by an access token
module.exports = async function fetchProfileDisplayName(accessToken) {
  const { displayName, id } = await fetchJSON(
    'https://graph.microsoft.com/v1.0/me',
    {
      headers: {
        authorization: `Bearer ${ accessToken }`
      }
    }
  );

  return { name: displayName, id } ;
}

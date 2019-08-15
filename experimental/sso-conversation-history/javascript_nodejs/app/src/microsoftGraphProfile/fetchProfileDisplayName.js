import fetchJSON from '../utils/fetchJSON';

// Get the user display name from Microsoft Graph
// https://docs.microsoft.com/en-us/graph/api/resources/users?view=graph-rest-1.0
export default async function fetchProfileDisplayName(accessToken) {
  if (accessToken) {
    const { displayName } = await fetchJSON(
      'https://graph.microsoft.com/v1.0/me',
      {
        headers: {
          authorization: `Bearer ${ accessToken }`
        }
      }
    );

    return displayName;
  }
}

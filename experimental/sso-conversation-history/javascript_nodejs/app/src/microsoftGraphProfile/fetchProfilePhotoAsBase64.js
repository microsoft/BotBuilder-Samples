import { encode } from 'base64-arraybuffer';

// Get the profile picture from Microsoft Graph; only works with work/school accounts.
// https://docs.microsoft.com/en-us/graph/api/profilephoto-get?view=graph-rest-1.0
export default async function fetchProfilePhotoInBase64(accessToken) {
  if (accessToken) {
    const res = await fetch(
      'https://graph.microsoft.com/v1.0/me/photos/48x48/$value',
      {
        headers: {
          authorization: `Bearer ${ accessToken }`
        }
      }
    );

    if (!res.ok) {
      if (res.status === 401) {
        // Personal account does not have profile photo
        return 'images/Microsoft-Graph-64px.png';
      } else {
        throw new Error('Microsoft Graph: Failed to fetch user profile photo.');
      }
    }

    return `data:${ res.headers.get('content-type') };base64,${ encode(await res.arrayBuffer()) }`;
  }
}

import fetchJSON from '../utils/fetchJSON';

// Get a GitHub user profile, including user display name and avatar URL.
export default async function fetchUserProfile(accessToken) {
  if (accessToken) {
    return await fetchJSON(
      'https://api.github.com/user',
      {
        headers: {
          authorization: `Token ${ accessToken }`
        }
      }
    );
  }

  return {};
}

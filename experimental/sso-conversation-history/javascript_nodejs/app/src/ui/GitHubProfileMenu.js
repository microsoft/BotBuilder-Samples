import PropTypes from 'prop-types';
import React, { useCallback, useEffect, useMemo, useState } from 'react';

import './ProfileMenu.css';
import compose from '../utils/compose';
import fetchJSON from '../utils/fetchJSON';

import connectGitHubProfileAvatar from '../gitHubProfile/hoc/avatarURL';
import connectGitHubProfileName from '../gitHubProfile/hoc/name';
import connectGitHubSignInButton from '../gitHubProfile/hoc/signInButton';
import connectGitHubSignOutButton from '../gitHubProfile/hoc/signOutButton';
import GitHubProfileComposer from '../gitHubProfile/Composer';

const SETTINGS_URL = '/api/github/settings';

// We will fetch the authorize URL and client ID for the GitHub sign-in flow from the server.
// This helps decouple the server settings (e.g. client ID) from the HTML code.
async function fetchSettings() {
  try {
    const { authorizeURL, clientId } = await fetchJSON(SETTINGS_URL);

    return {
      authorizeURL,
      clientId
    };
  } catch (err) {
    throw new Error('OAuth: Failed to fetch settings');
  }
}

// The props are passed by GitHubProfileContext and its related composer.
const GitHubProfileMenu = ({
  avatarURL,
  name,
  oauthReviewAccessURL,
  onSignIn, // This will become falsy if sign in is not available, e.g. already signed in or misconfiguration
  onSignOut // This will become falsy if sign out is not available, e.g. not signed in
}) => {
  const [expanded, setExpanded] = useState(false);
  const signedIn = !!onSignOut;

  // Listen to "signin" event from the window.
  // The "signin" event is fired when the user click on the "Sign in" button in Web Chat.
  useEffect(() => {
    window.addEventListener('signin', ({ data: { provider } = {} }) => provider === 'github' && onSignIn && onSignIn());

    return () => window.removeEventListener('signin', onSignIn);
  });

  // Listen to "signout" event from the window.
  // The "signout" event is fired when the bot requests the webpage to sign out.
  useEffect(() => {
    window.addEventListener('signout', onSignOut);

    return () => window.removeEventListener('signout', onSignOut);
  });

  // CSS style for displaying avatar as background image.
  // Background image will ease handling 404 or other HTTP errors by not showing the image.
  const avatarStyle = useMemo(() => ({
    backgroundImage: `url(${ avatarURL || '/images/GitHub-Mark-64px-DDD-White.png' })`
  }), [avatarURL]);

  // In addition to running the sign in logic from OAuth context, we will also collapse the menu.
  const handleSignIn = useCallback(() => {
    onSignIn && onSignIn();
    setExpanded(false);
  }, [onSignIn]);

  // In addition to running the sign out logic from OAuth context, we will also collapse the menu.
  const handleSignOut = useCallback(() => {
    onSignOut && onSignOut();
    setExpanded(false);
  }, [onSignOut]);

  const handleToggleExpand = useCallback(() => setExpanded(!expanded), [expanded]);

  return (
    <div
      aria-expanded={ expanded }
      className="sso__profile"
    >
      <button
        aria-label="Open profile menu"
        className="sso__profileAvatar"
        onClick={ signedIn ? handleToggleExpand : handleSignIn }
        style={ avatarStyle }
      >
        { signedIn && <div className="sso__profileAvatarBadge sso__profileAvatarBadge__gitHub" /> }
      </button>
      {
        signedIn && expanded &&
          <ul className="sso__profileMenu">
            {
              name &&
                <li className="sso__profileMenuItem">
                  <span>
                    Signed in as <strong>{ name }</strong>
                  </span>
                </li>
            }
            {
              onSignOut && oauthReviewAccessURL &&
                <li className="sso__profileMenuItem">
                  <a href={ oauthReviewAccessURL } rel="noopener noreferrer" target="_blank">
                    Review access on GitHub
                  </a>
                </li>
            }
            {
              onSignOut &&
                <li className="sso__profileMenuItem">
                  <button
                    onClick={ handleSignOut }
                    type="button"
                  >
                    Sign out
                  </button>
                </li>
            }
          </ul>
      }
    </div>
  );
}

GitHubProfileMenu.defaultProps = {
  accessToken: '',
  avatarURL: '',
  name: '',
  oauthReviewAccessURL: '',
  onSignIn: undefined,
  onSignOut: undefined,
  setAccessToken: undefined
};

GitHubProfileMenu.propTypes = {
  accessToken: PropTypes.string,
  avatarURL: PropTypes.string,
  name: PropTypes.string,
  oauthReviewAccessURL: PropTypes.string,
  onSignIn: PropTypes.func,
  onSignOut: PropTypes.func,
  setAccessToken: PropTypes.func
};

// Borrowed from react-redux, "compose" is a function that combines the results of the functions.
// The functions listed here will retrieve corresponding information from React context.
const ComposedGitHubProfileMenu = compose(
  connectGitHubProfileAvatar(),
  connectGitHubProfileName(),
  connectGitHubSignInButton(({ onClick }) => ({ onSignIn: onClick })),
  connectGitHubSignOutButton(({ onClick }) => ({ onSignOut: onClick }))
)(GitHubProfileMenu);

const ConnectedGitHubProfileMenu = ({
  accessToken,
  onAccessTokenChange
}) => {
  const [oauthAuthorizeURL, setOAuthAuthorizeURL] = useState('');
  const [oauthReviewAccessURL, setOAuthReviewAccessURL] = useState('');

  useMemo(async () => {
    const { authorizeURL, clientId } = await fetchSettings();

    setOAuthAuthorizeURL(authorizeURL);

    // The OAuth review access URL is constructed based on OAuth client ID.
    setOAuthReviewAccessURL(`https://github.com/settings/connections/applications/${ clientId }`);
  }, []);

  return (
    <GitHubProfileComposer
      accessToken={ accessToken }
      oauthAuthorizeURL={ oauthAuthorizeURL }
      onAccessTokenChange={ onAccessTokenChange }
    >
      <ComposedGitHubProfileMenu
        oauthReviewAccessURL={ oauthReviewAccessURL }
      />
    </GitHubProfileComposer>
  );
};

ConnectedGitHubProfileMenu.defaultProps = {
  onSignedInChange: undefined
};

ConnectedGitHubProfileMenu.propTypes = {
  accessToken: PropTypes.string.isRequired,
  onAccessTokenChange: PropTypes.func.isRequired,
  onSignedInChange: PropTypes.func
};

export default ConnectedGitHubProfileMenu

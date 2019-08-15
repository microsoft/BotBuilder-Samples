import PropTypes from 'prop-types';
import React, { useMemo, useState } from 'react';

import compose from '../utils/compose';
import connectSignInButton from '../oauth/hoc/signInButton';
import connectSignOutButton from '../oauth/hoc/signOutButton';
import fetchUserProfile from './fetchUserProfile';
import GitHubProfileContext from './Context';
import OAuthComposer from '../oauth/Composer';

// Composer is a React component with a React context
const GitHubProfileComposer = ({
  accessToken,
  children,
  onSignIn,
  onSignOut
}) => {
  const [avatarURL, setAvatarURL] = useState('');
  const [name, setName] = useState('');

  // If access token change, refresh the profile name and picture from GitHub.
  useMemo(async () => {
    const { avatar_url: avatarURL, name } = await fetchUserProfile(accessToken);

    setAvatarURL(avatarURL);
    setName(name);
  }, [accessToken]);

  // Build a new React context object if anything has changed.
  const context = useMemo(() => ({
    avatarURL,
    name,
    onSignIn,
    onSignOut
  }), [
    avatarURL,
    name,
    onSignIn,
    onSignOut
  ]);

  return (
    <GitHubProfileContext.Provider value={ context }>
      { children }
    </GitHubProfileContext.Provider>
  );
}

GitHubProfileComposer.defaultProps = {
  accessToken: '',
  children: undefined,
  onSignIn: undefined,
  onSignOut: undefined
};

GitHubProfileComposer.propTypes = {
  accessToken: PropTypes.string,
  children: PropTypes.any,
  onSignIn: PropTypes.func,
  onSignOut: PropTypes.func
};

// Hoist the functionality from generic OAuth composer to GitHub composer.
// The generic OAuth composer provides sign in and sign out logic.
// The GitHub composer provides profile-related information.
const ComposedGitHubProfileComposer = compose(
  connectSignInButton(({ onClick }) => ({ onSignIn: onClick })),
  connectSignOutButton(({ onClick }) => ({ onSignOut: onClick }))
)(GitHubProfileComposer)

// This is the exported React component, provide basic UI-less functionality to its descendants.
const ConnectedGitHubProfileComposer = ({
  accessToken,
  children,
  oauthAuthorizeURL,
  onAccessTokenChange
}) =>
  <OAuthComposer
    accessToken={ accessToken }
    oauthAuthorizeURL={ oauthAuthorizeURL }
    onAccessTokenChange={ onAccessTokenChange }
  >
    <ComposedGitHubProfileComposer accessToken={ accessToken }>
      { children }
    </ComposedGitHubProfileComposer>
  </OAuthComposer>

export default ConnectedGitHubProfileComposer

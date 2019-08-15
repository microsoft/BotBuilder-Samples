import PropTypes from 'prop-types';
import React, { useMemo } from 'react';

import Context from './Context';
import openCenter from '../utils/openCenter';

// Composer will prepare a React context object to use by consumer.
// The composer and context used here is very generic and will be extended by GitHub and Microsoft Graph-specific composer and context.
const Composer = ({
  accessToken,
  children,
  oauthAuthorizeURL,
  onAccessTokenChange = () => {},
  onError = () => {}
}) => {
  const context = useMemo(() => ({
    onSignIn: accessToken ? undefined : () => {
      // When context.onSignIn is called, we will:
      // 1. Open a new popup and navigate to the URL stored in "oauthAuthorizeURL" prop.
      // 2. OAuth provider will call our OAuth callback page.
      // 3. The callback page uses "postMessage" to inform the parent window (this window) about the access token through the "message" event.
      const handleMessage = ({ data, origin }) => {
        const oauthAuthorizeLocation = new URL(oauthAuthorizeURL, window.location.href);

        if (origin !== oauthAuthorizeLocation.origin) {
          return;
        }

        try {
          // The counterpart of URLSearchParams used here can be found in /rest-api/src/routes/{aad|github}/oauth/callback.js.
          const params = new URLSearchParams(data);

          if (params.has('error')) {
            const error = params.get('error');

            console.error(error);

            onError(new Error(error));
          } else {
            onAccessTokenChange(params.get('access_token'));
          }
        } catch (err) {
          console.warn(err);

          onError(err);
        } finally {
          window.removeEventListener('message', handleMessage);
        }
      };

      window.addEventListener('message', handleMessage);
      openCenter(oauthAuthorizeURL, 'oauth', 360, 640);
    },

    // For sign out, we simply remove the token.
    // Some OAuth providers support an optional logout URL.
    // When the user signs out from the provider page, the logout URL for the specific application is being called.
    onSignOut: accessToken ? () => onAccessTokenChange('') : undefined
  }), [accessToken, oauthAuthorizeURL, onAccessTokenChange, onError]);

  return (
    <Context.Provider value={ context }>
      { children }
    </Context.Provider>
  );
};

Composer.defaultProps = {
  accessToken: '',
  children: undefined,
  onAccessTokenChange: undefined,
  onError: undefined
};

Composer.propTypes = {
  accessToken: PropTypes.string,
  children: PropTypes.any,
  onAccessTokenChange: PropTypes.func,
  onError: PropTypes.func
};

export default Composer

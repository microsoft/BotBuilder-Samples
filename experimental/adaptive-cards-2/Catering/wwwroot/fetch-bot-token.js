'use strict';

window.App || (window.App = {});

window.App.fetchBotToken = async () => {
  const res = await fetch('https://webchat-mockbot.azurewebsites.net/directline/token', { method: 'POST' });

  if (!res.ok) {
    return alert('Failed to retrieve token.');
  }

  const { token } = await res.json();

  return token;
};

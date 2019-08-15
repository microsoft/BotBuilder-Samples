const fetch = require('node-fetch');

module.exports = async function fetchJSON(url, options) {
  const res = await fetch(url, {
    ...options,
    headers: {
      ...options.headers,
      accept: 'application/json'
    }
  });

  if (!res.ok) {
    throw new Error(`Failed to fetch JSON from server due to ${ res.status }`);
  }

  return await res.json();
}

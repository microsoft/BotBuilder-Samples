const fetch = require('node-fetch');

// Helper function for fetching network resource as JSON
module.exports = async function fetchJSON(url, options) {
  const res = await fetch(url, {
    ...options,
    headers: {
      ...options.headers,
      accept: 'application/json',
      'Content-Type': 'application/json'
    },
    body: JSON.stringify(options.body)
  });

  if (!res.ok) {
    throw new Error(`Failed to fetch JSON from server due to ${ res.status }`);
  }

  return await res.json();
}

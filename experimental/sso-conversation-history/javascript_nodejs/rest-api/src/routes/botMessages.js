const { URL } = require('url');
const httpProxy = require('http-proxy');

// To simplify deployment for our demo, we aggregate web and bot server into a single endpoint.
// If the HTTP POST is going to /api/messages, we will reverse-proxy the request to the bot server at http://localhost:3978/.

const { PROXY_BOT_URL } = process.env;

if (PROXY_BOT_URL) {
  const proxy = httpProxy.createProxyServer();

  console.log(`Will redirect /api/messages to ${ new URL('api/messages', PROXY_BOT_URL).href }`);

  module.exports = (req, res) => {
    proxy.web(req, res, { target: PROXY_BOT_URL });
  };
} else {
  let warningShown;

  module.exports = (_, res) => {
    if (!warningShown) {
      warningShown = true;
      console.warn('PROXY_BOT_URL is not set, we are not reverse-proxying /api/messages.');
    }

    res.send(502);
  };
}

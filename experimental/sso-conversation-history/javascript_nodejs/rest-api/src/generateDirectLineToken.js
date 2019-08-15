const fetchJSON = require("./utils/fetchJSON");
const uuidv4 = require('uuid/v4');

// Generates a new Direct Line token given the secret.
module.exports = async function generateDirectLineToken(secret) {
  // You should consider using Enhanced Direct Line Authentication to protect the user ID.
  // https://blog.botframework.com/2018/09/25/enhanced-direct-line-authentication-features/

  const uuid = 'dl_' + uuidv4();

  const { token } = await fetchJSON(
    "https://directline.botframework.com/v3/directline/tokens/generate",
    {
      headers: {
        authorization: `Bearer ${secret}`,
        'Content-Type': 'application/json',
      },
      method: "POST",
      body: { user: { id: uuid }}
    }
  );
  
  return { token: token, id: uuid };
};

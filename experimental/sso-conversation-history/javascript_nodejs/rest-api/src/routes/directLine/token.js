const generateDirectLineToken = require('../../generateDirectLineToken');

const { DIRECT_LINE_SECRET } = process.env;

// GET /api/directline/token
// Generates a new Direct Line token
module.exports = async (_, res) => {
  res.json({ data: await generateDirectLineToken(DIRECT_LINE_SECRET) });
};

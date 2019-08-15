require('dotenv').config();

// Checks for required environment variables.
[
  'MICROSOFT_APP_ID',
  'MICROSOFT_APP_PASSWORD',
  'DB_SERVICE_ENDPOINT',
  'AUTH_KEY',
  'ACTIVITY_LOGS_DATABASE',
  'ACTIVITY_LOGS_COLLECTION',
  'BOT_STATE_DATABASE',
  'BOT_STATE_COLLECTION',

].forEach(name => {
  if (!process.env[name]) {
    throw new Error(`Environment variable ${ name } must be set.`);
  }
});

const { createServer } = require('restify');
const createUserStates = require('./createUserStates');
const { ConversationHistoryBot } = require('./conversationHistoryBot');
const createBotAdapter = require('./createBotAdapter');

const PORT = process.env.port || process.env.PORT || 3978;
const userStates = createUserStates();
const adapter = createBotAdapter(userStates);
const bot = new ConversationHistoryBot(userStates);
const server = createServer();

server.post('/api/messages', (req, res) => {
   adapter.processActivity(req, res, async (context) => await bot.run(context));
});

server.listen(PORT, () => {
  console.log(`Bot is now listening to port ${ PORT }`);
});

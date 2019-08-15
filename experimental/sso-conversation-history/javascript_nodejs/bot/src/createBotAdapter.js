const { BotFrameworkAdapter } = require('botbuilder');
const createTranscriptLogger = require('./utils/createTranscriptLogger');
const createHistoryMiddleware = require('./utils/createHistoryMiddleware');

const {
  MICROSOFT_APP_ID,
  MICROSOFT_APP_PASSWORD,
} = process.env;

module.exports = function createBotAdapter(userStates) {
  const adapter = new BotFrameworkAdapter({
    appId: MICROSOFT_APP_ID,
    appPassword: MICROSOFT_APP_PASSWORD
  });

  // Catch-all for errors.
  adapter.onTurnError = async (context, error) => {
    // Write out errors to console log.
    // NOTE: In production environment, you should consider logging this to Azure
    //       application insights.
    console.error(`\n [onTurnError]:`);
    console.error(error);
    // Send a message to the user.
    await context.sendActivity(`Oops. Something went wrong!`);
  };

  const historyMiddleware = createHistoryMiddleware(userStates.userState);
  adapter.use(historyMiddleware);

  const logger = createTranscriptLogger();

  adapter.use(logger);
  return adapter;
};

const { ActivityHandler, TurnContext } = require('botbuilder');
const { findBestMatch } = require('string-similarity');

const createBotAdapter = require('./createBotAdapter');
const fetchGitHubProfileName = require('./fetchGitHubProfileName');
const fetchMicrosoftGraphProfileName = require('./fetchMicrosoftGraphProfileName');
const createConversationHistory = require('./utils/createConversationHistory');

const QUESTIONS = {
  bye1: 'bye',
  bye2: 'goodbye',
  hello1: 'hello',
  hello2: 'hi',
  order: 'where are my orders',
  time: 'what time is it'
};

const SUGGESTED_ACTIONS = {
  suggestedActions: {
    actions: [{
      type: 'imBack',
      value: 'What time is it?'
    }, {
      type: 'imBack',
      value: 'Where are my orders?'
    }],
    to: []
  }
};

const SIGN_IN_MESSAGE = {
  type: 'message',
  attachments: [{
    content: {
      buttons: [{
        title: 'Sign into Azure Active Directory',
        type: 'openUrl',
        value: 'about:blank#sign-into-aad'
      }, {
        title: 'Sign into GitHub',
        type: 'openUrl',
        value: 'about:blank#sign-into-github'
      }],
      text: 'Please sign in so I can help tracking your orders.'
    },
    contentType: 'application/vnd.microsoft.card.hero',
  }]
};

// For simplicity, we are using "string-similarity" package to guess what the user asked.
function guessQuestion(message) {
  const match = findBestMatch(message, Object.values(QUESTIONS));

  if (match.bestMatch.rating > .5) {
    return Object.keys(QUESTIONS)[match.bestMatchIndex];
  }
}

async function setUserIdAndSendHistory(context, profile, userState, authProvider){
    // Store the id from the auth provider in channelData so it can be used
    // in history logging and retrieval
    context.activity.channelData = { ...context.activity.channelData, authUserId: profile.id };

    const history = createConversationHistory();
    
    // Update history record's userId so retrieving future history will also retrieve
    // messages communicated to the bot before being signed in
    await history.updateUserId(context, userState, context.activity.from.id, profile.id);

    // When the profile is fetched, send a welcome message and history based on
    // auth provider id.
    await context.sendActivity({
      text: `Welcome back, ${profile.name} id: ${profile.id} (via ${authProvider}).`,
      ...SUGGESTED_ACTIONS
    });

    await history.sendUserHistory(context, userState, true);
}

module.exports = (userStates) => {
  this.userState = userStates.userState;
  this.authUserState = userStates.authUserState;

  const bot = new ActivityHandler();

  // Handler for "event" activity
  bot.onEvent(async (context, next) => {
    const { activity: { channelData, name } } = context;

    // When we receive an event activity of "oauth/signin", set the access token to conversation state.
    if (name === 'oauth/signin') {
      const { oauthAccessToken, oauthProvider } = channelData;

      await context.sendActivity({ type: 'typing' });

      switch (oauthProvider) {
        case 'github':
          await fetchGitHubProfileName(oauthAccessToken).then(async profile => {
            await setUserIdAndSendHistory(context, profile, this.userState, 'GitHub');
          });

          break;

        case 'microsoft':
          await fetchMicrosoftGraphProfileName(oauthAccessToken).then(async profile => {
            await setUserIdAndSendHistory(context, profile, this.userState, 'Azure AD');
          });

          break;
      }
    } else if (name === 'oauth/signout') {
      // If we receive the event activity with no access token inside, this means the user is signing out from the website.
      await context.sendActivity('See you later!');
    } else if(name === 'GetUserHistory'){
      await createConversationHistory().sendUserHistory(context, this.userState, context.activity.channelData.initialHistory);
    }

    await next();
  });

  // Handler for "message" activity
  bot.onMessage(async (context, next) => {
    const { activity: { channelData: { oauthAccessToken } = {}, text } } = context;

    const match = guessQuestion(text);

    if (/^hello\d+$/.test(match)) {
      // When the user say, "hello" or "hi".
      await context.sendActivity({
        text: 'Hello there. What can I help you with?',
        ...SUGGESTED_ACTIONS
      });
    } else if (/^bye\d+$/.test(match)) {
      // When the user say "bye" or "goodbye".
      await context.sendActivity({
        name: 'oauth/signout',
        type: 'event'
      });
    } else if (match === 'time') {
      // When the user say "what time is it".
      const now = new Date();

      await context.sendActivity({
        text: `The time is now ${now.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}. What can I do to help?`,
        ...SUGGESTED_ACTIONS
      });
    } else if (
      match === 'order'
    ) {
      // When the user says "where are my orders".

      if (oauthAccessToken) {
        // Tell them they have a package if they are signed in.
        await context.sendActivity({
          text: 'There is a package arriving later today.',
          ...SUGGESTED_ACTIONS
        });
      } else {
        // Send them a sign in card if they are not signed in.
        await context.sendActivity(SIGN_IN_MESSAGE);
      }
    } else {

      if (!isNaN(text)) {
        if(!oauthAccessToken){
          // Deep copy, so we can change the .text
          let signInMessage = JSON.parse(JSON.stringify(SIGN_IN_MESSAGE));
          signInMessage.attachments[0].content.text = 'Please sign in to keep a running total.';
          await context.sendActivity(signInMessage);
        }
        else{
          // If the user sent a number, add it to a running total.
          const runningTotalProperty = this.authUserState.createProperty("RunningTotal");
          let total = await runningTotalProperty.get(context, 0);
          total += Number(text);
          await runningTotalProperty.set(context, total);

          await context.sendActivity({
            text: 'Running total:' + total
          });
        }
      } else {
        // Unknown phrases.
        await context.sendActivity({
          text: 'Sorry, I don\'t know what you mean.',
          ...SUGGESTED_ACTIONS
        });
      }
    }

    await next();
  });

  bot.onDialog(async (context, next) => {
    // Save any state changes. The load happened during the execution of the Dialog.
    await this.userState.saveChanges(context, false);
    await this.authUserState.saveChanges(context, false);

    // By calling next() you ensure that the next BotHandler is run.
    await next();
  });

  return bot;
};

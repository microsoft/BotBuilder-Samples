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
  
module.exports = {
    QUESTIONS,
    SUGGESTED_ACTIONS,
    SIGN_IN_MESSAGE,
};
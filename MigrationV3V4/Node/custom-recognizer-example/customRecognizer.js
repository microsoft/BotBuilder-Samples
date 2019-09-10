class CustomRecognizer {

    construction() {}

    recognize(text) {
        const recognizerResult = {
            text: text,
            intents: []
        };
        const regex = /(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|"(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])/;
        const isEmailAddress = regex.test(text);
        if (isEmailAddress) {
            recognizerResult.intents.push({
                SearchForEmailAddress: {
                    score: 60
                }
            });
            recognizerResult.intents.push({
                SendEmail: {
                    score: 90
                }
            });
            recognizerResult.intents.push({
                OpenEmailAccount: {
                    score: 30
                }
            });
        }
        return recognizerResult;
    }

    getTopIntent(recognizerResult) {
      const sortedResults = recognizerResult.intents.sort((a, b) => {
          const keyA = Object.keys(a);
          const keyB = Object.keys(b);
          return (a[keyA].score > b[keyB].score) ? -1 : 1;
      });
      const topIntent = Object.keys(sortedResults[0])[0];
      return topIntent;
    }
    
}

module.exports = CustomRecognizer;
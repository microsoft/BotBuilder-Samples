# Speech To Text Bot Sample

A sample bot that illustrates how to use the Microsoft Cognitive Services Bing Speech API to analyze an audio file and return the text.

[![Deploy to Azure][Deploy Button]][Deploy Node/SpeechToText]

[Deploy Button]: https://azuredeploy.net/deploybutton.png
[Deploy Node/SpeechToText]: https://azuredeploy.net

### Prerequisites

The minimum prerequisites to run this sample are:
* Latest Node.js with NPM. Download it from [here](https://nodejs.org/en/download/).
* The Bot Framework Emulator. To install the Bot Framework Emulator, download it from [here](https://emulator.botframework.com/). Please refer to [this documentation article](https://github.com/microsoft/botframework-emulator/wiki/Getting-Started) to know more about the Bot Framework Emulator.
* Bing Speech Api Key. You can obtain one from [Microsoft Cognitive Services Subscriptions Page](https://www.microsoft.com/cognitive-services/en-us/subscriptions?productId=/products/Bing.Speech.Preview).
* **[Recommended]** Visual Studio Code for IntelliSense and debugging, download it from [here](https://code.visualstudio.com/) for free.
* This sample currently uses a free trial Microsoft Cognitive service key with limited QPS. Please subscribe to Bing Speech Api services [here](https://www.microsoft.com/cognitive-services/en-us/subscriptions) and update the `MICROSOFT_SPEECH_API_KEY` key in [.env](.env) file to try it out further.

### Usage

Attach an audio file (wav format).

### Code Highlights

Microsoft Cognitive Services provides a Speech Recognition API to convert audio into text. Check out [Bing Speech API](https://www.microsoft.com/cognitive-services/en-us/speech-api) for a complete reference of Speech APIs available. In this sample we are using the Speech Recognition API using the [REST API](https://www.microsoft.com/cognitive-services/en-us/Speech-api/documentation/API-Reference-REST/BingVoiceRecognition).

The main components are:

* [speech-service.js](speech-service.js): is the core component illustrating how to call the Bing Speech RESTful API.
* [app.js](app.js): is the bot service listener receiving messages from the connector service and passing them down to speech-service.js and doing text processing on them.

In this sample we are using the API to get the text and send it back to the user. Check out the use of the `speechService.getTextFromAudioStream(stream)` method in [app.js](app.js#L35-L44).

````JavaScript
if (hasAudioAttachment(session)) {
    var stream = getAudioStreamFromMessage(session.message);
    speechService.getTextFromAudioStream(stream)
        .then(function (text) {
            session.send(processText(text));
        })
        .catch(function (error) {
            session.send('Oops! Something went wrong. Try again later.');
            console.error(error);
        });
}
````

And here is the implementation of `speechService.getTextFromAudioStream(stream)` in [speech-service.js](speech-service.js)

````JavaScript
exports.getTextFromAudioStream = function (stream) {
    return new Promise(
        function (resolve, reject) {
            if (!speechApiAccessToken) {
                try {
                    authenticate(function () {
                        streamToText(stream, resolve, reject);
                    });
                } catch (exception) {
                    reject(exception);
                }
            } else {
                streamToText(stream, resolve, reject);
            }
        }
    );
};
````

### Outcome

You will see the following when connecting the Bot to the Emulator and send it an audio file and a command:

Input:

["What's the weather like?"](audio/whatstheweatherlike.wav)

Output:

![Sample Outcome](images/outcome-emulator.png)

### More Information

To get more information about how to get started in Bot Builder for Node and Microsoft Cognitive Services Bing Speech API please review the following resources:
* [Bot Builder for Node.js Reference](https://docs.microsoft.com/en-us/bot-framework/nodejs/)
* [Microsoft Cognitive Services Bing Speech API](https://www.microsoft.com/cognitive-services/en-us/speech-api)

# Roller Sample Skill

A simple dice rolling skill/bot that's been optimized for speech enabled channels. 

[![Deploy to Azure][Deploy Button]][Deploy Node/RollerSkill]

[Deploy Button]: https://azuredeploy.net/deploybutton.png
[Deploy Node/RollerSkill]: https://azuredeploy.net

### Prerequisites

The minimum prerequisites to run this sample are:
* Latest Node.js with NPM. Download it from [here](https://nodejs.org/en/download/).
* The Bot Framework Emulator. To install the Bot Framework Emulator, download it from [here](https://emulator.botframework.com/). Please refer to [this documentation article](https://github.com/microsoft/botframework-emulator/wiki/Getting-Started) to know more about the Bot Framework Emulator.
* **[Recommended]** Visual Studio Code for IntelliSense and debugging, download it from [here](https://code.visualstudio.com/) for free.

### Code Highlights

The sample showcases the use of new features designed specifically for speech:

* **IMessage.speak**: Lets include Speech Synthesis Markup Language (SSML) in your bots responses to control what the bot says, in addition to what it shows. A small [utility module](ssml.js) is included to simplify building up your bots SSML based responses.
* **IMessage.inputHint**: Used to provide a speech based client with hints as to how they should manage the microphone. A hint of `InputHints.ignoringInput` can be sent to tell the client that the bot will be sending more messages and should wait to open the mic. The `InputHints.acceptingInput` hint means that the bot is finished speaking and ready to receive additional requests from the user but the mic should be closed. And the `InputHints.expectingInput` hint indicates that the bot is expecting an answer to a prompt and the mic should be left open. In general, the SDK will send these hints for you automatically so you don't have to worry too much about them.
* **Session.say()**: A new method that can be called in place of `Session.send()` and includes additional parameters for sending SSML output, as well as attachments like cards. 
* **IPromptOptions.speak & retrySpeak**: The built-in prompts have all been updated to support sending SSML as part of the prompts output.
* **Prompts.choice() synonyms**: The built-in choice() prompt has been updated to support passing in synonyms which allows for more flexibility recognition wise.
* **Other Prompt Improvements**: A number of other improvements have been made to the built-in prompts to add with building bots that work better with speech recognition.

### More Information

To get more information about how to get started in Bot Builder for Node please review the following resources:
* [Bot Builder for Node.js Reference](https://docs.microsoft.com/en-us/bot-framework/nodejs/)
* [SSML Language Reference](https://msdn.microsoft.com/en-us/library/hh378377(v=office.14).aspx)

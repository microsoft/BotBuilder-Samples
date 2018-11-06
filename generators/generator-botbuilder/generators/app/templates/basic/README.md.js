# <%= botName %>
<%= description %>

This bot has been created using [Microsoft Bot Framework][1].

This samples shows how to:
- Use [LUIS][2] to implement core AI capabilities
- Implement a multi-turn conversation using Dialogs
- Handle user interruptions for such things as Help or Cancel
- Prompt for and validate requests for information from the user
- Demonstrate how to handle any unexpected errors


## To run this bot
- In a terminal,
  ```bash
  cd <%= botName %>
  ```
- Install modules
  ```bash
  npm install
  ```
- Create [required services][3]
- Run the bot
  ```bash
  <%= runCmd %>
  ```

## Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator][4] is a desktop application that allows bot developers to test and debug their bots running locally or running remotely in Microsoft Azure.

- Install the Bot Framework Emulator from [here][5]

### Connect to the bot using Bot Framework Emulator v4
- Launch Bot Framework Emulator
- File -> Open Bot Configuration and navigate to `<%= botName %>` folder
- Select `<%= botName %>.bot` file

## Deploy this bot to Azure
See [DEPLOYMENT.md][3] to learn more about deploying this bot to Azure and using the CLI tools to build the LUIS models this bot depends on.

## Further Reading
- [Bot Framework Documentation][6]
- [Bot basics][7]
- [Activity processing][8]
- [LUIS][2]
- [Prompt Types][9]
- [Azure Bot Service Introduction][10]
- [Channels and Bot Connector Service][11]
- [QnA Maker][12]

## Additional Resources

### Dependencies

- **[Restify][13]** Used to host the web service for the bot, and for making REST calls
- **[dotenv][14]** Used to manage environmental variables

### Project Structure
`index.<%= extension %>` references the bot and starts a Restify server. `bot.<%= extension %>` loads the main dialog router and determines how activities are processed.

### Configuring the bot

The generator created a `.env` file with the two necessary keys `botFilePath` and `botFileSecret`.  The `botFilePath` key is set to `<%= botName %>.bot`.  All of the services and their respective configuration settings are stored in the .bot file.
  - For Azure Bot Service bots, you can find the `botFileSecret` under application settings.
  - It is recommended that you encrypt your bot file before you commit it to your source control system and/or before you deploy your bot to Azure or similar hosting service.  There are two ways to encrypt your `<%= botName %>.bot` file.  You can use [MSBot CLI][15] to encrypt your bot file or you can use [Bot Framework Emulator **V4**][16] to encrypt your bot file.  Both options will product a `botFileSecret` for you.  You will need to remember this in order to decrypt your .bot file.

### Running the bot

```
<%= runCmd %>
```
### Developing the bot

```
<%= watchCmd %>
```

[1]: https://dev.botframework.com
[2]: https://luis.ai
[3]: ./deploymentScripts/DEPLOYMENT.md
[4]: https://github.com/microsoft/botframework-emulator
[5]: https://aka.ms/botframework-emulator
[6]: https://docs.botframework.com
[7]: https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0
[8]: https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-activity-processing?view=azure-bot-service-4.0
[9]: https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-prompts?view=azure-bot-service-4.0&tabs=javascript
[10]: https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0
[11]: https://docs.microsoft.com/en-us/azure/bot-service/bot-concepts?view=azure-bot-service-4.0
[12]: https://qnamaker.ai
[13]: http://restify.com
[14]: https://github.com/motdotla/dotenv
[15]: https://github.com/microsoft/botbuilder-tools
[16]: https://github.com/microsoft/botframework-emulator

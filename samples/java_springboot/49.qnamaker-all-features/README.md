# QnA Maker

Bot Framework v4 QnA Maker bot sample. This sample shows how to integrate Multiturn and Active learning in a QnA Maker bot with Java. Click [here][72] to know more about using follow-up prompts to create multiturn conversation. To know more about how to enable and use active learning, click [here][71].

This bot has been created using [Bot Framework](https://dev.botframework.com), it shows how to create a bot that uses the [QnA Maker Cognitive AI](https://www.qnamaker.ai) service.

The [QnA Maker Service](https://www.qnamaker.ai) enables you to build, train and publish a simple question and answer bot based on FAQ URLs, structured documents or editorial content in minutes. In this sample, we demonstrate how to use the QnA Maker service to answer questions based on a FAQ text file used as input.

## Concepts introduced in this sample
The [QnA Maker Service][7] enables you to build, train and publish a simple question and answer bot based on FAQ URLs, structured documents or editorial content in minutes.
In this sample, we demonstrate
- how to use the Active Learning to generate suggestions for knowledge base.
- how to use the Multiturn experience for the knowledge base.

# Prerequisites
- Follow instructions [here](https://docs.microsoft.com/en-us/azure/cognitive-services/qnamaker/how-to/set-up-qnamaker-service-azure) to create a QnA Maker service.
- Follow instructions [here](https://docs.microsoft.com/en-us/azure/cognitive-services/qnamaker/how-to/multiturn-conversation) to create multiturn experience.
- Follow instructions [here](https://docs.microsoft.com/en-us/azure/cognitive-services/qnamaker/quickstarts/create-publish-knowledge-base) to import and publish your newly created QnA Maker service.
- Update [application.properties](src/main/resources/application.properties) with your kbid (KnowledgeBase Id), endpointKey and endpointHost. You may also change the default answer by updating `DefaultAnswer` (optional) field. QnA knowledge base setup and application configuration steps can be found [here](https://aka.ms/qna-instructions).
- (Optional) Follow instructions [here](https://github.com/microsoft/botframework-cli/tree/main/packages/qnamaker) to set up the
  QnA Maker CLI to deploy the model.


### Create a QnAMaker Application to enable QnA Knowledge Bases

QnA knowledge base setup and application configuration steps can be found [here](https://aka.ms/qna-instructions).

# Configure Cognitive Service Model
- Create a Knowledge Base in QnAMaker Portal.
- Import "smartLightFAQ.tsv" file, in QnAMaker Portal.
- Save and Train the model.
- Create Bot from Publish page.
- Test bot with Web Chat.
- Capture values of settings like "QnAAuthKey" from "Configuration" page of created bot, in Azure Portal.
- Updated application.properties with values as needed.
- Use value of "QnAAuthKey" for setting "QnAEndpointKey".
- Capture KnowledgeBase Id, HostName and EndpointKey current published app

# Try Active Learning
- Once your QnA Maker service is up and you have published the sample KB, try the following queries to trigger the Train API on the bot.
- Sample query: "light"
- You can observe that, Multiple answers are returned with high score.

# Try Multi-turn prompt
- Once your QnA Maker service is up and you have published the sample KB, try the following queries to trigger the Train API on the bot.
- Sample query: "won't turn on"
- You can notice a prompt, included as part of  answer to query.

## To try this sample

- From the root of this project folder:
    - Build the sample using `mvn package`
    - Run it by using `java -jar .\target\bot-qnamaker-all-features-sample.jar`

##### Microsoft Teams channel group chat fix
- Goto `QnABot.java`
- Add References
- Modify `onTurn` function as:
    ```java
    @Override
    public CompletableFuture<Void> onTurn(TurnContext turnContext) {
        // Teams group chat
        if (turnContext.getActivity().getChannelId().equals(Channels.MSTEAMS)) {
            turnContext.getActivity().setText(turnContext.getActivity().removeRecipientMention());
        }

        return super.onTurn(turnContext)
            // Save any state changes that might have occurred during the turn.
            .thenCompose(turnResult -> conversationState.saveChanges(turnContext, false))
            .thenCompose(saveResult -> userState.saveChanges(turnContext, false));
    }
    ```

## Testing the bot using Bot Framework Emulator

[Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator version 4.3.0 or greater from [here](https://github.com/Microsoft/BotFramework-Emulator/releases)

### Connect to the bot using Bot Framework Emulator

- Launch Bot Framework Emulator
- File -> Open Bot
- Enter a Bot URL of `http://localhost:3978/api/messages`

# Deploy the bot to Azure
See [Deploy your Java bot to Azure][50] for instructions.

The deployment process assumes you have an account on Microsoft Azure and are able to log into the [Microsoft Azure Portal][60].

If you are new to Microsoft Azure, please refer to [Getting started with Azure][70] for guidance on how to get started on Azure.

# Further reading
- [Active learning Documentation][40]
- [Bot Framework Documentation][80]
- [Bot Basics][90]
- [Azure Bot Service Introduction][100]
- [Azure Bot Service Documentation][110]
- [QnA Maker CLI][170]
- [BF-CLI][130]
- [Azure Portal][140]
- [Spring Boot][160]

[1]: https://dev.botframework.com
[4]: https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0
[5]: https://github.com/microsoft/botframework-emulator
[6]: https://aka.ms/botframeworkemulator
[7]: https://www.qnamaker.ai

[40]: https://docs.microsoft.com/en-us/azure/cognitive-services/qnamaker/how-to/improve-knowledge-base
[50]: https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-deploy-azure?view=azure-bot-service-4.0
[60]: https://portal.azure.com
[70]: https://azure.microsoft.com/get-started/
[80]: https://docs.botframework.com
[90]: https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0
[100]: https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0
[110]: https://docs.microsoft.com/en-us/azure/bot-service/?view=azure-bot-service-4.0
[120]: https://docs.microsoft.com/en-us/cli/azure/?view=azure-cli-latest
[130]: https://github.com/microsoft/botframework-cli
[140]: https://portal.azure.com
[150]: https://www.luis.ai
[160]: https://spring.io/projects/spring-boot
[170]: https://github.com/microsoft/botframework-cli/tree/main/packages/qnamaker

[71]: https://docs.microsoft.com/en-us/azure/cognitive-services/qnamaker/how-to/improve-knowledge-base
[72]: https://docs.microsoft.com/en-us/azure/cognitive-services/qnamaker/how-to/multiturn-conversation

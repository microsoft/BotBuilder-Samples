## Overview

This repository contains samples for the Microsoft **Bot Builder V4 SDK** - [dotnet SDK](https://github.com/Microsoft/botbuilder-dotnet), [JS SDK](https://github.com//microsoft/botbuilder-js).

Samples for the Bot Builder V3 SDK are available [here](https://github.com/Microsoft/BotBuilder-Samples/tree/v3-sdk-samples).

## Resources
Bot Builder provides the most comprehensive experience for building conversation applications and includes the following SDKs and tools:

- Bot Builder V4 SDK
    - [**C#** (stable release)](https://github.com/microsoft/botbuilder-dotnet)
    - [**JS** (stable release)](https://github.com/microsoft/botbuilder-js)
    - [**Java** (preview release)](https://github.com/microsoft/botbuilder-java)
    - [**Python** (preview release)](https://github.com/microsoft/botbuilder-python).
- Bot Framework Emulator
    - [Bot Framework **V4 Emulator**](https://github.com/microsoft/botframework-emulator).
- Bot Builder CLI tools (**stable release**)
    - [Chatdown CLI](https://github.com/Microsoft/botbuilder-tools/tree/master/packages/Chatdown)
    - [MSBot CLI](https://github.com/Microsoft/botbuilder-tools/tree/master/packages/MSBot)
    - [Ludown CLI](https://github.com/Microsoft/botbuilder-tools/tree/master/packages/Ludown)
    - [LUIS CLI](https://github.com/Microsoft/botbuilder-tools/tree/master/packages/LUIS)
    - [QnAMaker CLI](https://github.com/Microsoft/botbuilder-tools/tree/master/packages/QnAMaker)
    - [Dispatch CLI](https://github.com/Microsoft/botbuilder-tools/tree/master/packages/Dispatch)
    - [LuisGen CLI](https://github.com/Microsoft/botbuilder-tools/tree/master/packages/LUISGen)
- Bot Framework webchat
    - [Available here](https://github.com/microsoft/botframework-webchat)

Please see [here](https://aka.ms/BotBuilderOverview) for an overview of the end-to-end bot development workflow. To get started, you can create a bot with [Azure Bot Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-quickstart?view=azure-bot-service-4.0). Click [here](https://account.azure.com/signup) if you need a trial Azure subscription.

## Samples list

Samples are organized per platform and are numbered to provide a suggested reading order.

To use the samples clone this GitHub repository using Git.

```bash
    git clone https://github.com/Microsoft/BotBuilder-Samples.git
    cd BotBuilder-Samples
```

:runner: - Indicates planned and work in progress.

| Sample Name           | Description                                                                    | .NET CORE   | NodeJS      | .NET Web API | JS (es6)    | Typescript  |
|-----------------------|--------------------------------------------------------------------------------|-------------|-------------|--------------|-------------|-------------|
|1.console-echo         | Introduces the concept of adapter and demonstrates a simple echo bot on console adapter and how to send a reply and access the incoming message.                                                                                                 |[View][cs#1] |[View][js#1] |      |             |[View][ts#1] |
|1.a.browser-echo       | Introduces browser adapter                                                     |             |             |              |[View][es#1a]|             |
|2.a.echobot            | Demonstrates how to receive and send messages.                                 | [View][cs#2a] |[View][js#2a]| :runner: |           |[View][ts#2a] |
|2.b.echo-with-counter  | Demonstrates how to use state. Shows commented out code for all natively supported storage providers. Storage providers should include InMemory and Blob storage.                                                                                            |[View][cs#2b] |[View][js#2b]|[View][wa#2b] |           |[View][ts#2b] |
|3.welcome-user         | Introduces activity types and provides a welcome message on conversation update activity. |[View][cs#3] |[View][js#3] |              | | |
|4.simple-prompt        | Demonstrates prompt pattern by prompting user for a property. Introduces user state .vs. conversation state. Ask for name and prints back that information. Uses sequence dialogs if available or default option is to use waterfall dialogs            |[View][cs#4] |[View][js#4] |              |             | |
|5.multi-turn-prompt    | Demonstrates more complex pattern by prompting user for multiple properties. Ask for name, age and prints back that information. Uses sequence dialogs if available or default option is to use waterfall dialogs.                                              |[View][cs#5] |[View][js#5] |              |             | |
|6.using-cards          | Introduces all card types including thumbnail, audio, media etc. Builds on Welcoming user + multi-prompt bot by presenting a card with buttons in welcome message that route to appropriate dialog.                                                        |[View][cs#6] |[View][js#6] |              |             | |
|7.using-adaptive-cards | Introduces adaptive cards - demonstrates how the multi-turn dialog can be augmented to also use a card to get user input for name and age. |[View][cs#7] |[View][js#7] |              |             | |
|8.suggested-actions    | Demonstrates how to use suggested actions                                      |[View][cs#8] |[View][js#8] |              |             | |
|9.message-routing      | Demonstrates the main dialog or root dispatcher paradigm. Needs to show how to handle interruptions like cancel, help. |[View][cs#9] |[View][js#9] |              |            | |
|10.prompt-validations  | Demonstrates how to take advantage of different prompt types and prompt validators. |[View][cs#10]|[View][js#10]|              |             | |
|11.qnamaker            | Demonstrates how to use QnA Maker to have simple single-turn conversations     |[View][cs#11]|[View][js#11]|[View][wa#11] |             |[View][ts#11]|
|12.nlp-with-luis       | Demonstrates how to use LUIS to understand natural language                    |[View][cs#12]|[View][js#12]|[View][wa#12] |             |[View][ts#12]|
|13.basic-bot           | Basic bot template that puts together cards, NLP (LUIS)                        |[View][cs#13]|[View][js#13]|[View][wa#13] |             |[View][ts#13]|
|14.nlp-with-dispatch   | Demonstrates how to dispatch across LUIS, QnA Maker                            |[View][cs#14]|[View][js#14]|              |             | |
|15.handling-attachments| Demonstrates how to listen for/handle user provided attachments                |[View][cs#15]|[View][js#15]|              |             | |
|16.proactive-messages  | Demonstrates how to send proactive messages to users                           |[View][cs#16]|[View][js#16]|              |             | |
|17.multilingual-bot    | Using translate middleware to support a multi-lingual bot. Demonstrates custom middleware. |[View][cs#17]|[View][js#17]|              |             | |
|18.bot-authentication  | Bot that demonstrates how to integration with OAuth providers                  |[View][cs#18]|[View][js#18]|              |             | |
|19.custom-dialogs      | Demonstrates different ways to model conversations. Waterfall .vs. using your own dialog management |[View][cs#19]|[View][js#19]|              |             | |
|20.qna-with-appinsights| Demonstrates how to use QnA Maker and Azure Application insights               |[View][cs#20]|[View][js#20]|      |             | |
|21.luis-appinsights    | Demonstrates how to use LUIS and Azure Application insights                    |[View][cs#21]|[View][js#21]|      |             | |
|22.conversation-history| Demonstrates the use of SendConversationHistoryAsync API to upload conversation history stored in the conversation Transcript.|[View][cs#22]|:runner:|              |             | |
|23.facebook-events     | Integrate and consume Facebook specific payloads, such as post-backs, quick replies and opt-in events.|[View][cs#23] |[View][js#23] |              |             | |
|24.bot-auth-msgraph    | Demonstrates bot authentication capabilities of Azure Bot Service. Demonstrates utilizing the Microsoft Graph API to retrieve data about the user.|[View][cs#24] |[View][js#24] |              |             | |
|25.bot-logging    | This bot demonstrates the logging middleware in the nodejs SDK| |[View][js#25] |              |             | |
|26.bot-transcript-logging    | This bot demonstrates the transcript logging middleware in the nodejs SDK| |[View][js#26] |              |             | |
|30.asp-mvc-bot         | Demonstrates how to build a bot as an ASP.NET MVC Controller |[View][cs#30] ||              | | |
|40.timex-resolution    | Demonstrates various ways to parse and manipulate the TIMEX expressions you get from LUIS and the [DateTimeRecognizer](https://github.com/Microsoft/recognizers-text) used by the DateTimePrompt. |[View][cs#40] |[View][js#40]|              | | |
|42.scaleout            | Demonstrates how you can build your own state solution from the ground up that supports scaled out deployment with ETag based optimistic locking. |[View][cs#42] |:runner:|              | | |
|50.diceroller-skill    | This sample demonstrates how to implement a Cortana Skill that properly handles EndOfConversation events.|  |[View][js#50] |              |             | |
|51.cafe-bot            | A complete E2E Cafe bot that has all capabilities and includes best practices|[View][cs#51]|[View][js#51]|              |             | |
|52.enterprise-bot      | Enterprise bot that demonstrates use of Dialogs, Template Manager, Dispatch across different services and implementing custom middleware.| [View][cs#52] |           |              |             | [View][ts#52] |
|70.styling-webchat     | This sample shows how to create a web page with custom Web Chat component.|         |          |              |  [View][es#70] |     |

[cs#1]:samples/csharp_dotnetcore/01.console-echo
[cs#2a]:samples/csharp_dotnetcore/02.a.echo-bot
[cs#2b]:samples/csharp_dotnetcore/02.b.echo-with-counter
[cs#3]:samples/csharp_dotnetcore/03.welcome-user
[cs#4]:samples/csharp_dotnetcore/04.simple-prompt
[cs#5]:samples/csharp_dotnetcore/05.multi-turn-prompt
[cs#6]:samples/csharp_dotnetcore/06.using-cards
[cs#7]:samples/csharp_dotnetcore/07.using-adaptive-cards
[cs#8]:samples/csharp_dotnetcore/08.suggested-actions
[cs#9]:samples/csharp_dotnetcore/09.message-routing
[cs#10]:samples/csharp_dotnetcore/10.prompt-validations
[cs#11]:samples/csharp_dotnetcore/11.qnamaker
[cs#12]:samples/csharp_dotnetcore/12.nlp-with-luis
[cs#13]:samples/csharp_dotnetcore/13.basic-bot
[cs#14]:samples/csharp_dotnetcore/14.nlp-with-dispatch
[cs#15]:samples/csharp_dotnetcore/15.handling-attachments
[cs#16]:samples/csharp_dotnetcore/16.proactive-messages
[cs#17]:samples/csharp_dotnetcore/17.multilingual-bot
[cs#18]:samples/csharp_dotnetcore/18.bot-authentication
[cs#19]:samples/csharp_dotnetcore/19.custom-dialogs
[cs#20]:samples/csharp_dotnetcore/20.qna-with-appinsights
[cs#21]:samples/csharp_dotnetcore/21.luis-with-appinsights
[cs#22]:samples/csharp_dotnetcore/22.conversation-history
[cs#23]:samples/csharp_dotnetcore/23.facebook-events
[cs#24]:samples/csharp_dotnetcore/24.bot-authentication-msgraph
[cs#30]:samples/csharp_dotnetcore/30.asp-mvc-bot
[cs#40]:samples/csharp_dotnetcore/40.timex-resolution
[cs#42]:samples/csharp_dotnetcore/42.scaleout
[cs#51]:samples/csharp_dotnetcore/51.cafe-bot
[cs#52]:https://github.com/Microsoft/AI/tree/master/templates/Enterprise-Template
[cs#60]:experimental/csharp_dotnetcore/60.multilinugal-luis-bot

[wa#2a]:samples/csharp_webapi/02.a.echobot
[wa#2b]:samples/csharp_webapi/02.b.echo-with-counter
[wa#11]:samples/csharp_webapi/11.QnAMaker
[wa#12]:samples/csharp_webapi/12.NLP-With-LUIS
[wa#13]:samples/csharp_webapi/13.Basic-Bot-Template

[es#1a]:samples/javascript_es6/01.a.browser-echo
[es#70]:samples/javascript_es6/70.styling-webchat

[ts#1]:samples/javascript_typescript/01.console-echo
[ts#2a]:samples/javascript_typescript/02.a.echobot
[ts#2b]:samples/javascript_typescript/02.b.echobot-with-counter
[ts#11]:samples/javascript_typescript/11.qnamaker
[ts#12]:samples/javascript_typescript/12.nlp-with-luis
[ts#13]:samples/javascript_typescript/13.basic-bot
[ts#52]:https://github.com/Microsoft/AI/tree/master/templates/Enterprise-Template
[ts#60]:experimental/javascript_typescript/60.multilingual-luis-bot

[js#1]:samples/javascript_nodejs/01.console-echo
[js#2a]:samples/javascript_nodejs/02.a.echobot
[js#2b]:samples/javascript_nodejs/02.b.echobot-with-counter
[js#3]:samples/javascript_nodejs/03.welcome-users
[js#4]:samples/javascript_nodejs/04.simple-prompt
[js#5]:samples/javascript_nodejs/05.multi-turn-prompt
[js#6]:samples/javascript_nodejs/06.using-cards
[js#7]:samples/javascript_nodejs/07.using-adaptive-cards
[js#8]:samples/javascript_nodejs/08.suggested-actions
[js#9]:samples/javascript_nodejs/09.message-routing
[js#10]:samples/javascript_nodejs/10.prompt-validations
[js#11]:samples/javascript_nodejs/11.qnamaker
[js#12]:samples/javascript_nodejs/12.nlp-with-luis
[js#13]:samples/javascript_nodejs/13.basic-bot
[js#14]:samples/javascript_nodejs/14.nlp-with-dispatch
[js#15]:samples/javascript_nodejs/15.handling-attachments
[js#16]:samples/javascript_nodejs/16.proactive-messages
[js#17]:samples/javascript_nodejs/17.multilingual-conversations
[js#18]:samples/javascript_nodejs/18.bot-authentication
[js#19]:samples/javascript_nodejs/19.custom-dialogs
[js#20]:samples/javascript_nodejs/20.qna-with-appinsights
[js#21]:samples/javascript_nodejs/21.luis-with-appinsights

[js#23]:samples/javascript_nodejs/23.facebook-events
[js#24]:samples/javascript_nodejs/24.bot-authentication-msgraph

[js#25]:samples/javascript_nodejs/25.logger-bot
[js#26]:samples/javascript_nodejs/26.transcript-logger-bot

[js#40]:samples/javascript_nodejs/40.timex-resolution

[js#50]:samples/javascript_nodejs/50.diceroller-skill
[js#51]:samples/javascript_nodejs/51.cafe-bot


## Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.microsoft.com.

When you submit a pull request, a CLA-bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., label, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

When you submit a pull request, a CLA-bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., label, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.
This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

## Reporting Security Issues
Security issues and bugs should be reported privately, via email, to the Microsoft Security Response Center (MSRC) at [secure@microsoft.com](mailto:secure@microsoft.com). You should receive a response within 24 hours. If for some reason you do not, please follow up via email to ensure we received your original message. Further information, including the [MSRC PGP](https://technet.microsoft.com/en-us/security/dn606155) key, can be found in the [Security TechCenter](https://technet.microsoft.com/en-us/security/default).

## License

Copyright (c) Microsoft Corporation. All rights reserved.

Licensed under the [MIT](/LICENSE.md) License.


This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

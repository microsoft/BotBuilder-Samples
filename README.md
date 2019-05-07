
# ![Bot Framework Samples](./docs/media/BotFrameworkSamples_header.png)

### [Click here to find out what's new for //build2019!](https://github.com/Microsoft/botframework/blob/master/whats-new.md#whats-new)

## Overview

This branch contains samples for the released version of the **Microsoft Bot Framework V4 SDK** for [.NET](https://github.com/Microsoft/botbuilder-dotnet) and [JS](https://github.com//microsoft/botbuilder-js). If you need samples for the Bot Framework _V3_ SDK, go [here](https://github.com/Microsoft/BotBuilder-Samples/tree/v3-sdk-samples).  If you need Bot Framework V4 Python samples, go [here](https://github.com/Microsoft/botbuilder-python/tree/master/samples)

## Samples list
Samples are designed to illustrate scenarios you'll need to implement to build great bots! To use the samples, clone this GitHub repository using Git.

```bash
    git clone https://github.com/Microsoft/BotBuilder-Samples.git
    cd BotBuilder-Samples
```

:runner: - Indicates planned and work in progress.

| Sample Name           | Description                                                                    | .NET CORE   | JavaScript      | .NET Web API | JS (es6)    | TypeScript  |
|-----------------------|--------------------------------------------------------------------------------|-------------|-------------|--------------|-------------|-------------|
|1.console-echo         | Introduces the concept of adapter and demonstrates a simple echo bot on console adapter and how to send a reply and access the incoming message.           |[View][cs#1] |[View][js#1] |      |             |[View][ts#1] |
|1.browser-echo         | Demonstrates how to host a bot in the browser using Web Chat and a custom Web Chat Adapter.   |  |  |    |[View][es#1]|             |
|2.echo-bot             | Demonstrates how to receive and send messages.                                 |[View][cs#2] |[View][js#2]|  |           |[View][ts#2] |
|3.welcome-user         | Introduces activity types and provides a welcome message on conversation update activity. |[View][cs#3] |[View][js#3] |              | | |
|5.multi-turn-prompt    | Demonstrates how to use waterfall dialog, prompts, and component dialog to create a simple interaction that asks the user for name, age, and prints back that information.          |[View][cs#5] |[View][js#5] |              |             | |
|6.using-cards          | Introduces all card types including thumbnail, audio, media etc. Builds on Welcoming user + multi-prompt bot by presenting a card with buttons in welcome message that route to appropriate dialog.     |[View][cs#6] |[View][js#6] |              |             | |
|7.using-adaptive-cards | Demonstrates how the multi-turn dialog can use a card to get user input for name and age. |[View][cs#7] |[View][js#7] |         | | |
|8.suggested-actions    | Demonstrates how to enable your bot to present buttons that the user can tap to provide input.                                      |[View][cs#8] |[View][js#8] |              |             | |
|11.qnamaker            | Demonstrates how to use QnA Maker to have simple single-turn conversations     |[View][cs#11]|[View][js#11]|              |             | |
|13.core-bot            | Core bot shows how to use cards, dialog, and Langugage Understanding (LUIS).                         |[View][cs#13]|[View][js#13]|[View][wa#13] |             |[View][ts#13]|
|14.nlp-with-dispatch   | Demonstrates how to dispatch across LUIS and QnA Maker.                            |[View][cs#14]|[View][js#14]|              |             | |
|15.handling-attachments| Demonstrates how to listen for/handle user provided attachments.                |[View][cs#15]|[View][js#15]|              |             | |
|16.proactive-messages  | Demonstrates how to send proactive messages to users.                           |[View][cs#16]|[View][js#16]|              |             | |
|17.multilingual-bot    | Using translate middleware to support a multi-lingual bot. Demonstrates custom middleware. |[View][cs#17]|[View][js#17]|              |             | |
|18.bot-authentication  | Bot that demonstrates how to integrate OAuth providers.                  |[View][cs#18]|[View][js#18]|              |             | |
|19.custom-dialogs      | Demonstrates complex conversation flow using the Dialogs library. |[View][cs#19]|[View][js#19]|              |             | |
|23.facebook-events     | Integrate and consume Facebook specific payloads, such as post-backs, quick replies and opt-in events.|[View][cs#23] |[View][js#23] |              |             | |
|24.bot-auth-msgraph    | Demonstrates bot authentication capabilities of Azure Bot Service. Demonstrates utilizing the Microsoft Graph API to retrieve data about the user.|[View][cs#24] |[View][js#24] |              |             | |
|40.timex-resolution    | Demonstrates various ways to parse and manipulate the TIMEX expressions you get from LUIS and the [DateTimeRecognizer](https://github.com/Microsoft/recognizers-text) used by the DateTimePrompt. |[View][cs#40] |[View][js#40]|              | | |
|42.scaleout            | Demonstrates how you can build your own state solution from the ground up that supports scaled out deployment with ETag based optimistic locking. |[View][cs#42] |    |              | | |
|43.complex-dialog      | Demonstrates different ways for composing dialogs. |[View][cs#43]|[View][js#43] |              |             | |
|44.prompt-for-user-input | Demonstrates how to implement your own _basic_ prompts to ask the user for information. |[View][cs#44]|[View][js#44]|              |             | |
|45.state-management    | Demonstrates how to use state management and storage objects to manage and persist state. | [View][cs#45] | [View][js#45]   |              |             |  |
|70.styling-webchat     | This sample shows how to create a web page with custom Web Chat component.|         |          |              |  [View][es#70] |     |

[cs#1]:samples/csharp_dotnetcore/01.console-echo
[cs#2]:samples/csharp_dotnetcore/02.echo-bot
[cs#3]:samples/csharp_dotnetcore/03.welcome-user
[cs#5]:samples/csharp_dotnetcore/05.multi-turn-prompt
[cs#6]:samples/csharp_dotnetcore/06.using-cards
[cs#7]:samples/csharp_dotnetcore/07.using-adaptive-cards
[cs#8]:samples/csharp_dotnetcore/08.suggested-actions
[cs#11]:samples/csharp_dotnetcore/11.qnamaker
[cs#13]:samples/csharp_dotnetcore/13.core-bot
[cs#14]:samples/csharp_dotnetcore/14.nlp-with-dispatch
[cs#15]:samples/csharp_dotnetcore/15.handling-attachments
[cs#16]:samples/csharp_dotnetcore/16.proactive-messages
[cs#17]:samples/csharp_dotnetcore/17.multilingual-bot
[cs#18]:samples/csharp_dotnetcore/18.bot-authentication
[cs#19]:samples/csharp_dotnetcore/19.custom-dialogs
[cs#21]:samples/csharp_dotnetcore/21.luis-with-appinsights
[cs#23]:samples/csharp_dotnetcore/23.facebook-events
[cs#24]:samples/csharp_dotnetcore/24.bot-authentication-msgraph
[cs#40]:samples/csharp_dotnetcore/40.timex-resolution
[cs#42]:samples/csharp_dotnetcore/42.scaleout
[cs#43]:samples/csharp_dotnetcore/43.complex-dialog
[cs#44]:samples/csharp_dotnetcore/44.prompt-users-for-input
[cs#45]:samples/csharp_dotnetcore/45.state-management
[cs#52]:https://github.com/Microsoft/AI/tree/master/templates/Enterprise-Template

[wa#13]:samples/csharp_webapi/13.core-bot

[es#1]:samples/javascript_es6/01.browser-echo
[es#70]:samples/javascript_es6/70.styling-webchat

[ts#0]:samples/typescript_nodejs/00.empty-bot
[ts#1]:samples/typescript_nodejs/01.console-echo
[ts#2]:samples/typescript_nodejs/02.echo-bot
[ts#13]:samples/typescript_nodejs/13.core-bot
[ts#52]:https://github.com/Microsoft/AI/tree/master/templates/Enterprise-Template


[js#1]:samples/javascript_nodejs/01.console-echo
[js#2]:samples/javascript_nodejs/02.echo-bot
[js#3]:samples/javascript_nodejs/03.welcome-users
[js#5]:samples/javascript_nodejs/05.multi-turn-prompt
[js#6]:samples/javascript_nodejs/06.using-cards
[js#7]:samples/javascript_nodejs/07.using-adaptive-cards
[js#8]:samples/javascript_nodejs/08.suggested-actions
[js#11]:samples/javascript_nodejs/11.qnamaker
[js#13]:samples/javascript_nodejs/13.core-bot
[js#14]:samples/javascript_nodejs/14.nlp-with-dispatch
[js#15]:samples/javascript_nodejs/15.handling-attachments
[js#16]:samples/javascript_nodejs/16.proactive-messages
[js#17]:samples/javascript_nodejs/17.multilingual-bot
[js#18]:samples/javascript_nodejs/18.bot-authentication
[js#19]:samples/javascript_nodejs/19.custom-dialogs
[js#21]:samples/javascript_nodejs/21.luis-with-appinsights
[js#23]:samples/javascript_nodejs/23.facebook-events
[js#24]:samples/javascript_nodejs/24.bot-authentication-msgraph
[js#40]:samples/javascript_nodejs/40.timex-resolution
[js#43]:samples/javascript_nodejs/43.complex-dialog
[js#44]:samples/javascript_nodejs/44.prompt-for-user-input
[js#45]:samples/javascript_nodejs/45.state-management


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

Copyright (c) Microsoft Corporation. All rights reserved.

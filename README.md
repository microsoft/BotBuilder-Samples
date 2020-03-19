
# ![Bot Framework Samples](./docs/media/BotFrameworkSamples_header.png)

### [Click here to find out what's new with Bot Framework](https://github.com/Microsoft/botframework/blob/master/whats-new.md#whats-new)

## Overview

This branch contains samples for the released version of the **Microsoft Bot Framework V4 SDK** for [.NET](https://github.com/Microsoft/botbuilder-dotnet), [JS](https://github.com//microsoft/botbuilder-js) and [Python](https://github.com//microsoft/botbuilder-python). If you need samples for the Bot Framework _V3_ SDK, go [here](https://github.com/Microsoft/BotBuilder-Samples/tree/v3-sdk-samples).

## Samples list
Samples are designed to illustrate scenarios you'll need to implement to build great bots! To use the samples, clone this GitHub repository using Git.

```bash
    git clone https://github.com/Microsoft/BotBuilder-Samples.git
    cd BotBuilder-Samples
```

| Sample Name           | Description                                                                    | .NET CORE   | JavaScript      | .NET Web API | JS (es6)    | TypeScript  | Python
|-----------------------|--------------------------------------------------------------------------------|-------------|-------------|--------------|-------------|-------------|-------------|
|1.console-echo         | Introduces the concept of adapter and demonstrates a simple echo bot on console adapter and how to send a reply and access the incoming message.           |[View][cs#1] |[View][js#1] |      |             |[View][ts#1] |[View][py#1]
|1.browser-echo         | Demonstrates how to host a bot in the browser using Web Chat and a custom Web Chat Adapter.   |  |  |    |[View][es#1]|             |
|2.echo-bot             | Demonstrates how to receive and send messages.                                 |[View][cs#2] |[View][js#2]|  |           |[View][ts#2] |[View][py#2]
|3.welcome-user         | Introduces activity types and provides a welcome message on conversation update activity. |[View][cs#3] |[View][js#3] |              | |[View][ts#3] |[View][py#3]
|5.multi-turn-prompt    | Demonstrates how to use waterfall dialog, prompts, and component dialog to create a simple interaction that asks the user for name, age, and prints back that information.          |[View][cs#5] |[View][js#5] |              |             |[View][ts#5] |[View][py#5]
|6.using-cards          | Introduces all card types including thumbnail, audio, media etc. Builds on Welcoming user + multi-prompt bot by presenting a card with buttons in welcome message that route to appropriate dialog.     |[View][cs#6] |[View][js#6] |              |             |[View][ts#6] |[View][py#6]
|7.using-adaptive-cards | Demonstrates how the multi-turn dialog can use a card to get user input for name and age. |[View][cs#7] |[View][js#7] |         | | |[View][py#7]
|8.suggested-actions    | Demonstrates how to enable your bot to present buttons that the user can tap to provide input.                                      |[View][cs#8] |[View][js#8] |              |             | |[View][py#8]
|11.qnamaker            | Demonstrates how to use QnA Maker to have simple single-turn conversations     |[View][cs#11]|[View][js#11]|              |             | |[View][py#11]
|13.core-bot            | Core bot shows how to use cards, dialog, and Langugage Understanding (LUIS).                         |[View][cs#13]|[View][js#13]|[View][wa#13] |             |[View][ts#13]|[View][py#13]
|13.core-bot.tests            | Unit test project Core bot shows how to use use Bot Framework testing framework.                         |[View][cs#13.b]|   |   |             |  |
|14.nlp-with-dispatch   | Demonstrates how to dispatch across LUIS and QnA Maker.                            |[View][cs#14]|[View][js#14]|              |            | |[View][py#14]
|15.handling-attachments| Demonstrates how to listen for/handle user provided attachments.                |[View][cs#15]|[View][js#15]|              |             | |[View][py#15]
|16.proactive-messages  | Demonstrates how to send proactive messages to users.                           |[View][cs#16]|[View][js#16]|              |             | [View][ts#16]|[View][py#16]
|17.multilingual-bot    | Using translate middleware to support a multi-lingual bot. Demonstrates custom middleware. |[View][cs#17]|[View][js#17]|              |             | |[View][py#17]
|18.bot-authentication  | Bot that demonstrates how to integrate OAuth providers.                  |[View][cs#18]|[View][js#18]|              |             | |[View][py#18]
|19.custom-dialogs      | Demonstrates complex conversation flow using the Dialogs library. |[View][cs#19]|[View][js#19]|              |             | |[View][py#19]
|21.corebot-app-insights     | Demonstrates how to add telemetry logging to your bot, storing telemetry within Application Insights.|[View][cs#21] |[View][js#21] |              |             | |
|23.facebook-events     | Integrate and consume Facebook specific payloads, such as post-backs, quick replies and opt-in events.|[View][cs#23] |[View][js#23] |              |             | |[View][py#23]
|24.bot-auth-msgraph    | Demonstrates bot authentication capabilities of Azure Bot Service. Demonstrates utilizing the Microsoft Graph API to retrieve data about the user.|[View][cs#24] |[View][js#24] |              |             | |[View][py#24]
|40.timex-resolution    | Demonstrates various ways to parse and manipulate the TIMEX expressions you get from LUIS and the [DateTimeRecognizer](https://github.com/Microsoft/recognizers-text) used by the DateTimePrompt. |[View][cs#40] |[View][js#40]|              | | |[View][py#40]
|42.scaleout            | Demonstrates how you can build your own state solution from the ground up that supports scaled out deployment with ETag based optimistic locking. |[View][cs#42] |    |              | | |[View][py#42]
|43.complex-dialog      | Demonstrates different ways for composing dialogs. |[View][cs#43]|[View][js#43] |              |             | |[View][py#43]
|44.prompt-for-user-input | Demonstrates how to implement your own _basic_ prompts to ask the user for information. |[View][cs#44]|[View][js#44]|              |             | |[View][py#44]
|45.state-management    | Demonstrates how to use state management and storage objects to manage and persist state. | [View][cs#45] | [View][js#45]   |              |             |  |[View][py#45]
|46.teams-auth    | Demonstrates how to use authentication for a bot running in Microsoft Teams. | [View][cs#46] | [View][js#46]   |              |             |  |[View][py#46]
|47.inspection    | Demonstrates how to use middleware to allow the Bot Framework Emulator to debug traffic into and out of the bot in addition to looking at the current state of the bot. | [View][cs#47] | [View][js#47]   |              |             |  |[View][py#47]
|48.qnamaker-active-learning-bot     | Demonstrates how to integrate Active Learning in a QnA Maker bot.|[View][cs#48]|[View][js#48]          |              |     |     |
|49.qnamaker-all-features     | Demonstrates how to integrate Multiturn and Active learning in a QnA Maker bot.|[View][cs#49]|[View][js#49]          |              |     |     |
|50.teams-messaging-extensions-search     |  A Messaging Extension that accepts search requests and returns results.|[View][cs#50]|[View][js#50]          |              |     |     |[View][py#50]
|51.teams-messaging-extensions-action     |  A Messaging Extension that accepts parameters and returns a card.  Also, how to receive a forwarded message as a parameter in a Messaging Extension.|[View][cs#51]|[View][js#51]          |              |     |     |[View][py#51]
|52.teams-messaging-extensions-search-auth-config     | A Messaging Extension that has a configuration page, accepts search requests and returns results after the user has signed in.|[View][cs#52]|[View][js#52]          |              |     |     |
|53.teams-messaging-extensions-action-preview     | Demonstrates how to create a Preview and Edit flow for a Messaging Extension.|[View][cs#53]|[View][js#53]          |              |     |     |[View][py#53]
|54.teams-task-module     | Demonstrates how to retrieve a Task Module, and values from cards in the Task Module, for a Messaging Extension.|[View][cs#54]|[View][js#54]          |              |     |     |[View][py#54] 
|55.teams-link-unfurling     | A Messaging Extension that performs link unfurling.|[View][cs#55]|[View][js#55]          |              |     |     |[View][py#55]
|56.teams-file-upload     | Demonstrates how to obtain file consent, and upload files to Teams from a bot. Also, how to receive a file sent to a bot.|[View][cs#56]|[View][js#56]          |              |     |     |[View][py#56]
|57.teams-conversation-bot     | Demonstrates various features of bots on Teams: message all members in a Team or Channel, @mention a user from a bot, update previously sent messages, etc. |[View][cs#57]|[View][js#57]          |              |     |     |[View][py#57]
|70.styling-webchat     | This sample shows how to create a web page with custom Web Chat component.|         |          |              |  [View][es#70] |     |
|80.skills-simple-bot-to-bot     | This sample shows how to connect a skill to a skill consumer.| [View][cs#80] | [View][js#80]       |              |   |     |[View][py#80]

[cs#1]:samples/csharp_dotnetcore/01.console-echo
[cs#2]:samples/csharp_dotnetcore/02.echo-bot
[cs#3]:samples/csharp_dotnetcore/03.welcome-user
[cs#5]:samples/csharp_dotnetcore/05.multi-turn-prompt
[cs#6]:samples/csharp_dotnetcore/06.using-cards
[cs#7]:samples/csharp_dotnetcore/07.using-adaptive-cards
[cs#8]:samples/csharp_dotnetcore/08.suggested-actions
[cs#11]:samples/csharp_dotnetcore/11.qnamaker
[cs#12]:samples/csharp_dotnetcore/11a.qnamaker
[cs#13]:samples/csharp_dotnetcore/13.core-bot
[cs#13.b]:samples/csharp_dotnetcore/13.core-bot.tests
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
[cs#46]:samples/csharp_dotnetcore/46.teams-auth
[cs#47]:samples/csharp_dotnetcore/47.inspection
[cs#48]:samples/csharp_dotnetcore/48.qnamaker-active-learning-bot
[cs#49]:samples/csharp_dotnetcore/49.qnamaker-all-features
[cs#50]:samples/csharp_dotnetcore/50.teams-messaging-extensions-search
[cs#51]:samples/csharp_dotnetcore/51.teams-messaging-extensions-action
[cs#52]:samples/csharp_dotnetcore/52.teams-messaging-extensions-search-auth-config
[cs#53]:samples/csharp_dotnetcore/53.teams-messaging-extensions-action-preview
[cs#54]:samples/csharp_dotnetcore/54.teams-task-module
[cs#55]:samples/csharp_dotnetcore/55.teams-link-unfurling
[cs#56]:samples/csharp_dotnetcore/56.teams-file-upload
[cs#57]:samples/csharp_dotnetcore/57.teams-conversation-bot
[cs#80]:samples/csharp_dotnetcore/80.skills-simple-bot-to-bot

[wa#13]:samples/csharp_webapi/13.core-bot

[es#1]:samples/javascript_es6/01.browser-echo
[es#70]:samples/javascript_es6/70.styling-webchat

[ts#0]:samples/typescript_nodejs/00.empty-bot
[ts#1]:samples/typescript_nodejs/01.console-echo
[ts#2]:samples/typescript_nodejs/02.echo-bot
[ts#3]:samples/typescript_nodejs/03.welcome-users
[ts#5]:samples/typescript_nodejs/05.multi-turn-prompt
[ts#6]:samples/typescript_nodejs/06.using-cards
[ts#13]:samples/typescript_nodejs/13.core-bot
[ts#16]:samples/typescript_nodejs/16.proactive-messages
[ts#52]:https://github.com/Microsoft/AI/tree/master/templates/Enterprise-Template


[js#1]:samples/javascript_nodejs/01.console-echo
[js#2]:samples/javascript_nodejs/02.echo-bot
[js#3]:samples/javascript_nodejs/03.welcome-users
[js#5]:samples/javascript_nodejs/05.multi-turn-prompt
[js#6]:samples/javascript_nodejs/06.using-cards
[js#7]:samples/javascript_nodejs/07.using-adaptive-cards
[js#8]:samples/javascript_nodejs/08.suggested-actions
[js#11]:samples/javascript_nodejs/11.qnamaker
[js#12]:samples/javascript_nodejs/11a.qnamaker
[js#13]:samples/javascript_nodejs/13.core-bot
[js#14]:samples/javascript_nodejs/14.nlp-with-dispatch
[js#15]:samples/javascript_nodejs/15.handling-attachments
[js#16]:samples/javascript_nodejs/16.proactive-messages
[js#17]:samples/javascript_nodejs/17.multilingual-bot
[js#18]:samples/javascript_nodejs/18.bot-authentication
[js#19]:samples/javascript_nodejs/19.custom-dialogs
[js#21]:samples/javascript_nodejs/21.corebot-app-insights
[js#23]:samples/javascript_nodejs/23.facebook-events
[js#24]:samples/javascript_nodejs/24.bot-authentication-msgraph
[js#40]:samples/javascript_nodejs/40.timex-resolution
[js#43]:samples/javascript_nodejs/43.complex-dialog
[js#44]:samples/javascript_nodejs/44.prompt-for-user-input
[js#45]:samples/javascript_nodejs/45.state-management
[js#46]:samples/javascript_nodejs/46.teams-auth
[js#47]:samples/javascript_nodejs/47.inspection
[js#48]:samples/javascript_nodejs/48.qnamaker-active-learning-bot
[js#49]:samples/javascript_nodejs/49.qnamaker-all-features
[js#50]:samples/javascript_nodejs/50.teams-messaging-extensions-search
[js#51]:samples/javascript_nodejs/51.teams-messaging-extensions-action
[js#52]:samples/javascript_nodejs/52.teams-messaging-extensions-search-auth-config
[js#53]:samples/javascript_nodejs/53.teams-messaging-extensions-action-preview
[js#54]:samples/javascript_nodejs/54.teams-task-module
[js#55]:samples/javascript_nodejs/55.teams-link-unfurling
[js#56]:samples/javascript_nodejs/56.teams-file-upload
[js#57]:samples/javascript_nodejs/57.teams-conversation-bot
[js#80]:samples/javascript_nodejs/80.skills-simple-bot-to-bot

[py#1]:samples/python/01.console-echo
[py#2]:samples/python/02.echo-bot
[py#3]:samples/python/03.welcome-user
[py#5]:samples/python/05.multi-turn-prompt
[py#6]:samples/python/06.using-cards
[py#7]:samples/python/07.using-adaptive-cards
[py#8]:samples/python/08.suggested-actions
[py#11]:samples/python/11.qnamaker
[py#13]:samples/python/13.core-bot
[py#14]:samples/python/14.nlp-with-dispatch
[py#15]:samples/python/15.handling-attachments
[py#16]:samples/python/16.proactive-messages
[py#17]:samples/python/17.multilingual-bot
[py#18]:samples/python/18.bot-authentication
[py#19]:samples/python/19.custom-dialogs
[py#21]:samples/python/21.corebot-app-insights
[py#23]:samples/python/23.facebook-events
[py#24]:samples/python/24.bot-authentication-msgraph
[py#40]:samples/python/40.timex-resolution
[py#42]:samples/python/42.scaleout
[py#43]:samples/python/43.complex-dialog
[py#44]:samples/python/44.prompt-for-user-input
[py#45]:samples/python/45.state-management
[py#46]:samples/python/46.teams-auth
[py#47]:samples/python/47.inspection
[py#50]:samples/python/50.teams-messaging-extensions-search
[py#51]:samples/python/51.teams-messaging-extensions-action
[py#52]:samples/python/52.teams-messaging-extensions-search-auth-config
[py#53]:samples/python/53.teams-messaging-extensions-action-preview
[py#54]:samples/python/54.teams-task-module
[py#55]:samples/python/55.teams-link-unfurling
[py#56]:samples/python/56.teams-file-upload
[py#57]:samples/python/57.teams-conversation-bot
[py#80]:samples/python/80.skills-simple-bot-to-bot

## Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.microsoft.com.

When you submit a pull request, a CLA-bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., label, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.
This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

## Reporting Security Issues
Security issues and bugs should be reported privately, via email, to the Microsoft Security Response Center (MSRC) at [secure@microsoft.com](mailto:secure@microsoft.com). You should receive a response within 24 hours. If for some reason you do not, please follow up via email to ensure we received your original message. Further information, including the [MSRC PGP](https://technet.microsoft.com/en-us/security/dn606155) key, can be found in the [Security TechCenter](https://technet.microsoft.com/en-us/security/default).

Copyright (c) Microsoft Corporation. All rights reserved.

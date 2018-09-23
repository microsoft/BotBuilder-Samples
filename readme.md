## Overview

This repository contains samples for the Microsoft **Bot Builder V4 SDK** - [dotnet SDK](/microsoft/botbuilder-dotnet), [JS SDK](/microsoft/botbuilder-js).

Samples for the Bot Builder V3 SDK are available [here](/tree/v3).

Samples are organized per platform and are numbered to provide a suggested reading order.

To use the samples clone our GitHub repository using Git.

```bash
    git clone https://github.com/Microsoft/BotBuilder-Samples.git
    cd BotBuilder-Samples
```

## Samples list
|Sample Name|Description|.NET CORE|.NET Web API|JS (es6)|TS|
|-----------|-----------|---------|------------|--------|--|
|1.Console-EchoBot|Introduces the concept of adapter and demonstrates a simple echo bot on console adapter and how to send a reply and access the incoming the incoming message. Doc topic from readme lists other adapter types|[View Sample](csharp_dotnetcore/1.console-echobot)||[View Sample](javascript_nodejs/1.console-echobot)||
|1.a. Browser-EchoBot|Introduces browser adapter|||:heavy_check_mark:||
|2.EchoBot-With-Counter|Demonstrates how to use state. Shows commented out code for all natively supported storage providers. Storage providers should include InMemory, CosmosDB and Blob storage. Also a template served via ABS, VSIX, yeoman. |[View Sample](/csharp_dotnetcore/2.echobot-with-counter)[![Deploy to Azure][Deploy Button]][Deploy csharp_dotnetcore/2.echobot-with-counter]|[View Sample](csharp_webapi/2.echobot-with-counter)[![Deploy to Azure][Deploy Button]][Deploy csharp_webapi/2.echobot-with-counter]|[View Sample](javascript_nodejs/2.echobot-with-counter)[![Deploy to Azure][Deploy Button]][Deploy javascript_nodejs/2.echobot-with-counter]|[View Sample](javascript_ts/2.echobot-with-counter)[![Deploy to Azure][Deploy Button]][Deploy javascript_ts/2.echobot-with-counter]|
|3.Welcome-User|Introduces activity types and providing a welcome message on conversation update activity. This should be the “who to” sample and tie back to a Concept doc. |:heavy_check_mark:||:heavy_check_mark:||
|4.Simple-Prompt-Bot|Demonstrates prompt pattern by prompting user for a property. Introduces user state .vs. conversation state. Ask for name and prints back those information. Uses sequence dialogs if available or default option is to use waterfall dialogs|:heavy_check_mark:||:heavy_check_mark:||
|5.MultiTurn-Prompts-Bot|Demonstrates more complex pattern by prompting user for multiple properties. Ask for name, age and prints back that information. Uses sequence dialogs if available or default option is to use waterfall dialogs.|:heavy_check_mark:||:heavy_check_mark:||
|6.Using-Cards|Introduces all card types including thumbnail, audio, media etc. Builds on Welcoming user + multi-prompt bot by presenting a card with buttons in welcome message that route to appropriate dialog.|:heavy_check_mark:||:heavy_check_mark:||
|7.Using-Adaptive-Cards|Introduces adaptive cards - demonstrates how the multi-turn dialog can be augmented to also use a card to get user input for name and age.|:heavy_check_mark:||:heavy_check_mark:||
|8.Suggested-Actions|Demonstrates how to use suggested actions |:heavy_check_mark:||:heavy_check_mark:||
|9.Message-Routing|Demonstrates the main dialog or root dispatcher paradigm. Needs to show how to handle interruptions like cancel, help, start over. Tie to conceptual documentation and best practice/ patterns recommendations.|:heavy_check_mark:||:heavy_check_mark:||
|10.Prompt-Validations|Demonstrates how to take advantage of different prompt types and prompt validators. In this example, we will expand the multi-turn prompt sample to accept name, age, date of birth, favorite color.Name uses text prompt with 1min character and 50 max character limitation; Age uses number prompt with valid age between 1 - 99; Date of birth uses date time prompt with valid date of birth between 8/24/1918 through 8/24/2018; Favorite color uses choice prompt with red, blue, green, .. as choices|:heavy_check_mark:||:heavy_check_mark:||
|11.QnAMaker|Demonstrates how to use QnA Maker to have simple single-turn conversations|:heavy_check_mark:|:heavy_check_mark:|:heavy_check_mark:|:heavy_check_mark:|
|12.NLP-With-LUIS|Demonstrates how to use LUIS to understand natural language|:heavy_check_mark:|:heavy_check_mark:|:heavy_check_mark:|:heavy_check_mark:|
|13.Basic-Bot-Template|Basic bot template that puts together cards, NLP (LUIS)|:heavy_check_mark:|:heavy_check_mark:|:heavy_check_mark:|:heavy_check_mark:|
|14.NLP-With-Dispatch|Demonstrates how dispatch should be used E2E|:heavy_check_mark:||:heavy_check_mark:||
|15.Handling-No-Match|Provides guidance on how to handle no-match. Its appropriate to have this covered after LUIS and QnA maker to show post NLP step as well.|:heavy_check_mark:||:heavy_check_mark:||
|16.Handling-Atachments|Demonstrates how to listen for/handle user provided attachments|:heavy_check_mark:||:heavy_check_mark:||
|17.Proactive-Messages|Demonstrates how to send proactive messages|:heavy_check_mark:||:heavy_check_mark:||
|18.Multi-Lingual-Bot|Using translate middleware to support a multi-lingual bot|:heavy_check_mark:||:heavy_check_mark:||
|19.Bot-uthentication|Bot that demonstrates how to integration with auth providers|:heavy_check_mark:||:heavy_check_mark:||
|20.Handling-End-Of-Conversation|Bot that demonstrates how to handle end of conversation events|:heavy_check_mark:||:heavy_check_mark:||
|21.Custom-Dialogs|Demonstrates different ways to model conversations. Waterfall .vs. using your own dialog management|:heavy_check_mark:||:heavy_check_mark:||
|50.Contoso-Café-Bot|A complete E2E Cafe bot that has all capabilities and includes best practices|:heavy_check_mark:||:heavy_check_mark:||

[Deploy Button]: https://azuredeploy.net/deploybutton.png
[Deploy csharp_dotnetcore/2.echobot-with-counter]: https://azuredeploy.net?repository=https://github.com/microsoft/BotBuilder-Samples/tree/v4/csharp_dotnetcore/2.echobot-with-counter
[Deploy csharp_webapi/2.echobot-with-counter]: https://azuredeploy.net?repository=https://github.com/microsoft/BotBuilder-Samples/tree/v4/csharp_webapi/2.echobot-with-counter
[Deploy javascript_nodejs/2.echobot-with-counter]: https://azuredeploy.net?repository=https://github.com/microsoft/BotBuilder-Samples/tree/v4/javascript_nodejs/2.echobot-with-counter
[Deploy javascript_ts/2.echobot-with-counter]: https://azuredeploy.net?repository=https://github.com/microsoft/BotBuilder-Samples/tree/v4/javascript_ts/2.echobot-with-counter
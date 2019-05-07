# Adaptive Dialog

**Pre-read:** [Dialogs library][1] in Bot Framework V4 SDK.

Dialogs are a central concept in the Bot Framework SDK, and provide a way to manage a conversation with the user. Bot Framework V4 SDK Dialogs library offers [waterfall dialogs][3], [prompts][2] and [component dialogs][4] as built-in constructs to model conversations via Dialogs. These set of dialog types offered by the SDK put you in control of managing your bot's conversations. However, they also require you to write a bunch of boiler plate code for sophisticated conversation modelling concepts like building a dialog dispatcher, ability to handle interruptions elegantly and to build a pluggable, extensible dialog system.

The new **Adaptive dialog** is a new way to model conversations that takes the best of waterfall dialogs and prompts and simplifies sophisticated conversation modelling primitives like building a dialog dispatcher and ability to handle interruptions elegantly. The new **Adaptive dialog** and the event model simplify sophisticated conversation modelling primitives, eliminate much of the boiler plate code and helps you focus on the model of the conversation rather than the mechanics of dialog management. An Adaptive dialog is a derivative of a Dialog and interacts with the rest of the SDK dialog system.

## Getting started
To get started, you can check out the various samples [here][5]. The following are additional documents to help you get oriented with some of the new concept introduced with Adaptive dialogs:  
1. [Why Adaptive dialog?](#Why-Adaptive-Dialog)
2. [New memory model overview][6]
3. [Adaptive dialogs - anatomy][7]
4. [Adaptive dialogs - runtime behavior][8]
5. [Recognizers, rules and steps references][9]
6. [Language generation][17]
6. [Debugging Adaptive Dialog][10]
7. [Packages](#Packages-and-source-code)
8. [Reporting issues](#Reporting-issues)

## Why Adaptive dialog?
We set out with the following goals for Adaptive dialogs - 
* It enables you to think and model conversations as a sequence of steps but allows for rules to **dynamically adjust to context** - especially when users do not provide requested information in order, want to start a new conversation about something else while they are in the middle of an active dialog, etc. 
* It supports and sits on top of a **rich event system** for dialogs and so modelling interruptions, cancellation and execution planning semantics are lot easier to describe and manage.
* It brings input recognition, event handling via rules, model of the conversation (dialog) and output generation into one **cohesive, self-contained** unit. 
* It supports **extensibility** points for recognition, event rules and machine learning.
* It was designed to be **declarative** from the start

## Packages and source code
Packages for C# are available on [BotBuilder MyGet][14]. We will update this section once packages for JS is available.
Source code: 
- [C# repository][15]
- [JS repository][16]

## Reporting issues
You can report any issues you find or feature suggestions on our GitHub repositories
- [BotBuilder C# GitHub repository][12]
- [BotBuilder JS GitHub repository][13]

[1]:https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-dialog?view=azure-bot-service-4.0
[2]:https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-dialog?view=azure-bot-service-4.0#prompts
[3]:https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-dialog?view=azure-bot-service-4.0#waterfall-dialogs
[4]:https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-dialog?view=azure-bot-service-4.0#component-dialog
[5]:./csharp_dotnetcore
[6]:./docs/memory-model-overview.md
[7]:./docs/anatomy-and-runtime-behavior.md#anatomy-adaptive-dialog
[8]:./docs/anatomy-and-runtime-behavior.md#runtime-behavior-adaptive-dialog
[9]:./docs/recognizers-rules-steps-reference.md
[10]:./docs/debugger-extension.md
[12]:https://github.com/microsoft/botbuilder-dotnet/issues
[13]:https://github.com/microsoft/botbuilder-js/issues
[14]:https://botbuilder.myget.org/gallery
[15]:https://github.com/Microsoft/botbuilder-dotnet/tree/4.next
[16]:https://github.com/Microsoft/botbuilder-js/tree/4.next
[17]:./docs/language-generation.md

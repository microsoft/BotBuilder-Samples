# Adaptive dialog - Declarative

The new **Adaptive dialog** is a new way to model conversations that takes the best of waterfall dialogs and prompts and simplifies sophisticated conversation modelling primitives like building a dialog dispatcher and ability to handle interruptions elegantly. The new **Adaptive dialog** and the event model simplify sophisticated conversation modelling primitives, eliminate much of the boiler plate code and helps you focus on the model of the conversation rather than the mechanics of dialog management. An Adaptive dialog is a derivative of a Dialog and interacts with the rest of the SDK dialog system.

See [here][1] to learn more about Adaptive dialogs. 

This section and related samples [EchoBot][2] and [AdaptiveBot][3] document and demonstrate declarative (.dialog) file definition for adaptive dialogs. This is particularly helpful when considered as the standard serialization format by UI based experiences for dialog definition.  

## Why Adaptive dialog?
We set out with the following goals for Adaptive dialogs - 
* It enables you to think and model conversations as a sequence of steps but allows for rules to **dynamically adjust to context** - especially when users do not provide requested information in order, want to start a new conversation about something else while they are in the middle of an active dialog, etc. 
* It supports and sits on top of a **rich event system** for dialogs and so modelling interruptions, cancellation and execution planning semantics are lot easier to describe and manage.
* It brings input recognition, event handling via rules, model of the conversation (dialog) and output generation into one **cohesive, self-contained** unit. 
* It supports **extensibility** points for recognition, event rules and machine learning.
* It was designed to be **declarative** from the start

## Declarative properties
- See [here][6] for the anatomy and runtime behavior for adaptive dialogs.

## Debugging Adaptive dialogs
You can use the [Dialog debugger for Visual Studio Code][7] to debug through Adaptive dialogs (both code based as well as declarative).
## Packages
All packages used by the samples are available on the [BotBuilder MyGet feed][4].

## Packages and source code
Packages for C# are available on [BotBuilder MyGet][14]. We will update this section once packages for JS is available.
Source code: 
- [C# repository][15]
- [JS repository][16]

## Reporting issues
You can report any issues you find or feature suggestions on our GitHub repositories
- [BotBuilder C# GitHub repository][12]
- [BotBuilder JS GitHub repository][13]

[1]:../README.md
[2]:./10.EchoBot
[3]:./AdaptiveBot
[4]:https://botbuilder.myget.org/gallery/botbuilder-declarative
[6]:../docs/anatomy-and-runtime-behavior.md
[7]:https://marketplace.visualstudio.com/items?itemName=tomlm.vscode-dialog-debugger
[12]:https://github.com/microsoft/botbuilder-dotnet/issues
[13]:https://github.com/microsoft/botbuilder-js/issues
[14]:https://botbuilder.myget.org/gallery
[15]:https://github.com/Microsoft/botbuilder-dotnet/tree/4.next
[16]:https://github.com/Microsoft/botbuilder-js/tree/4.next

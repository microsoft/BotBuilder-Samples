## Installing CLI tools

The Bot Framework now has CLI tools to help quickly create bots and bot-specific resources such as LUIS and QnA Maker. Additionally, it's now possible for users to deploy bots from their CLI using these tools.

|   | Tool | Description |
|---|------|--------------|
| [![npm version](https://badge.fury.io/js/chatdown.svg)](https://badge.fury.io/js/chatdown) | [Chatdown](packages/Chatdown) | Prototype mock conversations in markdown and convert the markdown to transcripts you can load and view in the new V4 Bot Framework Emulator |
| [![npm version](https://badge.fury.io/js/msbot.svg)](https://badge.fury.io/js/msbot) |[MSBot](packages/MSBot)| Create and manage connected services in your bot configuration file|
| [![npm version](https://badge.fury.io/js/ludown.svg)](https://badge.fury.io/js/ludown) |[LUDown](packages/Ludown)| Build LUIS language understanding models using markdown files|
| [![npm version](https://badge.fury.io/js/luis-apis.svg)](https://badge.fury.io/js/luis-apis) |[LUIS](packages/LUIS)| Create and manage your [LUIS.ai](http://luis.ai) applications |
| [![npm version](https://badge.fury.io/js/qnamaker.svg)](https://badge.fury.io/js/qnamaker) |[QnAMaker](packages/QnAMaker) | Create and manage [QnAMaker.ai](http://qnamaker.ai) Knowledge Bases. |
| [![npm version](https://badge.fury.io/js/botdispatch.svg)](https://badge.fury.io/js/botdispatch) | [Dispatch](packages/Dispatch) | Build language models allowing you to dispatch between disparate components (such as QnA, LUIS and custom code)|
| [![npm version](https://badge.fury.io/js/luisgen.svg)](https://badge.fury.io/js/luisgen)| [LUISGen](packages/LUISGen) | Auto generate backing C#/Typescript classes for your LUIS intents and entities.|

## Install CLI tools:
Pre-requisites:
- [Node.js](https://nodejs.org/) version 10.14 or higher
- [dotnetcore 2.1](https://www.microsoft.com/net/download/dotnet-core/2.1) for `luisgen` and `botdispatch`

Windows users can type the following into their CLI to install the tools:
```bash
npm i -g chatdown msbot ludown luis-apis qnamaker botdispatch luisgen
```

For Linux users, to install NPM modules globally you need to use `sudo`:
```bash
sudo npm i -g chatdown msbot ludown luis-apis qnamaker botdispatch luisgen
```
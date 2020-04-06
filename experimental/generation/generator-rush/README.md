

![Bot Framework CLI](./media/BFCLI-header.png)

# BF Command Line Interface
[![Build Status](https://fuselabs.visualstudio.com/SDK_Public/_apis/build/status/microsoft.botframework-cli?branchName=master)](https://fuselabs.visualstudio.com/SDK_Public/_build/latest?definitionId=713&branchName=master)

The new BF Command Line Interface (CLI) tool replaces the collection of standalone tools used to manage Bot Framework bots and related services. We have ported most tools and are in process of porting the rest. The new BF CLI aggregates the collection of cross-platform tools into one cohesive and consistent interface.

The old tools will be deprecated in subsequent releases. All new investments, bug fixes, and new features will be implemented in the new consolidated BF CLI alone.

## Installation

BF is based on the Node.js platform and the [OClif](https://github.com/oclif/oclif) framework where it inherits its command line parsing style, and plugin architecture platform. 

You must download the following prerequisites:

* [Node.js](https://nodejs.org/) version 10.14.1 or higher

Install the tool using the following command: 

~~~
$ npm i -g @microsoft/botframework-cli
$ bf
~~~

## Available Commands
The following commands are currently available:
* [Chatdown][1] 
* [QnAMaker][2]
* [Config](https://github.com/microsoft/botframework-cli/tree/master/packages/cli#bf-config)
* [Luis][3]

#### Future Commands
The following commands will be ported in upcoming releases:
* Dispatch

See [Porting Map](https://github.com/microsoft/botframework-cli/blob/master/PortingMap.md) for a mapping reference between old and new tools

## Overview

The  Bot Framework Command Line Interface (BF CLI) cross-platform tool is used to manage Bot Framework bots and related services. It is part the [Microsoft Bot Framework](https://github.com/Microsoft/botframework), a comprehensive framework for building enterprise-grade conversational AI experiences.  In particular, BF CLI provides fundamental functionality when used in conjunction with Continuous Integration, and Continuous Deployment (CI/CD) pipelines. 

As you build your bot, you may also need to integrate AI services like [LUIS.ai](http://luis.ai) for language understanding, [QnAMaker.ai](http://qnamaker.ai) for your bot to respond to simple questions in a Q&A format, and more. The _[bf luis](./packages/cli#bf-luis)_ command is used to convert, and translate language definition _.lu_ files or generate corresponding source (C# or JavaScript) code. Then, use the [Luis Tool](https://github.com/microsoft/botbuilder-tools/tree/master/packages/LUIS)  to deploy the local files, train, test, and publish them as Language Understanding models within the LUIS service. If used to define QnAMaker question/answer Knowledgebase, use the _[bf qnamaker](./packages/cli#bf-qnamaker)_ command to create and manage QnAMaker assets both locally, and on the QnAMaker service. Please refer to the[ _lu_ library documentation](./packages/lu)  for extended discussion on how to work with .lu file formats. _Note: You may be familiar with the Luis command if you used the legacy [LuDown](https://github.com/microsoft/botbuilder-tools/tree/master/packages/Ludown) and [LuisGen](https://github.com/microsoft/botbuilder-tools/tree/master/packages/LUISGen) tools._

As your bot grows in sophistication, use [Dispatch](https://github.com/Microsoft/botbuilder-tools/tree/master/packages/Dispatch) CLI  to create and evaluate LUIS models used to dispatch intent across multiple bot modules such as LUIS models, QnA knowledgebases, and assist in routing messages to backend bot skills.

To test and refine your bot, you can use the new [V4 Bot Framework Emulator](https://github.com/Microsoft/BotFramework-Emulator/releases). The Bot Framework Emulator is a cross-platform [Electron](https://electronjs.org/) application that enables you to test and debug your bots on local machine or in the cloud.

Also, during early designs stages you may want to create mockup of conversations between the user and the bot for the specific scenarios your bot will support. Use [bf chatdown](./packages/cli#bf-chatdown) command to author conversation mockup .chat files and convert them into rich transcripts and view the conversations in the the Emulator. 

Lastly, with the [Azure CLI Bot extension](./AzureCli.md) (_az bot_ command), you can create, download, publish, configure channels with the [Azure Bot Service](https://azure.microsoft.com/en-us/services/bot-service/). It is a plugin that extends the functionality of Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli?view=azure-cli-latest) to manage your Azure Bot Service assets.


### See Also
* [Detailed Usage Information](https://github.com/microsoft/botframework-cli/tree/master/packages/cli)
* [Bot Framework Homepage](https://dev.botframework.com/)
* [Azure Bot Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
* [LUIS](https://docs.microsoft.com/en-us/azure/cognitive-services/luis/index)
* [QnAMaker](https://docs.microsoft.com/en-us/azure/cognitive-services/qnamaker/)
* [Bot Design Guidelines](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-design-principles)

## Issues and Feature Requests
Please file issues and feature requests [here](https://github.com/microsoft/botframework-cli/issues). 

Also, see current [known issues](https://github.com/microsoft/botframework-cli/labels/known-issues).

Thank you for your support.

## Privacy
Privacy is very important to us. BF CLI contains optional instrumentation that is designed to help us improve the tool based on **anonymous** usage patterns. __It is disabled, opted-out by default__. If you elect to opt-in, we will gather some usage data as follows:
* Command group calls
* Flags used **excluding** specific values (i.e. if used parameter _--folder:name_, we will only gather the use of _--folder_ but will not capture _name_).

To disable data collection see the  __*bf config*__ command.


Please refer to [Microsoft Privacy Statement](https://privacy.microsoft.com/en-US/privacystatement) for more details.

## Nightly builds

Nightly builds are generated using the latest code. Therefore, they may not be stable, and most likely lack up to date documentation. These builds are better suited for more experienced users, although everyone is welcome to use them and provide feedback.

You can get the latest nightly build of bot framework cli from the [BotBuilder MyGet](https://botbuilder.myget.org/gallery) feed. To install the nightly -

```shell
npm config set registry https://botbuilder.myget.org/F/botframework-cli/npm/
```

Install using npm:
```shell
npm i -g @microsoft/botframework-cli
```

To reset registry:
```shell
npm config set registry https://registry.npmjs.org/
```

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

[1]:./packages/chatdown/README.md
[2]:./packages/qnamaker/README.md
[3]:./packages/luis/README.md

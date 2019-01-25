# Bot Prerequisites
This bot has prerequisites that must be installed in order for the bot to function properly.

This document will enumerate the required prerequisites and show how to install them.

## Overview
This bot uses [QnA Maker Service][1], an AI based cognitive service, to implement a powerful question and answer service from your semi-structured content.  The Bot Framework provides a set of CLI tools that will help setup QnA Maker so the bot can be run and tested locally.  Additionally, prerequisites are provided that will enable the bot to be deployed to Azure using additional CLI tools.

## Prerequisites
- [Node.js][4] version 8.5 or higher.
- If you don't have an Azure subscription, create a [free account][5].
- Install the latest version of the [Azure CLI][6] tool. Version 2.0.54 or higher.
- Install latest version of the `MSBot` CLI tool. Version 4.3.2 or higher.
    ```bash
    # install msbot CLI tool
    npm install -g msbot
    ```
- Install latest version of the `QnAMaker` CLI tool. Version 1.1.0 or higher.
    ```bash
    # install QnA Maker CLI tool
    npm install -g qnamaker
    ```

[Return to README.md][3]

# Further reading
The sample will use `msbot` to provision all the service resources this sample requires.  Specifically, the sample will use `msbot` to provision a QnA Maker service application.

The following links document how to create a QnA Maker service manually instead of using `msbot` to do the provisioning for you.  The sample will use `msbot`, but incase you want to understand the manual steps, they are cateloged below.

- [QnA Maker][7] service application
    - Follow instructions [here][9] to create a QnA Maker service.
    - Follow instructions [here][10] to import the [smartLightFAQ.tsv](cognitiveModels/smartLightFAQ.tsv) to your newly created QnA Maker service.
    - Update [qnamaker.bot](qnamaker.bot) with your QnAMaker-Host, QnAMaker-KnowledgeBaseId and QnAMaker-EndpointKey. You can find this information under "Settings" tab for your QnA Maker Knowledge Base at [QnAMaker.ai][7].


[1]: https://www.qnamaker.ai
[3]: ./README.md
[4]: https://nodejs.org
[5]: https://azure.microsoft.com/free/
[6]: https://docs.microsoft.com/cli/azure/install-azure-cli?view=azure-cli-latest
[7]: https://www.qnamaker.ai
[8]: https://dotnet.microsoft.com/download
[9]: https://docs.microsoft.com/en-us/azure/cognitive-services/qnamaker/how-to/set-up-qnamaker-service-azure
[10]: https://docs.microsoft.com/en-us/azure/cognitive-services/qnamaker/quickstarts/create-publish-knowledge-base#create-a-qna-maker-knowledge-base

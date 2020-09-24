# Adaptive Cards Schema 2.0 Sample

This bot has been created using [Bot Framework](https://dev.botframework.com). It shows how to create a simple bot that returns a series of Adaptive Cards that leverage the Adaptive Card 2.0 schema that includes Action.Execute and the Invoke Activity processing to handle these Action.Execute requests.

## Prerequisites

- [.NET Core SDK](https://dotnet.microsoft.com/download) version 3.1

  ```bash
  # determine dotnet version
  dotnet --version
  ```

## Key concepts in this sample

The solution includes a Catering bot that asks users for an entre and drink order. Orders are stored in a Cosmos DB. The key conecepts include:

1. How to return an Adaptive Card that uses schema 2.0
2. How to use Action.Execute in the JSON of an Adaptive Card
3. How to perform an automatic card refresh of an Adaptive Card 2.0
4. How to process the Invoke Activities that an Action.Execute emits
5. How to respond to an Action.Execute Invoke
6. How to update Web Chat to use Adaptive Cards 2.0

## To try this sample

- Clone the repository

    ```bash
    git clone https://github.com/microsoft/botbuilder-samples.git
    ```
- Create a bot registration in the azure portal for the `Catering Bot` and update [appsettings.json](CateringBot)
- Add the Microsoft AppID and Password to the appsettings.json file
- Regsiter for the Direct Line channel and copy the secret into the appsettings.json file
- Create a Cosmos DB in Azure and add the endpoint and key to the appsettings.json file  
- Deploy your bot to Azure and navigate to the home page
- You can use a tunnelling tool such as ngrok to point your bot registration to your local machine; Currently the Bot Framework Emulator does not support Adaptive Cards with schema 2.0


## Deploy the bots to Azure

To learn more about deploying a bot to Azure, see [Deploy your bot to Azure](https://aka.ms/azuredeployment) for a complete list of deployment instructions.

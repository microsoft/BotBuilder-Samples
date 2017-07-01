# Natural Language over Azure Search Sample
A common need in a Bot is to be able to use natural language to filter over a large database to select one more items.  
This sample includes a generic bot control for achieving this over an Azure Search index. The bot control supports both freeform and guided dialogs in order to help learn what you can ask.  
The bot control is demonstrated through a bot that allows picking multiple items out of a 5 million item real estate index with queries like "3+ bedroom house in seattle with a fireplace for less than $800k".  
This query shows a mixture of meta-data comparisons ("3+ bedroom", "less than $800k"), attributes ("house", "seattle") and keywords ("fireplace").

[![Deploy to Azure][Deploy Button]][Deploy CSharp/NLSearch/RealEstateBot]

[Deploy Button]: https://azuredeploy.net/deploybutton.png
[Deploy CSharp/Search/RealEstateBot]: https://azuredeploy.net?repository=https://github.com/microsoft/BotBuilder-Samples/tree/master/CSharp/demo-NLSearch/RealEstateBot

## Prerequisites

The minimum prerequisites to run this sample are:
* The latest update of Visual Studio 2017. You can download the community version [here](http://www.visualstudio.com) for free.
* The Bot Framework Emulator to run locally. To install the Bot Framework Emulator, download it from [here](https://aka.ms/bf-bc-emulator). Please refer to [this documentation article](https://docs.botframework.com/en-us/csharp/builder/sdkreference/gettingstarted.html#emulator) to know more about the Bot Framework Emulator.

## Azure Search

The sample uses [Azure Search](https://azure.microsoft.com/en-us/services/search/) as the backend. This is an Azure service that offers most of the needed pieces of functionality, including keyword search, built-in linguistics, custom scoring, faceted navigation and more. 
Azure Search can also index content from various sources (Azure SQL DB, DocumentDB, Blob Storage, Table Storage), supports "push" indexing for other sources of data, and can crack open PDFs, Office documents and other formats containing unstructured data. 
The content catalog goes into an Azure Search index, which we can then query from a bot control dialog.

> As a good practice, all the Azure Search specific components are implemented in the [Search.Azure](Search.Azure/) project while implementation agnostic interfaces and models can be found in the [Search.Contracts](Search.Contracts/)  project.

## Reusable components
Tools and dialogs you can reuse in your own projects.
* [Search.Tools.Extract](Core\Search.Tools.Extract\Readme.md) : A tool for analyzing an Azure Search index in order to generate a description of the schema and contents of the index.
* [Search.Tools.Generate](Core\Search.Tools.Generate\Read.md) : A tool for taking the meta-data with a LUIS template that supports comparisons to make a custom LUIS model.
* [SearchDialog](Core\Search.Dialogs\AzureSearchDialog.cs) : A generic Search Bot Control that uses the custom LUIS model to interpret user conversation for finding matches in a generic search provider.  The control is multi-turn and supports both guided and freefrom interactions.
* [AzureSearchDialog](Core\Search.Dialogs\AzureSearchDialog.cs) : A generic Azure Search Bot Control that specializes SearchDialog for Azure Search.
* [Microsoft.LUIS.API](Core\Microsoft.LUIS.API) : A library for programattically manipulating LUIS models.

To stitch together multiple instances of these dialogs and have filters and other search options carry over, you can use a shared instance of [SearchQueryBuilder](Search.Contracts/Models/SearchQueryBuilder.cs), which captures all the search-related state.

If you want to apply this to your own Azure search instance you should follow these steps:
* extract <searchServiceName> <searchIndexName> <searchAdminKey> : this will result in a schema file named <searchIndexName>.json file that defines the schema and some default annotations.
* generate <schemaFile> -l <LUIS Subscription Key> -u : Generate a LUIS model <schemaFilename>Model by combining a template LUIS model with the schema information and upload it to your LUIS account.
* Run your application against this LUIS model as in the samples.

At this point you can add additional examples to your LUIS model using the LUIS portal, but you should not modify any of the phrase lists that come from the schema file.  To that you need to add annotations to the schema
file and then do:
* generate <schemaFile> -l <LUIS Subscription Key> -tm <LUIS Model Name> -u : This will download the existing <LUIS Model name>, modify it with the information from <schemaFile> and then upload it again.

### Samples

1. RealEstateBot is a bot for exploring a real estate catalog. 
  It starts by taking an arbitrary set of keywords.
  
  | Emulator | Facebook | Skype |
  |----------|-------|----------|
  |![Search](images/realstate-keywords-emulator.png)|![Search](images/realstate-keywords-facebook.png)|![Search](images/realstate-keywords-skype.png)|
  
  From there you can go back and forth between keyword search and refinement using region, city and type of property.
  
  | Emulator | Facebook | Skype |
  |----------|-------|----------|
  |![Search](images/realstate-refine-emulator.png)|![Search](images/realstate-refine-facebook.png)|![Search](images/realstate-refine-skype.png)|

  You can pick one or more properties and at the end you'll get a list of your choices (a real bot would probably contact your agent with your elections, or send you a summary email for future reference).
  
  | Emulator | Facebook | Skype |
  |----------|-------|----------|
  |![Search](images/realstate-pick-emulator.png)|![Search](images/realstate-pick-facebook.png)|![Search](images/realstate-pick-skype.png)|

  https://realestate-sample.search.windows.net

2. JobListingBot is a bot for browing a catalog of job offerings.
  It starts by asking for a top-level refinement, a useful things to do in order to save users from an initial open-ended interation with the bot where they don't know what they can say.
  
  | Emulator | Facebook | Skype |
  |----------|-------|----------|
  |![Search](images/joblisting-refine-emulator.png)|![Search](images/joblisting-refine-facebook.png)|![Search](images/joblisting-refine-skype.png)|

> The sample targets a shared, ready-to-use [Azure Search service](https://realestate-sample.search.windows.net) with sample real estate data, so you don't need to provision your own to try it out. 

### More Information

To get more information about how to get started in Bot Builder for .NET please review the following resources:

* [Dialogs](https://docs.botframework.com/en-us/csharp/builder/sdkreference/dialogs.html)
* [Azure Search](https://azure.microsoft.com/en-us/services/search/)

> **Limitations**  
> The functionality provided by the Bot Framework Activity can be used across many channels. Moreover, some special channel features can be unleashed using the [ChannelData property](https://docs.botframework.com/en-us/csharp/builder/sdkreference/channels.html).
> 
> The Bot Framework does its best to support the reuse of your Bot in as many channels as you want. However, due to the very nature of some of these channels, some features are not fully portable.
> 
> The features used in these samples are fully supported in the following channels:
> - Skype
> - Facebook
> - Telegram
> - DirectLine
> - WebChat
> - Slack
> - GroupMe
> 
> They are also supported, with some limitations, in the following channel:
> - Email
> 
> On the other hand, they are not supported and the sample won't work as expected in the following channels:
> - SMS
> - Kik

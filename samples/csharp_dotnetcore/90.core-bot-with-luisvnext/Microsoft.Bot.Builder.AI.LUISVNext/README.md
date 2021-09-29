# LUISVNext Recognizer

The LUISVNext Recognizer is built to support the LUISVNext service in your bot. You can utilize the recognizer to connect your bot to a [LUISVNext][LuisVNext_ServiceDocHomepage] model, which would enhance the bot with the language understanding features provided by the AI based cognitive service. LuisVNext is considered the new iteration of LUIS and provides some advantages to its users; namely they are:

-   Natively multilingual models that enable you to train in one language and test in others.
-   Simpler model building experience.
-   Robust classification and extraction models.

## Samples

The best way to learn how to use a new coding interface is by looking at a sample! The following sample: [90.core-bot-with-luisvnext][CoreBotWithLUISVNext_Sample] utilizes the LUISVNext Recognizer to connect CoreBot with the LUISVNext service. 

## Prerequisites

To incorporate this LUISVNext Recognizer in your bot follow the following steps.

### Install .NET Core CLI

- [.NET Core SDK](https://dotnet.microsoft.com/download) version 3.1

  ```bash
  # determine dotnet version
  dotnet --version
  ```

### Create a LUISVNext Application to enable language understanding

Refer to the [following documentation](https://github.com/Azure/clu-private-preview/blob/main/articles/how-to-create-project.md) on how to create a new LUISVnext application. Note that you will require the following three fields in order to be able to connect your LUISVNext project to your recognizer:

- Project Name 
- Endpoint Key
- Endpoint 

 LUISVNext will provide you with the sample request from which you will be able to extract the previous three strings. The Endpoint key will be present in the request header with the key `Ocp-Apim-Subscription-Key`. The URI will contain the other two strings in the following format:
 
 `[Endpoint]//language/:analyze-conversations?projectName=[Project Name]...`


## Including the Recognizer in your solution

To start using the recognizer, you may either choose to use our sample as a starting point for your project or to include the recognizer in your project directly. 

### To start from a prebuilt sample bot

Refer to the following page on how to run the [90.core-bot-with-luisvnext][CoreBotWithLUISVNext_Sample] sample, then feel free to change the sample to suit your needs.

### To include the project in your solution

- Clone this repository

    ```bash
    git clone https://github.com/microsoft/botbuilder-samples.git
    ```

- Include this project (`Microsoft.Bot.Builder.AI.LuisVNext`) in your solution
  - Copy the folder `Microsoft.Bot.Builder.AI.LuisVNext` and paste it into your solution workspace
  - Launch your solution through visual studio
  - Right Click on your solution -> Add -> Existing project and navigate to the `Microsoft.Bot.Builder.AI.LuisVNext.csproj` in your solution

- You now have access to the exposed LUISVNext Recognizer as well as the LuisVNextOptions and Application classes. See the [following section](#Using-the-Recognizer-in-code) on how to use the recognizer.

## Using the Recognizer in code

This section will illustrate how to instantiate and call the `LuisVNextRecognizer`, to see it how it can be used in the context of a fully functional bot, please refer to the sample [90.core-bot-with-luisvnext](https://github.com/microsoft/CognitiveServicesLanguageUtilities/tree/AhmedLeithy/BotBuilderInit/BotFramework/BotBuilderDotNet/RecognizerExtensions/Samples/13.core-bot).

#### Setting up and Constructing the recognizer

To Instantiate the recognizer, you will need to supply your applications credenials in a `LuisApplication` instance as follows:

```C#
var projectName = "<Insert project name here>";
var endpointKey = "<Insert endpoint access key>";
var endpoint = "<Insert endpoint here>";

var myApplication = new LuisApplication(projectName, endpointKey, endpoint);
```

Your LuisApplication can now be used to create your `LuisVNextOptions` instance in a similar fashion to previous iterations of the Luis Recognizer. You may set other options related to your prediction preferences as shown below:

```C#
var myOptions = new LuisVNextOptions(myApplication)
{
    Verbose = true,
    IsLoggingEnabled = true,
    Language = "en",
    ApiVersion = "2021-07-15-preview",
    Slot = LuisSlot.Production
};
```

All option parameters are optional. If the user chooses to not include them, they will either be omitted from request paramaters or default values will be used depending on the property.

You are now ready to instantiate and use the recognizer. It can simply be constructed as follows:

```C#
var myRecognizer = new LuisVNextRecognizer(myOptions);
```

#### Calling RecognizeAsync()

When it is time to query the LuisVNext service, the recognizer offers a RecognizeAsync() method to perform this action. It can be called as follows: 

```C#
RecognizerResult result = await myRecognizer.RecognizeAsync(turnContext, cancellationToken);
```
The turn context will contain the message recieved by the bot, which is used as a query to the LuisVNext service. Much like any other Recognizer, the `RecognizeAsync` returns an object of type `RecognizerResult`, which has the following properties:

- `Text`: the original user Query
- `AlteredText`: an altered version of the query (does not apply to LuisVNext)
- `Intents`: A <string, IntentScore> dictionary which contains a value `IntentScore` for every key intent name that is included in the service response.  
- `Properties`: A dicionary containing additional tokens of the response. Namely, they are :
  - `topIntent`
  - `projectType`
- `Entities`: A JObject that contains the entities detected by LUISVNext and returned in the JSON Response. It is an object that simply contains the entities token, which corresponds to a list of Entities detected by LUIS. An example of what the JSON object might look like is shown below:

    Query: I want to book a flight from London to Cairo
    ```JSON
    "entities": [
                {
                    "category": "fromCity",
                    "text": "London",
                    "offset": 29,
                    "length": 6,
                    "confidenceScore": 0.81079376
                },
                {
                    "category": "toCity",
                    "text": "Cairo",
                    "offset": 39,
                    "length": 5,
                    "confidenceScore": 0.5041834
                }
            ],
    ```
Note that `RecognizeAsync` also has a template overload of type `RecognizeAsync<T>`, where T implements the interface IRecognizerConvert. This is useful for creating a wrapper model for the `RecognizerResult` object, as can be found in the sample [90.core-bot-with-luisvnext][CoreBotWithLUISVNext_Sample].

## Further reading

- [Bot Framework Documentation](https://docs.botframework.com)
- [Bot Basics](https://docs.microsoft.com/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0)
- [Dialogs](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-dialog?view=azure-bot-service-4.0)
- [Gathering Input Using Prompts](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-prompts?view=azure-bot-service-4.0&tabs=csharp)
- [Activity processing](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-activity-processing?view=azure-bot-service-4.0)
- [Azure Bot Service Introduction](https://docs.microsoft.com/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Azure Bot Service Documentation](https://docs.microsoft.com/azure/bot-service/?view=azure-bot-service-4.0)
- [.NET Core CLI tools](https://docs.microsoft.com/en-us/dotnet/core/tools/?tabs=netcore2x)
- [Azure CLI](https://docs.microsoft.com/cli/azure/?view=azure-cli-latest)
- [Azure Portal](https://portal.azure.com)
- [Language Understanding using LUIS](https://docs.microsoft.com/en-us/azure/cognitive-services/luis/)
- [Channels and Bot Connector Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-concepts?view=azure-bot-service-4.0)

[CoreBotWithLUISVNext_Sample]: https://github.com/microsoft/CognitiveServicesLanguageUtilities/tree/AhmedLeithy/BotBuilderInit/BotFramework/BotBuilderDotNet/RecognizerExtensions/Samples/13.core-bot
[LuisVNext_ServiceDocHomepage]: https://www.LuisVNext.placeholder.com

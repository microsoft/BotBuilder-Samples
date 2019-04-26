This sample demonstrates using [Adaptive dialog][1],  [Language Generation][2] PREVIEW features with [LUIS][5] to demonstrate an end-to-end ToDo bot in action.

Preview NuGet packages used by this sample is available [here][4].

# To try this sample
- Clone the repository.
```bash
git clone https://github.com/microsoft/botbuilder-samples.git
```
- Get set up with the LUIS application - instructions [here][6]
# Running Locally
## Visual Studio
- Navigate to the samples folder (`botbuilder-samples/samples/csharp_dotnetcore/06.using-cards`) and open CardsBot.csproj in Visual Studio
- Run the project (press `F5` key)

## .NET Core CLI
- Install the [.NET Core CLI tools](https://docs.microsoft.com/dotnet/core/tools/?tabs=netcore2x).
- Using the command line, navigate to `botbuilder-samples/samples/csharp_dotnetcore/06.using-cards`
- Type `dotnet run`.

## Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot
developers to test and debug their bots on localhost or running remotely through a tunnel.
- Install the [Bot Framework emulator](https://aka.ms/botframeworkemulator).

## Connect to bot using Bot Framework Emulator **V4**
- Launch the Bot Framework Emulator.
- Click Open bot -> http://localhost:58261/api/messages and click connect

## LUIS Setup
### Using LUIS portal
- Navigate and sign in to [Luis.ai][5]
- Under "My apps", click on "Import new app"
- Click on "Choose app file (JSON format) ..."
- Select `botbuilder-samples/experimental/adaptive-dialog/csharp_dotnetcore/ToDoBotWithLUIS/CognitiveModels/ToDoLuisBot.luis.json
- Once the application is imported
    - Click on 'Train' to train the application
    - Click on 'Publish' to publish the application.
- Update appsettings.json
    - You can get your 'Authoring key' by following instructions [here][9]
    - You can get your application id and endpoint region by following instructions [here][10]

### Using CLI
- Install [nodejs][8] version 8.5 or higher
- Install [botbuilder-tools][7] CLI
```bash
> npm i -g ludown luis-apis
```
- In a command prompt, navigate to `botbuilder-samples/experimental/adaptive-dialog/csharp_dotnetcore/ToDoBotWithLUIS`
- To parse ToDoBot.lu to a LUIS json model
```bash
> ludown parse toluis --in ./Dialogs/Resources/ToDoBot.lu -o CognitiveModels -n ToDoLuisBot --out ToDoLuisBot.luis.json
```
- To create a new LUIS application using this model. Note: You see [here][9] for instructions on getting your authoirng key.
```bash
> luis import application --in ./CognitiveModels/ToDoLuisBot.luis.json --authoringKey <YOUR-AUTHORING-KEY>
```
- Copy the relevant Application Id, endpoint information as well as your authoring key to appsettings.json.
- To train and publish the LUIS application,
```bash
> luis train version --appId <YOUR-APP-ID> --versionId 0.1 --wait --authoringKey <YOUR-AUTHORING-KEY>
> luis publish version --appId <YOUR-APP-ID> --versionId 0.1 --wait --authoringKey <YOUR-AUTHORING-KEY>
```

[1]:../../README.md
[2]:../../language-generation/README.md
[3]:../../../../samples/csharp_dotnetcore/06.using-cards
[4]:https://botbuilder.myget.org/gallery/botbuilder-declarative
[5]:https://luis.ai
[6]:#LUIS-Setup
[7]:https://github.com/Microsoft/botbuilder-tools
[8]:https://nodejs.org/en/
[9]:https://docs.microsoft.com/en-us/azure/cognitive-services/luis/luis-how-to-account-settings#authoring-key
[10]:https://docs.microsoft.com/en-us/azure/cognitive-services/luis/luis-concept-keys
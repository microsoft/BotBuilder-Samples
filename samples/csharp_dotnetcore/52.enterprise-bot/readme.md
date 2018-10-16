# Enterprise Bot Template


## Concepts introduced in this sample
- Dialogs
- Template Manager
- Dispatch
- Middleware


## To try this sample
1. Install the Bot CLI Tools

    `npm install -g chatdown msbot ludown luis-apis qnamaker botdispatch luisgen`
  
2. Setup Azure Powershell (If you have never done so before)
    - To login, run:
            
        `Connect-AzureRmAccount' to login
    - To select your Azure subscription, run:

        `Select-AzureRmSubscription -Subscription "subscription-name"`
 
3. Collect your Luis Authoring Key from the the luis.ai portal by selecting your name in the top right corner. Save this key for the next step.

4. Run the following command from the project directory:
 
    `msbot clone services --name "<NAME>" --luisAuthoringKey "<YOUR AUTHORING KEY>" --folder "DeploymentScripts/msbotClone" --location "westus" --verbose`

    **NOTE**: By default your Luis Applications will be deployed to your free starter endpoint. An Azure LUIS service will be deployed along with your bot but you must manually add and publish to it from the luis.ai portal and update your key in the .bot file.

5. Update your `appsettings.json` with your .bot file path and .bot file secret (if set).

6. There are two locations that need to be updated in the code to resolve the LUIS service.
In the following files:
    MainDialog.cs
    EnterpriseDialog.cs
The following line needs to be updated in both files:
    var luisService = _services.LuisServices[<YOUR MS BOT NAME>_General];
The <YOUR MS BOT NAME> must be replaced with the <NAME> provided in step 4 above.

7. Check the QnAService name in your `.bot` file and update the following line in MainDialog.cs.
    var qnaService = _services.QnAServices[<YOUR_QNA_SERVICE_NAME>];
The <YOUR_QNA_SERVICE_NAME> must be replaced with the <NAME> of your QnA service.
    
8. Run your bot project and type "hi" to verify your services are correctly configured


## Enabling more scenarios
### Authentication
To enable authentication follow these steps:.....

Register the SignInDialog in the MainDialog constructor
    
    AddDialog(new SignInDialog(_services.AuthConnectionName));


Add the following in your code at your desired location to test a simple login flow:
    
    var signInResult = await dc.BeginAsync(SignInDialog.Name);

### Content Moderation
Content moderation can be used to identify PII and adult content in the messages sent to the bot. To enable this functionality, go to the azure portal
and create a new content moderator service. Collect your subscription key and region to configure your .bot file. 

Add the following code to the bottom of your service.AddBot<>() method in startup to enable content moderation on every turn. 
The result of content moderation can be accessed via your bot state 
    
    // Content Moderation Middleware (analyzes incoming messages for inappropriate content including PII, profanity, etc.)
    var moderatorService = botConfig.Services.Where(s => s.Name == ContentModeratorMiddleware.ServiceName).FirstOrDefault();
    if (moderatorService != null)
    {
        var moderator = moderatorService as GenericService;
        var moderatorKey = moderator.Configuration["key"];
        var moderatorRegion = moderator.Configuration["region"];
        var moderatorMiddleware = new ContentModeratorMiddleware(moderatorKey, moderatorRegion);
        options.Middleware.Add(moderatorMiddleware);
    }

Access the middleware result by calling this from within your dialog stack
        
    var cm = dc.Context.TurnState.Get<Microsoft.CognitiveServices.ContentModerator.Models.Screen>(ContentModeratorMiddleware.TextModeratorResultKey);


## Prerequisites
- NodeJS & Node Package Manager

## Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework emulator from [here](https://github.com/Microsoft/BotFramework-Emulator/releases)

## Connect to bot using Bot Framework Emulator V4
- Launch Bot Framework Emulator
- File -> Open bot and navigate to your .bot file
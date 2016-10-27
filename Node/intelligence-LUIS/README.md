# LUIS Bot Sample

A sample bot using IntentDialog to integrate with a LUIS.ai application.

[![Deploy to Azure][Deploy Button]][Deploy LUIS/Node]
[Deploy Button]: https://azuredeploy.net/deploybutton.png
[Deploy LUIS/Node]: https://azuredeploy.net

### Prerequisites

The minimum prerequisites to run this sample are:
* Latest Node.js with NPM. Download it from [here](https://nodejs.org/en/download/).
* The Bot Framework Emulator. To install the Bot Framework Emulator, download it from [here](https://aka.ms/bf-bc-emulator). Please refer to [this documentation article](https://docs.botframework.com/en-us/csharp/builder/sdkreference/gettingstarted.html#emulator) to know more about the Bot Framework Emulator.
* **[Recommended]** Visual Studio Code for IntelliSense and debugging, download it from [here](https://code.visualstudio.com/) for free.

#### LUIS Application
You can test this sample as-is, using the hosted sample application, or you can import the pre-built [LuisBot.json](LuisBot.json) file to your own LUIS account.

In the case you choose to import the application, the first step to using LUIS is to create or import an application. Go to the home page, www.luis.ai, and log in. After creating your LUIS account you'll be able to Import an Existing Application where can you can select a local copy of the LuisBot.json file an import it. For more information, take a look at [Importing and Exporting Applications](https://www.luis.ai/Help#ImportingApps)

![Import an Existing Application](images/prereqs-import.png)

Once you imported the application you'll need to "train" the model ([Training](https://www.luis.ai/Help#Training)) before you can "Publish" the model in an HTTP endpoint. For more information, take a look at [Publishing a Model](https://www.luis.ai/Help#PublishingModel).

Finally, edit [app.js](app.js#L21-L22) file and update the `LuisModelUrl` variable, or if you are using Visual Studio Code to run the sample edit [launch.json](.vscode/launch.json#L21) and update the `LUIS_MODEL_URL` environment variable.

#### Where to find the Model URL

In the LUIS application's dashboard, click the "Publish" button in the upper left-hand corner, and then "Publish web service" in the resulting window. After a couple of moments, you will see a url that makes your models available as a web service.

![Publishing a Model](images/prereqs-publish.png)

### Code Highlights

One of the key problems in human-computer interactions is the ability of the computer to understand what a person wants, and to find the pieces of information that are relevant to their intent. In the LUIS application, you will bundle together the intents and entities that are important to your task. Read more about [Creating an Application](https://www.luis.ai/Help#CreatingApplication) in the LUIS Help Docs. 

The IntentDialog class lets you listen for the user to say a specific keyword or phrase. We call this intent detection because you are attempting to determine what the user is intending to do. IntentDialogs are useful for building more open ended bots that support natural language style understanding.

The IntentDialog class can be configured to use cloud based intent recognition services like [LUIS](http://luis.ai/) through an extensible set of recognizer plugins. Out of the box, Bot Builder comes with a [LuisRecognizer](https://docs.botframework.com/en-us/node/builder/chat-reference/classes/_botbuilder_d_.luisrecognizer) that can be used to call a machine learning model you have trained via their web site. You can create a LuisRecognizer that is pointed at your model and then pass that recognizer into your IntentDialog at creation time using the [options](https://docs.botframework.com/en-us/node/builder/chat-reference/interfaces/_botbuilder_d_.iintentdialogoptions) structure. Check out how [app.js initializes the IntentDialog](app.js#L21-L26):

````JavaScript
const LuisModelUrl = 'https://api.projectoxford.ai/luis/v1/application?id=...&subscription-key=...';

// Main dialog with LUIS
var recognizer = new builder.LuisRecognizer(LuisModelUrl);
var intents = new builder.IntentDialog({ recognizers: [recognizer] });
````

#### Intent Recognizers

Intent recognizers return matches as named intents. To match against an intent from a recognizer you pass the name of the intent you want to handle to [IntentDialog.matches()](https://docs.botframework.com/en-us/node/builder/chat-reference/classes/_botbuilder_d_.intentdialog#matches) as a string instead of a RegExp. This lets you mix in the matching of regular expressions alongside your cloud based recognition model. See how the bot matches the [`SearchHotels`](app.js#L27), [`ShowHotelsReviews`](app.js#L77) and [`Help`](app.js#L91) intents.

````JavaScript
intents.matches('SearchHotels', [ ...waterfall dialog handler... ]);
intents.matches('ShowHotelsReviews', (session, args) => { ... });
intents.matches('Help', builder.DialogAction.send('Hi! Try asking me things like ...'));
````

#### Entity Recognition

LUIS can not only identify a users intention given an utterance, it can extract entities from their utterance as well. Any entities recognized in the users utterance will be passed to the intent handler via its [`args`](https://docs.botframework.com/en-us/node/builder/chat-reference/interfaces/_botbuilder_d_.iintentrecognizerresult) parameter.

Bot Builder includes an [`EntityRecognizer`](https://docs.botframework.com/en-us/node/builder/chat-reference/classes/_botbuilder_d_.entityrecognizer.html) class to simplify working with these entities. You can use [`EntityRecognizer.findEntity()`](https://docs.botframework.com/en-us/node/builder/chat-reference/classes/_botbuilder_d_.entityrecognizer.html#findentity) and [`EntityRecognizer.findAllEntities()`](https://docs.botframework.com/en-us/node/builder/chat-reference/classes/_botbuilder_d_.entityrecognizer.html#findallentities) to search for entities of a specific type by name. Check out how [city and airport entities are extracted](app.js#L32-L33).

````JavaScript
var cityEntity = builder.EntityRecognizer.findEntity(args.entities, 'builtin.geography.city');
var airportEntity = builder.EntityRecognizer.findEntity(args.entities, 'AirportCode');
````

The `AirportCode` entity makes use of the LUIS Regex Features which helps LUIS infer entities based on an Regular Expression match, for instance, Airport Codes consist of three consecutive alphabetic characters. You can read more about Regex Features in the [Improving Performance](https://www.luis.ai/Help/#Performance) section of the LUIS Help Docs.

![Edit Regex Feature](images/highlights-regex.png)

Another LUIS Model Feature used is Phrase List Features, for instance, the model includes a phrase list named Near which categorizes the words: near, around, close and nearby. Phrase list features work for both words and phrase and what LUIS learns about one phrase will automatically be applied to the others as well.

> Note: Both RegEx and Phrase List are transparent from the Bot's implementation perspective. Think of model features as "hints" used by the Machine Learning algorithm to help categorize and recognize words that compound Entities and Intents.

![Phrase List Feature](images/highlights-phrase.png)

In our sample, we are using a [waterfall dialog](https://docs.botframework.com/en-us/node/builder/chat/dialogs/#waterfall) for the hotel search. This is a common pattern that you'll likely use for most of your intent handlers. The way waterfalls work in Bot Builder is the very first step of the waterfall is called when a dialog (or in this case intent handler) is triggered. The step then does some work and continues execution of the waterfall by either calling another dialog (like a built-in prompt) or calling the optional `next()` function passed in. When a dialog is called in a step, any result returned from the dialog will be passed as input to the results parameter for the next step. 

Our bot tries to check if an entity of city or airport type were [matched and forwards it](app.js#L31-L45) to the next step. If that's not the case, the user is [prompted with a destination](app.js#L44). The [next step](app.js#L47-L49) will receive the destination or airport code in the `results` argument.

````JavaScript
intents.matches('SearchHotels', [
    function (session, args, next) {
        session.send('Welcome to the Hotels finder!');

        // try extracting entities
        var cityEntity = builder.EntityRecognizer.findEntity(args.entities, 'builtin.geography.city');
        var airportEntity = builder.EntityRecognizer.findEntity(args.entities, 'AirportCode');
        if (cityEntity) {
            // city entity detected, continue to next step
            session.dialogData.searchType = 'city';
            next({ response: cityEntity.entity });
        } else if (airportEntity) {
            // airport entity detected, continue to next step
            session.dialogData.searchType = 'airport';
            next({ response: airportEntity.entity });
        } else {
            // no entities detected, ask user for a destination
            builder.Prompts.text(session, 'Please enter your destination');
        }
    },
    function (session, results) {

        var destination = results.response;

        var message = 'Looking for hotels';
        if (session.dialogData.searchType === 'airport') {
            message += ' near %s airport...';
        } else {
            message += ' in %s...';
        }

        session.send(message, destination);
        
        ...
    }]);
````

Similarly, the [`ShowHotelsReviews`](app.js#L77) uses a single closure to search for hotel reviews.

````
intents.matches('ShowHotelsReviews', (session, args) => {
    // retrieve hotel name from matched entities
    var hotelEntity = builder.EntityRecognizer.findEntity(args.entities, 'Hotel');
    if (hotelEntity) {
        session.send('Looking for reviews of \'%s\'...', hotelEntity.entity);
        Store.searchHotelReviews(hotelEntity.entity)
            .then((reviews) => {
                var message = new builder.Message()
                    .attachmentLayout(builder.AttachmentLayout.carousel)
                    .attachments(reviews.map(reviewAsAttachment));
                session.send(message)
            });
    }
});
````

The [`onDefault`](https://docs.botframework.com/en-us/node/builder/chat-reference/classes/_botbuilder_d_.intentdialog.html#ondefault) handler is invoked anytime the users utterance doesn't match one of the registered patterns:

````JavaScript
intents.onDefault((session) => {
    session.send('Sorry, I did not understand \'%s\'. Type \'help\' if you need assistance.', session.message.text);
});
````

> **NOTE:** you should avoid adding a matches() handler for LUIS’s “None” intent. Add a onDefault() handler instead. The reason for this is that a LUIS model will often return a very high score for the None intent if it doesn’t understand the users utterance. In the scenario where you’ve configured the IntentDialog with multiple recognizers that could cause the None intent to win out over a non-None intent from a different model that had a slightly lower score. Because of this the LuisRecognizer class suppresses the None intent all together. If you explicitly register a handler for “None” it will never be matched. The onDefault() handler, however can achieve the same effect because it essentially gets triggered when all of the models reported a top intent of “None”.

### Spelling Correction

IF you want to enable spelling correction, set the `IS_SPELL_CORRECTION_ENABLED` key to `true` in the [.env](.env) file.

Microsoft Bing Spell Check API provides a module that allows you to to correct the spelling of the text. Check out the [reference](https://dev.cognitive.microsoft.com/docs/services/56e73033cf5ff80c2008c679/operations/56e73036cf5ff81048ee6727) to know more about the modules available. 

[spell-service.js](spell-service.js) is the core component illustrating how to call the Bing Spell Check RESTful API.

In this sample we added spell correction as a middleware. Check out the middleware in [app.js](app.js).

````JavaScript
if (process.env.IS_SPELL_CORRECTION_ENABLED == "true") {
    bot.use({
        botbuilder: function (session, next) {
            spellService
                .getCorrectedText(session.message.text)
                .then(text => {
                    session.message.text = text;
                    next();
                })
                .catch((error) => {
                    console.error(error);
                    next();
                });
        }
    })
}
````

### Outcome

You will see the following in the Bot Framework Emulator when opening and running the sample solution.

![Sample Outcome](images/outcome.png)

### More Information

To get more information about how to get started in Bot Builder for Node and LUIS please review the following resources:
* [Bot Builder for Node.js Reference](https://docs.botframework.com/en-us/node/builder/overview/#navtitle)
* [Understanding Natural Language](https://docs.botframework.com/en-us/node/builder/guides/understanding-natural-language/)
* [LUIS Help Docs](https://www.luis.ai/Help/)
* [Cognitive Services Documentation](https://www.microsoft.com/cognitive-services/en-us/luis-api/documentation/home)
* [IntentDialog](https://docs.botframework.com/en-us/node/builder/chat/IntentDialog/)
* [EntityRecognizer](https://docs.botframework.com/en-us/node/builder/chat-reference/classes/_botbuilder_d_.entityrecognizer.html)
* [Alarm Bot in Node](https://github.com/Microsoft/BotBuilder/tree/master/Node/examples/basics-naturalLanguage)
* [Microsoft Bing Spell Check API](https://www.microsoft.com/cognitive-services/en-us/bing-spell-check-api)

> **Limitations**  
> The functionality provided by the Bot Framework Activity can be used across many channels. Moreover, some special channel features can be unleashed using the [Message.sourceEvent](https://docs.botframework.com/en-us/node/builder/chat-reference/classes/_botbuilder_d_.message.html#sourceevent) method.
> 
> The Bot Framework does its best to support the reuse of your Bot in as many channels as you want. However, due to the very nature of some of these channels, some features are not fully portable.
> 
> The features used in this sample are fully supported in the following channels:
> - Skype
> - Facebook
> - DirectLine
> - WebChat
> - Slack
> - GroupMe
> - Telegram
> 
> They are also supported, with some limitations, in the following channels:
> - Kik
> - Email
> 
> On the other hand, they are not supported and the sample won't work as expected in the following channel:
> - SMS

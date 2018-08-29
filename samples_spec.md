This specification outlines sample structure, requirements 
# Goals
- Make it easy for developers to learn basic bot concepts
- Provide a numbering scheme for samples so developers can start simple and layer in sophistication
- Samples targeted at scenarios rather than technology involved
- Consistent set of samples across supported languages
- Samples are also consistently used in the docs topics for continuity and are developed by the documentation team, reviewed by the feature team.
- Each sample MUST include
    - README with steps to run the sample, concepts involved and links to additional reading (docs)
    - deep link to V4 Emulator, include a .bot file as well as point to relevant tools (UI based or CLI) as appropriate
    - .chat files as appropriate that provide scenario overview and demonstrates how to construct mock conversations for the specific scenario
    - .lu files as appropriate
    - Include source JSON model files for LUIS, QnA, Dispatch where applicable
    -	Include LUISGen strong typed LUIS class for LUIS samples
    -	Any required build scripts for sample to work locally
    - Well defined naming convention for setting, service endpoint, and secrets in setting files (.bot, app.json, etc.)
    - Deploy to Azure button including all resources (similar to V3 samples)
- Each sample SHOULD include
    - Welcome message (conversation update activity)
    - Flow control and interruptions (help, cancel, detecting and handling interruptions) 
- All samples are built and deployable on local dev env (Emulator) and Azure (WebChat)
    - Samples (and docs) should add channel specific notes (if applicable). e.g. document list of supported channels for Adaptive cards sample etc.
- JS and TS samples should be in separate folders 

# Samples structure - C#
    | - <sampleName>.bot                        // bot file for this sample
    | - README.md                               // Markdown readme file that includes steps to run this sample 
                                                (including steps to create and configure required services), 
                                                overview of concepts covered in this sample + links to 
                                                additional topics to read
    | - Program.cs                              // Default program.cs
    | - Startup.cs                              // Default startup.cs – configuration builder + middlewares
    | - appsettings.json                        // Has bot configuration information
    
    | - Dialogs    
        | - MainDialog
            | - MainDialog.cs                   // Main router/ dispatcher for the bot
            | - <botName_state>.cs              // state definitions that are shared across dialogs/ components are here
            | - Resources
                | - <scenario>.lu               // LU file that has intents that apply globally - e.g. help/ cancel etc.
                | - <scenario_card>.cshtml      // Cards that are shared across dialogs.                 
      | - <scenario name>           
            | - <scenario>.cs                   // Dialog definition for this scenario
            | - <scenario_state>.cs             // State object definitions for this scenario
            | - Resources
                  | - <scenario>.lu             // LU file with intents + entities/ QnA pairs for this scenario
                  | - <scenario_card>.cshtml    // cards for this particular scenario – template file for cards
                  | - <scenario>.chat           // Chat file for this specific scenario; shows happy path or variations.
    | - CognitiveModels
        | - <bot_name>.luis                     // LUIS model file for this sample
        | - <bot_name>.qna                      // QnA Maker JSON model file
        | - <bot_name>.dispatch                 // Dispatch JSON model file
    | - DeploymentScripts
        | - DEPLOYMENT.md                       // Readme for deployment scripts.   
        | - azuredeploy.json                    // Azure deployment ARM template

# Samples structure - JS

    | - <sample-name>.bot                       // bot file for this sample
    | - README.md                               // Markdown readme file that includes steps to run this sample 
                                                (including steps to create and configure required services), 
                                                overview of concepts covered in this sample + links to 
                                                additional topics to read
    | - index.js                                // Default app.js - startup, middlewares
    | - .env                                    // Has bot configuration information
    | - dialogs 
        | - mainDialog
            | - index.js                        // Main router/ dispatcher for the bot
            | - <bot-name-state>.js             // state definitions that are shared across dialogs/ components are here
            | - resources
                | - <scenario>.lu               // LU file that has intents that apply globally - e.g. help/ cancel etc.                    
      | - <scenario-name>           
            | - index.js                        // Dialog definition for this scenario
            | - <scenario-state>.js             // State object definitions for this scenario
            | - resources
                  | - <scenario>.lu             // LU file with intents + entities/ QnA pairs for this scenario
                  | - <scenario-card>.json      // cards for this particular scenario – template file for cards
                  | - <scenario>.chat           // Chat file for this specific scenario; shows happy path or variations.
    | - cognitiveModels
        | - <bot-name>.luis                     // LUIS model file for this sample
        | - <bot-name>.qna                      // QnA Maker JSON model file
        | - <bot-name>.dispatch                 // Dispatch JSON model file
    | - deploymentScripts
        | - DEPLOYMENT.md                       // Readme for deployment scripts.   
        | - azuredeploy.json                    // Azure deployment ARM template

# README.md template
```markdown
<INSERT AT MOST ONE PARAGRAPH DESCRIPTION OF WHAT THIS SAMPLE DOES> 

# Concepts introduced in this sample
<DESCRIPTION OF THE CONCEPTS>


# To try this sample
-	<STEPS TO CLONE REPO AND GET SETUP>
## Prerequisites
-	<REQUIRED TOOLS, VERSIONS>
-	<STEPS TO GET SET UP WITH THE SAMPLE. E.g. RUN AN INCLUDED SCRIPT OR MANUALLY DO SOMETHING ETC>

NOTE: <ANY NOTES ABOUT THE PREREQUISITES OR ALTERNATE THINGS TO CONSIDER TO GET SET UP>

## Visual studio
-	<STEPS TO RUN THIS SAMPLE FROM VISUAL STUDIO>

## Visual studio code
-	<STEPS TO RUN THIS SAMPLE FROM VISUAL STUDIO CODE>

## Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework emulator from [here](https://github.com/Microsoft/BotFramework-Emulator/releases)

## Connect to bot using Bot Framework Emulator **V4**
- Launch Bot Framework Emulator
- File -> Open bot and navigate to samples\8.AspNetCore-LUIS-Bot folder
- Select AspNetCore-LUIS-Bot.bot file

# Further reading
-	<LINKS TO ADDITIONAL READING>
```

# Samples repo structure, naming conventions
-	All samples will live under the BotBuilder-Samples repository, master branch. 
-	Language/ platform specific samples go under respective folders – ‘dotnet’/ ‘JS’/ ‘Java’/ ‘Python’
-	Samples should use published packages, available on NuGet or npmjs
-	Each sample sits in its own folder
-	Each sample folder is named as “\<\#\>. \<KEY SCENARIO INTRODUCED BY THE SAMPLE\>”
-	Each solution/ project is named as “\<KEY SCENARIO INTRODUCED BY THE SAMPLE\>”
-	C# - each sample has its own solution file

# Samples list
|Sample Name|Description|.NET CORE|.NET Web API|JS (es6)|TS|
|-----------|-----------|---------|------------|--------|--|
|1.Console-EchoBot|Introduces the concept of adapter and demonstrates a simple echo bot on console adapter and how to send a reply and access the incoming the incoming message. Doc topic from readme lists other adapter types|:heavy_check_mark:||:heavy_check_mark:||
|1.a. Browser-EchoBot|Introduces browser adapter|||:heavy_check_mark:||
|2.EchoBot-With-Counter|Demonstrates how to use state. Shows commented out code for all natively supported storage providers. Storage providers should include InMemory, CosmosDB and Blob storage. Default bot template for ABS, VSIX, Generators and dotnet new template|:heavy_check_mark:|:heavy_check_mark:|:heavy_check_mark:|:heavy_check_mark:|
|3.Welcome-User|Introduces activity types and providing a welcome message on conversation update activity. This should be the “who to” sample and tie back to a Concept doc. |:heavy_check_mark:||:heavy_check_mark:||
|4.Simple-Prompt-Bot|Demonstrates prompt pattern by prompting user for a property. Introduces user state .vs. conversation state. Ask for name and prints back those information. Uses sequence dialogs if available or default option is to use waterfall dialogs|:heavy_check_mark:||:heavy_check_mark:||
|5.MultiTurn-Prompts-Bot|Demonstrates more complex pattern by prompting user for multiple properties. Ask for name, age and prints back that information. Uses sequence dialogs if available or default option is to use waterfall dialogs.|:heavy_check_mark:||:heavy_check_mark:||
|6.Using-Cards|Introduces all card types including thumbnail, audio, media etc. Builds on Welcoming user + multi-prompt bot by presenting a card with buttons in welcome message that route to appropriate dialog.|:heavy_check_mark:||:heavy_check_mark:||
|7.Using-Adaptive-Cards|Introduces adaptive cards - demonstrates how the multi-turn dialog can be augmented to also use a card to get user input for name and age.|:heavy_check_mark:||:heavy_check_mark:||
|8.Suggested-Actions|Demonstrates how to use suggested actions |:heavy_check_mark:||:heavy_check_mark:||
|9.Message-Routing|Demonstrates the main dialog or root dispatcher paradigm. Needs to show how to handle interruptions like cancel, help, start over. Tie to conceptual documentation and best practice/ patterns recommendations.|:heavy_check_mark:||:heavy_check_mark:||
|10.Prompt-Validations|Demonstrates how to take advantage of different prompt types and prompt validators. In this example, we will expand the multi-turn prompt sample to accept name, age, date of birth, favorite color.Name uses text prompt with 1min character and 50 max character limitation; Age uses number prompt with valid age between 1 - 99; Date of birth uses date time prompt with valid date of birth between 8/24/1918 through 8/24/2018; Favorite color uses choice prompt with red, blue, green, .. as choices|:heavy_check_mark:||:heavy_check_mark:||
|11.QnAMaker|Demonstrates how to use QnA Maker to have simple single-turn conversations|:heavy_check_mark:|:heavy_check_mark:|:heavy_check_mark:|:heavy_check_mark:|
|12.NLP-With-LUIS|Demonstrates how to use LUIS to understand natural language|:heavy_check_mark:|:heavy_check_mark:|:heavy_check_mark:|:heavy_check_mark:|
|13.Basic-Bot-Template|Basic bot template that puts together cards, NLP (LUIS)|:heavy_check_mark:|:heavy_check_mark:|:heavy_check_mark:|:heavy_check_mark:|
|14.NLP-With-Dispatch|Demonstrates how dispatch should be used E2E|:heavy_check_mark:||:heavy_check_mark:||
|15.Handling-No-Match|Provides guidance on how to handle no-match. Its appropriate to have this covered after LUIS and QnA maker to show post NLP step as well.|:heavy_check_mark:||:heavy_check_mark:||
|16.Handling-Atachments|Demonstrates how to listen for/handle user provided attachments|:heavy_check_mark:||:heavy_check_mark:||
|17.Proactive-Messages|Demonstrates how to send proactive messages|:heavy_check_mark:||:heavy_check_mark:||
|18.Multi-Lingual-Bot|Using translate middleware to support a multi-lingual bot|:heavy_check_mark:||:heavy_check_mark:||
|19.Bot-uthentication|Bot that demonstrates how to integration with auth providers|:heavy_check_mark:||:heavy_check_mark:||
|20.Handling-End-Of-Conversation|Bot that demonstrates how to handle end of conversation events|:heavy_check_mark:||:heavy_check_mark:||
|21.Custom-Dialogs|Demonstrates different ways to model conversations. Waterfall .vs. using your own dialog management|:heavy_check_mark:||:heavy_check_mark:||
|50.Contoso-Café-Bot|A complete E2E Cafe bot that has all capabilities and includes best practices|:heavy_check_mark:||:heavy_check_mark:||
|51.Enterprise-Bot-Template|Bot that includes scaffolding and references to commonly used services for an enterprise scale bot. Also used as the enterprise bot template|:heavy_check_mark:||:heavy_check_mark:|| 

# Advanced topics
Additional advanced topics to consider based on feedback

- Create your own custom
    - adapter
    - context object
    - middleware
    - state provider
    - clients
    - events that clients can listen for

Feedback welcome.
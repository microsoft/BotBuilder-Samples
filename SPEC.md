This specification outlines the requirements for botbuilder samples.

## Goals
- Make it easy for developers to learn basic bot concepts
- Provide a numbering scheme for samples so developers can start simple and layer in sophistication
- Samples targeted at scenarios rather than technology involved
- Consistent set of samples across supported languages
- Samples are also consistently used in the docs topics for continuity and are developed by the documentation team, reviewed by the feature team.
- Each sample MUST include
    - README with steps to run the sample, concepts involved and links to additional reading (docs)
    - Deep link to V4 Emulator, include a .bot file as well as point to relevant tools (UI based or CLI) as appropriate
    - .chat files as appropriate that provide scenario overview and demonstrates how to construct mock conversations for the specific scenario
    - .lu files as appropriate
    - Include source JSON model files for LUIS, QnA, Dispatch where applicable
    - Include LUISGen strong typed LUIS class for LUIS samples
    - Any required build scripts for sample to work locally
    - Well defined naming convention for setting, service endpoint, and secrets in setting files (.bot, app.json, etc.)
    - Deploy to Azure button including all resources (similar to V3 samples)
- All samples are built and deployable on local dev env (Emulator) and Azure (WebChat)
    - Samples (and docs) should add channel specific notes (if applicable). e.g. document list of supported channels for Adaptive cards sample etc.

## Samples structure - C#
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

## Samples structure - JS

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

## README.md template
```markdown
<INSERT AT MOST ONE PARAGRAPH DESCRIPTION OF WHAT THIS SAMPLE DOES> 

# Table of Contents
<LINKS TO SECTIONS OF THE README>

# Concepts introduced in this sample
<DESCRIPTION OF THE CONCEPTS>
- Services used in this sample
- - <INTRODUCE SERVICES>

# To try this sample

## Prerequisites

###	Required Tools
- <Required TOOLS WITH MINIMUM VERSION NUMBERS>

### Clone the repo
- <STEPS TO CLONE REPO>

**NOTE:** <ANY NOTES ABOUT THE PREREQUISITES OR ALTERNATE THINGS TO CONSIDER TO GET SET UP>

## Configure Services

### Using CLI Tools

### Use portals (Manual)
<BOT FILE EXAMPLE>

## Run the Sample

### Visual Studio
- <STEPS TO RUN THIS SAMPLE FROM VISUAL STUDIO>

### Visual Studio Code
- <STEPS TO RUN THIS SAMPLE FROM VISUAL STUDIO CODE>

## Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework emulator from [here](https://github.com/Microsoft/BotFramework-Emulator/releases)

### Connect to bot using Bot Framework Emulator **V4**
- Launch Bot Framework Emulator
- From the *File* menu select *Open Bot Configuration*
- Navigate to your `.bot` file

## Deploy to Azure

### Using CLI Tools

### Deploy from Visual Studio

### Deprovision your bot
- <STEPS TO DEPROVISION>

# Further reading
- <LINKS TO ADDITIONAL READING>
```

## Samples repo structure, naming conventions
-	All samples will live under the BotBuilder-Samples repository, master branch. 
-	Language/ platform specific samples go under respective folders – ‘dotnet’/ ‘JS’/ ‘Java’/ ‘Python’
-	Samples should use published packages, available on NuGet or npmjs
-	Each sample sits in its own folder
-	Each sample folder is named as “\<\#\>. \<KEY SCENARIO INTRODUCED BY THE SAMPLE\>”
-	Each project is named as “\<KEY SCENARIO INTRODUCED BY THE SAMPLE\>”


## Static Code Analysis

### StyleCop - C#

To benefit from [StyleCop](https://github.com/StyleCop/StyleCop) static code analysis, all samples should have the samples-specific ruleset that can be found [here](csharp_dotnetcore/samples.ruleset).

When creating a new sample, follow these steps to apply the ruleset to the sample:

* Copy the [samples.ruleset](csharp_dotnetcore/samples.ruleset) file to your bot directory, at the same level as the project file.
* Rename the file to match your bot project name. For example, if your project is AspNetCore-QnA-Bot.csproj, rename the ruleset to AspNetCore-QnA-Bot.ruleset.

### Linting

All samples must have the following linting configuration enabled.
<TBD>

## Bot Styles
Our samples have consistent patterns for how to use the Bot Builder SDK.  The following should be followed for all of our samples.

### General Rules

#### Startup initialization should only initialize middleware and bot

> We want to minimize the cognitive load of what happens in startup and 
> foster code being organized along class boundaries.

#### CreateProperty() calls should be scoped to the class which consumes it and not in startup code

> Organize CreateProperty() calls at class boundaries to make it clearer 
> how code reuse happens and foster good component isolation.

*C# Example*
```cs
    public class ExampleDialog : ComponentDialog
    {
        public ExampleDialog(IPropertyManager propertyManager)
        {
            this.AlarmsProperty = propertyManager.CreateProperty<Alarm[]>("Alarms");
        }

    	public IStatePropertyAccessor<Alarms> AlarmsProperty { get;set;}
    }
    ...
    var alarms = await this.AlarmsProperty.Get(context);
```

*Typescript Example*
```typescript
    export class ExampleDialog extends ComponentDialog
    {
        constructor(propertyManager: PropertyManager)
        {
            this.AlarmsProperty = propertyManager.CreateProperty<Alarm[]>("Alarms");
        }

        public readonly alarmsProperty: StatePropertyAccessor<Alarm[]>;
    }
    ...
    var alarms = await this.AlarmsProperty.Get(context);
```

#### Shared Properties should be passed to a class if they are shared among multiple classes

> If you have 3 dialogs which all need the same property accessor, then the higher level
> class should define the property and pass the accessor to the child dialogs. 

*C# Example*
```cs
    public class ExampleDialog : ComponentDialog
    {
        public ExampleDialog(IPropertyManager propertyManager)
        {
            // create shared alarms property and pass to dialogs which use it
            this.AlarmsProperty = propertyManager.CreateProperty<List<Alarm>>("Alarms");

            this.Dialogs.Add(new AddAlarmDialog(alarmsProperty));
            this.Dialogs.Add(new ShowAlarmsDialog(alarmsProperty));
            this.Dialogs.Add(new DeleteAlarmsDialog(alarmsProperty));
        } 
    }
```

*Typescript Example*
```typescript
    export class ExampleDialog extends ComponentDialog
    {
        constructor(PropertyManager propertyManager)
        {
            // create shared alarms property and pass to dialogs which use it
            this.AlarmsProperty = propertyManager.CreateProperty<List<Alarm>>("Alarms");

            this.Dialogs.Add(new AddAlarmDialog(alarmsProperty));
            this.Dialogs.Add(new ShowAlarmsDialog(alarmsProperty));
            this.Dialogs.Add(new DeleteAlarmsDialog(alarmsProperty));
        } 
    }
```

#### Constructors should use IPropertyManager interface to create properties 

> It is best practice to consume an unbiased resource unless you need a specific class type.
> Our samples should use IPropertyManager as a component so the caller can control the policy 
> of the where the property is stored.

#### Bot, Dialog, and Prompt class names should have standard postfix of the class type
> Our samples use standard postfix names to make it clear what the class does.  
> This enhances  readability and understanding the structure of the sample.

| Base Class | Custom Class |
|------------|--------------|
| Bot        | MyBot        |
| Dialog     | MyDialog     |
| Prompt     | MyPrompt     |


*C# Example*
```cs
    var myBot = new MyBot();
    var exampleDialog = new ExampleDialog(...);
    var titlePrompt = new TitlePrompt(...);
```

*Typescript Example*
```typescript
    var myBot = new MyBot();
    var exampleDialog = new ExampleDialog(...);
    var titlePrompt = new TitlePrompt(...);
```

#### Bot, Dialog Prompt and Property variable names should have standard postfix.
> Clear and consistent variable name helps with readability and 
> understanding the structure of a sample.

| Class  | Type     | variable name    |
|--------|----------|------------------|
| Bot    | MyBot    | var myBot =      |
| Dialog | MyDialog | var myDialog =   |
| Prompt | MyPrompt | var myProperty = |

*C# Example*
```cs
    var myBot = new MyBot();
    var exampleDialog = new ExampleDialog(...);
    var titlePrompt = new TitlePrompt(...);
```

*Typescript Example*
```typescript
    var myBot = new MyBot();
    var exampleDialog = new ExampleDialog(...);
    var titlePrompt = new TitlePrompt(...);
```

#### Property Accessors variables should have standard postfix 
> property accessors do not define the state but are an interface to 
> get access to the object.  Postfix naming makes this clear and prevents confusion  
> between the actual state object and the interface which gives you access to it.

*C# Example*
```cs
    IStatePropertyAccessor<Alarm> AlarmProperty { get;set;}
    ...
    var alarm = await this.AlarmProperty.Get(context, ()=> new Alarm());
```

*Typescript Example*
```typescript
    public AlarmProperty :StatePropertyAccessor<Alarm>;
    ...
    var alarm = await this.AlarmProperty.Get(context, ()=> new Alarm());
```

#### Dependency Injection should be avoided unless it is super clear

> Our samples will use a minimum of dependency injection in order to keep the 
> structure and logic as clear and succinct as possible.







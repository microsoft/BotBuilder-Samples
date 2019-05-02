This specification outlines the requirements for botbuilder samples.

## Goals
- Make it easy for developers to learn basic bot concepts
- Provide a numbering scheme for samples so developers can start simple and layer in sophistication
- Samples targeted at scenarios rather than technology involved
- Consistent set of samples across supported languages
- Samples are also consistently used in the docs topics for continuity and are developed by the documentation team, reviewed by the feature team.
- Each sample MUST include
    - README with steps to run the sample, concepts involved and links to additional reading (docs)
    - Deep link to V4 Emulator, include how to connect the Emulator to a locally running bot.
    - .chat files as appropriate that provide scenario overview and demonstrates how to construct mock conversations for the specific scenario
    - .lu files as appropriate
    - Include source JSON model files for LUIS, QnA, Dispatch where applicable
    - Include LUISGen strong typed LUIS class for LUIS samples
    - Any required build scripts for sample to work locally
    - Well defined naming convention for setting, service endpoint, and secrets in setting files (appsettings.json, .env, etc.)
    - deploymentTemplates folder with ARM templates to enable deploying to Azure
- All samples are built and deployable on local dev env (Emulator) and Azure (WebChat)
    - Samples (and docs) should add channel specific notes (if applicable). e.g. document list of supported channels for Adaptive cards sample etc.

## Samples structure - C#
    | - README.md                               // Markdown readme file that includes steps to run this sample
                                                (including steps to create and configure required services),
                                                overview of concepts covered in this sample + links to
                                                additional topics to read
    | - Program.cs                              // Default program.cs
    | - Startup.cs                              // Default startup.cs – configuration builder + middlewares
    | - appsettings.json                        // Stores bot configuration information
    | - appsettings.Development.json            // Development environment configuraiton settings
    | - Bots                                    // Main ActivityHandler for the bot
    | - Cards                                   // Adaptive Cards if used by the sample
    | - Controllers                             // ASP.net MVC Controllers
    | - Dialogs                                 // Multi-turn dialog classes
    | - CognitiveModels
        | - <bot_name>.luis                     // LUIS model file for this sample
        | - <bot_name>.qna                      // QnA Maker JSON model file
        | - <bot_name>.dispatch                 // Dispatch JSON model file
    | - DeploymentTemplates
        | - template-with-new-rg.json           // ARM Template that creates a new Azure Resource Group
        | - template-with-preexisting-rg.json   // ARM Template that uses a preexisting Azure Resource Group

## Samples structure - JS

    | - README.md                               // Markdown readme file that includes steps to run this sample
                                                (including steps to create and configure required services),
                                                overview of concepts covered in this sample + links to
                                                additional topics to read
    | - index.js                                // Default app.js - startup, middlewares
    | - .env                                    // Stores bot configuration information
    | - bots
    | - dialogs                                 // Multi-turn dialog classes
    | - cognitiveModels
        | - <bot-name>.luis                     // LUIS model file for this sample
        | - <bot-name>.qna                      // QnA Maker JSON model file
        | - <bot-name>.dispatch                 // Dispatch JSON model file
    | - deploymentTemplates
        | - template-with-new-rg.json           // ARM Template that creates a new Azure Resource Group
        | - template-with-preexisting-rg.json   // ARM Template that uses a preexisting Azure Resource Group
    | - resources                               // Adaptive Card, LU, LG files

## README.md template
Every samples *must* have a README.md file.  The file must be named and cased as `README.md`.  The README.md is meant to convey the following:
    - Name of the sample
    - Concepts introduced in the sample
    - Prerequisites required to run the sample
    - How to run the sample locally
    - How to use the Emulator to debug the sample
    - A link to the docs explaining how to deploy the sample to Azure
    - Further reading section with topics and links to introduce additional topics to explore

There are canonical examples of README.md files for each of the langues supported in the samples repo.  Refer to these examples for how to write a README.md for any new sample proposals.
[.NET Core Example README.md](./csharp_dotnetcore/13.core-bot/README.md)
[JavaScript Example README.md](./javascript_nodejs/13.core-bot/README.md)
[TypeScript Example README.md](./javascript_typescript/13.core-bot/README.md)


## Static Code Analysis

### StyleCop and Linting Rules
All JavaScript and TypeScript samples *must* be free of *all* lint rule warnings and errors.  This includes any code generated by any of our code generator options (VSIX, Yeoman, .NET Templates).

#### JavaScript Linting Rules
All JavaScript samples and generated code use eslint.  All samples must use the following `.eslintrc.js` configuration file:

```js
module.exports = {
    "extends": "standard",
    "rules": {
        "semi": [2, "always"],
        "indent": [2, 4],
        "no-return-await": 0,
        "space-before-function-paren": [2, {
            "named": "never",
            "anonymous": "never",
            "asyncArrow": "always"
        }],
        "template-curly-spacing": [2, "always"]
    }
};
```
#### TypeScript Linting Rules
All TypeScript samples and generated code currently use tslint.  We will be moving to eslint in the future.  For now, all TypeScript samples must use the following `.tslint.json` configuration file.

```js
{
    "defaultSeverity": "error",
    "extends": [
        "tslint:recommended"
    ],
    "jsRules": {},
    "rules": {
        "interface-name" : [true, "never-prefix"],
        "max-line-length": [false],
        "no-console": [false, "log", "error"],
        "no-var-requires": false,
        "quotemark": [true, "single"]
    },
    "rulesDirectory": []
}

```

## Bot Styles
Our samples have consistent patterns for how to use the Bot Builder SDK.  Strutural consistency is manditory for any sample published in the repo.  Core Bot is a good sample to use as guidance.  It contains variable naming conventions, idiomatic for each language, folder structure, README.md and ARM deployment templates that are the architype example of how a sample should be structured.

[.NET Core Sample](./csharp_dotnetcore/13.core-bot/)
[JavaScript Sample](./javascript_nodejs/13.core-bot/)
[TypeScript Sample](./javascript_typescript/13.core-bot/)


# Extending your bot with code

# Add custom triggers in C#

## In this article

In Bot Framework Composer, [actions](concept-dialog#action) are the main
contents of a [trigger](concept-dialog#trigger). Actions help maintain
conversation flow and instruct bots on how to fulfill user requests.
Composer provides different types of actions, such as **Send a
response**, **Ask a question**, and **Create a condition**. Besides
these built-in actions, you can create and customize your own actions in
Composer.

This article shows you how to include a custom triggers OnMembersAdded and OnMembersRemoved.

#### Note

Composer currently supports the C\# runtime and JavaScript (preview)
Adaptive Runtime.

## Prerequisites

- A basic understanding of [triggers](concept-dialog#triggers) in Composer.
- [A basic bot built using Composer](quickstart-create-bot).
- [Bot Framework CLI 4.10](https://botbuilder.myget.org/feed/botframework-cli/package/npm/@microsoft/botframework-cli) or later.

## Setup the Bot Framework CLI tool
----------------------

The Bot Framework CLI tools include the *bf-dialog* tool which will
create a *schema file* that describes the built-in and custom
capabilities of your bot project. It does this by merging partial schema
files included with each component with the root schema provided by Bot
Framework.

Open a command line and run the following command to install the Bot
Framework tools:

    npm i -g @microsoft/botframework-cli

## Key points
----------------------

This C\# sample consists of the following:

- A Composer project targeted for Dotnet.  This can be any Composer project.  One that already exists, or a new one you create.  This sample provides an Empty Bot for demonstration purposes. The important points are:
  - A project dependency on the MembersUpdates project
  - appsettings has been updated to include MembersUpdated pacakge for the Adaptive Runtime.  This allows the Adaptive Runtime to load this component at startup.

- A project containing the custom trigger(s), implemented as a BotComponent
  - The custom triggers code [OnMembersAdded.cs](MemberUpdates/TriggerConditions/OnMembersAdded.cs) and [OnMembersRemoved.cs](MemberUpdates/TriggerConditions/OnMembersRemoved.cs) classes.

  - The custom trigger schema [OnMembersAdded.schema](MemberUpdates/Schemas/TriggerConditions/OnMembersAdded.schema), which describes the operations available, and [OnMembersAdded.uischema](MemberUpdates/Schemas/TriggerConditions/OnMembersAdded.uischema), which describe how its displayed in Composer.

  - The custom trigger schema [OnMembersRemoved.schema](MemberUpdates/Schemas/TriggerConditions/OnMembersAdded.schema), which describes the operations available, and [OnMembersRemoved.uischema](MemberUpdates/Schemas/TriggerConditions/OnMembersRemoved.uischema), which describe how its displayed in Composer.

    [Bot Framework Schemas](https://github.com/microsoft/botframework-sdk/tree/master/schemas)
    are specifications for JSON data. They define the shape of the data
    and can be used to validate JSON. All of Bot Framework's [adaptive
    dialogs](/en-us/azure/bot-service/bot-builder-adaptive-dialog-introduction)
    are defined using this JSON schema. The schema files tell Composer
    what capabilities the bot runtime supports. Composer uses the schema
    to help it render the user interface when using the trigger in a
    dialog. Read the section about [creating schema files in adaptive
    dialogs](/en-us/azure/bot-service/bot-builder-dialogs-declarative)
    for more information.

  - A BotComponent, [MemberUpdatesBotComponent.cs](MemberUpdates/MemberUpdatesBotComponent.cs) for component registration.  BotComponents are loaded by your bot (specifically by Adaptive Runtime), and made available to Composer.

    **Note** You can create a custom action without implementing BotComponent.  However, the Component Model in Bot Framework allows for easier reuse and is only slightly more work.  In a BotComponent, you add the needed services and objects via Dependency Injection, just as you would in Startup.cs.

## Outline of adding a custom trigger
------------------------------

1. Create a new project for the custom trigger.  Use the settings in [csproj](MemberUpdates/MemberUpdates.csproj) as a template.  This project contains important settings that are required if publishing your custom trigger.

1. Implement your custom trigger by subclassing the appropriate Condition class.  In this case, OnActivity. See [OnMembersAdded.cs](MemberUpdates/TriggerConditions/OnMembersAdded.cs).

1. Create the [OnMembersAdded.schema](MemberUpdates/Schemas/TriggerConditions/OnMembersAdded.schema)

1. Create the [OnMembersAdded.uischema](MemberUpdates/Schemas/TriggerConditions/OnMembersAdded.uischema)

1. Create your BotComponent per [MemberUpdatesBotComponent.cs](MemberUpdates/MemberUpdatesBotComponent.cs).

1. Add Existing project to the solution.

1. In the bot project, add a project reference to the MultiplyDialog project.

1. Run the command `dotnet build` on the project to
    verify if it passes build after adding custom trigger to it. You
    should be able to see the "Build succeeded" message after this
    command.

1. Edit {bot}\settings\appsettings.json to include the BotComponent in the `runtimeSettings/components` list.

   ```json
   "runtimeSettings": {
      "components": [
        {
          "name": "MemberUpdates"
        }
      ]
   }
   ```

## Update the schema file
----------------------

Now you have customized your bot, the next step is to update the
`sdk.schema` file to include the `OnMemberAdded.Schema` file.  This makes your custom trigger available for use in Composer.

**You only need to perform these steps when adding new code extensions, or when the Schema for a component changes.**

1) Navigate to the `C:\CustomTrigger\CustomTrigger\schemas` folder. This
folder contains a PowerShell script and a bash script. Run either one of
the following commands:

       ./update-schema.ps1

    **Note**

    The above steps should generate a new `sdk.schema` file inside the
    `schemas` folder.

1) Search for `OnMembersAdded` inside the `CustomTrigger\schemas\sdk.schema` file and
    validate that the partial schema for [OnMembersAdded.schema](MemberUpdates/Schemas/TriggerConditions/OnMembersAdded.schema) is included in `sdk.schema`.

### Tip

Alternatively, you can select the `update-schema.sh` file inside the
`CustomTrigger\schemas` folder to run the bash script. You can't click and run the
`powershell` file directly.

## Test
----

Open the bot project in Composer and you should be able to test your
added custom trigger.  If the project is already loaded, return to `Home` in Composer, and reload the project.

1. Select **+ Add new trigger** under a dialogs **`...`** menu. Under "Member Updates", select either "Members Added" or "Members Removed".

1. Add actions to the trigger, for example *Send a response** action.

1. Select **Restart Bot** to test the bot in the Emulator. Your bot
   will respond with the test result.

## Additional information
----------------------

- [Bot Framework SDK Schemas](https://github.com/microsoft/botframework-sdk/tree/master/schemas)
- [Create schema files](/en-us/azure/bot-service/bot-builder-dialogs-declarative)

## Docs table of contents

1. [Overview](/docs/overview.md)
2. [Extending your bot using packages](/docs/extending-with-packages.md)
3. Extending your bot with code (this document)
4. [Creating your own packages](/docs/creating-packages.md)
5. [Creating your own templates](/docs/creating-templates.md)

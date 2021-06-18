# Extending your bot with code

# Add custom actions in C#

## In this article

In Bot Framework Composer, [actions](concept-dialog#action) are the main
contents of a [trigger](concept-dialog#trigger). Actions help maintain
conversation flow and instruct bots on how to fulfill user requests.
Composer provides different types of actions, such as **Send a
response**, **Ask a question**, and **Create a condition**. Besides
these built-in actions, you can create and customize your own actions in
Composer.

This article shows you how to include a custom action named
`MultiplyDialog`.

#### Note

Composer currently supports the C\# runtime and JavaScript (preview)
Adaptive Runtime.

## Prerequisites

- A basic understanding of [actions](concept-dialog#action) in Composer.
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
  - A project dependency on the MultiplyDialog project
  - appsettings has been updated to include MultiplyDialog for the Adaptive Runtime.  This allows the Adaptive Runtime to load this component at startup.

- A project containing the custom action, implemented as a BotComponent
  - The custom action code [MultiplyDialog.cs](MultiplyDialog/Action/MultiplyDialog.cs) class, which defines the business logic of the custom action. In this example, two numbers passed as inputs are multiplied, and the result is the output.

  - The custom action schema [MultiplyDialog.schema](MultiplyDialog/Schemas/MultiplyDialog.schema), which describes the operations available.

    [Bot Framework Schemas](https://github.com/microsoft/botframework-sdk/tree/master/schemas)
    are specifications for JSON data. They define the shape of the data
    and can be used to validate JSON. All of Bot Framework's [adaptive
    dialogs](/en-us/azure/bot-service/bot-builder-adaptive-dialog-introduction)
    are defined using this JSON schema. The schema files tell Composer
    what capabilities the bot runtime supports. Composer uses the schema
    to help it render the user interface when using the action in a
    dialog. Read the section about [creating schema files in adaptive
    dialogs](/en-us/azure/bot-service/bot-builder-dialogs-declarative)
    for more information.

  - A BotComponent, [MultiplyDialogBotComponent.cs](MultiplyDialog/MultiplyDialogBotComponent.cs) for component registration.  BotComponents are loaded by your bot (specifically by Adaptive Runtime), and made available to Composer.

    **Note** You can create a custom action without implementing BotComponent.  However, the Component Model in Bot Framework allows for easier reuse and is only slightly more work.  In a BotComponent, you add the needed services and objects via Dependency Injection, just as you would in Startup.cs.

## Outline of adding a custom action
------------------------------

1. Create a new project for the custom action.  Use the settings in [csproj](MultiplyDialog/Microsoft.Bot.Components.Samples.MultiplyDialog.csproj) as a template.  This project contains important settings that are required when publishing your custom action.

1. Implement your custom action by subclassing Dialog, and implementing BeginDialogAsync. See [MultiplyDialog.cs](MultiplyDialog/Actions/MultiplyDialog.cs)

1. Create the [MultiplyDialog.schema](MultiplyDialog/Schemas/MultiplyDialog.schema)

1. Create your BotComponent per [MultiplyDialogBotComponent.cs](MultiplyDialog/MultiplyDialogBotComponent.cs).

1. Add Existing project to the solution.

1. In the bot project, add a project reference to the MultiplyDialog project.

1. Run the command `dotnet build` on the project to
    verify if it passes build after adding custom actions to it. You
    should be able to see the "Build succeeded" message after this
    command.

1. Edit {bot}\settings\appsettings.json to include the BotComponent in the `runtimeSettings/components` list.

   ```json
   "runtimeSettings": {
      "components": [
        {
          "name": "CustomAction.MultiplyDialog"
        }
      ]
   }
   ```

## Update the schema file
----------------------

Now you have customized your bot, the next step is to update the
`sdk.schema` file to include the `MultiplyDialog.Schema` file.  This makes your custom action available for use in Composer.

**You only need to perform these steps when adding new code extensions, or when the Schema for a component changes.**

1) Navigate to the `C:\CustomAction\CustomAction\schemas` folder. This
folder contains a PowerShell script and a bash script. Run either one of
the following commands:

       ./update-schema.ps1

    **Note**

    The above steps should generate a new `sdk.schema` file inside the
    `schemas` folder.

1) Search for `MultiplyDialog` inside the `CustomAction\schemas\sdk.schema` file and
    validate that the partial schema for [MultiplyDialog.schema](assets/MultiplyDialog.schema) is included in `sdk.schema`.

### Tip

Alternatively, you can select the `update-schema.sh` file inside the
`CustomAction\schemas` folder to run the bash script. You can't click and run the
`powershell` file directly.

## Test
----

Open the bot project in Composer and you should be able to test your
added custom action.  If the project is already loaded, return to `Home` in Composer, and reload the project.

1. Open your bot in Composer. Select a trigger you want to associate this custom action with.

2. Select **+** under the trigger node to see the actions menu. You
   will see **Custom Actions** added to the menu. Select **Multiply**
   from the menu.

3. On the **Properties** panel on the right side, enter two numbers in
   the argument fields: **Arg1** and **Arg2**. Enter **dialog.result**
   in the **Result** property field. For example, enter `99` for each field.

4. Add a **Send a response** action. Enter `99*99=${dialog.result}` in the Language Generation editor.

5. Select **Restart Bot** to test the bot in the Emulator. Your bot
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

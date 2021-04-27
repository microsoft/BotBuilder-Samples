# Add custom actions
## In this article

In Bot Framework Composer, [actions](concept-dialog#action) are the main
contents of a [trigger](concept-dialog#trigger). Actions help maintain
conversation flow and instruct bots on how to fulfill user requests.
Composer provides different types of actions, such as **Send a
response**, **Ask a question**, and **Create a condition**. Besides
these built-in actions, you can create and customize your own actions in
Composer.

This article shows you how to include a sample custom action named
`MultiplyDialog`.

#### Note

Composer currently supports the C\# runtime and JavaScript (preview)
runtime.

## Prerequisites

-   A basic understanding of [actions](concept-dialog#action) in
    Composer.
-   [A basic bot built using Composer](quickstart-create-bot).
-   [Bot Framework CLI
    4.10](https://botbuilder.myget.org/feed/botframework-cli/package/npm/@microsoft/botframework-cli)
    or later.

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

### Tip

Read more about [Bot Framework SDK
schemas](https://github.com/microsoft/botframework-sdk/tree/master/schemas)
and [how to create schema
files](/en-us/azure/bot-service/bot-builder-dialogs-declarative&tabs=csharp#creating-the-schema-file).

## About the example custom action
----------------------

The example C\# custom action component consists of the following:

-   A `CustomAction.sln` solution file.

-   A Blank Bot created from Composer (EmptyBot)

-   A CustomAction project (`CustomAction\Microsoft.BotFramework.Runtime.CustomAction.csproj` project
    file.

-   A `MultiplyDialogBotComponent.cs` code file for component
    registration.

-   A `Schemas` folder that contains the `MultiplyDialog.schema` file.
    This schema file describes the properties of the example dialog
    component, `arg1`, `arg2`, and `resultProperty`.

        {
            "$schema": "https://schemas.botframework.com/schemas/component/v1.0/component.schema",
            "$role": "implements(Microsoft.IDialog)",
            "title": "Multiply",
            "description": "This will return the result of arg1*arg2",
            "type": "object",
            "additionalProperties": false,
            "properties": {
               "arg1": {
                     "$ref": "schema:#/definitions/integerExpression",
                     "title": "Arg1",
                     "description": "Value from callers memory to use as arg 1"
               },
               "arg2": {
                     "$ref": "schema:#/definitions/integerExpression",
                     "title": "Arg2",
                     "description": "Value from callers memory to use as arg 2"
               },
               "resultProperty": {
                     "$ref": "schema:#/definitions/stringExpression",
                     "title": "Result",
                     "description": "Value from callers memory to store the result"
               }
            }
        }

    Important

    [Bot Framework
    Schemas](https://github.com/microsoft/botframework-sdk/tree/master/schemas)
    are specifications for JSON data. They define the shape of the data
    and can be used to validate JSON. All of Bot Framework's [adaptive
    dialogs](/en-us/azure/bot-service/bot-builder-adaptive-dialog-introduction)
    are defined using this JSON schema. The schema files tell Composer
    what capabilities the bot runtime supports. Composer uses the schema
    to help it render the user interface when using the action in a
    dialog. Read the section about [creating schema files in adaptive
    dialogs](/en-us/azure/bot-service/bot-builder-dialogs-declarative)
    for more information.

-   An `Action` folder that contains the `MultiplyDialog.cs` class,
    which defines the business logic of the custom action. In this
    example, two numbers passed as inputs are multiplied, and the result
    is the output.

        using System;
        using System.Runtime.CompilerServices;
        using System.Threading;
        using System.Threading.Tasks;
        using AdaptiveExpressions.Properties;
        using Microsoft.Bot.Builder.Dialogs;
        using Newtonsoft.Json;

        namespace Microsoft.BotFramework.Runtime.CustomAction
        {
            /// <summary>
            /// Custom command which takes takes 2 data bound arguments (arg1 and arg2) and multiplies them returning that as a databound result.
            /// </summary>
            public class MultiplyDialog : Dialog
            {
               [JsonConstructor]
               public MultiplyDialog([CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
                     : base()
               {
                     // enable instances of this command as debug break point
                     this.RegisterSourceLocation(sourceFilePath, sourceLineNumber);
               }

               [JsonProperty("$kind")]
               public const string Kind = "MultiplyDialog";

               /// <summary>
               /// Gets or sets memory path to bind to arg1 (ex: conversation.width).
               /// </summary>
               /// <value>
               /// Memory path to bind to arg1 (ex: conversation.width).
               /// </value>
               [JsonProperty("arg1")]
               public NumberExpression Arg1 { get; set; }

               /// <summary>
               /// Gets or sets memory path to bind to arg2 (ex: conversation.height).
               /// </summary>
               /// <value>
               /// Memory path to bind to arg2 (ex: conversation.height).
               /// </value>
               [JsonProperty("arg2")]
               public NumberExpression Arg2 { get; set; }

               /// <summary>
               /// Gets or sets caller's memory path to store the result of this step in (ex: conversation.area).
               /// </summary>
               /// <value>
               /// Caller's memory path to store the result of this step in (ex: conversation.area).
               /// </value>
               [JsonProperty("resultProperty")]
               public StringExpression ResultProperty { get; set; }

               public override Task<DialogTurnResult> BeginDialogAsync(DialogContext dc, object options = null, CancellationToken cancellationToken = default(CancellationToken))
               {
                     var arg1 = Arg1.GetValue(dc.State);
                     var arg2 = Arg2.GetValue(dc.State);

                     var result = Convert.ToInt32(arg1) * Convert.ToInt32(arg2);
                     if (this.ResultProperty != null)
                     {
                        dc.State.SetValue(this.ResultProperty.GetValue(dc.State), result);
                     }

                     return dc.EndDialogAsync(result: result, cancellationToken: cancellationToken);
               }
            }
        }

    To create a class like `MultiplyDialog.cs`, shown above, take the
    following steps:

    -   Create a class which inherits from the *Dialog* class.
    -   Define the properties for input and output. These will appear in
        Composer's property editor, and they need to be described in the
        [schema file](#update-the-schema-file).
    -   Implement the required `BeginDialogAsync()` method, which will
        contain the logic of the custom action. You can use
        `Property.GetValue(dc.State)` to get value, and
        `dc.State.SetValue(Property, value)` to set value.
    -   Register the [custom action
        component](#customize-the-exported-runtime) where it's called.
    -   (optional) If there's more than one turn, you might need to add
        the `ContinueDialogAsync` class. Read more in the [Actions
        sample
        code](https://github.com/microsoft/botbuilder-dotnet/tree/master/libraries/Microsoft.Bot.Builder.Dialogs.Adaptive/Actions)
        in the Bot Framework SDK.

The following sections show how to add the custom action in Composer and
test it.

## Customize the Runtime
------------------------------

1.  Navigate to the runtime location (for example,
    `C:\CustomAction\EmptyBot`).

1.  Edit the `EmptyBot.csproj` to include a project reference to the custom action project like the
    following:

        <ProjectReference Include="..\CustomAction\Microsoft.BotFramework.Runtime.CustomAction.csproj" />

1.  Run the command `dotnet build` on the project to
    verify if it passes build after adding custom actions to it. You
    should be able to see the "Build succeeded" message after this
    command.

1. Edit EmptyBot\settings\appsettings.json to include the Custom Action component in the `runtimeSettings/components` list.
    ```
    "runtimeSettings": {
        "components": [
        {
            "name": "Microsoft.BotFramework.Runtime.CustomAction"
        }
    }
    ```

## Update the schema file
----------------------

Now you have customized your runtime, the next step is to update the
`sdk.schema` file to include the `MultiplyDialog.Schema` file.

Navigate to the `C:\CustomAction\EmptyBot\schemas` folder. This
folder contains a PowerShell script and a bash script. Run either one of
the following commands:

       ./update-schema.ps1

       sh ./update-schema.sh

Note

You can validate that the partial schema (`MultiplyDialog.schema` inside
the `CustomAction\Schema` folder) has been appended to the default
`sdk.schema` file to generate one single consolidated `sdk.schema` file.

The above steps should generate a new `sdk.schema` file inside the
`schemas` folder for Composer to use. Reload the bot and you should be
able to include your custom action.

### Tip

Alternatively, you can select the `update-schema.sh` file inside the
`bot/schema` folder to run the bash script. You can't click and run the
`powershell` file directly.

3.  Search `MultiplyDialog` inside the `bot\schemas\sdk.schema` file and
    validate that the partial schema (`MultiplyDialog.schema` inside the
    `customaction` folder has been appended to the default `sdk.schema`
    file (`bot\schemas\sdk.schema`) to generate one single consolidated
    `sdk.schema` file.

The above steps should generate a new `sdk.schema` file inside the
`bot\schemas` folder for Composer to use. Reload the bot and you should
be able to include your custom action.

## Test
----

Reopen the bot project in Composer and you should be able to test your
added custom action.

1.  Open your bot in Composer. Select a trigger you want to associate
    this custom action with.

2.  Select **+** under the trigger node to see the actions menu. You
    will see **Custom Actions** added to the menu. Select **Multiply**
    from the menu.

3.  On the **Properties** panel on the right side, enter two numbers in
    the argument fields: **Arg1** and **Arg2**. Enter **dialog.result**
    in the **Result** property field. For example, you can enter the
    following:

4.  Add a **Send a response** action. Enter `99*99=${dialog.result}` in
    the Language Generation editor.

5.  Select **Restart Bot** to test the bot in the Emulator. Your bot
    will respond with the test result.


## Additional information
----------------------

-   [Bot Framework SDK
    Schemas](https://github.com/microsoft/botframework-sdk/tree/master/schemas)
-   [Create schema
    files](/en-us/azure/bot-service/bot-builder-dialogs-declarative)

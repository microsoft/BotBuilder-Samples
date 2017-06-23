# Global Message Handlers Sample

A bot can use the [Bot Builder for .NET SDK](https://dev.botframework.com/) to define global message handlers that execute code whenever the user replies to the conversation with a specific word or phrase from anywhere in the bot. These global message handlers allow you to create bot features that are accessible from anywhere in your bot.

This sample shows how to create global message handlers and illustrates two common scenarios:
* Global Dialogs - A global message handler that responds to a global command to add a dialog to the conversation from anywhere in the bot. In this sample, the settings dialog can be accessed anywhere in the bot by replying with 'settings' anywhere in the bot. When the settings dialog is complete, the conversation returns to the prior dialog. 
* Stack Manipulation - A global message handler that responds to a global command to manipulate the dialog stack. In this sample, the user is returned to the root dialog whenever the user responds with the word 'cancel' anywhere in the bot.

[![Deploy to Azure][Deploy Button]][Deploy CSharp/GlobalMessageHandlers]

[Deploy Button]: https://azuredeploy.net/deploybutton.png
[Deploy CSharp/GlobalMessageHandlers]: https://azuredeploy.net

### Prerequisites

To run this sample, install the prerequisites by following the steps in the [Create a bot with the Bot Builder SDK for .NET](https://docs.microsoft.com/en-us/bot-framework/dotnet/bot-builder-dotnet-quickstart) section of the documentation.

This sample is based on the Basic Multi-Dialog Sample, so be sure to review that sample before getting started with this one.

### Overview

The Bot Builder for .NET SDK uses [AutoFac](https://autofac.org/) for [inversion of control and dependency injection](https://martinfowler.com/articles/injection.html). If you're not famililar with AutoFac, you can learn more in this [Quick Start Guide](http://autofac.readthedocs.io/en/latest/getting-started/index.html).

One of the ways the Bot Builder for .NET SDK uses AutoFac is Scorables. Scorables intercept every message sent to a Conversation and apply a score to the message based on logic you define. The Scorable with the highest score 'wins' the opportunity to process the message, rather the message being sent to the Conversation. You can implement global message handlers by createing a Scorable for each global command you want to implement in your bot.

To create a Scorable you create a class that implements the [`IScorable`](https://docs.botframework.com/en-us/csharp/builder/sdkreference/d2/dd9/interface_microsoft_1_1_bot_1_1_builder_1_1_internals_1_1_scorables_1_1_i_scorable.html) interface by inheriting from the [`ScorableBase`](https://docs.botframework.com/en-us/csharp/builder/sdkreference/de/d7b/class_microsoft_1_1_bot_1_1_builder_1_1_internals_1_1_scorables_1_1_scorable_base.html) abstract class. To have that Scorable applied to every message in the conversation, the bot registers that `IScorable` interface as a [`Service`](http://autofac.readthedocs.io/en/latest/resolve/index.html) with the [`Conversation`](https://docs.botframework.com/en-us/csharp/builder/sdkreference/d9/de8/class_microsoft_1_1_bot_1_1_builder_1_1_dialogs_1_1_conversation.html)'s `Container`.  When a new message arrives to the `Conversation`, the `Conversation` passes that message to each implementation of `IScorable` in the `Container` to get a score. The `Container` then passes that message to the `IScorable` with the highest score for processing.

Let's look at how this is done in the sample.

### Create the Settings Dialog

The [`SettingsDialog`](Dialogs/SettingsDialog.cs) is the dialog we'll add to the dialog stack whenever the user responds with 'settings' in the conversation.

````C#
public class SettingsDialog : IDialog<object>
{
    public async Task StartAsync(IDialogContext context)
    {
        await context.PostAsync("This is the Settings Dialog. Reply with anything to return to prior dialog.");

        context.Wait(this.MessageReceived);
    }

    private async Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> result)
    {
        var message = await result;

        if ((message.Text != null) && (message.Text.Trim().Length > 0))
        {
            context.Done<object>(null);
        }
        else
        {
            context.Fail(new Exception("Message was not a string or was an empty string."));
        }
    }
}
````

### Create SettingsScorable

The [`SettingsScorable`](Dialogs/SettingsScorable.cs) class provides an implementation of the `ScorableBase` abstract class in order to implement the `IScorable` interface. 

In the `PrepareAsync()` method, we inspect the incoming message to see if it matches the text we are looking for ('settings'). If there's a match, we return the message to be used as state for scoring, otherwise we return null (no match). 

````C#
protected override async Task<string> PrepareAsync(IActivity activity, CancellationToken token)
{
    var message = activity as IMessageActivity;

    if (message != null && !string.IsNullOrWhiteSpace(message.Text))
    {
        if (message.Text.Equals("settings", StringComparison.InvariantCultureIgnoreCase))
        {
            return message.Text;
        }
    }

    return null;
}
````
The `HasScore()` method is called by the calling component to determine if the Scorable has a score (we have a match).

````C#
protected override bool HasScore(IActivity item, string state)
{
    return state != null;
}
````
The `GetScore()` method is called by the calling component to get the score for this Scorable. This score is compared to all other Scorables that have a score. 
````C#
protected override double GetScore(IActivity item, string state)
{
    return 1.0;
}
````
The Scorable with the highest score will process the message when the `PostAsync()` method is called by the calling component. In the `PostAsync()` method, we add the `SettingsDialog` to the stack so it will become the active dialog.
````C#
protected override async Task PostAsync(IActivity item, string state, CancellationToken token)
{
    var message = item as IMessageActivity;

    if (message != null)
    {
        var settingsDialog = new SettingsDialog();

        var interruption = settingsDialog.Void<object, IMessageActivity>();

        this.task.Call(interruption, null);

        await this.task.PollAsync(token);
    }
}
````
When the scoring process is complete, the calling component calls the `DoneAsync()` method where any resources used in scoring are freed.
````C#
protected override Task DoneAsync(IActivity item, string state, CancellationToken token)
{
    return Task.CompletedTask;
}
````
### Create Module to Register IScorable Service
In [`GlobalMessageHandlersBotModule`](GlobalMessageHandlersBotModule.cs), we define a `Module` that registers the `SettingsScorable` as a Component that provides the `IScorable` service.
````C#
public class GlobalMessageHandlersBotModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);

        builder
            .Register(c => new SettingsScorable(c.Resolve<IDialogTask>()))
            .As<IScorable<IActivity, double>>()
            .InstancePerLifetimeScope();
    }
}
````

### Register the Module with the Conversation's Container
In [`Global.asax.cs`](Global.asax.cs), the `SettingsScorable` can be applied to the `Conversation`'s `Container` by registering the `GlobalMessageHandlersBotModule` with the `Container`.
````C#
public class WebApiApplication : System.Web.HttpApplication
{
    protected void Application_Start()
    {
        this.RegisterBotModules();

        GlobalConfiguration.Configure(WebApiConfig.Register);
    }

    private void RegisterBotModules()
    {
        var builder = new ContainerBuilder();

        builder.RegisterModule(new ReflectionSurrogateModule());

        builder.RegisterModule<GlobalMessageHandlersBotModule>();

        builder.Update(Conversation.Container);
    }
}
````
### Implementing CancelScorable
The [`CancelScorable`](Dialogs/CancelScorable.cs) is implemented the same way, but resets the dialog stack when the Scorable is called.
````C#
protected override async Task PostAsync(IActivity item, string state, CancellationToken token)
{
    this.task.Reset();
}
````

### Outcome

Here's what the conversation looks like in the [Bot Framework Emulator](https://docs.microsoft.com/en-us/bot-framework/debug-bots-emulator) when replying to the to any dialog with 'settings'. 

Note: When the `SettingsDialog` completes, the `NameDialog` is returned to the top of the dialog stack, so the next reply will be applied to the 'What is your name?' prompt.

![Settingsoutcome](images/settingsoutcome.png)

Here's what the conversation looks like when replying to a the `AgeDialog` with 'cancel'. 

Note: When `CancelScorable` is complete, the dialog returns to the [`RootDialog`](Dialogs/RootDialog.cs), which waits for a message from the user ('Hi' below) before showing it's greeting and the first prompt.

![Canceloutcome](images/canceloutcome.png)

### More Information

For more information on creating global message handlers using Scorables, check out the following resources:
* [Implement global message handlers](https://docs.microsoft.com/en-us/bot-framework/dotnet/bot-builder-dotnet-global-handlers)
* [AutoFac](https://autofac.org/) 
* [AutoFac Quick Start Guide](http://autofac.readthedocs.io/en/latest/getting-started/index.html)
* [IScorable](https://docs.botframework.com/en-us/csharp/builder/sdkreference/d2/dd9/interface_microsoft_1_1_bot_1_1_builder_1_1_internals_1_1_scorables_1_1_i_scorable.html)
* [ScorableBase](https://docs.botframework.com/en-us/csharp/builder/sdkreference/de/d7b/class_microsoft_1_1_bot_1_1_builder_1_1_internals_1_1_scorables_1_1_scorable_base.html)


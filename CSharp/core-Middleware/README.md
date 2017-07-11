# Middleware Bot Sample

A sample bot showing how to intercept messages and log them.

[![Deploy to Azure][Deploy Button]][Deploy CSharp/MiddlewareLogging]

[Deploy Button]: https://azuredeploy.net/deploybutton.png
[Deploy CSharp/MiddlewareLogging]: https://azuredeploy.net

### Prerequisites

The minimum prerequisites to run this sample are:
* The latest update of Visual Studio 2015. You can download the community version [here](http://www.visualstudio.com) for free.
* The Bot Framework Emulator. To install the Bot Framework Emulator, download it from [here](https://emulator.botframework.com/). Please refer to [this documentation article](https://github.com/microsoft/botframework-emulator/wiki/Getting-Started) to know more about the Bot Framework Emulator.

### Code Highlights

One of the most common operations when working with conversational history is to intercept and log message activities between bots and users. The [`Microsoft.Bot.Builder.History`](https://github.com/Microsoft/BotBuilder/tree/master/CSharp/Library/Microsoft.Bot.Builder.History) namespace provides interfaces and classes for doing this. In particular, the [`IActivityLogger`](https://github.com/Microsoft/BotBuilder/blob/master/CSharp/Library/Microsoft.Bot.Builder/Dialogs/IActivityLogger.cs) interface contains the definition of the functionality that a class needs to implement to log message activities.

Check out the [`DebugActivityLogger`](DebugActivityLogger.cs) implementation of the `IActivityLogger` interface that writes message activities to the trace listeners only when running in debug.

````C#
public class DebugActivityLogger : IActivityLogger
{
    public async Task LogAsync(IActivity activity)
    {
        Debug.WriteLine($"From:{activity.From.Id} - To:{activity.Recipient.Id} - Message:{activity.AsMessageActivity()?.Text}");
    }
}
````

By default, the BotBuilder library registers a [`NullActivityLogger`](https://github.com/Microsoft/BotBuilder/blob/master/CSharp/Library/Microsoft.Bot.Builder/Dialogs/IActivityLogger.cs#L81) with the conversation container, which is a no-op activity logger. Check out the registration in the Autofac container of the [`DebugActivityLogger`](DebugActivityLogger.cs) of the sample in the [`Global.asax.cs`](Global.asax.cs#L11-L13).

````C#
var builder = new ContainerBuilder();
builder.RegisterType<DebugActivityLogger>().AsImplementedInterfaces().InstancePerDependency();
builder.Update(Conversation.Container);
````

### Outcome

You will see the following result in the [Debug text pane of the Visual Studio Output window](https://blogs.msdn.microsoft.com/visualstudioalm/2015/02/09/the-output-window-while-debugging-with-visual-studio/) when opening and running the sample solution.

A message received by the bot:
````
From:default-user - To:ii845fc9l02209hh6 - Message:Hello bot!
````

A the message sent to the user:
````
From:ii845fc9l02209hh6 - To:default-user - Message:You sent Hello bot! which was 10 characters
````

### More Information

To get more information about how to get started in Bot Builder for .NET and Conversational history please review the following resources:

* [Bot Builder for .NET](https://docs.microsoft.com/en-us/bot-framework/dotnet/)
* [Intercept messages](https://docs.microsoft.com/en-us/bot-framework/dotnet/bot-builder-dotnet-middleware)
* [Microsoft.Bot.Builder.History namespace](https://docs.botframework.com/en-us/csharp/builder/sdkreference/dc/dc6/namespace_microsoft_1_1_bot_1_1_builder_1_1_history.html)
* [TableLogger (Activity logger using Azure Table Storage)](https://github.com/Microsoft/BotBuilder/blob/master/CSharp/Library/Microsoft.Bot.Builder.Azure/TableLogger.cs#L60)
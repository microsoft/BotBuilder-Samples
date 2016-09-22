# Create New Conversation Bot Sample

A sample bot that starts a new conversation using a previously stored user address.

[![Deploy to Azure][Deploy Button]][Deploy CreateNewConversation/CSharp]
[Deploy Button]: https://azuredeploy.net/deploybutton.png
[Deploy CreateNewConversation/CSharp]: https://azuredeploy.net

### Prerequisites

The minimum prerequisites to run this sample are:
* The latest update of Visual Studio 2015. You can download the community version [here](http://www.visualstudio.com) for free.
* The Bot Framework Emulator. To install the Bot Framework Emulator, download it from [here](https://aka.ms/bf-bc-emulator). Please refer to [this documentation article](https://docs.botframework.com/en-us/csharp/builder/sdkreference/gettingstarted.html#emulator) to know more about the Bot Framework Emulator.

### Code Highlights

Bot Builder uses dialogs to model a conversational process, the exchange of messages, between bot and user. Conversations are usually initiated by the user but sometimes it might be useful for the bot to proactively start a new dialog to interact with the user.
In this sample starting a new dialog is a two steps process: First, creating the new conversation, and then, passing the control to the new dialog.

Check out the use of the `ConnectorClient.CreateDirectConversationAsync()` method in the [MessagesController.cs](Controllers/MessagesController.cs#L33-L44) class to create a new Bot-to-User conversation. This is then stored in the `ResumptionCookie` for later use.

````C#
using (var scope = DialogModule.BeginLifetimeScope(this.scope, activity))
{
    ConnectorClient client = new ConnectorClient(new Uri(activity.ServiceUrl));

    var conversation = await client.Conversations.CreateDirectConversationAsync(activity.Recipient, activity.From);

    var cookie = scope.Resolve<ResumptionCookie>();
    cookie.Address = new Address(cookie.Address.BotId, cookie.Address.ChannelId, cookie.Address.UserId, conversation.Id, cookie.Address.ServiceUrl);

    var postToBot = scope.Resolve<IPostToBot>();
    await postToBot.PostAsync(activity, token);
}
````

The `ResumptionCookie` contains information that can be used to resume a conversation with a user. In this case, we'll use the cookie to craft a new direct message to the user within the conversation created in the previous step.
Check out the [SurveyTriggerer.cs](SurveyTriggerer.cs#L13-L51) class where the `cookie.GetMessage()` is used to find the proper dependencies to execute `stack.Call()` and initiate a new `SurveyDialog`.

````C#
public static async Task StartSurvey(ResumptionCookie cookie, CancellationToken token)
{
    var container = WebApiApplication.FindContainer();

    // the ResumptionCookie has the "key" necessary to resume the conversation
    var message = cookie.GetMessage();

    // we instantiate our dependencies based on an IMessageActivity implementation
    using (var scope = DialogModule.BeginLifetimeScope(container, message))
    {
        // find the bot data interface and load up the conversation dialog state
        var botData = scope.Resolve<IBotData>();
        await botData.LoadAsync(token);

        // resolve the dialog stack
        IDialogStack stack = stack = scope.Resolve<IDialogStack>();

        // make a dialog to push on the top of the stack
        var child = scope.Resolve<SurveyDialog>();

        // wrap it with an additional dialog that will restart the wait for
        // messages from the user once the child dialog has finished
        var interruption = child.Void<object, IMessageActivity>();

        try
        {
            // put the interrupting dialog on the stack
            stack.Call(interruption, null);

            // start running the interrupting dialog
            await stack.PollAsync(token);
        }
        finally
        {
            // save out the conversation dialog state
            await botData.FlushAsync(token);
        }
    }
}
````

Additionally, the sample includes some "plumbing" components mainly intended to make the `ResumptionCookie` dependency available to initiate the survey dialog.
For instance, check out the [SurveyService.cs](SurveyService.cs#L20-L23) which stores the `ResumptionCookie` in the [SurveyScheduler.cs](SurveyScheduler.cs).

````C#
public async Task QueueSurveyAsync()
{
    this.surveyScheduler.Add(this.cookie);
}
````

### Outcome

You will see the following when connecting the Bot to the Emulator and send it a message.

![Sample Outcome](images/outcome-emulator.png)

On the other hand, you will see the following in Skype.

![Sample Outcome](images/outcome-skype.png)

### More Information

To get more information about how to get started in Bot Builder for .NET and Conversations please review the following resources:
* [Bot Builder for .NET](https://docs.botframework.com/en-us/csharp/builder/sdkreference/index.html)
* [Starting Conversations](https://docs.botframework.com/en-us/csharp/builder/sdkreference/routing.html#conversationy)
* [ResumptionCookie class](https://docs.botframework.com/en-us/csharp/builder/sdkreference/dc/d2b/class_microsoft_1_1_bot_1_1_builder_1_1_dialogs_1_1_resumption_cookie.html)

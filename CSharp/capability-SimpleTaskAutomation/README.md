# Simple Task Automation Bot Sample

A sample bot showing how to do simple task automation scenarios.

[![Deploy to Azure][Deploy Button]][Deploy CSharp/SimpleTaskAutomation]

[Deploy Button]: https://azuredeploy.net/deploybutton.png
[Deploy CSharp/SimpleTaskAutomation]: https://azuredeploy.net

### Prerequisites

The minimum prerequisites to run this sample are:
* The latest update of Visual Studio 2015. You can download the community version [here](http://www.visualstudio.com) for free.
* The Bot Framework Emulator. To install the Bot Framework Emulator, download it from [here](https://emulator.botframework.com/). Please refer to [this documentation article](https://github.com/microsoft/botframework-emulator/wiki/Getting-Started) to know more about the Bot Framework Emulator.

### Code Highlights

Dialogs model a conversational process, where the exchange of messages between bot and user is the primary channel for interaction with the outside world.
Dialogs can be composed with other dialogs to maximize reuse, and a dialog context maintains a stack of dialogs active in the conversation.

The [`RootDialog`](Dialogs/RootDialog.cs) class, which represents our conversation, is wired into the `MessageController.Post()` method. Check out the [MessagesController](Controllers/MessagesController.cs#L22) class passing a delegate to the `Conversation.SendAsync()` method that will be used to construct a `RootDialog` and execute the dialog's `StartAsync()` method.


````C#
public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
{
    if (activity.Type == ActivityTypes.Message)
    {
        await Conversation.SendAsync(activity, () => new RootDialog());
    }
    else
    {
        this.HandleSystemMessage(activity);
    }

    var response = Request.CreateResponse(HttpStatusCode.OK);
    return response;
}
````

In the `StartAsync()` method, we are telling the bot to wait for a message from the user and call the `MessageReceivedAsync` resume method when the message ins received.

````C#
public async Task StartAsync(IDialogContext context)
{
    context.Wait(this.MessageReceivedAsync);
}
````
The Bot Framework comes with a number of built-in prompts encapsulated in the [PromptDialog](https://github.com/Microsoft/BotBuilder/blob/84e0973b7e4473b3a02c4e21233b82f439014c95/CSharp/Library/Microsoft.Bot.Builder/Dialogs/PromptDialog.cs) class, than can be used to collect input from a user.  Check out the [RootDialog](Dialogs/RootDialog.cs#L19-L28) class, in the `MessageReceivedAsync` method the usage of the [`PromptChoice`](https://github.com/Microsoft/BotBuilder/blob/84e0973b7e4473b3a02c4e21233b82f439014c95/CSharp/Library/Microsoft.Bot.Builder/Dialogs/PromptDialog.cs#L548) dialog to asks the user to pick up an option from a list.

````C#
private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
{
    PromptDialog.Choice(
        context, 
        this.AfterChoiceSelected, 
        new[] { ChangePasswordOption, ResetPasswordOption }, 
        "What do you want to do today?", 
        "I am sorry but I didn't understand that. I need you to select one of the options below",
        attempts: 2);
}
````

Once the user picks an option the `PromptChoice` dialog ends and return the result to the parent dialog (in this case the RootDialog) by calling to the `ResumeAfter<T>` delegate passed when calling to the child dialog.  The `IDialogContext.Call()` method can be used to Call a child dialog and add it to the top of the stack transferring control to the new dialog.

Check out the [`AfterChoiceSelected`](Dialogs/RootDialog.cs#L30-L52) resume method retrieving the user selection and the usage of of `context.Call()` to give control of the conversation to a new dialog depending on the selected option.

````C#
private async Task AfterChoiceSelected(IDialogContext context, IAwaitable<string> result)
{
    try
    {
        var selection = await result;

        switch (selection)
        {
            case ChangePasswordOption:
                await context.PostAsync("This functionality is not yet implemented! Try resetting your password.");
                await this.StartAsync(context);
                break;

            case ResetPasswordOption:
                context.Call(new ResetPasswordDialog(), this.AfterResetPassword);
                break;
        }
    }
    catch (TooManyAttemptsException)
    {
        await this.StartAsync(context);
    }
}
````

The [`ResetPasswordDialog`](Dialogs/ResetPassword.cs) uses a set of custom Prompts dialogs (classes inheriting from `Prompt<T,U>`) to ask the user for her phone number and her date of birth and validate their input. Prompts implement a retry mechanish and after X attemps they throw a `TooManyAttempsException`. Dialog exceptions can be handled in the `ResumeAfter<T>` delegate passed to the `Call` method



Once the child dialog finishes the `IDialogContext.Done()` should be called to complete the current dialog and return a result to the parent dialog. 

The sample shows how to handle dialog exceptions by awaiting the result argument within a `try/catch` block and how the  [`ResetPasswordDialog`](Dialogs/ResetPassword.cs#L68) uses `context.Done()` to return if the reset operation was successful or not to the parent dialog

````C#
private async Task AfterDateOfBirthEntered(IDialogContext context, IAwaitable<DateTime> result)
{
    try
    {
        var dateOfBirth = await result;

        if (dateOfBirth != DateTime.MinValue)
        {
            await context.PostAsync($"The date of birth you provided is: {dateOfBirth.ToShortDateString()}");

            // Add your custom reset password logic here.
            var newPassword = Guid.NewGuid().ToString().Replace("-", string.Empty);

            await context.PostAsync($"Thanks! Your new password is _{newPassword}_");

            context.Done(true);
        }
        else
        {
            context.Done(false);
        }
    }
    catch (TooManyAttemptsException)
    {
        context.Done(false);
    }
}
````


### Outcome

You will see the following result in the Bot Framework Emulator when opening and running the sample solution.

![Sample Outcome](images/outcome.png)

### More Information

To get more information about how to get started in Bot Builder for .NET and Conversations please review the following resources:
* [Bot Builder for .NET](https://docs.microsoft.com/en-us/bot-framework/dotnet/)
* [Dialogs](https://docs.microsoft.com/en-us/bot-framework/dotnet/bot-builder-dotnet-dialogs)
* [IDialogContext Interface](https://docs.botframework.com/en-us/csharp/builder/sdkreference/d1/dc6/interface_microsoft_1_1_bot_1_1_builder_1_1_dialogs_1_1_i_dialog_context.html)
* [PromptDialog](https://docs.botframework.com/en-us/csharp/builder/sdkreference/d9/d03/class_microsoft_1_1_bot_1_1_builder_1_1_dialogs_1_1_prompt_dialog.html)

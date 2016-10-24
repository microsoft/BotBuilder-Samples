# Application Insights integration Bot Sample
A sample bot which logs telemetry to an Application Insights instance.

[![Deploy to Azure][Deploy Button]][Deploy CSharp/State]
[Deploy Button]: https://azuredeploy.net/deploybutton.png
[Deploy CSharp/State]: https://azuredeploy.net

### Prerequisites

The minimum prerequisites to run this sample are:
* The latest update of Visual Studio 2015. You can download the community version [here](http://www.visualstudio.com) for free.
* The Bot Framework Emulator. To install the Bot Framework Emulator, download it from [here](https://aka.ms/bf-bc-emulator). Please refer to [this documentation article](https://docs.botframework.com/en-us/csharp/builder/sdkreference/gettingstarted.html#emulator) to know more about the Bot Framework Emulator.
* An Application Insights instance in Azure. The Instrumentation Key for which must be put in [ApplicationInsights.config](ApplicationInsights.config#L87)

### Code Highlights

This bot is based off the StateDialog bot, but adds in the ability to log custom telemetry events to an Application Insights instance in Azure.

The notable changes to the StateDialog bot which enable telemetry logging are threefold:
1. Addition of a `TelemetryClient` object in the Application Global namespace:
````C#
    public class WebApiApplication : System.Web.HttpApplication
    {
        public static Microsoft.ApplicationInsights.TelemetryClient Telemetry { get; } = new Microsoft.ApplicationInsights.TelemetryClient();
...
````
2. [Extension methods](TelemetryExtensions.cs) to enable creation of Telemetry objects that will be pre-populated with conversation and user data to enable quick filter/pivoting in the Application Insights dashboard.
3. Usage of these methods throughout the bot's code (eg: here, here, and here)

### Outcome

After configuring, running the bot (locally or in a deployed instance), and having a conversation with it [see the State bot sample for details on the conversation flow](../core-State) you will begin to see events hitting the Application Insights instance you configured within seconds. You can easily filter these events by turning **off** showing Dependency Events so that you're only looking at Custom telemetry (Events, Exceptions, Trace)

The first time you run this sample it will display a welcome message and configure itself to issue search queries for the 'Seattle' city, storing this value in the ConversationData bag. It will also prompt you for your name and store it in the UserData bag and display a help message. Issuing the `change my city` command will allow you to change the search city for this conversation only and just for your user, storing the value in the PrivateConversationData bag.

![Sample Outcome](images/outcome-1.png)

Subsequently, you can start a new conversation (In the Bot Framework Channel Emulator this can be done by using the 'ConversationNames - New' button) and this time the bot will remember you name but will forget the city override we executed in the previous conversation. Using the `change city` command this can be changed for all the users in the conversation.

![Sample Outcome](images/outcome-2.png)

When a another user arrives to the conversation (In the Bot Framework Emulator this is done by editing the 'User' field) the bot will remember its name (Or prompt for the user name depending on any previous conversation) and maintain the previous search city set.

![Sample Outcome](images/outcome-3.png)


To get more information about how to get started in Bot Builder for .NET and Conversations please review the following resources:
* [Bot Builder for .NET](https://docs.botframework.com/en-us/csharp/builder/sdkreference/index.html)
* [Bot State Service](https://docs.botframework.com/en-us/csharp/builder/sdkreference/stateapi.html)
* [Dialogs - Echo Bot with State](https://docs.botframework.com/en-us/csharp/builder/sdkreference/dialogs.html#echoBot)
* [IDialogContext Interface](https://docs.botframework.com/en-us/csharp/builder/sdkreference/d1/dc6/interface_microsoft_1_1_bot_1_1_builder_1_1_dialogs_1_1_i_dialog_context.html)

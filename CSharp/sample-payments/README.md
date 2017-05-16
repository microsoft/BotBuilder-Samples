# Payment Bot Sample

A sample bot showing how to integrate with Microsoft Seller Center for payment processing.

### Prerequisites

The minimum prerequisites to run this sample are:
* The latest update of Visual Studio 2015. You can download the community version [here](http://www.visualstudio.com) for free.
* Register your bot with the Microsoft Bot Framework. Please refer to [this](https://docs.botframework.com/en-us/csharp/builder/sdkreference/gettingstarted.html#registering) for the instructions. Once you complete the registration, update the [Bot's Web.config](PaymentsBot/Web.config#L9-L11) file with the registered config values (MicrosoftAppId and MicrosoftAppPassword).

#### Microsoft Bot Builder

This sample has been developed based on Microsoft Bot Builder Dialog system. You can follow the following [sample](https://github.com/Microsoft/BotBuilder-Samples/tree/master/CSharp/core-MultiDialogs) to become familiar with different kind of dialogs and dialog stack in Bot Builder.

#### Microsoft Seller Center

1. <a href="https://dashboard.stripe.com/register" target="_blank">Create and activate a Stripe account if you don't have one already.</a>

2. <a href="https://seller.microsoft.com/en-us/dashboard/registration/seller/?accountprogram=skypebots&setvar=fltsellerregistration:1" target="_blank">Sign in to Seller Center with your Microsoft account.</a>

3. Within Seller Center, connect your account with Stripe.

4. Within Seller Center, navigate to the Dashboard and copy the value of **MerchantID**.

5. Update your bot's **web.config** file to set `MerchantId` to the value that you copied from the Seller Center Dashboard. 

#### Publish
Also, in order to be able to run and test this sample you must [publish your bot, for example to Azure](https://docs.botframework.com/en-us/csharp/builder/sdkreference/gettingstarted.html#publishing). Alternatively, you can use [Ngrok to interact with your local bot in the cloud](https://blogs.msdn.microsoft.com/jamiedalton/2016/07/29/ms-bot-framework-ngrok/). 

### Outcome

To run the sample, you'll need to publish Bot to Azure or use [Ngrok to interact with your local bot in the cloud](https://blogs.msdn.microsoft.com/jamiedalton/2016/07/29/ms-bot-framework-ngrok/).
* Running Bot app
    1. In the Visual Studio Solution Explorer window, right click on the **PaymentsBot** project.
    2. In the contextual menu, select Debug, then Start New Instance and wait for the _Web application_ to start.

You can use the webchat control in bot framework developer portal to interact with your bot.

### More Information

To get more information about how to get started in Bot Builder for .NET and Conversations please review the following resources:
* [Bot Builder for .NET](https://docs.botframework.com/en-us/csharp/builder/sdkreference/index.html)
* [Bot Framework FAQ](https://docs.botframework.com/en-us/faq/#i-have-a-communication-channel-id-like-to-be-configurable-with-bot-framework-can-i-work-with-microsoft-to-do-that)
* [Bot Builder samples](https://github.com/microsoft/botbuilder-samples)
* [Bot Framework Emulator](https://github.com/microsoft/botframework-emulator/wiki/Getting-Started)

# Rich Cards Bot Sample

A sample bot to renders several types of cards as attachments.

[![Deploy to Azure][Deploy Button]][Deploy RichCards/Node]
[Deploy Button]: https://azuredeploy.net/deploybutton.png
[Deploy RichCards/Node]: https://azuredeploy.net

### Prerequisites

The minimum prerequisites to run this sample are:
* Latest Node.js with NPM. Download it from [here](https://nodejs.org/en/download/).
* The Bot Framework Emulator. To install the Bot Framework Emulator, download it from [here](https://aka.ms/bf-bc-emulator). Please refer to [this documentation article](https://docs.botframework.com/en-us/csharp/builder/sdkreference/gettingstarted.html#emulator) to know more about the Bot Framework Emulator.
* **[Recommended]** Visual Studio Code for IntelliSense and debugging, download it from [here](https://code.visualstudio.com/) for free.

### Code Highlights

Many messaging channels provide the ability to attach richer objects. The Bot Framework has the ability to render rich cards as attachments. There are several types of cards supported: Hero Card, Thumbnail Card, Receipt Card and Sign-In Card. Once the desired Card type is selected, it is mapped into an `Attachment` data structure. Check out the key code located in [app.js](app.js#L28-L34) where a card is attached to the constructed message.

````JavaScript
function (session, results) {

    // create the card based on selection
    var selectedCardName = results.response.entity;
    var card = createCard(selectedCardName, session);

    // attach the card to the reply message
    var msg = new builder.Message(session).addAttachment(card);
    session.send(msg);
}
````

#### Hero Card

The Hero card is a multipurpose card; it primarily hosts a single large image, a button, and a "tap action", along with text content to display on the card. Check out the `createHeroCard` function in [app.js](app.js#L59-L66) for a Hero Card sample.

````JavaScript
function createHeroCard(session) {
    return new builder.HeroCard(session)
        .title('BotFramework Hero Card')
        .subtitle('Your bots — wherever your users are talking')
        .text('Build and connect intelligent bots to interact with your users naturally wherever they are, from text/sms to Skype, Slack, Office 365 mail and other popular services.')
        .images(getSampleCardImages(session))
        .buttons(getSampleCardActions(session));
}
````

#### Thumbnail Card
The Thumbnail card is a multipurpose card; it primarily hosts a single small image, a button, and a "tap action", along with text content to display on the card. Check out the `createThumbnailCard` function in [app.js](app.js#L68-L75) for a Thumbnail Card sample.

```JavaScript
function createThumbnailCard(session) {
    return new builder.ThumbnailCard(session)
        .title('BotFramework Thumbnail Card')
        .subtitle('Your bots — wherever your users are talking')
        .text('Build and connect intelligent bots to interact with your users naturally wherever they are, from text/sms to Skype, Slack, Office 365 mail and other popular services.')
        .images(getSampleCardImages(session))
        .buttons(getSampleCardActions(session));
}
````

#### Receipt Card
The receipt card allows the Bot to present a receipt to the user. Check out the `createReceiptCard` function in [app.js](app.js#L78-L99) for a Receipt Card sample.

````JavaScript
function createReceiptCard(session) {
    return new builder.ReceiptCard(session)
        .title('John Doe')
        .facts([
            builder.Fact.create(session, '1234', 'Order Number'),
            builder.Fact.create(session, 'VISA 5555-****', 'Payment Method'),
        ])
        .items([
            builder.ReceiptItem.create(session, '$ 38.45', 'Data Transfer')
                .quantity(368)
                .image(builder.CardImage.create(session, 'https://github.com/amido/azure-vector-icons/raw/master/renders/traffic-manager.png')),
            builder.ReceiptItem.create(session, '$ 45.00', 'App Service')
                .quantity(720)
                .image(builder.CardImage.create(session, 'https://github.com/amido/azure-vector-icons/raw/master/renders/cloud-service.png'))
        ])
        .tax('$ 7.5')
        .total('$ 90.95')
        .buttons([
            builder.CardAction.openUrl(session, 'https://azure.microsoft.com/en-us/pricing/', 'More Information')
                .image('https://raw.githubusercontent.com/amido/azure-vector-icons/master/renders/microsoft-azure.png')
        ]);
}
````

#### Sign-In Card
The Sign-In card is a card representing a request to sign in the user. Check out the `createSigninCard` function in [app.js](app.js#L101-L105) class for a Sign-In Card sample.

> Note: The sign in card can be used to initiate an authentication flow which is beyond this sample. For a complete authentication flow sample take a look to the [AuthBot](https://github.com/matvelloso/authbot) (Please, notice the SimpleFacebookAuthBot sample is currently in C# only).

````JavaScript
function createSigninCard(session) {
    return new builder.SigninCard(session)
        .text('BotFramework Sign-in Card')
        .button('Sign-in', 'https://login.microsoftonline.com')
}
````

### Outcome

You will see the following in the Bot Framework Emulator, Facebook Messenger and Skype when opening and running the sample.

#### Hero Card

| Emulator | Facebook | Skype |
|----------|-------|----------|
|![Sample Outcome Hero Card](images/outcome-hero-emulator.png)|![Sample Outcome Hero Card](images/outcome-hero-facebook.png)|![Sample Outcome Hero Card](images/outcome-hero-skype.png)|

#### Thumbnail Card

| Emulator | Facebook | Skype |
|----------|-------|----------|
|![Sample Outcome Thumbnail Card](images/outcome-thumbnail-emulator.png)|![Sample Outcome Thumbnail Card](images/outcome-thumbnail-facebook.png)|![Sample Outcome Thumbnail Card](images/outcome-thumbnail-skype.png)|

#### Receipt Card

| Emulator | Facebook | Skype |
|----------|-------|----------|
|![Sample Outcome Receipt Card](images/outcome-receipt-emulator.png)|![Sample Outcome Receipt Card](images/outcome-receipt-facebook.png)|![Sample Outcome Receipt Card](images/outcome-receipt-skype.png)|

#### Sign-In Card

| Emulator | Facebook | Skype |
|----------|-------|----------|
|![Sample Outcome Sign-In Card](images/outcome-signin-emulator.png)|![Sample Outcome Sign-In Card](images/outcome-signin-facebook.png)|![Sample Outcome Sign-In Card](images/outcome-signin-skype.png)|

### More Information

To get more information about how to get started in Bot Builder for Node and Attachments please review the following resources:
* [Bot Builder for Node.js Reference](https://docs.botframework.com/en-us/node/builder/overview/#navtitle)
* [HeroCard](https://docs.botframework.com/en-us/node/builder/chat-reference/classes/_botbuilder_d_.herocard.html)
* [ThumbnailCard](https://docs.botframework.com/en-us/node/builder/chat-reference/classes/_botbuilder_d_.thumbnailcard.html)
* [ReceiptCard](https://docs.botframework.com/en-us/node/builder/chat-reference/classes/_botbuilder_d_.receiptcard.html)
* [SigninCard](https://docs.botframework.com/en-us/node/builder/chat-reference/classes/_botbuilder_d_.signincard.html)
* [Message.addAttachment](https://docs.botframework.com/en-us/node/builder/chat-reference/classes/_botbuilder_d_.message.html#addattachment)
* [Attachment](https://docs.botframework.com/en-us/node/builder/chat-reference/interfaces/_botbuilder_d_.iattachment.html)

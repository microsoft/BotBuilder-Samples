# Send Attachment Bot Sample

A sample bot that passes simple media attachments (images) to a message.

[![Deploy to Azure][Deploy Button]][Deploy SendAttachment/Node]
[Deploy Button]: https://azuredeploy.net/deploybutton.png
[Deploy SendAttachment/Node]: https://azuredeploy.net

### Prerequisites

The minimum prerequisites to run this sample are:
* Latest Node.js with NPM. Download it from [here](https://nodejs.org/en/download/).
* The Bot Framework Emulator. To install the Bot Framework Emulator, download it from [here](https://emulator.botframework.com/). Please refer to [this documentation article](https://github.com/microsoft/botframework-emulator/wiki/Getting-Started) to know more about the Bot Framework Emulator.
* **[Recommended]** Visual Studio Code for IntelliSense and debugging, download it from [here](https://code.visualstudio.com/) for free.

### Code Highlights

Many messaging channels provide the ability to attach richer objects. Bot Builder lets you express these attachments in a cross channel way and [connectors](https://docs.botframework.com/en-us/node/builder/chat-reference/interfaces/_botbuilder_d_.iconnector.html) will do their best to render the attachments using the channels native constructs. If you desire more control over the channels rendering of a message you can use [Message.sourceEvent](https://docs.botframework.com/en-us/node/builder/chat-reference/classes/_botbuilder_d_.message.html#sourceevent) to provide attachments using the channels native schema. The types of attachments that can be sent varies by channel but these are the basic types:
* **Media and Files**: Basic files can be sent by setting [contentType](https://docs.botframework.com/en-us/node/builder/chat-reference/interfaces/_botbuilder_d_.iattachment.html#contenttype) to the MIME type of the file and then passing a link to the file in [contentUrl](https://docs.botframework.com/en-us/node/builder/chat-reference/interfaces/_botbuilder_d_.iattachment.html#contenturl).
* **Cards and Keyboards**: A rich set of visual cards and custom keyboards can by setting [contentType](https://docs.botframework.com/en-us/node/builder/chat-reference/interfaces/_botbuilder_d_.iattachment.html#contenttype) to the cards type and then passing the JSON for the card in [content](https://docs.botframework.com/en-us/node/builder/chat-reference/interfaces/_botbuilder_d_.iattachment.html#content). If you use one of the rich card builder classes like [HeroCard](https://docs.botframework.com/en-us/node/builder/chat-reference/classes/_botbuilder_d_.herocard.html) the attachment will automatically filled in for you.

````JavaScript
function (session) {
    
    // Create and send attachment
    var attachment = {
        contentUrl: "https://docs.botframework.com/en-us/images/faq-overview/botframework_overview_july.png",
        contentType: "image/png",
        name: "BotFrameworkOverview.png"
    };

    var msg = new builder.Message(session)
        .addAttachment(attachment);

    session.send(msg);
}
````

### Outcome

You will see the following in the Bot Framework Emulator when opening and running the sample solution.

![Sample Outcome](images/outcome-emulator.png)

You will see the following in your Facebook Messenger.

![Sample Outcome](images/outcome-facebook.png)

On the other hand, you will see the following in Skype.

![Sample Outcome](images/outcome-skype.png)

### More Information

To get more information about how to get started in Bot Builder for Node and Attachments please review the following resources:
* [Bot Builder for Node.js Reference](https://docs.botframework.com/en-us/node/builder/overview/#navtitle)
* [Adding Attachments to a Message](https://docs.botframework.com/en-us/core-concepts/attachments)
* [Attachments](https://docs.botframework.com/en-us/node/builder/chat-reference/interfaces/_botbuilder_d_.iattachment.html)
* [Message.addAttachment method](https://docs.botframework.com/en-us/node/builder/chat-reference/classes/_botbuilder_d_.message.html#addattachment)

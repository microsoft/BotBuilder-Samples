# Facebook Quick Replies Bot Sample

A bot example demonstrating how to make use of Facebook Messenger Quick Reply functionality.

### Prerequisites

The minimum prerequisites to run this sample are:
* Latest Node.js with NPM. Download it from [here](https://nodejs.org/en/download/).
* A registered bot with the Facebook Channel enabled for debugging, register and enable [here](https://dev.botframework.com).
* **[Recommended]** Visual Studio Code for IntelliSense and debugging, download it from [here](https://code.visualstudio.com/) for free.

### Code Highlights

The bot demonstrates using middleware to extract Facebook Quick Reply payloads for text values, replacing the message text early on so that dialogs and recognizers can correctly handle the value, it also provides an example of collecting the users location using Facebooks 'location' Quick Reply by using a custom prompt to collect and return the location from the user.

Middleware used to extract text based quick replies:

````JavaScript
bot.use({
    botbuilder: function (session, next) {
        if (session.message.source == "facebook") {
            if (session.message.sourceEvent && session.message.sourceEvent.message) {
                if (session.message.sourceEvent.message.quick_reply) {
                    session.message.text = session.message.sourceEvent.message.quick_reply.payload;
                    session.send(`Quick reply text received: ${session.message.text}`);
                    session.endDialog();
                }
            }
        }

        next();
    }
});
````

### Outcome

When you initialise the bot using Facebook messenger the bot will prompt you to choose a demo, two options are available "Text" and "Location". Selecting either will begin the relevent demonstration.

### More Information

To get more information about how to get started in Bot Builder for Node please review the following resources:
* [Bot Builder for Node.js Reference](https://docs.botframework.com/en-us/node/builder/overview/#navtitle)
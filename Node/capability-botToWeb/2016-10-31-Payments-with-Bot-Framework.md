---
published: true
layout: post
title:  "Integrating Payments with Bots"
author: "Bhargav Nookala"
author-link: "http://bhargavsbox.com"
author-image: "images/authorsthumbnail/BhargavNookala.jpg"
author-twitter: "http://twitter.com/bhargav"
image: "images/2016-10-31-Payments-with-Bot-Framework/title.png"
thumbnail: "images/2016-10-31-Payments-with-Bot-Framework/title_thumbnail.png"
date:   2016-10-31 10:00:00
tags: bots Microsoft framework node SDK PayPal
color: "blue"
excerpt: "Integrating payment service providers with the Microsoft Bot Framework."
coderesource: "https://github.com/bnookala/node-paymentbot"
attributions:
    - text: 'PayPal Payments API documentation'
      url: 'https://developer.paypal.com/docs/api/payments/'
    - text: 'PayPal Node SDK'
      url: 'https://github.com/paypal/PayPal-node-SDK'

---

# Background

At a recent collaboration with a customer, we learned about a few integrations they had wanted to build for Bots running with the Microsoft Bot Framework. One such integration was with a payments service provider, like PayPal. We had created an example of this integration with the C# SDK for Bot Framework, but no such example existed for the Node SDK. As such, I'd like to demonstrate how straightforward it is to build a PayPal integration for bots made using Bot Framework's Node SDK.

## Set up an application

You will need to register the bot application with PayPal to generate the requisite keys to access the Paypal APIs. After logging into PayPal, [go to the developer center page to create an application](https://developer.paypal.com/developer/applications/create). Copy the **Client ID** and the **Client Secret** (the **Client Secret** will be hidden from you until you select 'Show' with your cursor).

We additionally need to [create a test user](https://developer.paypal.com/developer/accounts/create) to use with the PayPal sandbox. When testing in the **sandbox** environment, you can use this test account, which doesn't require adding a credit card or funding the account.

## Setup Node and grab the sample

The following steps assume you have Node.js, npm, and a Bot Framework Emulator installed on your computer. To install Node and npm, visit [the Node download page](https://nodejs.org/en/download/) and select 'Current.' At the time of writing, the current iteration of Node is version 6.7.0, but any version that is 4.x or above should work. We do not recommend lower versions as the semantics of some keywords have changed between versions.

The cross platform Bot Framework Emulator is [available to download here](https://docs.botframework.com/en-us/tools/bot-framework-emulator/).

From here on, we'll reference and discuss the code available [on Github](https://github.com/bnookala/node-paymentbot). Using `git`, clone the repository onto your computer in your terminal of choice: `git clone https://github.com/bnookala/node-paymentbot`, `cd` into this directory and run `npm install` to download the module dependencies.

## Running the sample

1. `cd` into the `node-paymentbot` directory
2. `npm install` to install the module dependencies
3. Set the `PAYPAL_CLIENT_ID`, `PAYPAL_CLIENT_SECRET`, and `PAYPAL_CLIENT_MODE` environment variables in your terminal to the copied **Client ID** from PayPal, the copied **Client Secret** from PayPal, and **"sandbox"** respectively. You can optionally set the `PORT` environment variable to customize which port the application is bound to, but it will default to 3978.
4. run `node app.js`, note the URL that it writes the console, i.e., `restify listening to http://[::]:3978`
5. To configure the Bot Framework Emulator, start the emulator application, and enter the bot messaging endpoint. For a locally running bot it should be: `http://localhost:3978/api/messages`
6. Start interacting with the bot!

# Building a payment flow from scratch

PayPal offers many APIs and products for creating and executing payments between users and businesses. To keep our integration straightforward, we'll focus on one relatively simple flow, which involves user approval of payment before we're able to execute and receive a payment. The full flow is described [in PayPal's documentation](https://developer.paypal.com/docs/integration/web/accept-paypal-payment/) in detail, but we'll be using [PayPal's Node SDK](https://github.com/paypal/PayPal-node-SDK) to remove some of the boilerplate in creating and executing the payments.


## Integrating PayPal into a Bot

To use the PayPal Node SDK, we must require it as a module, and configure it with the **Client ID** and **Client Secret** variables we generated from the PayPal dashboard earlier. PayPal also provides a sandbox environment for testing transactions. We'll use this environment here to test payments. In practice, when deploying to a production service, you'll likely want to use different credentials. One popular way to achieve this bucketed approach to deployment is through the underlying system's environment variables. This way, your code can be environment-agnostic, and you won't need to keep your **Client ID** and **Client Secret** variables in your code. When configured, the `paypal` module will be able to create and execute payments and transactions:

<script src="https://gist.github.com/bnookala/a8c8c352e4c6180e822655fa311aa204.js?file=paypal_ex.js"></script>
<noscript><pre><code>
File: paypal_ex.js
------------------

const paypal = require(&#39;paypal-rest-sdk&#39;);

paypal.configure({
    &#39;mode&#39;: process.env.PAYPAL_CLIENT_MODE,
    &#39;client_id&#39;: process.env.PAYPAL_CLIENT_ID,
    &#39;client_secret&#39;: process.env.PAYPAL_CLIENT_SECRET
});
</code></pre></noscript>

To set up our bot, we'll need to create an instance of a `ChatConnector` and an instance of a `UniversalBot`. We'll also need to create an HTTP server, for the `ChatConnector` instance to listen on. A `ChatConnector` allows our Bot to listen and respond on multiple types of messaging channels. Bot Framework currently supports numerous [popular messaging channels](https://docs.botframework.com/en-us/faq/#what-channels-does-the-bot-framework-currently-support), and is consistently adding more. We'll use the `UniversalBot` instance to define our bot's logic (i.e. how it should respond and react when a user performs an action):

<script src="https://gist.github.com/bnookala/a8c8c352e4c6180e822655fa311aa204.js?file=bot_ex.js"></script>
<noscript><pre><code>
File: bot_ex.js
---------------

const restify = require(&#39;restify&#39;);
const builder = require(&#39;botbuilder&#39;);

// A connector connects a bot on bot framework to various messaging services that a bot
// can talk to.
let connector = new builder.ChatConnector({
    appId: undefined,
    appPassword: undefined
});

// A bot listens and reacts to messages that the connector picks up on.
let bot = new builder.UniversalBot(connector);

// We&#39;re using restify here to set up an HTTP server, and then adding the queryParser middleware,
// which will parse the query string into on object on any requests.
let server = restify.createServer();
server.use(restify.queryParser());

// The server will start listening on this port defined in the environment.
server.listen(process.env.PORT, function () {
   console.log(&#39;%s listening to %s&#39;, server.name, server.url);
});

// Messages are posted to this endpoint. We ask the connector to listen at this endpoint for new messages.
server.post(&#39;/api/messages&#39;, connector.listen());
</code></pre></noscript>

This setup is great, but our bot doesn't do anything yet; we haven't defined any logic for it. Let's step back and define a use case for this bot: a city government charges a resident for their parking fine and the resident logs on to complete that payment. This example is straightforward enough that we can build it using a single dialog combined with a waterfalled conversation. For more background on dialogs and conversation, please [read the related section of the Bot Framework documentation](https://docs.botframework.com/en-us/node/builder/guides/core-concepts/#collecting-input). A stubbed out example is as follows:

<script src="https://gist.github.com/bnookala/a8c8c352e4c6180e822655fa311aa204.js?file=bot_dialog_stubbing.js"></script>
<noscript><pre><code>
File: bot_dialog_stubbing.js
----------------------------

// The root dialog of our bot simply just jumps straight into the
// business logic of paying a fine.
bot.dialog(&#39;/&#39;, function (session, args) {
        session.beginDialog(&#39;listFines&#39;);
});

// Simple three step dialog to list &#39;fines&#39; that a user has received, and allow
// a user to &#39;pay&#39; them.
bot.dialog(&#39;listFines&#39;, [
    function (session, args) {
        console.log(&#39;List Fines Dialog&#39;);
        session.send(&#39;You have 1 outstanding fine:&#39;);

        session.send(&#39;Parking Fine Violation&#39;);
        builder.Prompts.choice(session, &quot;What would you like to do?&quot;, [&quot;Pay fine&quot;, &quot;Cancel&quot;]);
    },
    function (session, results, next) {
        let choice = results.response;

        if (choice.entity === &#39;Cancel&#39;) {
            return;
        }

        // TODO: Create a payment, and ask the user to act on it.
    },
    function (session, results) {
        session.send(&#39;Thanks for your payment!&#39;)
    },
]);
</code></pre></noscript>

Now, our bot will be able to respond to conversation. Let's test it in the bot framework emulator:

![UI of Microsoft Bot Framework Channel Emulator. Bot tells user they have one outstanding fine for a parking fine violation, and asks 'what would you like to do?' with option to pay fine or cancel.]({{site.baseurl}}/images/2016-10-31-Payments-with-Bot-Framework/emulator.png)

Our bot can now recognize that we want to pay our fine, but it does not yet know how to process that fine. [Line 24](https://gist.github.com/bnookala/a8c8c352e4c6180e822655fa311aa204#file-bot_dialog_stubbing-js-L24) of the above gist describes a "TODO": creating a payment and asking the user to approve it. This point is the first step in our two-step approval/execution flow for collecting payment from a user. Let's create a function, `createAndSendPayment`, that can create the payment using the PayPal Node SDK, and provide a link that the user can go to the approve the payment. We'll use the function `paypal.payment.create` that the Paypal Node SDK offers to create this payment. The first argument to this function is a JSON object, and the second is a callback that is executed upon success or failure. The schema of the JSON object defines the payment that the user will approve, and includes the name, description, amount, and URLs that PayPal will redirect to on approval or cancellation. The full schema of the JSON object that PayPal requires for payment creation is [described in their documentation](https://developer.paypal.com/docs/api/payments/#payment_create_request). The code as follows describes a function that builds our payment JSON, and one that takes this built JSON, creates a payment and asks the user to approve it:

<script src="https://gist.github.com/bnookala/a8c8c352e4c6180e822655fa311aa204.js?file=payment_json_ex.js"></script>
<noscript><pre><code>
File: payment_json_ex.js
------------------------


/**
 * This function creates and returns an object that is passed through to the PayPal Node SDK
 * to create a payment that a user must manually approve.
 *
 * See https://developer.paypal.com/docs/api/payments/#payment_create_request for a description of the fields.
 */
function createPaymentJson (returnUrl, cancelUrl) {
    return {
        &quot;intent&quot;: &quot;sale&quot;,
        &quot;payer&quot;: {
            &quot;payment_method&quot;: &quot;paypal&quot;
        },
        &quot;redirect_urls&quot;: {
            &quot;return_url&quot;: returnUrl,
            &quot;cancel_url&quot;: cancelUrl
        },
        &quot;transactions&quot;: [{
            &quot;item_list&quot;: {
                &quot;items&quot;: [{
                    &quot;name&quot;: &quot;Fine&quot;,
                    &quot;sku&quot;: &quot;ParkingFine&quot;,
                    &quot;price&quot;: &quot;1.00&quot;,
                    &quot;currency&quot;: &quot;USD&quot;,
                    &quot;quantity&quot;: 1
                }]
            },
            &quot;amount&quot;: {
                &quot;currency&quot;: &quot;USD&quot;,
                &quot;total&quot;: &quot;1.00&quot;
            },
            &quot;description&quot;: &quot;This is your fine. Please pay it :3&quot;
        }]
    };
}
/**
 * Creates a payment on paypal that a user must approve.
 */
function createAndSendPayment (session) {
    console.log(&#39;Creating Payment&#39;);

    let paymentJson = createPaymentJson(&#39;http://localhost&#39;, &#39;http://localhost&#39;);

    paypal.payment.create(paymentJson, function (error, payment) {
        if (error) {
            throw error;
        } else {
            // The SDK returns a payment object when the payment is successfully created.
            // This object has a few properties, described at length here:
            // https://developer.paypal.com/docs/api/payments/#payment_create_response
            // We&#39;re looking for the &#39;approval_url&#39; property, which the user must go to
            // to approve the transaction before we can actively execute the transaction.
            for (var index = 0; index &lt; payment.links.length; index++) {
                if (payment.links[index].rel === &#39;approval_url&#39;) {
                    session.send(&quot;Please pay your fine: &quot; + payment.links[index].href);
                }
            }
        }
    });
};
</code></pre></noscript>

The callback ([starting at Line 44](https://gist.github.com/bnookala/a8c8c352e4c6180e822655fa311aa204#file-payment_json_ex-js-L44-L59)) passed as the second parameter looks through the `payment` object received as an argument and searches for an `approval_url` property, which can be presented to the user. The user must visit this URL and approve the payment before we can continue and execute the payment. We use a Bot Framework built-in, `builder.Prompts.text`, to display the approval URL to the user.

With the user having visited the URL and approved a payment, we must now execute the payment. To do that, we must create a `redirect_url` and provide that in the JSON object for payment creation. PayPal will then redirect our user to this endpoint, which will be hosted by us on the HTTP server we had set up earlier. Soon, we'll use this endpoint to perform the payment execution. Our code now has a endpoint ([lines 3 through 6](https://gist.github.com/bnookala/a8c8c352e4c6180e822655fa311aa204#file-redirect_ex-js-L3-L6)) that PayPal will redirect to:

<script src="https://gist.github.com/bnookala/a8c8c352e4c6180e822655fa311aa204.js?file=redirect_ex.js"></script>
<noscript><pre><code>
File: redirect_ex.js
--------------------

// This endpoint describes the approval redirect endpoint that you can provide in the JSON blob passed to PayPal.
// When a user approves a payment, PayPal will redirect the user to this.
server.get(&#39;approvalComplete&#39;, function (req, res, next) {
    console.log(&#39;User approved payment!&#39;);
    res.send(200);
});

function createReturnUrl () {
    let url = require(&#39;url&#39;);
    // This object encodes the endpoint that PayPal redirects to when.
    // a user approves the payment
    let urlObject = {
        protocol: &#39;http&#39;,
        hostname: &#39;localhost&#39;,
        port: configuration.PORT,
        pathname: &#39;approvalComplete&#39;,
    }

    return url.format(urlObject);
};

function createAndSendPayment (session) {
  let returnUrl = createReturnUrl();
  let paymentJson = createPaymentJson(returnUrl);

  // create payment, etcâ€¦
};
</code></pre></noscript>

To execute an approved payment, we use the `paypal.payment.execute` function provided by PayPal's Node SDK. This function takes three arguments: a payment ID, a JSON blob with [a few fields describing the payment to be executed](https://developer.paypal.com/docs/api/payments/#payment_execute_request), and a callback that is run on success or fail. When PayPal redirects to our endpoint, it modifies the query parameters of the redirect URL, adding a `paymentId` and a `PayerID`. These parameters must be used to execute the payment, with `paymentId` corresponding to the first argument of `paypal.payment.execute`, and `PayerID` corresponding to a field within the JSON blob:

<script src="https://gist.github.com/bnookala/a8c8c352e4c6180e822655fa311aa204.js?file=payment_execution_ex.js"></script>
<noscript><pre><code>
File: payment_execution_ex.js
-----------------------------

server.get(&#39;approvalComplete&#39;, function (req, res, next) {
    console.log(&#39;User approved transaction&#39;);
    executePayment(req.params);
    res.send(200);
});

function executePaymentJson (payerId) {
    return {
        &quot;payer_id&quot;: payerId,
        &quot;transactions&quot;: [{
            &quot;amount&quot;: {
                &quot;currency&quot;: &quot;USD&quot;,
                &quot;total&quot;: &quot;1.00&quot;
            }
        }]
    };
}

function executePayment(params) {
     console.log(&#39;Executing an Approved Payment&#39;);

    // Appended to the URL by PayPal during the approval step.
    let paymentId = parameters.paymentId;
    let payerId = parameters.PayerID;

    // Generate the sample payment execution JSON that paypal requires:
    let paymentJson = executePaymentJson(payerId)

    // Finally, execute the payment, and tell the user that we got their payment.
    paypal.payment.execute(paymentId, paymentJson, function (error, payment) {
        if (error) {
            console.log(error.response);
            throw error;
        } else {
            console.log(&#39;Payment Executed Successfully&#39;);
            // TODO: Inform the user on their bot channel.
        }
    });
};
</code></pre></noscript>

We now have the full payment flow, except the very last step which involves informing the user (via the bot, and on the user's native messaging channel) that their payment has been successfully processed. Since we're no longer operating within the context of the bot, but rather within the context of the HTTP server, we'll have to get creative.

We'll need to modify our code a bit and build up an `Address`, a Bot Framework model that identifies the channel and user with which a message is associated. These properties are all encoded on the `session.message` object normally, and can thus be re-encoded as query arguments in the `return_url` of the approval step. PayPal does not strip these parameters, so they get returned again when the user is redirected to the `approvalComplete` endpoint. When the payment is executed, we can pass these parameters along and create a message to be sent to the user, even in the context of the HTTP Server, without an active `session` object.

<script src="https://gist.github.com/bnookala/a8c8c352e4c6180e822655fa311aa204.js?file=redirect_bot_message_ex.js"></script>
<noscript><pre><code>
File: redirect_bot_message_ex.js
--------------------------------

function createReturnUrl (address) {
    console.log(&#39;Creating Return Url&#39;);

    // The address passed in is an Object that defines the context
    // of the conversation - the user, the channel, the http endpoint the bot
    // exists on, and so on. We encode this information into the return URL
    // to be parsed out by our approval completion endpoint.
    let addressEncoded = encodeURIComponent(JSON.stringify(address));

    // This object encodes the endpoint that PayPal redirects to when.
    // a user approves the transaction.
    let urlObject = {
        protocol: &#39;http&#39;,
        hostname: &#39;localhost&#39;,
        port: configuration.PORT,
        pathname: &#39;approvalComplete&#39;,
        query: addressEncoded
    }

    return url.format(urlObject);
}

function executePayment (parameters) {
    console.log(&#39;Executing an Approved Payment&#39;);

    // Appended to the URL by PayPal during the approval step.
    let paymentId = parameters.paymentId;
    let payerId = parameters.PayerID;

    // Generate the sample payment execution JSON that paypal requires:
    let paymentJson = executePaymentJson(payerId)

    // Grab the encoded address object, URL decode it, and parse it back into a JSON object.
    let addressEncoded = decodeURIComponent(parameters.addressEncoded);
    let address = JSON.parse(addressEncoded);

    // Finally, execute the payment, and tell the user that we got their payment.
    paypal.payment.execute(paymentId, paymentJson, function (error, payment) {
        if (error) {
            console.log(error.response);
            throw error;
        } else {
            console.log(&#39;Payment Executed Successfully&#39;);
            respondToUser(payment, address);
        }
    });
}

function respondToUser (payment, address) {
    let message = new builder.Message().address(address).text(&#39;Thanks for your payment!&#39;);

    // Asks the bot to send the message we built up above to the user.
    bot.send(message.toMessage());
}

bot.dialog(&#39;listFines&#39;, [
    function (session, args) {
        console.log(&#39;List Fines Dialog&#39;);
        session.send(&#39;You have 1 outstanding fine:&#39;);

        session.send(&#39;Parking Fine Violation&#39;);
        builder.Prompts.choice(session, &quot;What would you like to do?&quot;, [&quot;Pay fine&quot;, &quot;Cancel&quot;]);
    },
    function (session, results, next) {
        let choice = results.response;

        if (choice.entity === &#39;Cancel&#39;) {
            return;
        }

        // Starts the payment flow.
        createAndSendPayment(session);
    },
]);


</code></pre></noscript>

[Line 8](https://gist.github.com/bnookala/a8c8c352e4c6180e822655fa311aa204#file-redirect_bot_message_ex-js-L8) identifies the `Address` of the message to be encoded into the `return_url`. When the payment is executed, [lines 34-35](https://gist.github.com/bnookala/a8c8c352e4c6180e822655fa311aa204#file-redirect_bot_message_ex-js-L34-L35) pull these same encoded parameters out of the query string, and pass it along to a new function, `respondToUser`, which uses the address to build up a `builder.Message`. All messages that pass through the Bot Framework are represented internally through this model. Thus, we can create a response here, and send it along to our bot at [line 53](https://gist.github.com/bnookala/a8c8c352e4c6180e822655fa311aa204#file-redirect_bot_message_ex-js-L53). Our complete interaction now looks like this:

![Animated GIF of complete interaction with Microsoft Bot Framework Channel Emulator and PayPal]({{site.baseurl}}/images/2016-10-31-Payments-with-Bot-Framework/complete_flow.gif)


## Conclusion

The code highlighted above consists mostly of snippets and examples that intend to illustrate how we implemented payments with Bot Framework. A working code example is [available on GitHub](https://github.com/bnookala/node-paymentbot).

Through this tutorial, I've identified a common third party service integration, PayPal, and integrated it with a Bot built on Bot Framework's Node SDK. I've also demonstrated that Bot Framework is a flexible tool for integrating conversation into your platform and can coexist with many popular existing integrations.

# Adaptive Cards Bot Sample

A sample bot using [Adaptive Cards](http://adaptivecards.io/) and how to handle user interaction with them.

[![Deploy to Azure][Deploy Button]][Deploy CSharp/AdaptiveCards]

[Deploy Button]: https://azuredeploy.net/deploybutton.png
[Deploy CSharp/AdaptiveCards]: https://azuredeploy.net

### Prerequisites

The minimum prerequisites to run this sample are:
* The latest update of Visual Studio 2015. You can download the community version [here](http://www.visualstudio.com) for free.
* The Bot Framework Emulator. To install the Bot Framework Emulator, download it from [here](https://emulator.botframework.com/). Please refer to [this documentation article](https://github.com/microsoft/botframework-emulator/wiki/Getting-Started) to know more about the Bot Framework Emulator.

### Code Highlights

[Adaptive Cards](http://adaptivecards.io/) are an open card exchange format that enables developers to exchange UI content in a common and consistent way. The Bot Framework has the ability  to use this type of cards and provide a richer interaction experience.

The Adaptive Card can contain any combination of text, speech, images, buttons, and input fields. Adaptive Cards are created using the JSON format specified in Adaptive Cards, which gives you full control over card content and format.

The aesthetics of the card are adapted to the channel's look and feel, making it feel native to the app and familiar to the user. You can use the [Adaptive Cards' Visualizer](http://adaptivecards.io/visualizer) to see how your card renders on different channels.

> Note: At the time of writing this sample, the Adaptive Cards support on the different channels is limited. This sample works properly on the Emulator and WebChat channel. See [more information](https://github.com/Microsoft/AdaptiveCards/issues/367) about channel support.

See how the sample composes a [welcome card](app.js#L30-L156) along with search options:

````C#
````

The previous code will generate a card similar to this one:

![Welcome Card](images/welcome-card.png)

Adaptive Cards are created using JSON, like the one depicted above, and sent as a message attachment:

````C#
````

Adaptive Cards contain many elements that allow to exchange UI content in a common and consistent way. Some of these elements are:

- **TextBlock**

  The TextBlock element allows for the inclusion of text, with various font sizes, weight and color.

- **ImageSet** and **Image**

  The ImageSet allows for the inclusion of a collection images like a photo set, and the Image element allows for the inclusion of images.

- **Input elements**

  Input elements allow you to ask for native UI to build simple forms:

  - **Input.Text** - get text content from the user
  - **Input.Date** - get a Date from the user
  - **Input.Time** - get a Time from the user
  - **Input.Number** - get a Number from the user
  - **Input.ChoiceSet** - Give the user a set of choices and have them pick
  - **Input.ToggleChoice** - Give the user a single choice between two items and have them pick

- **Container**

  A Container is a CardElement which contains a list of CardElements that are logically grouped.

- **ColumnSet** and **Column**

  The columnSet element adds the ability to have a set of Column objects.

- **FactSet**

  The FactSet element makes it simple to display a series of "facts" (e.g. name/value pairs) in a tabular form.

Finally, Adaptive Cards support special elements that enable interaction:

- **Action.OpenUrl**

  When Action.OpenUrl is invoked it will show the given url, either by launching it to an external web browser or showing in-situ with embedded web browser.

- **Action.Submit**

  Action.Submit gathers up input fields, merges with optional data field and generates event to client asking for data to be submitted. The Bot Framework will send an activity through the messaging medium to the bot.

- **Action.Http**

  Action.Http represents the properties needed to do an Http request. All input properties are available for use via data binding. Properties can be data bound to the Uri and Body properties, allowing you to send a request to an arbitrary url.

- **Action.ShowCard**

  Action.ShowCard defines an inline AdaptiveCard which is shown to the user when it is clicked.

You can visit the [Adaptive Cards Schema Explorer](http://adaptivecards.io/explorer/) for samples and the properties each element supports.

#### Creating an inline Adaptive Card

A card may offer the user multiple options to continue. Each option can be offered as a button that, once clicked, expands into a new card within the existing one. This is accomplised using a *ShowCard Action*.
See app.js [Flight's option](app.js#L138-L153) for a simple card and [Hotel's option](app.js#L80-L137) for a complex one.
These are defined within the `actions` element of the main card. See below how the `type` of each action is defined as `Action.ShowCard` and the `card` property contains a new Adaptive Card.

````C#
````

#### Collecting and handling input from the user

Adaptive Cards can include input controls for gathering information from the user that is viewing the card.

At the time of writing this sample, the Adaptive Cards support for input controls is: [Text](http://adaptivecards.io/explorer/#InputText), [Date](http://adaptivecards.io/explorer/#InputDate), [Time](http://adaptivecards.io/explorer/#InputTime), [Number](http://adaptivecards.io/explorer/#InputNumber) and for option selection there is the [Toggle](http://adaptivecards.io/explorer/#InputToggle) and [ChoiceSet](http://adaptivecards.io/explorer/#InputChoiceSet).

See app.js hotel's search form for a simple sample:

````C#
````

The above card will generate a card similar to this one:

![Search Form Card](images/search-form-card.png)

Submitting the information can be be done in two possible ways:

- **Http**

  Action.Http represents the properties needed to do an Http request. All input properties are available for use via data binding. Properties can be data bound to the Uri and Body properties, allowing you to send a request to an arbitrary url. This method can be used to call a service hosted elsewhere through HTTP.

- **Submit**

  Action.Submit gathers up input fields, merges with optional data field and generates event to client asking for data to be submitted. The Bot Framework will send an activity through the messaging medium to the bot. This is the method used in the sample.

When using the **Submit** method, the Bot Framework will handle the submission and your bot will receive a new message with its `value` field filled with the form data as a JSON object.

````C#
````

![Search Form Submission](images/search-form-submit.png)

You'll note there is also a `type` field with the `hotelSearch` value. It is used to later identify the originating submit action. When submitting, the Adaptive Card combines the form values to [Submit Action's `data` property](http://adaptivecards.io/explorer/#ActionSubmit).

Once received the search form parameters, [validation is triggered](app.js#L182), and once it passes, the [`hotels-search`](hotels-search.js) dialog is called with the search parameters as the dialog's argument:

````C#
````

#### Displaying information with ColumnSet

For displaying the hotel search results the sample uses ColumnSet and Columns to format into rows and columns. See how the [`hotels-search` dialog](hotels-search.js#L24-L43) makes use of these elements to create the layout depicted below:

````C#
````

![Search Results Layout](images/search-results-layout.png)

### Outcome

You will see the following in the Bot Framework Emulator when opening and running the sample.

![Sample Outcome Welcome](images/outcome-1.png)

![Sample Outcome Results](images/outcome-2.png)

### More Information

To get more information about how to get started in Bot Builder for Node and Attachments please review the following resources:
* [Adaptive Cards](http://adaptivecards.io/)
* [Adaptive Cards Visualizer](http://adaptivecards.io/visualizer/)
* [Adaptive Cards Schema Explorer](http://adaptivecards.io/explorer/)

> **Limitations**
> The functionality provided in this sample only works with WebChat and the Emulator. Other channels have limited functionality as described in the following [link](https://github.com/Microsoft/AdaptiveCards/issues/367).
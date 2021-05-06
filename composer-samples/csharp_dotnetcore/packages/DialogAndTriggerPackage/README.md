# Microsoft.Bot.Components.Samples.DialogAndTriggerPackage

This sample package contains a dialog and supporting declarative assets for greeting new and returning users, as well as a custom trigger that fires when new members join the conversation.

## Getting Started

Once you've installed the package using [Bot Framework Composer](https://docs.microsoft.com/composer), you'll want to perform the steps outlined below.

1. From Composer, add the new custom trigger to dispatch to your new dialog.
    1. Add the trigger: Activities > Members Added.
    1. If your bot is already using the Greetings trigger, set its Condition to 'false' for now so that it doesn't run.
1. Click the "+" button, and then "Dialog Management" > "Begin a new dialog".
1. In the "Dialog Name" dropdown select the "WelcomeDialog" option.

### Customizing your dialogs

The dialog contained in this package is an examples for welcoming new and returning users. You'll want to customize the messages to meet your specific needs.

## Feedback and issues

If you encounter any issues with this package, or would like to share any feedback please open an Issue in our [GitHub repository](https://github.com/microsoft/botframework-components/issues/new/choose).
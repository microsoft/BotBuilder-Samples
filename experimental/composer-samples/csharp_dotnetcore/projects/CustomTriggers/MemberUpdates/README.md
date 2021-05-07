# Microsoft.Bot.Component.Samples.MemberUpdates

This sample demonstrates custom triggers for [Bot Framework Composer](https://docs.microsoft.com/composer) that fires when members are added or removed from the conversation.

## Getting started

There are two ways you can use this sample:

1. Published it to a NuGet feed.  For testing purposes, this is easiest when done to a [local feed](https://docs.microsoft.com/nuget/hosting-packages/local-feeds). After setting up the local feed, add it to the Package Manager in Composer so that your local packages can be installed in a bot.
   * Once you've installed the package in Composer, you can use it in your bot.

1. Use it directly in your bot project
   1. Copy this project into your bot, and add it to the solution.
   1. Add a project reference to this in your bot project
   1. Perform a "clean", then "build"
   1. Run `update-schema.ps1` in your bots `schemas` folder
   1. Update appsettings `runtimeSettings.components` to include this package:

      ```json
      "runtimeSettings": {
        "components": [
          {
            "name": "Microsoft.Bot.Component.Samples.MemberUpdates"
          }
        ]
      }
      ```

   1. Reload your bot project in Composer.

## Feedback and issues

If you encounter any issues with this package, or would like to share any feedback please open an Issue in our [GitHub repository](https://github.com/microsoft/botbuilder-samples/issues/new/choose).

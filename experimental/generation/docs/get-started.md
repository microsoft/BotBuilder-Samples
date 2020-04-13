# Dialog generation: Get Started

adf

## Installing

asdf

## Using the plugin

asdf


# Setup
This requires Node.js version 12 or higher.  You can check your version with
`node --version` and download the latest from
[node](https://nodejs.org/en/download/) if needed.  

To use this plugin you need to install it into the latest version of the [bf cli
tool][bf].  This makes use of the daily build from [myget](myget) in order to ensure that all
functionality is working. To do so:
1.  `npm config set registry https://botbuilder.myget.org/F/botframework-cli/npm/`
2.  `npm install -g @microsoft/botframework-cli` 
3.  `npm install -g @microsoft/bf-generate`
4.  `npm config set registry https://registry.npmjs.org/`

If you want to easily execute the resulting dialogs you can optionally install
the [runbot][runbot] CLI.  Alternatively you can setup your own Bot Framework runtime
which provides the ability to extend the framework using code.



## Example
1. Download an [example sandwich JSON
   schema](https://raw.githubusercontent.com/microsoft/botframework-cli/master/packages/dialog/test/commands/dialog/forms/sandwich.schema).
2. `bf dialog:generate sandwich.schema -o bot`
3. This will generate .lg, .lu and .dialog assets in the bot directory.  In
   order to run them, you will need to build a LUIS model.
   1. `bf luis:build --in bot\luis --authoringKey <yourKey> --botName sandwich --dialog --suffix %USERNAME%`
4. At this point you have a complete bot rooted in `bot\sandwich.main.dialog`.
5. If you have installed [runbot](runbot), you can run this bot and test in the
   emulator like this:
   1. Start the web server `dotnet
      <pathToRepo>/experimental/generation/runbot/runbot.csproj --root
      <dialogFolder>`
   2. Connect emulator to `http://localhost:5000/api/messages` and interact with your bot.

## Documentation Index

1. [Get started][start]
1. Working with schema
    1. [Writing schemas][schema]
    1. [Sample schemas][sample-schemas]
1. Working with templates
    1. [Writing templates][templates-overview]
    1. [Pre-built templates][templates]
1. [Presentation (pptx)](2020%20Feb%20MVP%20Generated%20Dialogs.pptx)
1. [White paper (docx)](Generating%20Dialogs%20from%20Schema,%20APIs%20and%20Databases.docx)

[schema]:bot-schema.md
[templates]:../generator/templates
[templates-overview]:templates.md
[start]:get-stared.md
[sample-schemas]:example-schemas

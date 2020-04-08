<!-- omit in TOC -->
@microsoft/bf-generate
======================

Generate Bot Framework Adaptive Dialogs from JSON schema.

[![oclif](https://img.shields.io/badge/cli-oclif-brightgreen.svg)](https://oclif.io)
[![Version](https://img.shields.io/npm/v/@microsoft/bf-generate.svg)](https://npmjs.org/package/@microsoft/bf-generate)
[![Downloads/week](https://img.shields.io/npm/dw/@microsoft/bf-generate.svg)](https://npmjs.org/package/@microsoft/bf-generate)
[![License](https://img.shields.io/npm/l/@microsoft/bf-generate.svg)](https://github.com/Microsoft/https://github.com/Microsoft/BotBuilder-Samples/blob/master/package.json)

- [Intro](#intro)
- [Setup](#setup)
- [Usage](#usage)
  - [dialog:generate Arguments](#dialoggenerate-arguments)
  - [Example](#example)
  - [Defining JSON Schema](#defining-json-schema)
    - [Advanced JSON Schema](#advanced-json-schema)
  - [Writing Templates](#writing-templates)

# Intro
The Bot Framework has a rich collection of building blocks, but creating a bot
requires understanding and cooridinating across language understanding, language
generation and dialog mangement.  To simplify this process and capture best
practices, this plugin for the [bf cli tool][bf] will automatically generate a
complete declarative dialog for collecting all of the information defined in a
[JSON Schema](#defining-json-schema).  The generated dialog makes use of default
templates that generate event driven dialogs that support a rich and evolving set
of [capabilities](templates/readme.md) including:
- Generating .lg, .lu and .dialog files that robustly handle out of order and
  multiple responses for simple and array properties.
- Add, remove, clear and show for properties.
- Support for choosing between ambiguous entity values and entity property mappings.
- Recognizing and mapping all LUIS prebuilt entities.
- Help including auto-help on multiple retries.
- Cancel
- Confirmation

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

# Usage
The overall workflow for generation is to:
1. Define your [JSON schema](#defining-json-schema) with optional extensions.
2. Generate your dialog assets using [dialog:generate](#dialog:generate-arguments).
3. You can test the resulting assests in your own runtime using Bot Framework
   Adaptive dialogs or use the [runbot][runbot] CLI if you use standard Bot
   Framework SDK components.
4. You can modify the generated assests using [Visual Studio Code][vscode] which
   supports intellisense and validation according to your runtime schema.
   Eventually you will also be able to edit using [Bot Framework Composer][composer].
5. **Coming soon** If you change your schema you can update the generated assets
   using the --integrate switch.

## dialog:generate Arguments
The dialog:generate command generates .lu, .lg, .qna and .dialog assets from a
schema defined using [JSON Schema](#defining-json-schema). The parameters to the
command are:

- **--force, -f** Force overwriting generated files.
- **--help, -h** Generate help.
- **--locale, -l** Locales to generate. By default en-us.
- **--output, -o** Output directory.
- **--schema, -s** Path to your app.schema file. By default is the standard SDK app.schema.
- **--templates, -t** Directories with templates to use for generating assets.
  First definition wins.  A directory of "standard" includes the standard
  templates included with the tool.  You can also use the "template:<file>" URI to refer to files found
  in template directories.
- **--verbose, -v** Verbose logging of generated files.

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

## Defining JSON Schema

Schemas are specified using [JSON Schema][JSONSchema]. You can use the normal
JSON Schema mechanisms including `$ref` and `allOf` which will be resolved into a
single schema. This makes the schema easier to use for things like language
generation. For most purposes you just need to include the
[standard.schema](templates/readme.md#usage) or make use of [prebuilt LUIS
Schema](templates/readme.md#luis-prebuilt-entities).  You can look at an
[example schema](test/commands/dialog/forms/sandwich.schema) that shows how to
define a schema.

### Advanced JSON Schema

Globally there are a few extra keywords you can add to your schema.  Most of these keywords are
automatically filled during generation and you do not need to worry about them if you make use of [standard.schema](templates/standard.schema).  Extra keywords include:
- **\$public** List of the public properties in the schema. By default these are
  the top-level properties in the root schema and this will be automatically
  generated if missing.
- **\$requires** A list of JSON Schema to include.  This is different than $ref
  in that you can either use a URL or just a filename which will be looked for
  in the template directories.  If you want to include the standard
  confirmation/cancel/navigation functionality you should include include
  `standard.schema`.
- **\$expectedOnly** A global list of entities which will only be bound to
  properties if the property is expected.  This can be overriden for specific
  properties as well.
- **\$triggerIntent** Name of the trigger intent--by default the name of the
  schema.
- **\$templates** Global templates to include.
- **\$defaultOperation** Default operation to use for assigning entities to
  properties.  This is overriden for a given Ask by setting
  `$dialog.expectedOperation`.

 The final schema and all of the required schemas will have the top-level
 `properties`, `definitions`, `required`, `$expectedOnly`, `$public` and
 `$templates` merged. For other properties, last definition is included.

In addition there are a few extra keywords per-property including:
- **\$entities** List of entity names that can map to a property. The order of
  the entities also defines the precedence to use when resolving entities. If
  not present, default \$entities can be specified in a `# entities` template
  definition.
  - **\$templates** The template names to use for generating assets. The list usually comes from a `# templates` definition in the template for a property type.
- **\$expectedOnly** A list of entities that are only possible if they are
  expected.  This overrides the top-level \$expectedOnly.

You can use expression syntax, i.e. `${<expr>}` to dynamically
generate schema, either before generation or after generation is done if there
are properties that are only available at the end. `<schemaName>.schema.dialog`
will have the resulting schema with all references resolved and expanded to
simplify usage in language generation and dialogs. 

## Writing Templates

You do not need to write or even understand templates in order to use generated
dialogs. For most purposes the standard templates do everything needed and you
can ignore this section.  Templates provide an opportunity to completely
customize the generation process by extending or replacing the standard
templates.  The templates themselves make use of the botbuilder-lg library to do
the generation.  The sections below describe the mechanisms used for using
templates to generate dialogs.

Each entity or property can have associated .lu, .lg, .qna and .dialog files
that are generated by copying or instantiating templates found in the template
directories. If a template name matches exactly it is just copied. If the
template ends with .lg then it is analyzed to check for these template names:
- **filename** Define the filename to generate.
- **template** Define the content of the file.
- **entities** If a property does not have an explict `$entities` defined, this
  provides a default one.
- **templates** A list of other templates to look for. If the resulting filename
  is found explictly in a template directory, then that replaces the generated
  file. This provides the ability to override generated files for specific
  properties.

When evaluating templates there are a number of variables defined in the scope
including:
- **prefix** Prefix to use for generated files.
- **appSchema** The path to the app.schema to use.
- **schema** The full JSON Schema including the root schema + internal
  properties.
- **locales** The list of all locales being generated.
- **properties** All of the $public property names.
- **entities** All of the entities being used in the schema.
- **triggerIntent** \$triggerIntent or the schema name by default.
- **locale** The locale being generated.
- **property** Current property being generated. 
- **templates** Object with generated templates per lu, lg, qna, json and
  dialog. The object contains:
  - **name** Base name of the template without final extension.
  - **fallbackName** For .lg files the base filename without locale.
  - **fullName** The name of the template including the extension.
  - **relative** Path relative to the output directory of where template is.

The generation process at a high-level: 
- Evalaute schema expressions and expand them if possible.  (It may not be
  possible because it depends on generated information.)
- Per-locale
  - Per-property
    - Look for `<propertyType>.lg` which usually defines `# entities` and `#
      templates`.
    - Per-template in the property schema `$templates` or in `# templates`
      generate .lg, .lu, .qna, .dialog files.
  - Evaluate templates defined in the schema root level `\$templates` of the
    merged schema.
- Evaluate schema expressions and write the full schema out to
  `<prefix>.schema.dialog`.

All generated assets are named using a standard pattern which is what enables
updating generated files when changing the schema. In the below, $italics$ are
place holders. 
* $schema$-library -- Defines library files that are either common building
  blocks like [library.lg.lg](templates/library.lg.lg) or are internal
  mechanisms like [library-help.lg.lg](templates/library-Help.lg.lg).
* $schema$-$entity$Entity -- Defines entity specific files.
* $schema$-$property$ -- Defines property related files when $property$ is found
  in the schema.
* $locale$/ -- Assets for a particular locale that follows the above naming
  patterns and adds $locale$ to the file extension, an example would be
  `en-us/sandwich-Bread.en-us.lg` which would be a localized .lg asset for the
  `Bread` property in the `sandwich` schema. You can define your own templates
  that add to the naming conventions, but they must extend these conventions.

[JSONSchema]:https://json-schema.org/
[bf]:https://github.com/microsoft/botframework-cli
[myget]:https://botbuilder.myget.org/gallery
[runbot]:../runbot/readme.md
[composer]:https://github.com/Microsoft/BotFramework-Composer
[vscode]:https://code.visualstudio.com/Download
# Dialog Generation

The Bot Framework has a rich collection of conversational building blocks, but
creating a bot that feels natural to converse with requires understanding and
coordinating across language understanding, language generation and dialog
management. To simplify this process and capture best practices, we've created
the [bf-generate][bf-generate] plugin for the [BotFramework CLI tool][bf]. The
generated dialogs make use of event-driven adaptive dialogs with a rich and
evolving set of capabilities including:

- Handle out of order and multiple responses for simple and array properties.
- Add, remove, clear and show properties.
- Support for choosing between ambiguous entity values and entity property mappings.
- Recognizing and mapping for all LUIS prebuilt entities.
- Help function, including auto-help on multiple retries.
- Cancel
- Confirmation

**Ready to [Get Started][start]?** If not, keep reading for more information.

We welcome your feedback as we work through this feature - see the
[Feedback](#feedback) section for details on how you can help shape the future
of this tool.

## Overview

The `bf-generate` plugin is an experimental plugin for the `bf cli` tool that
generates adaptive dialog assets. The overall workflow for generation is (see
[Get Started][start] for detailed instructions):

1. Define your [schema](#schema-file) with optional extensions.
2. Generate your dialog assets using [dialog:generate][bf-generate].
3. Run the generated assets using your own runtime, or use the [RunBot][runbot]
   runtime if you use standard Bot Framework SDK components.
4. Test your bot using the [Bot Framework Emulator][emulator].
5. You can modify the generated assets using [Visual Studio Code][vscode] which
   supports IntelliSense and validation according to your runtime schema.
   Eventually you will also be able to edit using [Bot Framework
   Composer][composer].
6. If you change your schema you can update the generated assets using the
   `--merge` option.

There are three key components to understand, which are outlined below.

### Schema file

A JSON schema file defines the properties your bot needs to collect, and
(optionally) the entity definitions those properties map to. Schemas are written
using [JSON Schema][JSONSchema]. You can use the normal mechanisms including
`$ref` and `allOf` which will be resolved into a single schema.

For additional information see:

- [Schema overview][schema]
- [Sample schemas][sample-schemas]

### Templates

Templates are `.lg` files the generation tool uses to inject intelligence into
your generated dialogs. They handle things like ambiguity resolution,
interruption, cancellation and more, as well as allowing you to map complex
pre-built entity types to your properties.

In general, you won't need to understand how they work, or write any yourself -
the tool comes with a set of pre-built templates that are sufficient for most
cases. However, you may need to know what templates are available for you when
creating your schema in order to map your properties to entities.

For additional information see:

- [Templates overview][templates-overview]
- [Pre-built templates][templates]

### Merging

Once you have generated a bot and customized it, you might want to change your
schema and generate again.  In order to support this, the --merge flag will
merge the newly generated assets into your existing assets.  The merge should
not overwrite any of your work, but it is a good idea to commit before merging.
Rules for merging:
 1) A file unchanged since last generated will be overwritten by the new file.
 2) A changed file will have its .lg/.lu enum or .dialog triggers overwritten,
    but nothing else and its hash code should not be updated.
 3) If a property existed in the old schema, but does not exist in the new
    schema all files for that property should be deleted and have references
    removed.
 4) If a property exists in both old and new schema, but a file is not present
    in the new directory, the file should not be copied over again and
    references should not be added.
 5) The order of .dialog triggers should be respected, i.e. if changed by the
    user it should remain the same. 
 6) If a file has changed and cannot be updated there will be a message to merge
    manually.
    
### Bot Runtime

The generation tool creates a set of adaptive assets for your bot.  Included in
those assets are two scripts:
* `build [region] [luis authoring key]` which will publish your LUIS model using
  [bf][bf].
* `run [region] [luis authoring key]` which will use [RunBot][runbot] to run
  your bot if you use just SDK components.


### Use Cases

Currently, generating dialogs works best for "slot/form-filling" style bots -
bots that collect a related set of information from a user in order to complete
a task. You can use this tool to:

- Generate a slot/form-filling style bot scaffold - you'll probably still want
  to tweak the generated `.lg` files to customize the language to fit your
  needs.
- Explore a 'best-practices' bot for examples of how to handle complex language
  interactions with adaptive dialogs.
  
## Adding chit-chat

To add professional chit-chat to your schema put this into your schema:
```json
"$requires": [
   "standard.template",
   "professional-chit-chat.template"
]
```
This indicates that you want to make use of the standard schema for your
properties and want add professional chit-chat.  This will add the resource to
your generated assets and automatically include cross-training in the scripts.

## Generating Schema from Swagger

You can generate a JSON schema from a swagger file using the [`bf
dialog:generate:swagger`](generator/packages/cli/readme.md#bf-dialoggenerateswagger-path) command. This will
create a bot for collecting the parameters to a method as well as the assets to
call that API once the parameters are collected.

## Generating a Test from a Transcript
A simple way to test your bot is to generate a test file from a transcript using [`bf
dialog:generate:test`](generator/packages/cli/readme.md#bf-dialoggeneratetest-transcript-dialog) and [TestBot][TestBot].  

## Planned enhancements

Over time we plan to add additional capabilities for generating dialogs from
databases or Virtual Assistant skills.

## Feedback

Help us build this tool! We need your help to refine and decide what to build
next. We're particularly intersted in:

1. Did you get stuck anywhere?
1. What types of data would you like to be able to generate adaptive assets from (other than JSON schema)?
1. What additional templates would you find useful?
1. And as always, did you encounter any bugs?

Open a [new issue](https://github.com/microsoft/BotBuilder-Samples/issues/new/choose) and tell us what you think!

## Documentation Index

1. [Get started][start]
1. Working with schema
    1. [Writing schemas][schema]
    1. [Sample schemas][sample-schemas]
1. Working with templates
    1. [Writing templates][templates-overview]
    1. [Pre-built templates][templates]
1. [Presentation (pptx)](docs/2020%20Feb%20MVP%20Generated%20Dialogs.pptx)
1. [White paper (docx)](docs/Generating%20Dialogs%20from%20Schema,%20APIs%20and%20Databases.docx)
2. [Event driven dialog (docx)](docs/Event%20Driven%20Dialog.docx)
3. [Form Generation NLU model (docx)](docs/Form%20Generation%20NLU%20Model.docx)

[schema]:docs/bot-schema.md
[templates]:generator/packages/library/templates
[templates-overview]:docs/templates.md
[start]:docs/get-started.md
[JSONSchema]:https://json-schema.org/
[bf]:https://github.com/microsoft/botframework-cli
[myget]:https://botbuilder.myget.org/gallery
[runbot]:runbot/
[composer]:https://github.com/Microsoft/BotFramework-Composer
[vscode]:https://code.visualstudio.com/Download
[bf-generate]:generator/packages/cli/README.md
[bf]:https://github.com/microsoft/botframework-cli
[composer]:https://github.com/Microsoft/BotFramework-Composer
[sample-schemas]:docs/example-schemas
[luis-prebuilt]:https://docs.microsoft.com/azure/cognitive-services/LUIS/luis-reference-prebuilt-entities#english-american-entity-support
[emulator]:https://github.com/microsoft/BotFramework-Emulator
[TestBot]:TestBot/readme.md#
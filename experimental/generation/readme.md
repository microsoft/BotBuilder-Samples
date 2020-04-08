# Dialog Generation

The Bot Framework has a rich collection of building blocks, but creating a bot
requires understanding and cooridinating across language understanding, language
generation and dialog mangement.  To simplify this process and capture best
practices we have created the [bf-generate][bf-generate] plugin for the [bf cli
tool][bf].  Given a JSON Schema, this tool will automatically generate a
complete declarative dialog to collect all of the information in the schema.  
The generated dialog makes use of event driven dialogs to support a rich and
evolving set of capabilities including:
- Generating .lg, .lu and .dialog files that robustly handle out of order and multiple responses for simple and array properties.
- Add, remove, clear and show for properties.
- Support for choosing between ambiguous entity values and entity property mappings.
- Recognizing and mapping all LUIS prebuilt entities.
- Help including auto-help on multiple retries.
- Cancel
- Confirmation 

Over time we plan to add additional capabilities for generating from swagger
files or databases and incorporating additional technologies like QnA Maker and
Virtual Assistant skills.

In order to simplify interacting with a bot that uses the standard Bot Framework SDK you can also install and use the CLI tool [runbot][runbot].  

Presentations and more detailed documents about generated dialogs.
* [Presentation](docs/2020%20Feb%20MVP%20Generated%20Dialogs.pptx)
* [White paper](docs/Generating%20Dialogs%20from%20Schema,%20APIs%20and%20Databases.docx)

[bf-generate]:generator/README.md
[bf]:https://github.com/microsoft/botframework-cli
[runbot]:runbot/readme.md
[composer]:https://github.com/Microsoft/BotFramework-Composer
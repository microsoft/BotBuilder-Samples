# Generating Dialogs
The [bf cli tool](https://github.com/microsoft/botframework-cli) allows you to automatically [generate a bot](https://github.com/microsoft/botframework-cli/blob/master/packages/dialog/src/commands/dialog/readme.md#generate) from a [JSON Schema](https://json-schema.org/) file. To use it you create a JSON schema file and the tool will generate .lg, .lu and .dialog files that are all wired together as declarative adaptive dialogs that can then be modified with composer.  This is a great way to bootstrap a bot that needs to fill in a 'form' and also to learn best practices from the structure.  The default (but overridable) templates generate a bot that supports:
- Generating .lg, .lu and .dialog files that robustly handle out of order and multiple responses.
- Support for choosing between ambiguous entity values and entity property mappings.
- Recognizing and mapping all LUIS prebuilt entities.
- Help including auto-help on multiple retries.
- Cancel
- Confirmation
- Navigating to a property


    
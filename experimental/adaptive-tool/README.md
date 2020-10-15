# Bot Framework Adaptive Tool

Bot Framework Adaptive Tool is a VS Code extension that helps developers handle LG (.lg), LU (.lu), and Dialog (.dialog) files efficiently. Adaptive Tool has tools and settings that make it easy to debug, analyze and enhance you language files.

Supported features include the following:
<!--
- LG/LU syntax highlighting
- LG/LU diagnostic checks
- LG/LU autocompletion
- LG template and function hover
- LG template definition
- LG template, function, and structure suggestions
- Dialog (.dialog) debugging
- LG debugging
-->

| Feature                                       | File type supported |
|-----------------------------------------------|---------------------|
| [Syntax highlighting](#syntax-highlighting-diagnostic-checks,-and-autocompletion)                           | LG, LU              |
| [Diagnostic checks](#syntax-highlighting-diagnostic-checks,-and-autocompletion)                             | LG, LU              |
| [Autocompletion](#syntax-highlighting-diagnostic-checks,-and-autocompletion)                                | LG, LU              |
| [Template and function hover](#hover,-suggestions,-and-navigation)                   | LG                  |
| [Template definition](#hover,-suggestions,-and-navigation)                           | LG                  |
| [Template, function, and structure suggestions](#hover,-suggestions,-and-navigation) | LG                  |
| [Debugging](#debugging)                                     | LG, LU, Dialog      |

## Getting started

Adaptive Tool can be installed from the [VS Code Extension Marketplace](#install-from-the-vs-code-extension-marketplace) or from a [local VSIX file](#install-from-a-local-vsix-file).

### Install from the VS Code Extension Marketplace

1. Install the Bot Framework Adaptive Tool [extension](https://marketplace.visualstudio.com/items?itemName=adaptive-tool) from the [VS Code Extension Marketplace](https://marketplace.visualstudio.com/vscode).
1. Open an LG, LU, or Dialog file and the extension will activate.

### Install from a local VSIX file

1. Clone the Adaptive Tool [repository](https://github.com/microsoft/BotBuilder-Samples/tree/main/experimental/adaptive-tool). In a terminal, navigate to the repository folder and run the following commands:

    ```cmd
    npm install
    npm run build
    ```

1. If `vsce` is not installed globally run `npm install -g vsce`.
1. Run `vsce package` to export the VSIX file. Now you're ready to install
1. Open Visual Studio Code and navigate to the **Extensions**.
1. Click the three dots in the top right corner of the Extensions. Then click **Install from VSIX...** and select the VSIX file you created earlier.
1. Open an LG, LU, or Dialog file and the extension will activate.

## Language features

Language features are driven by the [language server protocol](./languageServer.md). See the [Language Server Extension Guide](https://code.visualstudio.com/api/language-extensions/language-server-extension-guide) for more information.

### Syntax highlighting, diagnostic checks, and autocompletion

#### LU

Different colors and styles for `intent`, `entity`, and `comment` components:
![lu_syntax_highlighting](https://raw.githubusercontent.com/microsoft/BotBuilder-Samples/main/experimental/adaptive-tool/resources/images/lu_syntax_highlighting.png)

Formatting warnings and errors:
![lu_diagnostic](https://raw.githubusercontent.com/microsoft/BotBuilder-Samples/main/experimental/adaptive-tool/resources/images/lu_diagnostic.png)

Automatic completion of some entities:
![lu_completion](https://raw.githubusercontent.com/microsoft/BotBuilder-Samples/main/experimental/adaptive-tool/resources/images/lu_completion.gif)

#### LG

Different colors and styles for `template`, `function`, `multi line`, `structure`, `comment`, `condition`, and `switch` components:
![lg_syntax_highlighting](https://raw.githubusercontent.com/microsoft/BotBuilder-Samples/main/experimental/adaptive-tool/resources/images/lg_syntax_highlighting.png)

Formatting warnings and errors:
![lg_diagnostic](https://raw.githubusercontent.com/microsoft/BotBuilder-Samples/main/experimental/adaptive-tool/resources/images/lg_diagnostic.gif)

### Hover, suggestions, and navigation

#### LG

Template reference hover:
![template_hover](https://raw.githubusercontent.com/microsoft/BotBuilder-Samples/main/experimental/adaptive-tool/resources/images/template_hover.png)

Prebuilt function hover:
![function_hover](https://raw.githubusercontent.com/microsoft/BotBuilder-Samples/main/experimental/adaptive-tool/resources/images/function_hover.png)

Suggestions for templates and functions:
![function_template_suggestion](https://raw.githubusercontent.com/microsoft/BotBuilder-Samples/main/experimental/adaptive-tool/resources/images/function_template_suggestion.gif)

Structure property suggestions:
![structure_suggestion](https://raw.githubusercontent.com/microsoft/BotBuilder-Samples/main/experimental/adaptive-tool/resources/images/structure_suggestion.gif)

Template navigation:

![template_definition](https://raw.githubusercontent.com/microsoft/BotBuilder-Samples/main/experimental/adaptive-tool/resources/images/template_definition.gif)

## Debugging

Adaptive Tool lets developers debug LG, LU, and Dialog files in runtime. This section covers the steps and shows and example of how to [configure](#configuration), [initialize](#start-bot-runtime-and-complete-initialization), and [debug](#debug-the-runtime) a bot.

### Configuration

- [Install](#getting-started) the Adaptive Tool extension.
- Open the LG, LU ,or Dialog file to debug.
- To configure Visual Studio Code, add a target in your `launch.settings` file.

Here's an example of a typical `launch.json`:

```json
{
    "type": "json",
    "request": "attach",
    "name": "Attach to Dialog",
    "debugServer": 4712
}
```

`debugServer` refers to the port bot runs on. The default value is `4712`.

### Start bot runtime and complete initialization

1. Start a bot runtime. For this example we'll start the bot project `TodoBot` in [SampleBots](https://github.com/microsoft/botbuilder-dotnet/tree/main/tests/Microsoft.Bot.Builder.TestBot.Json).
1. Make sure the debugger port has been registered in `BotFrameworkHttpAdapter` with the `UseDebugger` method.
1. There are several ways to initialize a bot, and [BotFramework Emulator](https://github.com/microsoft/BotFramework-Emulator) is a typical approach. Open the Emulator and attach it to the bot to finish the initialization.

### Debug the runtime

1. Run the Visual Studio Code program by clicking **F5** and set break points in the LG, LU, and/or Dialog files.
1. Chat with the bot in the Emulator.
1. If the extension is working properly the cursor will stop when the code hits any of corresponding breakpoints.

![breakpoints](../adaptive-tool/resources/images/breakpoints.gif)

## Adaptive Tool settings

Settings for LG files can be found under **LG** in the Extensions settings.

|Setting name|Description|
|-----|---------------|
|`LG.Expression.ignoreUnknownFunction`|Show the diagnostic severity level of unknown functions in a LG file.<br><br>The levels include:<br/>`error`: treat unknown functions as error diagnostic<br>`warn`: treat unknown functions as warning diagnostic<br>`ignore`: ignore unknown functions|
|`LG.Expression.customFunctionList`| Create a comma-separated customized function list (example: a, b, c). You can use both custom functions added in your logic as well as functions added to the `customFunctionList` setting.|

## Contributing

Code contributions are welcome via the [BotBuilder-Samples](https://github.com/microsoft/BotBuilder-Samples) repo.

## Feedback

File bugs in [GitHub Issues](https://github.com/Microsoft/BotBuilder-Samples/issues).

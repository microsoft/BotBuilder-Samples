# Bot Framework Adaptive Tool

Bot Framework Adaptive Tool is a Visual Studio Code extension that helps developers handle LG (.lg), LU (.lu) files and Dialog (.dialog) files efficiently.

The support includes:
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
| Syntax highlighting                           | LG, LU              |
| Diagnostic checks                             | LG, LU              |
| Autocompletion                                | LG, LU              |
| Debugging                                     | LG, LU, Dialog      |
| Template and function hover                   | LG                  |
| Template definition                           | LG                  |
| Template, function, and structure suggestions | LG                  |
|||

## Getting started

Adaptive Tool can be installed from the [Visual Studio Code Marketplace](#install-from-the-visual-studio-code-marketplace) or from a [local VSIX file](#install-from-a-local-vsix-file).

### Install from the Visual Studio Code Marketplace

1. Install the [Bot Framework Adaptive Tool](https://marketplace.visualstudio.com/items?itemName=adaptive-tool) from the [Visual Studio Code marketplace](https://marketplace.visualstudio.com/vscode).
1. Open an LG (.lg) or LU (.lu) file and the extension will activate.

### Install from a local VSIX file

#### Step 1: Build the extension package

1. Clone the [repository](https://github.com/microsoft/BotBuilder-Samples/tree/main/experimental/adaptive-tool). In a terminal navigate to the repository folder and run the following commands:

```cmd
npm install
npm start
```

1. If `vsce` is not installed globally run `npm install -g vsce`.
1. Run `vsce package` to export the VSIX file.

#### Step 2: Install the extension package

1. Open Visual Studio Code and navigate to the **Extensions**.
1. Click the three dots in the top right corner of the Extensions. Then click **Install from VSIX...** and select the VSIX file you created in [step 1](#step-1:-build-the-extension-package).

## Language features

Language features are driven by the [language server protocol](./languageServer.md). For more information about Language Servers see the [Language Server Extension Guide](https://code.visualstudio.com/api/language-extensions/language-server-extension-guide)

### Syntax highlighting, diagnostic checks, and autocompletion

Adaptive Tool makes it easy to analyze your files with syntax highlighting, diagnostic checks, and autocompletion.

## [LU](#tab/lu)

- Different colors and styles for `intent`, `entity`, and `comment` components in LU files:
![lu_syntax_highlighting](https://raw.githubusercontent.com/microsoft/BotBuilder-Samples/main/experimental/adaptive-tool/resources/images/lu_syntax_highlighting.png)

- Formatting warnings and errors:
![lu_diagnostic](https://raw.githubusercontent.com/microsoft/BotBuilder-Samples/main/experimental/adaptive-tool/resources/images/lu_diagnostic.png)

- Automatic completion of some entities:
![lu_completion](https://raw.githubusercontent.com/microsoft/BotBuilder-Samples/main/experimental/adaptive-tool/resources/images/lu_completion.gif)

## [LG](#tab/lg)

- Different colors and styles for `template`, `function`, `multi line`, `structure`, `comment`, `condition`, and `switch` components in LG files:
![lg_syntax_highlighting](https://raw.githubusercontent.com/microsoft/BotBuilder-Samples/main/experimental/adaptive-tool/resources/images/lg_syntax_highlighting.png)

- Formatting warnings and errors:
![lg_diagnostic](https://raw.githubusercontent.com/microsoft/BotBuilder-Samples/main/experimental/adaptive-tool/resources/images/lg_diagnostic.gif)

- Template reference hover:
![template_hover](https://raw.githubusercontent.com/microsoft/BotBuilder-Samples/main/experimental/adaptive-tool/resources/images/template_hover.png)

- Prebuilt function hover:
![function_hover](https://raw.githubusercontent.com/microsoft/BotBuilder-Samples/main/experimental/adaptive-tool/resources/images/function_hover.png)

- Suggestions for templates and functions:
![function_template_suggestion](https://raw.githubusercontent.com/microsoft/BotBuilder-Samples/main/experimental/adaptive-tool/resources/images/function_template_suggestion.gif)

- Structure property suggestions:
![structure_suggestion](https://raw.githubusercontent.com/microsoft/BotBuilder-Samples/main/experimental/adaptive-tool/resources/images/structure_suggestion.gif)

- Template navigation:
![template_definition](https://raw.githubusercontent.com/microsoft/BotBuilder-Samples/main/experimental/adaptive-tool/resources/images/template_definition.gif)

---

## Debugging

Adaptive Tool lets developers debug LG, LU, and Dialog files in runtime.

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

1. Start a bot runtime. For example, start the bot project: `TodoBot` in [SampleBots](https://github.com/microsoft/botbuilder-dotnet/tree/hond/debugger/tests/Microsoft.Bot.Builder.TestBot.Json).
1. Make sure the debugger port has been registered in `BotFrameworkHttpAdapter` with `UseDebugger` method.
1. There are several ways to initialize a bot, and [BotFramework Emulator](https://github.com/microsoft/BotFramework-Emulator) is a typical approach. Open the Emulator and attach it to the bot to finish the initialization.

### Debug the runtime

1. Run the Visual Studio Code program by clicking **F5** and setting break points in the LG, LU, or Dialog file.
1. Chat with the bot in the Emulator.
1. If the extension is working properly the cursor will stop when the code hits any of corresponding breakpoints.

## Adaptive Tool settings

Settings for LG files can be found under **LG** in the Extensions settings.

|Name|Description|
|-----|---------------|
|`LG.Expression.ignoreUnknownFunction`|Show the diagnostic severity level of unknown functions in a LG file.<br><br>The levels include:<br/>`error` - treat unknown functions as error diagnostic<br>`warn` - treat unknown functions as warning diagnostic<br>`ignore` - ignore unknown functions|
|`LG.Expression.customFunctionList`| Create a comma-separated customized function list (example: a, b, c). You can use both custom functions added in your logic as well as functions added to the `customFunctionList` setting.|

## Contributing

Code contributions are welcome via the [BotBuilder-Samples](https://github.com/microsoft/BotBuilder-Samples) repo.

## Feedback

File bugs in [GitHub Issues](https://github.com/Microsoft/BotBuilder-Samples/issues).
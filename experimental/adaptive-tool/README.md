# Bot Framework Adaptive Tool
Bot Framework Adaptive Tool is a Visual Studio Code extension that helps developers handle LG (.lg) and LU (.lu) files efficiently.

The support includes:
- LG/LU syntax highlighting
- LG/LU diagnostic checks
- LG/LU completion
- LG template and function hover
- LG template definition
- LG template, function, and structure suggestions
- Dialog (.dialog) debugging
- LG debugging

# Quick Start
## Install from the marketplace
1. Install the [adaptive-tool](https://marketplace.visualstudio.com/items?itemName=adaptive-tool) from the [Visual Studio Code marketplace](https://marketplace.visualstudio.com/vscode).
1. Open an LG (.lg) or LU (.lu) file and the adaptive-tool extension will activate.

## Install From Local VSIX File
1. [Build VSIX from code.](#buildPackage)
1. Open Visual Studio Vode and navigate to the **Extensions**.
1. Click the three dots in the top right corner of the Extensions. Then click **Install from VSIX...** and select the VSIX file you created in the first step.

# Features

## Language Features
Language features are driven by the [language server protocol](./languageServer.md)

### Syntax highlighting, diagnostic check, auto-suggest, functionality

#### LU
- Different colors and styles for `intent`, `entity`, and `comment` components in LU files
![lu_syntax_highlighting](https://raw.githubusercontent.com/microsoft/BotBuilder-Samples/main/experimental/adaptive-tool/resources/images/lu_syntax_highlighting.png)

- Formatting warnings and errors
![lu_diagnostic](https://raw.githubusercontent.com/microsoft/BotBuilder-Samples/main/experimental/adaptive-tool/resources/images/lu_diagnostic.png)

- Automatic completion of some entities
![lu_completion](https://raw.githubusercontent.com/microsoft/BotBuilder-Samples/main/experimental/adaptive-tool/resources/images/lu_completion.gif)

#### LG
- Different colors and styles for `template`, `function`, `multi line`, `structure`, `comment`, `condition`, and `switch` components in LG files.
![lg_syntax_highlighting](https://raw.githubusercontent.com/microsoft/BotBuilder-Samples/main/experimental/adaptive-tool/resources/images/lg_syntax_highlighting.png)

- Formatting warnings and errors
![lg_diagnostic](https://raw.githubusercontent.com/microsoft/BotBuilder-Samples/main/experimental/adaptive-tool/resources/images/lg_diagnostic.gif)

- Template reference hover
![template_hover](https://raw.githubusercontent.com/microsoft/BotBuilder-Samples/main/experimental/adaptive-tool/resources/images/template_hover.png)

- Built-in function hover
![function_hover](https://raw.githubusercontent.com/microsoft/BotBuilder-Samples/main/experimental/adaptive-tool/resources/images/function_hover.png)

- Suggestions for templates and functions
![function_template_suggestion](https://raw.githubusercontent.com/microsoft/BotBuilder-Samples/main/experimental/adaptive-tool/resources/images/function_template_suggestion.gif)

- Structure property suggestions
![structure_suggestion](https://raw.githubusercontent.com/microsoft/BotBuilder-Samples/main/experimental/adaptive-tool/resources/images/structure_suggestion.gif)

- Template navigation 
![template_definition](https://raw.githubusercontent.com/microsoft/BotBuilder-Samples/main/experimental/adaptive-tool/resources/images/template_definition.gif)

## Debugging
Adaptive Tool lets developers debug .dialog and LG files in runtime.

### Configuration
- Install the adaptive-tool extension.
- Open the LG or .dialog file  to debug.
- To configure Visual Studio Code, add a target in your `launch.settings` file.

This is an example of a typical `launch.json`:
```json
{
    "type": "json",
    "request": "attach",
    "name": "Attach to Dialog",
    "debugServer": 4712
}
```

`debugServer` refers to the port bot runs on.

### Start bot runtime and complete initialization

Start a bot runtime. For example, start a bot project: `todobot` in [SampleBots](https://github.com/microsoft/botbuilder-dotnet/tree/hond/debugger/tests/Microsoft.Bot.Builder.TestBot.Json). Then make sure the debugger port has been registered in `BotFrameworkHttpAdapter` with `UseDebugger` method.

There are several ways to initialize a bot, and [BotFremawork Emulator](https://github.com/microsoft/BotFramework-Emulator) is a typical approach. Open the emulator and attach it to the bot to finish the initialization.

### Debug the runtime
- Run the Visual Studio Code program by clicking **F5** and setting break points in the .dialog or LG file.
- Chat with the bot in the emulator.
- The cursor will stop when the code hits any of corresponding breakpoints.


# Useful Commands
|Command|Description|
|-----|---------------|
|`lgLiveTest.start`|Start a WebView to test the current LG file|


# Adaptive-tool Settings
|Name|Description|
|-----|---------------|
|`LG.Expression.ignoreUnknownFunction`|Configure Diagnostics: Show the diagnostic severity level of unknown functions in a LG file|
|`LG.Expression.customFunctionList`|Customized function list, should be separated by comma (example: a, b, c)|


<a name="buildPackage"></a>

# Build Extension Package
- `npm install`.
- `npm run build`.
- `npm install -g vsce`, if `vsce` is not installed globally.
- Run `vsce package` to export VSIX files.

# Contributing

Code contributions are welcome via the [BotBuilder-Samples](https://github.com/microsoft/BotBuilder-Samples) repo.

# Feedback

- File a bug in [GitHub Issues](https://github.com/Microsoft/BotBuilder-Samples/issues).
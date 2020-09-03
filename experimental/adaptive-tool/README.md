# Bot Framework Adaptive Tool
BotFramework adaptive tool is a vscode extension to help developers to improve the efficiency of handling LG/LU files.
The support includes:
- LG/LU syntax highlighting
- LG/LU diagnostic check
- LG/LU completion feature
- LG template/function hover
- LG template definition
- LG template/function/structure suggestion
- Dialog(.dialog) debugging
- LG debugging

# Quick Start
## Install From Marketplace
1. Install the [adaptive-tool](https://marketplace.visualstudio.com/items?itemName=adaptive-tool) from marketplace.
2. Open a LG(.lg) or LU(.lu) file and the adaptive-tool extension will activate.

## Install From Local VSIX File
1. [Build VSIX from code.](#buildPackage)
2. Open vscode and navigate to extension tab.
3. Select 'install from VSIX...'.

# Features

## Language Features
language features are driven by [language server protocol](./languageServer.md)

### Syntax highlighting, diagnostic check, auto-suggest, functionality
#### LU documents
- Provide different colors and styles for `intent`, `entity`, `comment` components in LU file.
![lu_syntax_highlighting](https://raw.githubusercontent.com/microsoft/BotBuilder-Samples/main/experimental/adaptive-tool/resources/images/lu_syntax_highlighting.png)
- Provide warnings/errors for the wrong format.

![lu_diagnostic](https://raw.githubusercontent.com/microsoft/BotBuilder-Samples/main/experimental/adaptive-tool/resources/images/lu_diagnostic.png)
- Automatically complete some entities.
![lu_completion](https://raw.githubusercontent.com/microsoft/BotBuilder-Samples/main/experimental/adaptive-tool/resources/images/lu_completion.gif)

#### LG documents
- Provide different colors and styles for `template`, `function`, `multi line`, `structure`, `comment`, `condition`, `switch` components in LG file.
![lg_syntax_highlighting](https://raw.githubusercontent.com/microsoft/BotBuilder-Samples/main/experimental/adaptive-tool/resources/images/lg_syntax_highlighting.png)

- Provide warnings/errors for the wrong format.
![lg_diagnostic](https://raw.githubusercontent.com/microsoft/BotBuilder-Samples/main/experimental/adaptive-tool/resources/images/lg_diagnostic.gif)

- Template reference hover
![template_hover](https://raw.githubusercontent.com/microsoft/BotBuilder-Samples/main/experimental/adaptive-tool/resources/images/template_hover.png)

- Builtin function hover
![function_hover](https://raw.githubusercontent.com/microsoft/BotBuilder-Samples/main/experimental/adaptive-tool/resources/images/function_hover.png)

- Provide suggestions for template and function
![function_template_suggestion](https://raw.githubusercontent.com/microsoft/BotBuilder-Samples/main/experimental/adaptive-tool/resources/images/function_template_suggestion.gif)

- Structure property suggestion
![structure_suggestion](https://raw.githubusercontent.com/microsoft/BotBuilder-Samples/main/experimental/adaptive-tool/resources/images/structure_suggestion.gif)

- Template navigation feature
![template_definition](https://raw.githubusercontent.com/microsoft/BotBuilder-Samples/main/experimental/adaptive-tool/resources/images/template_definition.gif)

## Debugging Feature
Adaptive tool provides the feature to debug dialog/lg files in runtime.
### Setting up
- Install adaptive tool extension.
- Open the LG/dialog file which you want to debug.
- To configure Visual Studio Code you need to add a target in your launch.settings file.

The classic `launch.json` is like:
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
- Start a bot runtime.

For example, start a bot project: `todobot` in [SampleBots](https://github.com/microsoft/botbuilder-dotnet/tree/hond/debugger/tests/Microsoft.Bot.Builder.TestBot.Json)

Make sure the debugger port has been registered in `BotFrameworkHttpAdapter` with `UseDebugger` method.

- Complete initialization
There are several ways to initialize a bot.
[BotFremawork Emulator](https://github.com/microsoft/BotFramework-Emulator) is a typical solution.
Open emulator and attach it to the bot to finish the initialization.

### Debug the runtime
- Run the vscode program with `F5` and set some breaking points in dialog/LG.
- Chat with the bot in the emulator.
- The cursor would stop when the code hitting the corresponding breakpoint location.


# Useful Commands
|Command|Description|
|-----|---------------|
|`lgLiveTest.start`|Start a WebView to test current LG file|


# Adaptive-tool Settings
|Name|Description|
|-----|---------------|
|`LG.Expression.ignoreUnknownFunction`|Configure Diagnostics: Show diagnostic severity level of unknow functions in a LG file|
|`LG.Expression.customFunctionList`|Customized function list, should be separated by comma, eg. a, b, c|


<a name="buildPackage"></a>

# Build Extension Package
- `npm install`.
- `npm run build`.
- `npm install -g vsce`, if `vsce` is not installed globally.
- run `vsce package` to export vsix file.


# Contributing
Code contributions are welcomed via the [BotBuilder-Samples](https://github.com/microsoft/BotBuilder-Samples) repo.


# Feedback
- File a bug in [GitHub Issues](https://github.com/Microsoft/BotBuilder-Samples/issues).
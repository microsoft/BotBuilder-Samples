# Bot Framework Adaptive Tool
BotFramework adaptive tool is a vscode extension to help developers to improve the efficiency of handling LG/LU files.

# Quick Start
## Install From Marketplace
1. Install the [adaptive-tool](https://marketplace.visualstudio.com/items?itemName=adaptive-tool) from marketplace.
2. Open a LG(.lg) or LU(.lu) file and the adaptive-tool extension will activate.

## Install From Local VSIX File
1. [Build VSIX from code.](#buildPackage)
2. Open vscode and navigate to extension tab
3. Select 'install from VSIX...'

# Features

## Language Features
language features are driven by [language server protocol](./languageServer.md)

### Syntax highlighting, diagnostic check, auto-suggest, functionality
#### LU documents
- Syntax highlighting
![lu_syntax_highlighting](https://raw.githubusercontent.com/microsoft/BotBuilder-Samples/main/experimental/adaptive-tool/resources/images/lu_syntax_highlighting.png)
- Diagnostic check
![lu_diagnostic](https://raw.githubusercontent.com/microsoft/BotBuilder-Samples/main/experimental/adaptive-tool/resources/images/lu_diagnostic.png)
- Completion 
![lu_completion](https://raw.githubusercontent.com/microsoft/BotBuilder-Samples/main/experimental/adaptive-tool/resources/images/lu_completion.gif)

#### LG documents
- Syntax highlighting
![lg_syntax_highlighting](https://raw.githubusercontent.com/microsoft/BotBuilder-Samples/main/experimental/adaptive-tool/resources/images/lg_syntax_highlighting.png)

- Diagnostic check
![lg_diagnostic](https://raw.githubusercontent.com/microsoft/BotBuilder-Samples/main/experimental/adaptive-tool/resources/images/lg_diagnostic.gif)

- Template reference hover
![template_hover](https://raw.githubusercontent.com/microsoft/BotBuilder-Samples/main/experimental/adaptive-tool/resources/images/template_hover.png)

- Builtin function hover
![function_hover](https://raw.githubusercontent.com/microsoft/BotBuilder-Samples/main/experimental/adaptive-tool/resources/images/function_hover.png)

- Builtin function and template suggestion
![function_template_suggestion](https://raw.githubusercontent.com/microsoft/BotBuilder-Samples/main/experimental/adaptive-tool/resources/images/function_template_suggestion.gif)

- Structure property suggestion
![structure_suggestion](https://raw.githubusercontent.com/microsoft/BotBuilder-Samples/main/experimental/adaptive-tool/resources/images/structure_suggestion.gif)

- Template definition
![template_definition](https://raw.githubusercontent.com/microsoft/BotBuilder-Samples/main/experimental/adaptive-tool/resources/images/template_definition.gif)

## Debugging Feature
### Setting up and using Visual Studio Code to run client and server
#### Setting up
To configure Visual Studio Code you need to add a target in your launch.settings file.

* **Bot: Launch language server and client on vscode** - Configuration for building and launching your client on vscode and connecting to server
Example is:
```json
{
    "type": "extensionHost",
    "request": "launch",
    "name": "Launch Client",
    "runtimeExecutable": "${execPath}",
    "args": ["--extensionDevelopmentPath=${workspaceRoot}"],
    "outFiles": ["${workspaceRoot}/client/out/**/*.js"],
    "preLaunchTask": {
        "type": "npm",
        "script": "watch"
    },
    "sourceMaps": true
},
{
    "type": "node",
    "request": "attach",
    "name": "Attach to LgServer",
    "port": 6010,
    "restart": true,
    "sourceMaps": true,
    "outFiles": ["${workspaceRoot}/server/out/lg/**/*.js"]
}
```

#### Troubleshooting
There are 2 places your bot can be running depending on the tools you are using.

* **Visual Studio** - Visual studio runs using IIS Express.  IIS Express keeps running even after visual studio shuts down
* **Visual Studio Code** - VS code uses **dotnet run** to run your bot.

If you are switching between environments make sure you are talking to the right instance of your bot.


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
- `npm install`
- `npm run build`
- `npm install -g vsce`, if `vsce` is not installed globally.
- run `vsce package` to export vsix file


# Contributing
Code contributions are welcomed via the [BotBuilder-Samples](https://github.com/microsoft/BotBuilder-Samples) repo.


# Feedback
- File a bug in [GitHub Issues](https://github.com/Microsoft/BotBuilder-Samples/issues)
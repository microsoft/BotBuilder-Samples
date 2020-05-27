# Bot Framework Adaptive Tool

## Get started
- `npm install`
- `npm run build`
- `npm install -g vsce`, if `vsce` is not installed globally.
- run `vsce package` to export vsix file
- open vscode and navigate to extension tab
- select 'install from VSIX...'
- pick exported vsix file, and restart vscode if needed
- edit a lg file, try some features
- press `F1` then type `LG live test` to start LG webview test

## Features
### Debugging
[Setting up](#set_up)
- supporting adaptive dialog breaking point
- support LG template breaking points
- support Expression breaking points

### Syntax highlighting, auto-suggest, auto complete functionality
#### .lu documents
- syntax highlighting
![lu_syntax_highlighting](./resources/images/lu_syntax_highlighting.png)
- diagnostic check
![lu_diagnostic](./resources/images/lu_diagnostic.png)

#### .lg documents
- syntax highlighting
![lg_syntax_highlighting](./resources/images/lg_syntax_highlighting.png)

- diagnostic check
![lg_diagnostic](./resources/images/lg_diagnostic.gif)

- template reference hover
![template_hover](./resources/images/template_hover.png)

- builtin function hover
![function_hover](./resources/images/function_hover.png)

- buildin function and template suggestion
![function_template_suggestion](./resources/images/function_template_suggestion.gif)

- structure property suggestion
![structure_suggestion](./resources/images/structure_suggestion.gif)

- template definition
![template_definition](./resources/images/template_definition.gif)


### Expansion/ test UI for .lg documents
- template/free text evaluation
Press `F1` and select `LG live tester` to start LG tester.
This tool can be used to test specific template or free inline text.

<a name="set_up"></a>

## Setting up and using Visual Studio Code to use the debugger
### setting up
To configure Visual Studio Code you need to add a target in your launch.settings file.

You can do that by the **add a configuration** in the debug panel.

There should be 2 configuration templates available:

* **Bot: Launch .NET Core Configuration** - Configuration for building and launching your bot via **dotnet run** and connecting to it
* **Bot: Attach Configuration** - Configuration for attaching to the debug port of an already running bot (such as IIS express)

### Using

* Open any source file (*.dialog, *.lg) and set breakpoints.
* Hit F5 to start debugger.

As you interact with the bot your breakpoint should hit and you should be able to inspect memory, call context, etc.

### Troubleshooting
There are 2 places your bot can be running depending on the tools you are using.

* **Visual Studio** - Visual studio runs using IIS Express.  IIS Express keeps running even after visual studio shuts down
* **Visual Studio Code** - VS code uses **dotnet run** to run your bot.

If you are switching between environments make sure you are talking to the right instance of your bot.

## Contribute
- `npm install` to install packages
- `npm run build`
- press `F5` to start debug
- reference doc: [vscode extension](https://code.visualstudio.com/api/language-extensions/overview)


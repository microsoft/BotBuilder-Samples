# Bot Framework Adaptive Tool
BotFramework adaptive tool is a vscode extension to help developers to improve the fficiency of handering LG/LU files.


# Features

## language features
language features are driven by [language server protocol](./languageServer.md)

### Syntax highlighting, diagnostic check, auto-suggest, functionality
#### .lu documents
- syntax highlighting
![lu_syntax_highlighting](./resources/images/lu_syntax_highlighting.png)
- diagnostic check
![lu_diagnostic](./resources/images/lu_diagnostic.png)
- completion 
![lu_completion](./resources/images/lu_completion.gif)

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

## debugging feature
 see more details in [debugging](./debugging.md)


# how to build and use this extension
- `npm install`
- `npm run compile`
- `npm install -g vsce`, if `vsce` is not installed globally.
- run `vsce package` to export vsix file
- open vscode and navigate to extension tab
- select 'install from VSIX...'
- pick exported vsix file, and restart vscode if needed
- edit a lg file, try some features
- press `F1` then type `LG live test` to start LG webview test

# Contribute
ref to [contribute](./contribute.md)
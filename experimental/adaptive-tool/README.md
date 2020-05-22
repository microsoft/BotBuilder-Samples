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
### Debugging adaptive dialogs
- Supporting adaptive dialog breaking point

### Debugging language generation
- Support LG template breaking points
- Support Expression breaking points

### Syntax highlighting, auto-suggest, auto complete functionality
#### .lu documents
- Syntax highlighting

#### .lg documents
- Syntax highlighting
- template reference hover
- builtin function hover
- buildin function suggestion
- template reference suggestion
- template definition
- error check

### Expansion/ test UI for .lg documents
- template evaluation

## Contribute
- `npm install` to install packages
- `npm run build`
- press `F5` to start debug
- open lg file to debug
- reference doc: [vscode extension](https://code.visualstudio.com/api/language-extensions/overview)

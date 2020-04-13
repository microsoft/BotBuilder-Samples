# Bot Framework Adaptive Tool

## Features
### Debugging adaptive dialogs
- Supporting adaptive dialog breaking point

### Debugging language generation
- Support LG template breaking points
- Support Expression breaking points

### Syntax highlighting, auto-suggest, auto complete functionality
#### .lu documents
- coming soon

#### .lg documents
- Syntax highlighting
- template reference hover
- builtin function hover
- buildin function suggestion
- template reference suggestion
- template definition
- error check
- Builtin function signature

### Expansion/ test UI for .lg documents
- template evaluation


## Get Started
- into adaptive-tool folder
- `npm install` to install packages
- `npm run build`
- `npm install -g vsce`, if `vsce` is not installed globally.
- use `vsce package` to export vsix file
- open vscode, and extension tab
- select 'install from VSIX...'
- select vsix file, and reopen vscode
- edit a lg file, try some features.
- input `LG live test` in `F1` space, try lgfile webview test

## Contribute
- into LGvscodeExt folder
- `npm install` to install packages
- `npm run build`
- press `F5` to debug
- open lg file to debug
- reference doc: [vscode extension](https://code.visualstudio.com/api/language-extensions/overview)
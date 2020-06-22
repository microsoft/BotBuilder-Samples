## How to this extension form source code
- `npm install`
- `npm run build`
- `npm install -g vsce`, if `vsce` is not installed globally.
- run `vsce package` to export vsix file
- open vscode and navigate to extension tab
- select 'install from VSIX...'
- pick exported vsix file, and restart vscode if needed
- edit a lg file, try some features
- press `F1` then type `LG live test` to start LG webview test

## Contribute
- `npm install` to install packages
- `npm run build`
- press `F5` to start debug
- reference doc: [vscode extension](https://code.visualstudio.com/api/language-extensions/overview)

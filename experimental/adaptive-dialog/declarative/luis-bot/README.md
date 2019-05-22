To get set up, 

Install ludown from here - https://github.com/Microsoft/botbuilder-tools/tree/vishwac/v.future

```bash
> lerna bootstrap --hoist
> lerna run build
> cd packages\ludown
> npm uninstall -g ludown
> npm i -g
> cd packages\lubuild
> npm i -g
```

Once you have the latest ludown and lubuild CLI installed, simply run these two commands any time your .lu models change.

```bash
> cd 70.luis-todo-bot
> ludown parse tosuggest -f Dialogs -o generated -r RootDialog -e -q -u --verbose
> cd bin
> lubuild --authoringKey <YOUR-LUIS-KEY> --dialogs
```

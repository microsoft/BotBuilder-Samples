# @microsoft/generator-bot-command-list

This template creates a bot built around a set of simple commands. It includes cards with ["message back" actions](https://docs.microsoft.com/en-us/microsoftteams/platform/task-modules-and-cards/cards/cards-actions#messageback) for the commands "hello" and "help."

## What this template is for

Use this template if you want to...

- Create a bot that relies on cards actions and/or regular expressions.
- Build your bot by composing an ad-hoc set of packages.

## Languages

- English (en-US)

## Azure Resource Deployment

This template does not rely on any additional Azure Resources.

## Using this template

This template can be instantiated directly from the command line by cloning this repository to your local environment. 

First, install [Yeoman][yeoman] using [npm][npm] (we assume you have pre-installed [node.js][nodejs]):

```bash
npm install -g yo
```

Next, navigate to the `generator-bot-command-list` directory in your locally cloned repository, and install the package's dependencies using [npm][npm]:

```bash
cd experimental\generators\generator-bot-command-list
npm install
```

Now create the directory where you would like to create your bot project:

```bash
mkdir CommandList
cd CommandList
```

Finally, generate your new bot using [Yeoman][yeoman], taking note of the following:

- Replace `{REPO_ROOT_PATH}` with the directory that your copy of the repository has been cloned to.
- `--platform` may be one of either `dotnet` or `js`, and will default to `dotnet` if not specified.
- `--integration` may be one of either `webapp` or `functions`, and will default to `webapp` if not specified.

```bash
yo {REPO_ROOT_PATH}\experimental\generators\generator-bot-command-list\generators\app '{BOT_NAME}' --platform '{dotnet|js}' --integration '{webapp|functions}'
```

## Next Steps

You may now continue further editing your bot using [Bot Framework Composer](https://github.com/microsoft/botframework-composer). After opening your project, Composer will guide you through making customizations to your bot. If you'd like to extend your bot with code, you can open up your bot using your favorite IDE (like Visual Studio) from the directory you created earlier.

## License

[MIT License](https://github.com/microsoft/botframework-components/blob/main/LICENSE)

[npm]: https://npmjs.com
[nodejs]: https://nodejs.org/
[yeoman]: https://yeoman.io

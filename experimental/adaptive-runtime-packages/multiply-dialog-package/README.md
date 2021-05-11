# multiply-dialog-package

Bot Framework v4 adaptive runtime package sample.

This is an example of a package that can be consumed by the new adaptive runtime.

## Prerequisites

- [Node.js](https://nodejs.org) version 10.14.1 or higher

    ```bash
    # determine node version
    node --version
    ```

## To try this sample

- Clone the repository

    ```bash
    git clone https://github.com/microsoft/botbuilder-samples.git
    ```

- In a terminal, navigate to `experimental/adaptive-runtime-packages/multiply-dialog-package`

    ```bash
    cd samples/typescript_nodejs/00.empty-bot
    ```

- Install modules

    ```bash
    npm install
    ```

- Build the package

    ```bash
    npm run build
    ```

## Runtime details

You can see how the runtime [loads packages](https://github.com/microsoft/botbuilder-js/blob/main/libraries/botbuilder-dialogs-adaptive-runtime/src/index.ts#L309)
and where packages fit in to the [runtime code](https://github.com/microsoft/botbuilder-js/blob/main/libraries/botbuilder-dialogs-adaptive-runtime/src/index.ts).

## Further reading

- [Bot Framework Documentation](https://docs.botframework.com)
- [Bot Basics](https://docs.microsoft.com/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0)
- [TypeScript](https://www.typescriptlang.org)
- TODO: Package documentation

# $safeprojectname$

Bot Framework v4 custom action sample.

This sample demonstrates how to extend [Bot Framework Composer](https://docs.microsoft.com/en-us/composer/introduction) with a new DoWhile custom action component.

The following classes and schema files are included in the project:

| File | Description |
| ------ | ------ |
| [CustomAction.cs](#DoWhile) | Inherits from ActionScope and manages execution of a block of actions supporting Break and Continue semantics.. |
| [ComponentPlugin.cs](#Plugin) | Loaded by the adaptive runtime DependencyInjection Abstractions and calls ComponentRegistration, adding CustomComponentRegistration. |
| [CustomComponentRegistration.cs](#ComponentRegistration) | Registers the CustomAction component with the adaptive dialog system. |
| [.schema](#schema) | Declarative definition of the DoWhile component. |
| [.uischema](#uischema) | DoWhile component visual interface declarative definition. |

## Prerequisites

- [Bot Framework Composer](https://docs.microsoft.com/en-us/composer/install-composer) version 1.3.1
- [.NET Core SDK](https://dotnet.microsoft.com/download) version 3.1
- [nuget.exe](https://www.nuget.org/downloads) recommended latest
    - A local package feed is required. See ([nuget local feed](https://docs.microsoft.com/en-us/nuget/hosting-packages/local-feeds)) for details.

  ```bash
  # determine dotnet version
  dotnet --version
  ```

## To try this sample

- In a terminal, navigate to `$safeprojectname$`

    ```bash
    # change into project folder
    cd # $safeprojectname$
    ```

- Run the bot from a terminal or from Visual Studio, choose option A or B.

  A) From a terminal

  ```bash
  # run the bot
  dotnet run
  ```

  B) Or from Visual Studio

  - Launch Visual Studio
  - File -> Open -> Project/Solution
  - Navigate to `$safeprojectname$` folder
  - Select `$safeprojectname$.csproj` file
  - `Ctrl+Shift+B` to build the project

  C) The .nupkg will now be in the bin folder.  
  
  - Next, run `nuget add` 

    ```bash
    nuget add $safeprojectname$.1.0.0-preview.nupkg -source c:\nuget
    ```

## Usage

Once installed, the $safeprojectname$.CustomAction will be available in the `Custom Actions` menu.

## Deploy the bot to Azure

To learn more about deploying a bot to Azure, see [Deploy your bot to Azure](https://aka.ms/azuredeployment) for a complete list of deployment instructions.

## Further reading

- [Add custom actions](https://docs.microsoft.com/en-us/composer/how-to-add-custom-action)
- [Bot Framework Documentation](https://docs.botframework.com)
- [.NET Core CLI tools](https://docs.microsoft.com/en-us/dotnet/core/tools/?tabs=netcore2x)
- [Azure CLI](https://docs.microsoft.com/cli/azure/?view=azure-cli-latest)
- [Azure Portal](https://portal.azure.com)

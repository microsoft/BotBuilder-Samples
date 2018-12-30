# How To Version Our .NET Core Templates
This document outlines the steps necessary to bump the version information of a .NET Template.

# About
As of .NET Core 2.2 there isn't a supported way to display template version information of an installed template.
To work around this, the Bot Framework templates manually add the version number as part of the `name` attribute of the template's `.template.config/template.json`.

```bash
  "$schema": "http://json.schemastore.org/template",
  "author": "Microsoft",
  "classifications": [
    "Bot",
    "Bot Framework",
    "AI",
    "Echo Bot"
  ],
  "defaultName": "EchoBot",
  "groupIdentity": "Microsoft.BotFramework.CSharp.EchoBot",
  "identity": "Microsoft.BotFramework.CSharp.EchoBot",
  "name": "Bot Framework Echo Bot (v0.1.1)",                <<<-HAND-CRAFTED-semver
```

This document provides the steps necessary to update the version string in order to support the following scenarios:

- Provide a version string as part of the template's `name`.

```bash
# list installed templates
dotnet new --list
```

- Provide version information used for string replacement activities during new bot project generation.  Template version information is written out in the following places:
    - Project generated `README.md`
    - Project generated `PREREQUISITES.md`
    - Comment headers for all project generated `*.cs` files
- Provide `.nupkg` version information for publishing to MyGet/NuGet

# How to Update Version Information
## Update template.json
The `.template.config/template.json` file has two attributes that need to be updated.  They are:

```json
"name": "Bot Framework Echo Bot (v0.1.1)"     <<<-HAND-CRAFTED-semver
```

```json
"symbols": {
    "currentBuildVersion": {
        "type": "generated",
        "generator": "constant",
        "parameters": {
        "value": "v0.1.1"                     <<<-HAND-CRAFTED-semver
        },
      },
      "replaces": "vX.X.X"
    },
}
```

## Update the .nuspec
The `Microsoft.BotFramework.CSharp.xxxxxBot.nuspec` file has a a `<version></version>` tag that needs to be updated.  This is the version information used to version the .nupkg package.  It looks like the following:

```xml
<?xml version="1.0" encoding="utf-8"?>
<package xmlns="http://schemas.microsoft.com/packaging/2012/06/nuspec.xsd">
  <metadata>
    <id>Microsoft.BotFramework.CSharp.EchoBot</id>
    <version>0.1.1</version>                <<<-HAND-CRAFTED-semver
```

All of these version strings should be the exact same in order to ensure they are consistent when they show up in the various places they are used.


<!-- omit in TOC -->
@microsoft/bf-generate
======================

Generate Bot Framework Adaptive Dialogs from JSON schema.

[![oclif](https://img.shields.io/badge/cli-oclif-brightgreen.svg)](https://oclif.io)
[![Version](https://img.shields.io/npm/v/@microsoft/bf-generate.svg)](https://npmjs.org/package/@microsoft/bf-generate)
[![Downloads/week](https://img.shields.io/npm/dw/@microsoft/bf-generate.svg)](https://npmjs.org/package/@microsoft/bf-generate)
[![License](https://img.shields.io/npm/l/@microsoft/bf-generate.svg)](https://github.com/Microsoft/https://github.com/Microsoft/BotBuilder-Samples/blob/master/package.json)

# Relevant docs

- [Full documentation](https://github.com/microsoft/BotBuilder-Samples/tree/master/experimental/generation/generator)
- [Setup & get started](https://github.com/microsoft/BotBuilder-Samples/tree/master/experimental/generation/generator/docs/get-started.md)

# Commands

<!-- commands -->

- [Relevant docs](#relevant-docs)
- [Commands](#commands)
  - [`bf dialog:generate`](#bf-dialoggenerate)
<!-- - [bf dialog:integrate](#bf-dialogintegrate) -->

## `bf dialog:generate`

```
USAGE

`bf dialog:generate`

OPTIONS

- **--force, -f** Force overwriting generated files. (optional)
- **--help, -h** Generate help. (optional)
- **--locale, -l** Locales to generate. (optional, default: en-us)
- **--merge, -m** Merge the newly generated assets into the output directory. (optional)
- **--output, -o** Output directory. (optional)
- **--prefix, -p** Prefix for generated files. (optional, default: schema name)
- **--schema, -s** Path to your app.schema file. (optional, default: https://raw.githubusercontent.com/microsoft/botbuilder-samples/master/generation/runbot/runbot.schema)
- **--templates, -t** Directories with templates to use for generating assets. First definition wins.  
- **--verbose, -v** Verbose logging of generated files. (optional, default: standard)

EXAMPLES

  bf dialog:generate {JSON SCHEMA FILE} -o {OUTPUT LOCATION}

```

_See [source code](src/commands/generate.ts).

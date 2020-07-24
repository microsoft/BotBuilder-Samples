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
* [`bf dialog:generate SCHEMA`](#bf-dialoggenerate-schema)
* [`bf dialog:generate:swagger PATH`](#bf-dialoggenerateswagger-path)

## `bf dialog:generate SCHEMA`

[PREVIEW] Generate localized .lu, .lg, .qna and .dialog assets to define a bot based on a schema using templates.

```
USAGE
  $ bf dialog:generate SCHEMA

ARGUMENTS
  SCHEMA  JSON Schema .schema file used to drive generation.

OPTIONS
  -f, --force                Force overwriting generated files.
  -h, --help                 show CLI help
  -l, --locale=locale        Locales to generate. [default: en-us]
  -o, --output=output        [default: .] Output path for where to put generated .lu, .lg, .qna and .dialog files.
  -p, --prefix=prefix        Prefix to use for generated files. [default: schema name]
  -s, --schema=schema        Path to your app.schema file.

  -t, --templates=templates  Directory with templates to use for generating assets.  With multiple directories, the
                             first definition found wins.  To include the standard templates, just use "standard" as a
                             template directory name.

  --debug                    Show extra debugging information including templates.

  --merge                    Merge generated results into output directory.

  --verbose                  Output verbose logging of files as they are processed

EXAMPLE

         $ bf dialog:generate sandwich.schema --output c:/tmp
```

_See code: [src\commands\dialog\generate.ts](https://github.com/Microsoft/BotBuilder-Samples/blob/v1.0.0/src\commands\dialog\generate.ts)_

## `bf dialog:generate:swagger PATH`

[PREVIEW] Generate JSON schema given swagger file.

```
USAGE
  $ bf dialog:generate:swagger PATH

ARGUMENTS
  PATH  The path to the swagger file

OPTIONS
  -m, --method=method  (required) [default: GET] API method.
  -n, --name=name      (required) Define schema name.
  -o, --output=output  [default: .] Output path for generated swagger schema files. [default: .]
  -r, --route=route    (required) Route to the specific api.
  --verbose            Output verbose logging of files as they are processed.

EXAMPLE

         $ bf dialog:generate:swagger ./petSwagger.json -o . -r /store/order -m post -p dialog.response -n 
  petSearch.schema
```

_See code: [src\commands\dialog\generate\swagger.ts](https://github.com/Microsoft/BotBuilder-Samples/blob/v1.0.0/src\commands\dialog\generate\swagger.ts)_
<!-- commandsstop -->

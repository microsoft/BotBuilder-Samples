@microsoft/bf-dialog
====================

This package is intended for Microsoft use only and should be consumed through @microsoft/botframework-cli. It is not designed to be consumed as an independent package.

[![oclif](https://img.shields.io/badge/cli-oclif-brightgreen.svg)](https://oclif.io)
[![Version](https://img.shields.io/npm/v/@microsoft/bf-dialog.svg)](https://npmjs.org/package/@microsoft/bf-dialog)
[​![Downloads/week](https://img.shields.io/npm/dw/@microsoft/bf-dialog.svg)](https://npmjs.org/package/@microsoft/bf-dialog)
[![License](https://img.shields.io/npm/l/@microsoft/bf-dialog.svg)](https://github.com/microsoft/botframework-cli/blob/master/package.json)

# Commands
<!-- commands -->
* [`bf `](#bf-)
* [`bf dialog:merge GLOB1 [GLOB2] [GLOB3] [GLOB4] [GLOB5] [GLOB6] [GLOB7] [GLOB8] [GLOB9]`](#bf-dialogmerge-glob1-glob2-glob3-glob4-glob5-glob6-glob7-glob8-glob9)
* [`bf dialog:verify GLOB1 [GLOB2] [GLOB3] [GLOB4] [GLOB5] [GLOB6] [GLOB7] [GLOB8] [GLOB9]`](#bf-dialogverify-glob1-glob2-glob3-glob4-glob5-glob6-glob7-glob8-glob9)

## `bf `

The dialog commands allow you to work with dialog schema.

```
USAGE
  $ bf

OPTIONS
  -h, --help  show CLI help
```

_See code: [src/commands/index.ts](https://github.com/microsoft/botframework-cli/src/commands/index.ts)_

## `bf dialog:merge GLOB1 [GLOB2] [GLOB3] [GLOB4] [GLOB5] [GLOB6] [GLOB7] [GLOB8] [GLOB9]`

```
USAGE
  $ bf dialog:merge GLOB1 [GLOB2] [GLOB3] [GLOB4] [GLOB5] [GLOB6] [GLOB7] [GLOB8] [GLOB9]

OPTIONS
  -b, --branch=branch  [default: master] The branch to use for the meta-schema component.schema.
  -h, --help           show CLI help
  -o, --output=output  [default: app.schema] Output path and filename for merged schema. [default: app.schema]

  -u, --update         Update .schema files to point the <branch> component.schema and regenerate component.schema if
                       baseComponent.schema is present.

  --verbose            output verbose logging of files as they are processed
```

_See code: [src/commands/dialog/merge.ts](https://github.com/microsoft/botframework-cli/src/commands/dialog/merge.ts)_

## `bf dialog:verify GLOB1 [GLOB2] [GLOB3] [GLOB4] [GLOB5] [GLOB6] [GLOB7] [GLOB8] [GLOB9]`

```
USAGE
  $ bf dialog:verify GLOB1 [GLOB2] [GLOB3] [GLOB4] [GLOB5] [GLOB6] [GLOB7] [GLOB8] [GLOB9]

OPTIONS
  -h, --help  show CLI help
  --verbose   Show verbose output
```

_See code: [src/commands/dialog/verify.ts](https://github.com/microsoft/botframework-cli/src/commands/dialog/verify.ts)_
<!-- commandsstop -->
* [`bf dialog:merge [FILE]`](#bf-dialogmerge-file)
* [`bf dialog:verify [FILE]`](#bf-dialogverify-file)

## `bf dialog:merge [FILE]`

The bf dialog:merge creates an merged schema file which represents merging of all of the component
schemas and the SDK schemas together into an application .schema file.

The file pattern can be an arbitrary GLOB patterns for finding .schema files (such as myfolder/**/*.schema), but
the better way to use it is to invoke it in the folder that has a project in it (either .csproj or packages.json).
In that case the project file will be analyzed for all dependent folders and .schema files will be merged to create
the app.schema for the project.

```
USAGE
  $ bf dialog:merge GLOB1 [GLOB2] [GLOB3] [GLOB4] [GLOB5] [GLOB6] [GLOB7] [GLOB8] [GLOB9]

OPTIONS
  -b, --branch=branch  The branch to use for the meta-schema component.schema. [default: master] 
  -h, --help              show CLI help
  -o, --output=output  Output path and filename for merged schema. [default: app.schema]
  -u, --update         Update .schema files to point the <branch> component.schema and regenerate component.schema if baseComponent.schema is present.
  --verbose            output verbose logging of files as they are processed
```

Example:
```
bf dialog:merge -o app.schema
```

_See code: [src/commands/dialog/merge.ts](https://github.com/microsoft/botframework-cli/blob/v0.0.0/src/commands/dialog/merge.ts)_

## `bf dialog:verify [FILE]`

The dialog:verify command is used to validate that all of the .dialog file resources for a project are valid based on the 
applications app.schema file (see dialog:merge command).

```
USAGE
  $ bf dialog:verify GLOB1 [GLOB2] [GLOB3] [GLOB4] [GLOB5] [GLOB6] [GLOB7] [GLOB8] [GLOB9]

OPTIONS
  -h, --help  show CLI help
  --verbose   Show verbose output
```

Example:
``` 
bf dialog:verify test/**/*.dialog
```

_See code: [src/commands/dialog/verify.ts](https://github.com/microsoft/botframework-cli/blob/v0.0.0/src/commands/dialog/verify.ts)_

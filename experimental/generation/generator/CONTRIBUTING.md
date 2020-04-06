# 👩‍💻 Contribution guide

## 🛠 Required tools

We use [rush](https://rushjs.io/) with [pnpm](https://pnpm.js.org) to manage our monorepo. All your commands to build and test will use `rush`/`rushx`.

 - git
 - node 10 LTS or 12 LTS 
   - (other versions not officially supported) (recommend [nvs](https://github.com/jasongin/nvs/releases) to help manage node versions)
 - rush@5.13.x+
   - (rush newer rush versions will manage this for you)
 - (optional) pnpm@5+
   -  useful to make vs code happy when trying to resolve your typescript version
 - MacOSX, Windows, "modern" Linux
   - We develop on Mac and Windows machines, and test on Ubuntu 14+.

## ⚡ Quickstart to contribute

0. Fork the repo at https://github.com/microsoft/botframework-cli
0. Clone your fork - `git clone https://github.com/<YOU>/botframework-cli.git`
0. `npm i -g @microsoft/rush`
   - Install rush
0. `rush update` 
   - installs and links packages - use `rush install` and `rush link` for readonly
0. `rush build` 
   - builds all the projects
0. `rush test` 
   - runs tests in all the projects

## 💅 Tips

- DO NOT USE `npm install` (and in general avoid `npm` commands). If you need to add a package, look at `rush add` or modify the package.json directly.
- We don't hoist common dev dependencies from a single list. It must be declared in each package, but pnpm will only install it once.
- You can use `rush --help` to get more details about what commands you can run.
- You can also use `rushx` to run specific commands in a given package (but rush install/link usually needs to have run for it to work).

## 📃 General Guidelines

### Common Options Rules

* [--in|-i] [file|folder]  : Input filename or folder name
* [--out|-o] [file|folder] : output filename or folder
* [--recurse|-r] : Recursive action (e.g. into folder tree)
* [--version|-v] : displays version information
* [--help|-h|-?] : displays help, usage information
* [--log|-l]  [quite|normal|verbose]: Control STDOUT log level. 
  * Default: normal. Plugin owner must respect quite mode; but verbose implementation is optional
* \[--force|-f\]: Force action without prompting e.g. Overwrite (particularly in I/O operations)
* Short form:
  * Mandatory for common options: -i, -o, -r, -v, -h 
  * Suggested for frequent operations 
  * Optional otherwise

### File System

* STDIN/STDOUT - if expected I/O stream option is not specified assume piping from/to STDIN/STDOUT respectively.
* Files are read/written from/to process CWD if not explicitly specified or if path is relative
* File/folder specifications must accept wild cards if supported by corresponding functionality

- Always, if possible, detect input format from content 
- Detect --in / --out file or folder based on specified value. If need to disambiguate introduce param (only if no way to infer).
- Use the input/output stream tables below to guide command line I/O processing

#### Input Stream

| Specified | Expected | Exist  | File Action | Folder Action |
| --------- | -------- | ------ | ----------- | ------------- |
| Yes       | Yes      | Yes    | OK          | OK            |
| Yes       | Yes      | No     | Fail        | Fail          |
| No        | Yes      | Yes/No | STDIN       | Fail          |
| Yes       | No       | Yes/No | Fail        | Fail          |
| No        | No       | Yes/No | OK          | OK            |



#### Output Stream

| Specified | Expected | Exist  | File Action                                            | Folder Action                  |
| --------- | -------- | ------ | ------------------------------------------------------ | ------------------------------ |
| Yes       | Yes      | Yes    | if [--force] --> OK (overwrite); otherwise prompt user | Same logic for files in folder |
| Yes       | Yes      | No     | OK (create)                                            | Fail                           |
| No        | Yes      | Yes/No | STDOUT                                                 | Fail                           |
| Yes       | No       | Yes/No | Fail                                                   | Fail                           |
| No        | No       | Yes/No | OK                                                     | OK                             |






### Standard Command Types

Use the following verbs for standard commands

- CREATE - standard command to create a new resource. Usually backed server-side by a PUT request.
- UPDATE - command to selectively update properties of a resource and preserve existing values. May be backed server-side by either a PUT or PATCH request.
- SET - command to replace all properties of a resource without preserving existing values, typically backed server-side by a PUT request.
- SHOW - command to show the properties of a resource, backed server-side by a GET request.
- LIST - command to list instances of a resource, backed server-side by a GET request.
- DELETE - command to delete a resource, backed server-side by a DELETE request.
- WAIT - command that polls a GET endpoint until a condition is reached. 

### Other

* Commands and options are case SenSiTive, specified in lower case. *bf cmd:subcmd*.
* Multi word options are lowcase, nohyphen, multiwords. Multi word commands are forbidden.
* Prefer flags to args
* Commands shall follow the *bf \[noun\]\[noun\]\[verb\]* form for example *bf qnamaker:create:kb*. 
* FLags with specific units: 
  * In general, DO NOT put units in Flag names. ALWAYS put the expected units in the help text. Example: --durationinminutes should simply be --duration. This prevents the need to add more arguments later if more units are supported. 
  * Consider allowing a syntax that will let the user specify units. For example, even if the service requires a value in minutes, consider accepting 1h or 60m. It is fine to assume a default (i.e. 60 = 60 minutes).
  * It is acceptable to use a unit in the flag name when it is used like an enum. For example, --startday is okay when it accepts MON, TUE, etc. --starthour is okay when it indicates an hour of the day.
* Avoid having multiple arguments that simply represent different ways of getting the same thing. Instead, use a single descriptive name and overload it appropriately. For example, assume a command which can accept a parameter file through a URL or local path.

### Porting Rules

* Always cleanup, remove redundant, or legacy items.
* Always prefer global form and provided functionality.
* Always use global configuration - Do not port configuration files, move settings to global config.
* Correctness is higher priority then velocity (e.g. remove QnAMaker --legacy, chatdown --stdin, abstract out libraries, clean up messages - this is *not* 1-1 porting, it's more of 1 --> 1+cleanup+improve porting)

### Usage and Readme Rules

* Always decorate option as:

  * Optional: Option is not required
  * Mandatory: Option is required
  * Default=\<val\> : If option has a default value, it must be displayed

* Command and option descriptions shall be short, precise and leverage industry common notions

* All ReadMe's shall follow the same structure as the primary cli package (also use chatdown or ludown as examples)

* Ensure all usage contains the standard format:

  SYNOPSIS: what does it do
  VERSION:  git version
  USAGE:  command form

  OPTIONS: Available options at this command group level

  COMMANDS: Available sub-commands

  EXAMPLES: Representative usage examples

* **Important**: Pay special attention to descriptions, command & option names. That's your user primary discovery & understanding way to know what it does. 



## Software Development Lifecycle Requirement

CLI contribution Software Development Lifecycle is as follows:

1. Spec it out. 
   1. If new, provide a full spec per templates provided in this folder.
   2. If porting, specify changes from original implementation
   3. Ensure to conform to the above command line specification rules
2. Get spec reviewed & approved. Ensure sign off by gate-keeper (as will be triaged weekly).
3. Implement. All code must be test-covered at > 90% coverage integrated into CI. 
4. Schedule a team show & tell demo for introduction, feedback and fine tuning

## ❓ FAQ

### Why rush/pnpm?

Rush and pnpm are newer tools and not all that popular outside of Microsoft, why use them? 

- Speed. We get ordered and parallel builds for free.
- Standarization. Many of the Microsoft JS repos are moving to `rush`.
- Quality of life. The command line help, enforcement & customization tools, etc. that come from `rush` and `pnpm` are super useful. Also, we like avoiding the "parent" project for hoisting dependencies. That frequently bites us with our old system.

### Steps to create a new plugin

    1. Build the project
    2. Inside the packages folder(https://github.com/microsoft/botframework-cli/tree/master/packages) run 'npx oclif plugin <plugin-name>'
    3. Follow the wizard and set the prompts:
    
      ? npm package: @microsoft/bf-<plugin-name>
      ? description: <Plugin brief description>
      ? author: Microsoft
      ? version: 1.0.0
      ? license: MIT
      ? Who is the GitHub owner of repository (https://github.com/OWNER/repo): microsoft
      ? What is the GitHub name of repository (https://github.com/owner/REPO): botframework-cli
      ? Select a package manager: npm
      ? TypeScript: Y
      ? Use tslint (linter for TypeScript): Y
      ? Use mocha (testing framework): Y
      ? Add CI service config (Press <space> to select, <a> to toggle all, <i> to invert selection): select any
    
    4. Go to the folder created by the previous command and add @microsoft/bf-cli-command as a dependency in your package.json file
    
      "dependencies": {
        ...,
        "@microsoft/bf-cli-command": "1.0.0",
        ...
      }   
    
    5. Update and build again

### Steps to create a new command
    1. Inside the plugin folder run 'npx oclif command <command-name>'. 
    	a. To add a subcommand use a colon separated format as follows: 
    		<command-name:subcommand-name>
    2. Replace the import 'import {Command, flags} from '@oclif/command' line inside the newly created command with '@microsoft/bf-cli-command'
    
      - import {Command, flags} from '@oclif/command'
      + import {Command, flags} from '@microsoft/bf-cli-command'
    
    3. Add the typing to the flags property like this if needed:
    
      static flags: flags.Input<any> = {
        help: flags.help({description: 'Display help'}),
      }
    
    4. Implement the run method
    5. Run "rush update" and "rush build" from root or package root
    6. To run the new command locally, run "bin\run <command-name>" from the package root, ie \botframework-cli\packages\<package-name>\bin\run <command-name>

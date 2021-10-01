// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

'use strict';
const pkg = require('../../package.json');
const Generator = require('yeoman-generator');
const path = require('path');
const chalk = require('chalk');
const mkdirp = require('mkdirp');
const _ = require('lodash');

const BOT_TEMPLATE_NAME_EMPTY = 'Empty Bot';
const BOT_TEMPLATE_NAME_SIMPLE = 'Echo Bot';
const BOT_TEMPLATE_NAME_CORE = 'Core Bot';

const BOT_TEMPLATE_NOPROMPT_EMPTY = 'empty';
const BOT_TEMPLATE_NOPROMPT_SIMPLE = 'echo';
const BOT_TEMPLATE_NOPROMPT_CORE = 'core';

const bigBot =
    `               ╭─────────────────────────────╮\n` +
    `   ` +
    chalk.blue.bold(`//`) +
    `     ` +
    chalk.blue.bold(`\\\\`) +
    `   │        Welcome to the       │\n` +
    `  ` +
    chalk.blue.bold(`//`) +
    ` () () ` +
    chalk.blue.bold(`\\\\`) +
    `  │  Microsoft Java Bot Builder │\n` +
    `  ` +
    chalk.blue.bold(`\\\\`) +
    `       ` +
    chalk.blue.bold(`//`) +
    ` /│         generator!          │\n` +
    `   ` +
    chalk.blue.bold(`\\\\`) +
    `     ` +
    chalk.blue.bold(`//`) +
    `   ╰─────────────────────────────╯\n` +
    `                                    v${pkg.version}`;

const tinyBot =
    ` ` + chalk.blue.bold(`<`) + ` ** ` + chalk.blue.bold(`>`) + ` `;

module.exports = class extends Generator {
    constructor(args, opts) {
        super(args, opts);

        // allocate an object that we can use to store our user prompt values from our askFor* functions
        this.templateConfig = {};

        // configure the commandline options
        this._configureCommandlineOptions();
    }

    initializing() {
        // give the user some data before we start asking them questions
        this.log(bigBot);
    }

    prompting() {
        // if we're told to not prompt, then pick what we need and return
        if (this.options.noprompt) {
            // this function will throw if it encounters errors/invalid options
            return this._verifyNoPromptOptions();
        }

        const userPrompts = this._getPrompts();
        async function executePrompts([prompt, ...rest]) {
            if (prompt) {
                await prompt();
                return executePrompts(rest);
            }
        }

        return executePrompts(userPrompts);
    }

    writing() {
        // if the user confirmed their settings, then lets go ahead
        // an install module dependencies
        if (this.templateConfig.finalConfirmation === true) {
            const botName = this.templateConfig.botName;
            const packageName = this.templateConfig.packageName.replace(/-/g, '_').toLowerCase();
            const packageTree = packageName.replace(/\./g, '/');
            const artifact = _.kebabCase(this.templateConfig.botName).replace(/([^a-z0-9-]+)/gi, ``);
            const directoryName = _.camelCase(this.templateConfig.botName);
            const template = this.templateConfig.template.toLowerCase();

            if (path.basename(this.destinationPath()) !== directoryName) {
                mkdirp.sync(directoryName);
                this.destinationRoot(this.destinationPath(directoryName));
            }

            // Copy the project tree
            this.fs.copyTpl(
                this.templatePath(path.join(template, 'project', '**')),
                this.destinationPath(),
                {
                    botName,
                    packageName,
                    artifact
                }
            );

            // Copy main source
            this.fs.copyTpl(
                this.templatePath(path.join(template, 'src/main/java/**')),
                this.destinationPath(path.join('src/main/java', packageTree)),
                {
                    packageName
                }
            );

            // Copy test source
            this.fs.copyTpl(
                this.templatePath(path.join(template, 'src/test/java/**')),
                this.destinationPath(path.join('src/test/java', packageTree)),
                {
                    packageName
                }
            );
        }
    }

    end() {
        if (this.templateConfig.finalConfirmation === true) {
            this.log(chalk.green('------------------------ '));
            this.log(chalk.green(' Your new bot is ready!  '));
            this.log(chalk.green('------------------------ '));
            this.log(`Your bot should be in a directory named "${_.camelCase(this.templateConfig.botName)}"`);
            this.log('Open the ' + chalk.green.bold('README.md') + ' to learn how to run your bot. ');
            this.log('Thank you for using the Microsoft Bot Framework. ');
            this.log(`\n${tinyBot} The Bot Framework Team`);
        } else {
            this.log(chalk.red.bold('-------------------------------- '));
            this.log(chalk.red.bold(' New bot creation was canceled. '));
            this.log(chalk.red.bold('-------------------------------- '));
            this.log('Thank you for using the Microsoft Bot Framework. ');
            this.log(`\n${tinyBot} The Bot Framework Team`);
        }
    }

    _configureCommandlineOptions() {
        this.option('botName', {
            desc: 'The name you want to give to your bot',
            type: String,
            default: 'echo',
            alias: 'N'
        });

        this.option('packageName', {
            desc: `What's the fully qualified package name of your bot?`,
            type: String,
            default: 'com.mycompany.echo',
            alias: 'P'
        });

        const templateDesc = `The initial bot capabilities. (${BOT_TEMPLATE_NAME_EMPTY} | ${BOT_TEMPLATE_NAME_SIMPLE} | ${BOT_TEMPLATE_NAME_CORE})`;
        this.option('template', {
            desc: templateDesc,
            type: String,
            default: BOT_TEMPLATE_NAME_SIMPLE,
            alias: 'T'
        });

        this.argument('noprompt', {
            desc: 'Do not prompt for any information or confirmation',
            type: Boolean,
            required: false,
            default: false
        });
    }

    _getPrompts() {
        return [
            // ask the user to name their bot
            async () => {
                return this.prompt({
                    type: 'input',
                    name: 'botName',
                    message: `What's the name of your bot?`,
                    default: (this.options.botName ? this.options.botName : 'echo')
                }).then(answer => {
                    // store the botname description answer
                    this.templateConfig.botName = answer.botName;
                });
            },

            // ask for package name
            async () => {
                return this.prompt({
                    type: 'input',
                    name: 'packageName',
                    message: `What's the fully qualified package name of your bot?`,
                    default: (this.options.packageName ? this.options.packageName : 'com.mycompany.echo')
                }).then(answer => {
                    // store the package name description answer
                    this.templateConfig.packageName = answer.packageName;
                });
            },


            // ask the user which bot template we should use
            async () => {
                return this.prompt({
                    type: 'list',
                    name: 'template',
                    message: 'Which template would you like to start with?',
                    choices: [
                        {
                            name: BOT_TEMPLATE_NAME_SIMPLE,
                            value: BOT_TEMPLATE_NOPROMPT_SIMPLE
                        },
                        {
                            name: BOT_TEMPLATE_NAME_EMPTY,
                            value: BOT_TEMPLATE_NOPROMPT_EMPTY
                        },
                        {
                            name: BOT_TEMPLATE_NAME_CORE,
                            value: BOT_TEMPLATE_NOPROMPT_CORE
                        }
                    ],
                    default: (this.options.template ? _.toLower(this.options.template) : BOT_TEMPLATE_NOPROMPT_SIMPLE)
                }).then(answer => {
                    // store the template prompt answer
                    this.templateConfig.template = answer.template;
                });
            },

            // ask the user for final confirmation before we generate their bot
            async () => {
                return this.prompt({
                    type: 'confirm',
                    name: 'finalConfirmation',
                    message: 'Looking good.  Shall I go ahead and create your new bot?',
                    default: true
                }).then(answer => {
                    // store the finalConfirmation prompt answer
                    this.templateConfig.finalConfirmation = answer.finalConfirmation;
                });
            }
        ];
    }

    // if we're run with the --noprompt option, verify that all required options were supplied.
    // throw for missing options, or a resolved Promise
    _verifyNoPromptOptions() {
        this.templateConfig = _.pick(this.options, ['botName', 'packageName', 'template'])

        // validate we have what we need, or we'll need to throw
        if (!this.templateConfig.botName) {
            throw new Error('Must specify a name for your bot when using --noprompt argument.  Use --botName or -N');
        }
        if (!this.templateConfig.packageName) {
            throw new Error('Must specify a package name for your bot when using --noprompt argument.  Use --packageName or -P');
        }

        // make sure we have a supported template
        const template = (this.templateConfig.template ? _.toLower(this.templateConfig.template) : undefined);
        const tmplEmpty = _.toLower(BOT_TEMPLATE_NOPROMPT_EMPTY);
        const tmplSimple = _.toLower(BOT_TEMPLATE_NOPROMPT_SIMPLE);
        const tmplCore = _.toLower(BOT_TEMPLATE_NOPROMPT_CORE);
        if (!template || (template !== tmplEmpty && template !== tmplSimple && template !== tmplCore)) {
            throw new Error('Must specify a template when using --noprompt argument.  Use --template or -T');
        }

        // when run using --noprompt and we have all the required templateConfig, then set final confirmation to true
        // so we can go forward and create the new bot without prompting the user for confirmation
        this.templateConfig.finalConfirmation = true;

        return Promise.resolve();
    }
};

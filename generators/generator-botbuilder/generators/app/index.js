// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const Generator = require('yeoman-generator');
const _ = require('lodash');

const chalk = require('chalk');

const pkg = require('../../package.json');
const prompts = require('../../components/prompts');
const { coreTemplateWriter } = require('../../components/coreTemplateWriter');
const { echoTemplateWriter } = require('../../components/echoTemplateWriter');
const { emptyTemplateWriter } = require('../../components/emptyTemplateWriter');
const {
    BOT_LANG_NAME_CSHARP,
    BOT_LANG_NAME_JAVASCRIPT,
    BOT_LANG_NAME_TYPESCRIPT,
    BOT_TEMPLATE_NAME_EMPTY,
    BOT_TEMPLATE_NAME_SIMPLE,
    BOT_TEMPLATE_NAME_CORE,
    BOT_TEMPLATE_NOPROMPT_EMPTY,
    BOT_TEMPLATE_NOPROMPT_SIMPLE,
    BOT_TEMPLATE_NOPROMPT_CORE
    } = require('../../components/constants');


/**
 * Main Generator derivative.  This is what Yeoman calls to invoke our generator
 */
module.exports = class extends Generator {
    constructor(args, opts) {
        super(args, opts);

        // allocate an object that we can use to store our user prompt values from our askFor* functions
        this.templateConfig = Object.create(null);

        // configure the commandline options
        prompts.configureCommandlineOptions(this);
    }

    initializing() {
        // give the user some data before we start asking them questions
        this.log(`\nWelcome to the Microsoft Bot Builder generator v${pkg.version}. `);
        this.log('\nDetailed documentation can be found at ' + chalk.underline('https://aka.ms/botbuilder-generator\n'));
    }

    prompting() {
        const userPrompts = prompts.getPrompts(this);
        let result = Promise.resolve();

        // if we're told to not prompt, then pick what we need and return
        if(this.options.noprompt) {
            // this function will throw if it encounters errors/invalid options
            _verifyNoPromptOptions();
        }

        // run all prompts in sequence.  Results can be ignored.
        for(let taskName in userPrompts) {
            let prompt = userPrompts[taskName];
            result = result.then(_ => {
                return new Promise((s, r) => {
                    setTimeout(_ => prompt().then(s, r), 0);    // set timeout is required, otherwise node hangs
                });
            })
        }
        return result;
    }

    writing() {
        // if the user confirmed their settings, then lets go ahead
        // an install module dependencies
        if(this.templateConfig.finalConfirmation === true) {
            // figure out which language we're going to use
            const language = _.toLower(this.templateConfig.language);
            switch(language) {
            case _.toLower(BOT_LANG_NAME_JAVASCRIPT):
            case _.toLower(BOT_LANG_NAME_TYPESCRIPT):
                this._writeUsingScripting();
            break;

            case _.toLower(BOT_LANG_NAME_CSHARP):
                this._writeUsingDotNet();
            break;

            default:
                const errorMsg = `ERROR:  Unable to generate a new bot.  Invalid programming language: [${language}]`;
                this.log(chalk.red(errorMsg));
                throw new Error(errorMsg);
            break;
            }
        }
    }

    install() {
        // if the user confirmed their settings, then lets go ahead
        // an install module dependencies
        if(this.templateConfig.finalConfirmation === true) {
            this.installDependencies({ bower: false });
        }
    }

    end() {
        if(this.templateConfig.finalConfirmation === true) {
            this.log(chalk.green('------------------------ '));
            this.log(chalk.green(' Your new bot is ready!  '));
            this.log(chalk.green('------------------------ '));
            this.log('Open the ' + chalk.green.bold('README.md') + ' to learn how to run your bot. ');
            this.log('Thank you for using the Microsoft Bot Framework. ');
            this.log('\n< ** > The Bot Framework Team');
        } else {
            this.log(chalk.red.bold('-------------------------------- '));
            this.log(chalk.red.bold(' New bot creation was canceled. '));
            this.log(chalk.red.bold('-------------------------------- '));
            this.log('Thank you for using the Microsoft Bot Framework. ');
            this.log('\n< ** > The Bot Framework Team');
        }

    }

    // These routines are internal to the implementation and are not
    // part of the Yeoman generator required functions
    _writeUsingScripting() {
        // figure out which scripting language template to write
        // the scripting template writers handle whether
        // they should write a JavaScript or TypeScript template
        const template = _.toLower(this.templateConfig.template);
        switch(template) {
        case _.toLower(BOT_TEMPLATE_NAME_EMPTY):
        case _.toLower(BOT_TEMPLATE_NOPROMPT_EMPTY):
            emptyTemplateWriter(this);
        break;

        case _.toLower(BOT_TEMPLATE_NAME_SIMPLE):
        case _.toLower(BOT_TEMPLATE_NOPROMPT_SIMPLE):
            echoTemplateWriter(this);
        break;

        case _.toLower(BOT_TEMPLATE_NAME_CORE):
        case _.toLower(BOT_TEMPLATE_NOPROMPT_CORE):
            coreTemplateWriter(this);
        break;

        default:
            const errorMsg = `ERROR:  Unable to generate a new bot.  Invalid template: [${template}]`;
            this.log(chalk.red(errorMsg));
            throw new Error(errorMsg);
        break;
        }
    }

    _writeUsingDotNet() {
        // figure out which dot net language template to write
        const template = _.toLower(this.templateConfig.template);
        switch(template) {
            case _.toLower(BOT_TEMPLATE_NAME_EMPTY):
                emptyTemplateWriterForDotNet(this);
            break;

            case _.toLower(BOT_TEMPLATE_NAME_SIMPLE):
                echoTemplateWriterForDotNet(this);
            break;

            case _.toLower(BOT_TEMPLATE_NAME_CORE):
                coreTemplateWriterForDotNet(this);
            break;

            default:
                const errorMsg = `ERROR:  Unable to generate a new bot.  Invalid template: [${template}]`;
                this.log(chalk.red(errorMsg));
                throw new Error(errorMsg);
            break;
        }
    }

    // if we're run with the --noprompt option, verify that
    // we were all passed in all required options.
    // return true for success, false for failure
    _verifyNoPromptOptions() {
        this.templateConfig = _.pick(this.options, ['botname', 'description', 'language', 'template'])

        // validate we have what we need, or we'll need to throw
        if(!this.templateConfig.botname) {
          throw new Error('Must specify a name for your bot when using --noprompt argument.  Use --botname or -N');
        }
        if(!this.templateConfig.description) {
            throw new Error('Must specify a description for your bot when using --noprompt argument.  Use --description or -D');
        }

        // make sure we have a supported language
        const language = (this.templateConfig.language ? _.toLower(this.templateConfig.language) : undefined);
        const langJS = _.toLower(BOT_LANG_NAME_JAVASCRIPT);
        const langTS = _.toLower(BOT_LANG_NAME_TYPESCRIPT);
        if(!language || (language !== langJS && language !== langTS)) {
            throw new Error('Must specify a programming language when using --noprompt argument.  Use --language or -L');
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
    }
};

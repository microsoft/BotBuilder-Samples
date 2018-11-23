// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const Generator = require('yeoman-generator');
const _ = require('lodash');

const pkg = require('../../package.json');
const prompts = require('../../components/prompts');
const { basicTemplateWriter } = require('../../components/basicTemplateWriter');
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

        // configure the commandline options
        prompts.configureCommandlineOptions(this);
    }

    prompting() {
        // give the user some data before we start asking them questions
        const greetingMsg = `\nWelcome to the Microsoft Bot Builder generator v${pkg.version}. ` +
                            '\nDetailed documentation can be found at https://aka.ms/botbuilder-generator\n';
        this.log(greetingMsg);

        // if we're told to not prompt, then pick what we need and return
        if(this.options.noprompt) {
            this.props = _.pick(this.options, ['botname', 'description', 'language', 'template'])

            // validate we have what we need, or we'll need to throw
            if(!this.props.botname) {
              throw new Error('Must specify a name for your bot when using --noprompt argument.  Use --botname or -N');
            }
            if(!this.props.description) {
                throw new Error('Must specify a description for your bot when using --noprompt argument.  Use --description or -D');
            }

            // make sure we have a supported language
            const language = (this.props.language ? _.toLower(this.props.language) : undefined);
            const langJS = _.toLower(BOT_LANG_NAME_JAVASCRIPT);
            const langTS = _.toLower(BOT_LANG_NAME_TYPESCRIPT);
            const langCS = _.toLower(BOT_LANG_NAME_CSHARP);
            if(!language || (language !== langJS && language !== langTS && language !== langCS)) {
                throw new Error('Must specify a programming language when using --noprompt argument.  Use --language or -L');
            }

            // make sure we have a supported template
            const template = (this.props.template ? _.toLower(this.props.template) : undefined);
            const tmplEmpty = _.toLower(BOT_TEMPLATE_NOPROMPT_EMPTY);
            const tmplSimple = _.toLower(BOT_TEMPLATE_NOPROMPT_SIMPLE);
            const tmplCore = _.toLower(BOT_TEMPLATE_NOPROMPT_CORE);
            if (!template || (template !== tmplEmpty && template !== tmplSimple && template !== tmplCore)) {
                throw new Error('Must specify a template when using --noprompt argument.  Use --template or -T');
            }
            // when run using --noprompt and we have all the required props, then set final confirmation to true
            // so we can go forward and create the new bot without prompting the user for confirmation
            this.props.finalConfirmation = true;
            return;
        }

        // let's ask the user for data before we generate the bot
        const promptAnswers = prompts.getPrompts(this.options);

        return this.prompt(promptAnswers).then((props) => {
            this.props = props;
        });
    }

    writing() {
        // if the user confirmed their settings, then lets go ahead
        // an install module dependencies
        if(this.props.finalConfirmation === true) {
            // figure out which language we're going to use
            const language = _.toLower(this.props.language);
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
                this.log(errorMsg);
                throw new Error(errorMsg);
            break;
            }
        }
    }

    install() {
        // if the user confirmed their settings, then lets go ahead
        // an install module dependencies
        if (this.props.finalConfirmation === true) {
            this.installDependencies({ bower: false });
        }
    }

    end() {
        if (this.props.finalConfirmation === true) {
            const thankYouMsg = '------------------- \n' +
                'Your new bot is ready!  \n' +
                'Open the README.md to learn how to run your bot. \n' +
                'Thank you for using the Microsoft Bot Framework. \n' +
                '\n< ** > The Bot Framework Team';
            this.log(thankYouMsg);
        } else {
            const noThankYouMsg = '------------------- \n' +
                'Bot creation has been canceled.  \n' +
                'Thank you for using the Microsoft Bot Framework. \n' +
                '\n< ** > The Bot Framework Team';
            this.log(noThankYouMsg);
        }

    }

    // These routines are internal to the implementation and are not
    // part of the Yeoman generator required functions
    _writeUsingScripting() {
        // figure out which scripting language template to write
        // the scripting template writers handle whether
        // they should write a JavaScript or TypeScript template
        const template = _.toLower(this.props.template);
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
            basicTemplateWriter(this);
        break;

        default:
            const errorMsg = `ERROR:  Unable to generate a new bot.  Invalid template: [${template}]`;
            this.log(errorMsg);
            throw new Error(errorMsg);
        break;
        }
    }

    _writeUsingDotNet() {
        // figure out which dot net language template to write
        const template = _.toLower(this.props.template);
        switch(template) {
            case _.toLower(BOT_TEMPLATE_NAME_EMPTY):
                emptyTemplateWriterForDotNet(this);
            break;

            case _.toLower(BOT_TEMPLATE_NAME_SIMPLE):
                echoTemplateWriterForDotNet(this);
            break;

            case _.toLower(BOT_TEMPLATE_NAME_CORE):
                basicTemplateWriterForDotNet(this);
            break;

            default:
                const errorMsg = `ERROR:  Unable to generate a new bot.  Invalid template: [${template}]`;
                this.log(errorMsg);
                throw new Error(errorMsg);
            break;
        }
    }

};

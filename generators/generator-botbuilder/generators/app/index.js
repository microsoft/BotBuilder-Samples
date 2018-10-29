// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const Generator = require('yeoman-generator');
const _ = require("lodash");

const pkg = require('../../package.json');
const prompts = require('../../components/prompts');
const { basicTemplateWriter } = require('../../components/basicTemplateWriter');
const { echoTemplateWriter } = require('../../components/echoTemplateWriter');

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
        // if we're told to not prompt, then pick what we need and return
        if(this.options.noprompt) {
            this.props = _.pick(this.options, ["botName", "description", "language", "template"])

            // validate we have what we need, or we'll need to throw
            if(!this.props.botName) {
              throw new Error("Must specify a name for your bot when using --noprompt argument.  Use --botName or -N");
            }
            if(!this.props.description) {
                throw new Error("Must specify a description for your bot when using --noprompt argument.  Use --description or -D");
            }
            if(!this.props.language || (_.toLower(this.props.language) !== "javascript" && _.toLower(this.props.language) !== "typescript")) {
                throw new Error("Must specify a programming language when using --noprompt argument.  Use --language or -L");
            }
            if (!this.props.template || (_.toLower(this.props.template) !== "echo" && _.toLower(this.props.template) !== "basic")) {
              throw new Error("Must specify a template when using --noprompt argument.  Use --template or -T");
            }
            return;
        }

        // give the user some data before we start asking them questions
        const greetingMsg = `Welcome to the botbuilder generator v${pkg.version}.  \nMore detailed documentation can be found at https://aka.ms/botbuilder-generator`;
        this.log(greetingMsg);

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
            // the user confirmed that we should continue.
            if (_.toLower(this.props.template) === "echo") {
                echoTemplateWriter(this);
            } else {
                basicTemplateWriter(this);
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
            const thankYouMsg = "------------------- \n" +
                "Your new bot is ready!  \n" +
                "Open the README.md to learn how to run your bot. \n" +
                "Thank you for using the Microsoft Bot Framework. \n" +
                "\n< ** > The Bot Framework Team";
            this.log(thankYouMsg);
        } else {
            const noThankYouMsg = "------------------- \n" +
                "Bot creation has been canceled.  \n" +
                "Thank you for using the Microsoft Bot Framework. \n" +
                "\n< ** > The Bot Framework Team";
            this.log(noThankYouMsg);
        }

    }

};

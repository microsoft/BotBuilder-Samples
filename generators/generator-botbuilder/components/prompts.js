// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const _ = require('lodash');

const {
  BOT_TEMPLATE_NAME_EMPTY,
  BOT_TEMPLATE_NAME_SIMPLE,
  BOT_TEMPLATE_NAME_CORE,
  BOT_TEMPLATE_NOPROMPT_EMPTY,
  BOT_TEMPLATE_NOPROMPT_SIMPLE,
  BOT_TEMPLATE_NOPROMPT_CORE,
  BOT_HELP_URL_EMPTY,
  BOT_HELP_URL_SIMPLE,
  BOT_HELP_URL_CORE,
  BOT_LANG_NAME_JAVASCRIPT,
  BOT_LANG_NAME_TYPESCRIPT
  } = require('./constants');

/**
 * configureCommandlineOptions
 * does the work to configure the commandline options that this template will accept
 * this is mostly made available so that we can run the template without user
 * intervention.  e.g. automated test runs
 * @param {Generator} gen Yeoman's generator object
 */
module.exports.configureCommandlineOptions = gen => {
  gen.option('botname', {
    desc: 'The name you want to give to your bot',
    type: String,
    default: 'my-chat-bot',
    alias: 'N'
  });
  gen.option('description', {
    desc: 'A brief bit of text used to describe what your bot does',
    type: String,
    default: 'Demonstrate the core capabilities of the Microsoft Bot Framework',
    alias: 'D'
  });
  const langDesc = `The programming language use by the project. (${BOT_LANG_NAME_JAVASCRIPT} | ${BOT_LANG_NAME_TYPESCRIPT})`;
  gen.option('language', {
    desc: langDesc,
    type: String,
    default: BOT_LANG_NAME_JAVASCRIPT,
    alias: 'L'
  });

  const templateDesc = `The initial bot capabilities. (${BOT_TEMPLATE_NAME_EMPTY} | ${BOT_TEMPLATE_NAME_SIMPLE} | ${BOT_TEMPLATE_NAME_CORE})`;
  gen.option('template', {
    desc: templateDesc,
    type: String,
    default: BOT_TEMPLATE_NOPROMPT_SIMPLE,
    alias: 'T'
  });

  gen.argument('addtests', {
    desc: `Generate unit tests (${BOT_TEMPLATE_NAME_CORE} only).`,
    type: Boolean,
    required: false,
    default: false
  });

  gen.argument('noprompt', {
    desc: 'Do not prompt for any information or confirmation',
    type: Boolean,
    required: false,
    default: false
  });
};

/**
 * getPrompts
 * constructs an array of prompts name/value pairs. This is the input we need from the user
 * or passed into the command line to successfully configure a new bot
 * @param {Object} options
 */
module.exports.getPrompts = (generator) => {
  const noprompt = generator.options.noprompt;
  const prompts = {
    // ask the user to name their bot
    askForBotName: () => {
      if(noprompt) {
          return Promise.resolve();
      }

      return generator.prompt({
        type: 'input',
        name: 'botname',
        message: `What's the name of your bot?`,
        default: (generator.options.botname ? generator.options.botname : 'my-chat-bot')
      }).then(answer => {
        // store the botname description answer
        generator.templateConfig.botname = answer.botname;
      });
    },

    // as the user for a decription of their bot
    askForBotDescription: () => {
      if(noprompt) {
        return Promise.resolve();
      }

      return generator.prompt({
        type: 'input',
        name: 'description',
        message: 'What will your bot do?',
        default: (generator.options.description ? generator.options.description : 'Demonstrate the core capabilities of a Conversational AI bot')
      }).then(answer => {
        // store the language description answer
        generator.templateConfig.description = answer.description;
      });
    },

    // ask the user which programming language they want to use
    askForProgrammingLanguage: () => {
      if(noprompt) {
        return Promise.resolve();
      }

      return generator.prompt({
        type: 'list',
        name: 'language',
        message: 'What programming language do you want to use?',
        choices: [
          {
            name: BOT_LANG_NAME_JAVASCRIPT,
            value: _.toLower(BOT_LANG_NAME_JAVASCRIPT)
          },
          {
            name: BOT_LANG_NAME_TYPESCRIPT,
            value: _.toLower(BOT_LANG_NAME_TYPESCRIPT)
          }
        ],
        default: (generator.options.language ? _.toLower(generator.options.language) : BOT_LANG_NAME_JAVASCRIPT)
      }).then(answer => {
        // store the language prompt answer
        generator.templateConfig.language = answer.language;
      });
},

    // ask the user which bot template we should use
    askForBotTemplate: () => {
      if(noprompt) {
        return Promise.resolve();
      }

      return generator.prompt({
        type: 'list',
        name: 'template',
        message: 'Which template would you like to start with?',
        choices: [
          {
            name: `${BOT_TEMPLATE_NAME_SIMPLE} - ${BOT_HELP_URL_SIMPLE}`,
            value: BOT_TEMPLATE_NOPROMPT_SIMPLE
          },
          {
            name: `${BOT_TEMPLATE_NAME_CORE} - ${BOT_HELP_URL_CORE}`,
            value: BOT_TEMPLATE_NOPROMPT_CORE
          },
          {
            name: `${BOT_TEMPLATE_NAME_EMPTY} - ${BOT_HELP_URL_EMPTY}`,
            value: BOT_TEMPLATE_NOPROMPT_EMPTY
          }
        ],
        default: (generator.options.template ? _.toLower(generator.options.template) : BOT_TEMPLATE_NOPROMPT_SIMPLE)
        }).then(answer => {
          // store the template prompt answer
          generator.templateConfig.template = answer.template;

          if(_.toLower(answer.template) === _.toLower(BOT_TEMPLATE_NOPROMPT_CORE)) {
            return generator.prompt({
              type: 'confirm',
              name: 'addtests',
              message: 'Would you like to include a unit test project to test your new bot?',
              default: true
            }).then(answer => {
              // store the addtests prompt answer
              generator.templateConfig.addtests = answer.addtests;
            });
          }
        });
    },

    // ask the user for final confirmation before we generate their bot
    askForFinalConfirmation: () => {
      if(noprompt) {
        return Promise.resolve();
      }

      return generator.prompt({
        type: 'confirm',
        name: 'finalConfirmation',
        message: 'Looking good.  Shall I go ahead and create your new bot?',
        default: true
      }).then(answer => {
        // store the finalConfirmation prompt answer
        generator.templateConfig.finalConfirmation = answer.finalConfirmation;
      });
    }


  };
  return prompts;
};

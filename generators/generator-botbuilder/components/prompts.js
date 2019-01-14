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

  let templateDesc = `The initial bot capabilities. (${BOT_TEMPLATE_NAME_EMPTY} | ${BOT_TEMPLATE_NAME_SIMPLE} | ${BOT_TEMPLATE_NAME_CORE})`;
  gen.option('template', {
    desc: templateDesc,
    type: String,
    default: BOT_TEMPLATE_NOPROMPT_SIMPLE,
    alias: 'T'
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
module.exports.getPrompts = options => {
  const prompts = [
    {
      name: 'botname',
      message: `What's the name of your bot?`,
      default: (options.botname ? options.botname : 'my-chat-bot')
    },
    {
      name: 'description',
      message: 'What will your bot do?',
      default: (options.description ? options.description : 'Demonstrate the core capabilities of a Conversational AI bot')
    },
    {
      name: 'language',
      type: 'list',
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
      default: (options.language ? _.toLower(options.language) : BOT_LANG_NAME_JAVASCRIPT)
    },
    {
      name: 'template',
      type: 'list',
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
      default: (options.template ? _.toLower(options.template) : BOT_TEMPLATE_NOPROMPT_SIMPLE)
    },
    {
      name: 'finalConfirmation',
      type: 'confirm',
      message: 'Looking good.  Shall I go ahead and create your new bot?',
      default: true
    }
  ];

  return prompts;
};

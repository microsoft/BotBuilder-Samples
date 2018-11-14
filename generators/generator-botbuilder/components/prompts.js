// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

/**
 * configureCommandlineOptions
 * does the work to configure the commandline options that this template will accept
 * this is mostly made available so that we can run the template without user
 * intervention.  e.g. automated test runs
 * @param {Generator} gen Yeoman's generator object
 */
module.exports.configureCommandlineOptions = gen => {
  gen.option("botName", {
    desc: "The name you want to give to your bot",
    type: String,
    default: "my-chat-bot",
    alias: "N"
  });
  gen.option("description", {
    desc: "A brief bit of text used to describe what your bot does",
    type: String,
    default: "Demonstrate the core capabilities of the Microsoft Bot Framework",
    alias: "D"
  });
  gen.option("language", {
    desc: "The programming language you want to use. (JavaScript / TypeScript)",
    type: String,
    default: "JavaScript",
    alias: "L"
  });
  gen.option("template", {
    desc: "The initial AI capabilities your bot will have. (Echo / Basic)",
    type: String,
    default: "basic",
    alias: "T"
  });
  gen.argument("noprompt", { type: Boolean, required: false });
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
      name: "botName",
      message: `What's the name of your bot?`,
      default: (options.botname ? options.botname : "my-chat-bot")
    },
    {
      name: "description",
      message: "What will your bot do?",
      default: (options.description ? options.description : "Demonstrate the core capabilities of a Conversational AI bot")
    },
    {
      name: "language",
      type: "list",
      message: "What programming language do you want to use?",
      choices: ["JavaScript", "TypeScript"],
      default: (options.language ? options.language : "JavaScript")
    },
    {
      name: "template",
      type: "list",
      message: "Which template would you like to start with?",
      choices: ["Echo", "Basic"],
      default: (options.template ? options.template : "Basic")
    },
    {
      name: "finalConfirmation",
      type: "confirm",
      message: "Looking good.  Shall I go ahead and create your new bot?",
      default: true
    }
  ];

  return prompts;
};

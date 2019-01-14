// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const _ = require('lodash');
const mkdirp = require('mkdirp');
const path = require('path');

const { commonFilesWriter } = require('./commonFilesWriter');
const { BOT_TEMPLATE_NAME_CORE, BOT_TEMPLATE_NOPROMPT_CORE } = require('./constants');

// generators/app/templates folder name
const GENERATOR_TEMPLATE_NAME = 'core';

const LANG_JS = 'javascript';
const LANG_TS = 'typescript';

/**
 *
 * @param {String} language either 'javascript' or 'typescript'
 */
const getFolders = language => {
  if(!language || (_.toLower(language) !== LANG_JS && _.toLower(language) !== LANG_TS)) {
    throw new Error(`coreTemplateWriter.getFolders called for invalid language: ${ language }`);
  }

  let folders;
  if(_.toLower(language) === LANG_TS) {
    folders = [
      'cognitiveModels',
      'deploymentScripts',
      path.join('deploymentScripts', 'msbotClone'),
      path.join('src', 'dialogs', 'greeting'),
      path.join('resources', 'greeting'),
      path.join('src', 'dialogs', 'welcome'),
      path.join('resources', 'welcome')
    ];
  } else {
    folders = [
      'cognitiveModels',
      'deploymentScripts',
      path.join('deploymentScripts', 'msbotClone'),
      path.join('dialogs', 'greeting'),
      path.join('dialogs', 'greeting', 'resources'),
      path.join('dialogs', 'welcome'),
      path.join('dialogs', 'welcome', 'resources'),
    ];
  }
  return folders;
}

/**
 * Write the files that are specific to the core bot template
 *
 * @param {Generator} gen Yeoman's generator object
 * @param {String} templatePath file path to write the generated code
 */
const writeCoreTemplateFiles = (gen, templatePath) => {
  const COGNITIVE_MODELS = 0;
  const DEPLOYMENT_SCRIPTS = 1;
  const DEPLOYMENT_MSBOT = 2;
  const DIALOGS_GREETING = 3;
  const DIALOGS_GREETING_RESOURCES = 4;
  const DIALOGS_WELCOME = 5;
  const DIALOGS_WELCOME_RESOURCES = 6;
  const TS_SRC_FOLDER = 'src';

  // get the folder strucure, based on language
  const srcFolders = [
    'cognitiveModels',
    'deploymentScripts',
    path.join('deploymentScripts', 'msbotClone'),
    path.join('dialogs' , 'greeting'),
    path.join('dialogs', 'greeting', 'resources'),
    path.join('dialogs', 'welcome'),
    path.join('dialogs', 'welcome', 'resources'),
  ];
  const destFolders = getFolders(_.toLower(gen.props.language));

  const extension = _.toLower(gen.props.language) === 'javascript' ? 'js' : 'ts';
  const srcFolder = _.toLower(gen.props.language) === 'javascript' ? '' : TS_SRC_FOLDER;
  // if we're generating JS, then keep the json extension
  // if we're generating TS, then we need the extension to be js or tsc will complain (tsc v3.1.6)
  const cardExtension = _.toLower(gen.props.language) === 'javascript' ? 'json' : 'js';

  // create the core bot folder structure
  for (let cnt = 0; cnt < destFolders.length; ++cnt) {
    mkdirp.sync(destFolders[cnt]);
  }

  // write out the LUIS model
  let sourcePath = path.join(templatePath, srcFolders[COGNITIVE_MODELS]);
  let destinationPath = path.join(gen.destinationPath(), destFolders[COGNITIVE_MODELS]);
  gen.fs.copy(
    path.join(sourcePath, 'coreBot.luis'),
    path.join(destinationPath, 'coreBot.luis')
  );

  // if we're writing out TypeScript, then we need to add a webConfigPrep.js
  if(_.toLower(gen.props.language) === LANG_TS) {
    sourcePath = path.join(templatePath, srcFolders[DEPLOYMENT_SCRIPTS]);
    destinationPath = path.join(gen.destinationPath(), destFolders[DEPLOYMENT_SCRIPTS]);
    gen.fs.copy(
      path.join(sourcePath, 'webConfigPrep.js'),
      path.join(destinationPath, 'webConfigPrep.js')
    );
  }

  // write out deployment resources
  sourcePath = path.join(templatePath, srcFolders[DEPLOYMENT_MSBOT]);
  destinationPath = path.join(gen.destinationPath(), destFolders[DEPLOYMENT_MSBOT]);
  gen.fs.copy(
    path.join(sourcePath, '34.luis'),
    path.join(destinationPath, '34.luis')
  );
  gen.fs.copyTpl(
    path.join(sourcePath, 'bot.recipe'),
    path.join(destinationPath, 'bot.recipe'),
    {
      botname: gen.props.botname
    }
  );

  // write out the greeting dialog
  sourcePath = path.join(templatePath, srcFolders[DIALOGS_GREETING]);
  destinationPath = path.join(gen.destinationPath(), destFolders[DIALOGS_GREETING]);
  gen.fs.copyTpl(
    path.join(sourcePath, `greeting.${extension}`),
    path.join(destinationPath, `greeting.${extension}`),
    {
      botname: gen.props.botname
    }
  );
  gen.fs.copy(
    path.join(sourcePath, `index.${extension}`),
    path.join(destinationPath, `index.${extension}`)
  );
  gen.fs.copy(
    path.join(sourcePath, `userProfile.${extension}`),
    path.join(destinationPath, `userProfile.${extension}`)
  );
  // list the greeting dialog resources
  const greetingResources = [
    'cancel.lu',
    'greeting.lu',
    'help.lu',
    'main.lu',
    'none.lu',
  ];
  // write out greeting dialog resources
  sourcePath = path.join(templatePath, srcFolders[DIALOGS_GREETING_RESOURCES]);
  destinationPath = path.join(gen.destinationPath(), destFolders[DIALOGS_GREETING_RESOURCES]);
  for (let cnt = 0; cnt < greetingResources.length; cnt++) {
    gen.fs.copy(
      path.join(sourcePath, greetingResources[cnt]),
      path.join(destinationPath, greetingResources[cnt])
    );
  }

  // write out welcome named exports, optimization
  sourcePath = path.join(templatePath, srcFolders[DIALOGS_WELCOME]);
  destinationPath = path.join(gen.destinationPath(), destFolders[DIALOGS_WELCOME]);
  gen.fs.copy(
    path.join(sourcePath, `index.${extension}`),
    path.join(destinationPath, `index.${extension}`)
  );

  // write out welcome adaptive card
  sourcePath = path.join(templatePath, srcFolders[DIALOGS_WELCOME_RESOURCES]);
  destinationPath = path.join(gen.destinationPath(), destFolders[DIALOGS_WELCOME_RESOURCES]);
  gen.fs.copy(
    path.join(sourcePath, 'welcomeCard.json'),
    path.join(destinationPath, `welcomeCard.json`)
  );

  // write out the index.js and bot.js
  destinationPath = path.join(gen.destinationPath(), srcFolder);

  // gen index and main dialog files
  gen.fs.copyTpl(
    gen.templatePath(path.join(templatePath, `index.${extension}`)),
    path.join(destinationPath, `index.${extension}`),
    {
      botname: gen.props.botname
    }
  );
  // gen the main bot activity router
  gen.fs.copyTpl(
    gen.templatePath(path.join(templatePath, `bot.${extension}`)),
    path.join(destinationPath, `bot.${extension}`),
    {
      botname: gen.props.botname
    }
  );

  // write out PREREQUISITES.md
  gen.fs.copyTpl(
    gen.templatePath(path.join(templatePath, 'PREREQUISITES.md')),
    gen.destinationPath('PREREQUISITES.md'),
    {
      botname: gen.props.botname
    }
  );
}

/**
 * Write project files for Core template
 *
 * @param {Generator} gen Yeoman's generator object
 */
module.exports.coreTemplateWriter = gen => {
  // do some simple sanity checking to ensure we're being
  // called correctly
  const template = _.toLower(gen.props.template)
  if (template !== _.toLower(BOT_TEMPLATE_NAME_CORE) && template !== _.toLower(BOT_TEMPLATE_NOPROMPT_CORE)) {
    throw new Error(`coreTemplateWriter called for wrong template: ${ gen.props.template }`);
  }
  const templatePath = path.join(gen.templatePath(), GENERATOR_TEMPLATE_NAME);

  // write files common to all template options
  commonFilesWriter(gen, templatePath);

  // write files specific to the core bot template
  writeCoreTemplateFiles(gen, templatePath);
}

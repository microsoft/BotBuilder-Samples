// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const path = require('path');
const _ = require('lodash');
const mkdirp = require('mkdirp');

const { commonFilesWriter } = require('./commonFilesWriter');
const { BOT_TEMPLATE_NAME_EMPTY, BOT_TEMPLATE_NOPROMPT_EMPTY } = require('./constants');

// generators/app/templates folder name
const GENERATOR_TEMPLATE_NAME = 'empty';

const LANG_TS = 'typescript';

/**
 * Write the files that are specific to the empty bot template
 *
 * @param {Generator} gen Yeoman's generator object
 * @param {String} templatePath file path to write the generated code
 */
const writeEmptyTemplateFiles = (generator, templatePath) => {
  const DEPLOYMENT_SCRIPTS = 0;
  const TS_SRC_FOLDER = 'src'
  const folders = [
    'deploymentScripts',
  ];
  const extension = _.toLower(generator.templateConfig.language) === 'javascript' ? 'js' : 'ts';
  const srcFolder = _.toLower(generator.templateConfig.language) === 'javascript' ? '' : TS_SRC_FOLDER;

  // create the empty bot folder structure
  if (_.toLower(generator.templateConfig.language) === LANG_TS) {
    for (let cnt = 0; cnt < folders.length; ++cnt) {
      mkdirp.sync(folders[cnt]);
    }
  }
  // create a src directory if we are generating TypeScript
  if (_.toLower(generator.templateConfig.language) === LANG_TS) {
    mkdirp.sync(TS_SRC_FOLDER);
  }

  let sourcePath = path.join(templatePath, folders[DEPLOYMENT_SCRIPTS]);
  let destinationPath = path.join(generator.destinationPath(), folders[DEPLOYMENT_SCRIPTS]);

  // if we're writing out TypeScript, then we need to add a webConfigPrep.js
  if(_.toLower(generator.templateConfig.language) === LANG_TS) {
    generator.fs.copy(
      path.join(sourcePath, 'webConfigPrep.js'),
      path.join(destinationPath, 'webConfigPrep.js')
    );
  }


  // write out the index.js and bot.js
  destinationPath = path.join(generator.destinationPath(), srcFolder);

  // gen the main index file
  generator.fs.copyTpl(
    generator.templatePath(path.join(templatePath, `index.${extension}`)),
    path.join(destinationPath, `index.${extension}`),
    {
      botname: generator.templateConfig.botname
    }
  );
  // gen the main bot activity router
  generator.fs.copy(
    generator.templatePath(path.join(templatePath, `bot.${extension}`)),
    path.join(destinationPath, `bot.${extension}`)
  );
}

/**
 * Write project files for Empty template
 *
 * @param {Generator} generator Yeoman's generator object
 */
module.exports.emptyTemplateWriter = generator => {
  // do some simple sanity checking to ensure we're being
  // called correctly
  const template = _.toLower(generator.templateConfig.template)
  if (template !== _.toLower(BOT_TEMPLATE_NAME_EMPTY) && template !== _.toLower(BOT_TEMPLATE_NOPROMPT_EMPTY)) {
    throw new Error(`basicTemplateWriter called for wrong template: ${ generator.templateConfig.template }`);
  }

  // build the path to the empty template source folder
  const templatePath = path.join(generator.templatePath(), GENERATOR_TEMPLATE_NAME);

  // write files common to all our template options
  commonFilesWriter(generator, templatePath);

  // write files specific to the empty bot template
  writeEmptyTemplateFiles(generator, templatePath);
}


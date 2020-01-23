// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const _ = require('lodash');
const mkdirp = require('mkdirp');
const path = require('path');

const { commonFilesWriter } = require('./commonFilesWriter');
const { BOT_TEMPLATE_NAME_CORE, BOT_TEMPLATE_NOPROMPT_CORE } = require('./constants');
const pkg = require('../package.json');

// generators/app/templates folder name
const GENERATOR_TEMPLATE_NAME = 'core';

const LANG_JS = 'javascript';
const LANG_TS = 'typescript';

/**
 *
 * @param {String} language either 'javascript' or 'typescript'
 */
const _getSourceFolders = language => {
  if(!language || (_.toLower(language) !== LANG_JS && _.toLower(language) !== LANG_TS)) {
    throw new Error(`coreTemplateWriter._getTargetFolders called for invalid language: ${ language }`);
  }

  // get the folder structure, based on language
  let folders = [
    'bots',
    'cognitiveModels',
    'dialogs',
    'resources',
  ];
  // if we're generating TypeScript, then we need a deploymentScripts folder
  if(_.toLower(language) === LANG_TS) {
    folders = folders.concat(['deploymentScripts']);
  }
  return folders;
}

/**
 *
 * @param {String} language either 'javascript' or 'typescript'
 */
const _getTargetFolders = language => {
  if(!language || (_.toLower(language) !== LANG_JS && _.toLower(language) !== LANG_TS)) {
    throw new Error(`coreTemplateWriter._getTargetFolders called for invalid language: ${ language }`);
  }

  let folders;
  if(_.toLower(language) === LANG_TS) {
    folders = [
      path.join('src', 'bots'),
      'cognitiveModels',
      path.join('src', 'dialogs'),
      'resources',
      'deploymentScripts',
    ];
  } else {
    folders = [
      'bots',
      'cognitiveModels',
      'dialogs',
      'resources',
    ];
  }
  return folders;
}

/**
 *
 * @param {String} language either 'javascript' or 'typescript'
 */
const _getSourceTestFolders = language => {
  const lang = _.toLower(language);
  if(!lang || (lang !== LANG_JS && lang !== LANG_TS)) {
    throw new Error(`coreTemplateWriter._getTargetFolders called for invalid language: ${ language }`);
  }

  // get the folder structure, based on language
  const folders = [
    'tests',
    path.join('tests', 'bots'),
    path.join('tests', 'dialogs'),
    path.join('tests', 'dialogs', 'testData'),
    path.join('tests', 'dialogs', 'testData'),
  ];
  return folders;
}

/**
 *
 * @param {String} language either 'javascript' or 'typescript'
 */
const _getTargetTestFolders = language => {
  const lang = _.toLower(language);
  if(!lang || (lang !== LANG_JS && lang !== LANG_TS)) {
    throw new Error(`coreTemplateWriter._getTargetTestFolders called for invalid language: ${ language }`);
  }

  let folders;
  if(_.toLower(language) === LANG_TS) {
    folders = [
      path.join('src', 'tests'),
      path.join('src', 'tests', 'bots'),
      path.join('src', 'tests', 'dialogs'),
      path.join('src', 'tests', 'dialogs', 'testData'),
      'testResources',
    ];
  } else {
    folders = [
      'tests',
      path.join('tests', 'bots'),
      path.join('tests', 'dialogs'),
      path.join('tests', 'dialogs', 'testData'),
      path.join('tests', 'dialogs', 'testData'),
    ];
  }
  return folders;
}

/**
 * Write the files that are specific to the core bot template
 *
 * @param {Generator} generator Yeoman's generator object
 * @param {String} templatePath file path to write the generated code
 */
const writeCoreTemplateTestFiles = (generator, templatePath) => {
  // lets validate that we should be called
  if(generator.templateConfig.addtests !== true) {
    throw new Error(`writeCoreTemplateTestFiles called when 'addtests' flag is false: ${ generator.templateConfig.addtests }`);
  }
  // declare some constants that map to srcFolder and destFolder array offsets
  const TEST_FOLDER = 0;
  const BOTS_TEST_FOLDER = 1;
  const DIALOGS_TEST_FOLDER = 2;
  const DIALOGS_TESTDATA_FOLDER = 3;
  const DIALOGS_TESTDATA_JSON_FOLDER = 4;

  // get the folder structure, based on language
  const srcFolders = _getSourceTestFolders(_.toLower(generator.templateConfig.language));
  const destFolders = _getTargetTestFolders(_.toLower(generator.templateConfig.language));

  const extension = _.toLower(generator.templateConfig.language) === 'javascript' ? 'js' : 'ts';

  // create the core bot tests folder structure
  for (let cnt = 0; cnt < destFolders.length; ++cnt) {
    mkdirp.sync(destFolders[cnt]);
  }

  // overwrite the commonFilesWriter version as we want to version that has
  // the npm test script command `npm test'
  generator.fs.copyTpl(
    generator.templatePath(path.join(templatePath, 'package-with-tests.json.' + extension)),
    generator.destinationPath('package.json'),
    {
      botname: generator.templateConfig.botname,
      botDescription: generator.templateConfig.description,
      version: pkg.version,
      npmMain: (extension === 'js' ? `index.js` : `./lib/index.js`)
    }
  );



  // write out the test folder's README.md file
  let sourcePath = path.join(templatePath, srcFolders[TEST_FOLDER]);
  let destinationPath = path.join(generator.destinationPath(), destFolders[TEST_FOLDER]);
  generator.fs.copyTpl(
    path.join(sourcePath, 'README.md'),
    path.join(destinationPath, 'README.md'),
    {
      botname: generator.templateConfig.botname
    }
  );

  // write out the bots test folder
  sourcePath = path.join(templatePath, srcFolders[BOTS_TEST_FOLDER]);
  destinationPath = path.join(generator.destinationPath(), destFolders[BOTS_TEST_FOLDER]);
  generator.fs.copy(
    path.join(sourcePath, `dialogAndWelcomeBot.test.${extension}`),
    path.join(destinationPath, `dialogAndWelcomeBot.test.${extension}`)
  );

  // write out the dialogs test folder
  sourcePath = path.join(templatePath, srcFolders[DIALOGS_TEST_FOLDER]);
  destinationPath = path.join(generator.destinationPath(), destFolders[DIALOGS_TEST_FOLDER]);
  generator.fs.copy(
    path.join(sourcePath, `bookingDialog.test.${extension}`),
    path.join(destinationPath, `bookingDialog.test.${extension}`)
  );
  generator.fs.copy(
    path.join(sourcePath, `cancelAndHelpDialog.test.${extension}`),
    path.join(destinationPath, `cancelAndHelpDialog.test.${extension}`)
  );
  generator.fs.copy(
    path.join(sourcePath, `dateResolverDialog.test.${extension}`),
    path.join(destinationPath, `dateResolverDialog.test.${extension}`)
  );
  generator.fs.copy(
    path.join(sourcePath, `mainDialog.test.${extension}`),
    path.join(destinationPath, `mainDialog.test.${extension}`)
  );

  // write out the dialogs testData folder (treat .json files separately)
  sourcePath = path.join(templatePath, srcFolders[DIALOGS_TESTDATA_FOLDER]);
  destinationPath = path.join(generator.destinationPath(), destFolders[DIALOGS_TESTDATA_FOLDER]);
  generator.fs.copy(
      path.join(sourcePath, `bookingDialogTestCases.${extension}`),
      path.join(destinationPath, `bookingDialogTestCases.${extension}`)
  );
  generator.fs.copy(
    path.join(sourcePath, `dateResolverTestCases.${extension}`),
    path.join(destinationPath, `dateResolverTestCases.${extension}`)
  );

  // write out the dialogs testData folder (treat .json files separately)
  // tsc won't copy these where I want them, so we move them up to a root folder
  sourcePath = path.join(templatePath, srcFolders[DIALOGS_TESTDATA_JSON_FOLDER]);
  destinationPath = path.join(generator.destinationPath(), destFolders[DIALOGS_TESTDATA_JSON_FOLDER]);
  generator.fs.copy(
    path.join(sourcePath, 'FlightFromCdgToJfk.json'),
    path.join(destinationPath, 'FlightFromCdgToJfk.json')
  );
  generator.fs.copy(
    path.join(sourcePath, 'FlightFromMadridToChicago.json'),
    path.join(destinationPath, 'FlightFromMadridToChicago.json')
  );
  generator.fs.copy(
    path.join(sourcePath, 'FlightFromParisToNewYork.json'),
    path.join(destinationPath, 'FlightFromParisToNewYork.json')
  );
  generator.fs.copy(
    path.join(sourcePath, 'FlightToMadrid.json'),
    path.join(destinationPath, 'FlightToMadrid.json')
  );
}

/**
 * Write the files that are specific to the core bot template
 *
 * @param {Generator} generator Yeoman's generator object
 * @param {String} templatePath file path to write the generated code
 */
const writeCoreTemplateFiles = (generator, templatePath) => {
  const BOTS_FOLDER = 0;
  const COGNITIVE_MODELS_FOLDER = 1;
  const DIALOGS_FOLDER = 2;
  const RESOURCES_FOLDER = 3;
  const DEPLOYMENT_SCRIPTS_FOLDER = 4;
  const TS_SRC_FOLDER = 'src';

  // get the folder structure, based on language
  const srcFolders = _getSourceFolders(_.toLower(generator.templateConfig.language), generator.options.addtests);
  const destFolders = _getTargetFolders(_.toLower(generator.templateConfig.language), generator.options.addtests);

  const extension = _.toLower(generator.templateConfig.language) === 'javascript' ? 'js' : 'ts';
  const srcFolder = _.toLower(generator.templateConfig.language) === 'javascript' ? '' : TS_SRC_FOLDER;

  // create the core bot folder structure
  for (let cnt = 0; cnt < destFolders.length; ++cnt) {
    mkdirp.sync(destFolders[cnt]);
  }

  // write out the bots folder
  let sourcePath = path.join(templatePath, srcFolders[BOTS_FOLDER]);
  let destinationPath = path.join(generator.destinationPath(), destFolders[BOTS_FOLDER]);
  generator.fs.copy(
    path.join(sourcePath, `dialogAndWelcomeBot.${extension}`),
    path.join(destinationPath, `dialogAndWelcomeBot.${extension}`),
    {
      botname: generator.templateConfig.botname
    }
  );
  generator.fs.copy(
    path.join(sourcePath, `dialogBot.${extension}`),
    path.join(destinationPath, `dialogBot.${extension}`)
  );

  // write out the LUIS model
  sourcePath = path.join(templatePath, srcFolders[COGNITIVE_MODELS_FOLDER]);
  destinationPath = path.join(generator.destinationPath(), destFolders[COGNITIVE_MODELS_FOLDER]);
  generator.fs.copy(
    path.join(sourcePath, 'FlightBooking.json'),
    path.join(destinationPath, 'FlightBooking.json')
  );

  // if we're writing out TypeScript, then we need to add a webConfigPrep.js
  if(_.toLower(generator.templateConfig.language) === LANG_TS) {
    sourcePath = path.join(templatePath, srcFolders[DEPLOYMENT_SCRIPTS_FOLDER]);
    destinationPath = path.join(generator.destinationPath(), destFolders[DEPLOYMENT_SCRIPTS_FOLDER]);
    generator.fs.copy(
      path.join(sourcePath, 'webConfigPrep.js'),
      path.join(destinationPath, 'webConfigPrep.js')
    );
  }

  // write out the dialogs folder
  sourcePath = path.join(templatePath, srcFolders[DIALOGS_FOLDER]);
  destinationPath = path.join(generator.destinationPath(), destFolders[DIALOGS_FOLDER]);
  if(_.toLower(generator.templateConfig.language) === LANG_TS) {
      generator.fs.copy(
      path.join(sourcePath, `bookingDetails.${extension}`),
      path.join(destinationPath, `bookingDetails.${extension}`)
    );
  }
  generator.fs.copy(
    path.join(sourcePath, `bookingDialog.${extension}`),
    path.join(destinationPath, `bookingDialog.${extension}`)
  );
  generator.fs.copy(
    path.join(sourcePath, `cancelAndHelpDialog.${extension}`),
    path.join(destinationPath, `cancelAndHelpDialog.${extension}`)
  );
  generator.fs.copy(
    path.join(sourcePath, `dateResolverDialog.${extension}`),
    path.join(destinationPath, `dateResolverDialog.${extension}`)
  );
  generator.fs.copy(
    path.join(sourcePath, `flightBookingRecognizer.${extension}`),
    path.join(destinationPath, `flightBookingRecognizer.${extension}`)
  );
  generator.fs.copy(
    path.join(sourcePath, `mainDialog.${extension}`),
    path.join(destinationPath, `mainDialog.${extension}`)
  );

  // write out the resources folder
  // which contains the welcome adaptive card
  sourcePath = path.join(templatePath, srcFolders[RESOURCES_FOLDER]);
  destinationPath = path.join(generator.destinationPath(), destFolders[RESOURCES_FOLDER]);
  generator.fs.copy(
    path.join(sourcePath, 'welcomeCard.json'),
    path.join(destinationPath, 'welcomeCard.json')
  );

  // write out the index.js and bot.js
  destinationPath = path.join(generator.destinationPath(), srcFolder);

  // gen index and main dialog files
  generator.fs.copyTpl(
    generator.templatePath(path.join(templatePath, `index.${extension}`)),
    path.join(destinationPath, `index.${extension}`),
    {
      botname: generator.templateConfig.botname
    }
  );

  // if asked to write out unit tests, then let's have a go at it
  if(generator.options.addtests) {
    writeCoreTemplateTestFiles(generator, templatePath);
  }
}

/**
 * Write project files for Core template
 *
 * @param {Generator} generator Yeoman's generator object
 */
module.exports.coreTemplateWriter = generator => {
  // do some simple sanity checking to ensure we're being
  // called correctly
  const template = _.toLower(generator.templateConfig.template)
  if (template !== _.toLower(BOT_TEMPLATE_NAME_CORE) && template !== _.toLower(BOT_TEMPLATE_NOPROMPT_CORE)) {
    throw new Error(`coreTemplateWriter called for wrong template: ${ generator.templateConfig.template }`);
  }
  const templatePath = path.join(generator.templatePath(), GENERATOR_TEMPLATE_NAME);

  // write files common to all template options
  commonFilesWriter(generator, templatePath);

  // write files specific to the core bot template
  writeCoreTemplateFiles(generator, templatePath);

  // write out unit tests if asked to do so
  if(generator.templateConfig.addtests === true) {
    writeCoreTemplateTestFiles(generator, templatePath);
  }
}

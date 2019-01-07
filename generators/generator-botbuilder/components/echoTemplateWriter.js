// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

<<<<<<< HEAD
const path = require("path");
const _ = require("lodash");
const mkdirp = require("mkdirp");

const { commonFilesWriter } = require('./commonFilesWriter');

const TEMPLATE_NAME = "echo";
=======
const _ = require('lodash');
const mkdirp = require('mkdirp');
const path = require('path');

const { commonFilesWriter } = require('./commonFilesWriter');
const { BOT_TEMPLATE_NAME_SIMPLE,  BOT_TEMPLATE_NOPROMPT_SIMPLE } = require('./constants');

// generators/app/templates folder name
const GENERATOR_TEMPLATE_NAME = 'echo';

const LANG_TS = 'typescript';
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145

/**
 * Write the files that are specific to the echo bot template
 *
 * @param {Generator} gen Yeoman's generator object
 * @param {String} templatePath file path to write the generated code
 */
const writeEchoTemplateFiles = (gen, templatePath) => {
  const DEPLOYMENT_SCRIPTS = 0;
  const DEPLOYMENT_MSBOT = 1;
<<<<<<< HEAD
  const RESOURCES = 2;
  const TS_SRC_FOLDER = "src"
  const folders = [
    'deploymentScripts',
    path.join('deploymentScripts', 'msbotClone'),
    'resources'
  ];
  const extension = _.toLower(gen.props.language) === "javascript" ? "js" : "ts";
  const srcFolder = _.toLower(gen.props.language) === "javascript" ? "" : TS_SRC_FOLDER;
=======
  const TS_SRC_FOLDER = 'src'
  const folders = [
    'deploymentScripts',
    path.join('deploymentScripts', 'msbotClone')
  ];
  const extension = _.toLower(gen.props.language) === 'javascript' ? 'js' : 'ts';
  const srcFolder = _.toLower(gen.props.language) === 'javascript' ? '' : TS_SRC_FOLDER;
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145

  // create the echo bot folder structure common to both languages
  for (let cnt = 0; cnt < folders.length; ++cnt) {
    mkdirp.sync(folders[cnt]);
  }
  // create a src directory if we are generating TypeScript
<<<<<<< HEAD
  if (_.toLower(gen.props.language) === "typescript") {
    mkdirp.sync(TS_SRC_FOLDER);
  }

  // write out the deployment scripts
  let sourcePath = path.join(templatePath, folders[DEPLOYMENT_MSBOT]);
  let destinationPath = path.join(gen.destinationPath(), folders[DEPLOYMENT_MSBOT]);
=======
  if (_.toLower(gen.props.language) === LANG_TS) {
    mkdirp.sync(TS_SRC_FOLDER);
  }

  let sourcePath = path.join(templatePath, folders[DEPLOYMENT_SCRIPTS]);
  let destinationPath = path.join(gen.destinationPath(), folders[DEPLOYMENT_SCRIPTS]);

   // if we're writing out TypeScript, then we need to add a webConfigPrep.js
   if(_.toLower(gen.props.language) === LANG_TS) {
     gen.fs.copy(
       path.join(sourcePath, 'webConfigPrep.js'),
       path.join(destinationPath, 'webConfigPrep.js')
     );
   }

  // write out deployment resources
  sourcePath = path.join(templatePath, folders[DEPLOYMENT_MSBOT]);
  destinationPath = path.join(gen.destinationPath(), folders[DEPLOYMENT_MSBOT]);
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
  gen.fs.copyTpl(
    path.join(sourcePath, 'bot.recipe'),
    path.join(destinationPath, 'bot.recipe'),
    {
<<<<<<< HEAD
      botName: gen.props.botName
=======
      botname: gen.props.botname
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
    }
  );

  // write out the index.js and bot.js
  destinationPath = path.join(gen.destinationPath(), srcFolder);

  // gen the main index file
  gen.fs.copyTpl(
    gen.templatePath(path.join(templatePath, `index.${extension}`)),
    path.join(destinationPath, `index.${extension}`),
    {
<<<<<<< HEAD
      botName: gen.props.botName
    }
  );
=======
      botname: gen.props.botname
    }
  );

>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
  // gen the main bot activity router
  gen.fs.copy(
    gen.templatePath(path.join(templatePath, `bot.${extension}`)),
    path.join(destinationPath, `bot.${extension}`)
  );

<<<<<<< HEAD
  // write out the  AI resource(s)
  sourcePath = path.join(templatePath, folders[RESOURCES]);
  destinationPath = path.join(gen.destinationPath(), folders[RESOURCES]);
  gen.fs.copy(
    path.join(sourcePath, `echo.chat`),
    path.join(destinationPath, `echo.chat`)
=======
  // write out PREREQUISITES.md
  gen.fs.copyTpl(
    gen.templatePath(path.join(templatePath, 'PREREQUISITES.md')),
    gen.destinationPath('PREREQUISITES.md'),
    {
      botname: gen.props.botname
    }
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
  );
}

/**
 * Write project files for Echo template
 *
 * @param {Generator} gen Yeoman's generator object
 */
module.exports.echoTemplateWriter = gen => {
  // do some simple sanity checking to ensure we're being
  // called correctly
<<<<<<< HEAD
  if (_.toLower(gen.props.template) !== TEMPLATE_NAME) {
    throw new Error(`writeEchoProjectFiles called for wrong template: ${gen.props.template}`);
  }

  // build the path to the echo template source folder
  const templatePath = path.join(gen.templatePath(), TEMPLATE_NAME);
=======
  const template = _.toLower(gen.props.template)
  if (template !== _.toLower(BOT_TEMPLATE_NAME_SIMPLE) && template !== _.toLower(BOT_TEMPLATE_NOPROMPT_SIMPLE)) {
    throw new Error(`writeEchoProjectFiles called for wrong template: ${ gen.props.template }`);
  }

  // build the path to the echo template source folder
  const templatePath = path.join(gen.templatePath(), GENERATOR_TEMPLATE_NAME);
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145

  // write files common to all our template options
  commonFilesWriter(gen, templatePath);

  // write files specific to the echo bot template
  writeEchoTemplateFiles(gen, templatePath);
}


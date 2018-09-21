// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const path = require("path");
const _ = require("lodash");
const mkdirp = require("mkdirp");

const { commonFilesWriter } = require('./commonFilesWriter');

const TEMPLATE_NAME = "echo";
const TEMPLATE_PATH = "/echo/";

/**
 * Write the files that are specific to the echo bot template
 *
 * @param {Generator} gen Yeoman's generator object
 * @param {String} templatePath file path to write the generated code
 */
const writeEchoTemplateFiles = (gen, templatePath) => {
  const DEPLOYMENT_SCRIPTS = 0;
  const DEPLOYMENT_MSBOT = 1;
  const RESOURCES = 2;
  const TS_SRC_FOLDER = "src/"
  const folders = [
    'deploymentScripts/',
    'deploymentScripts/msbotClone/',
    'resources/'
  ];
  const extension = _.toLower(gen.props.language) === "javascript" ? "js" : "ts";
  const srcFolder = _.toLower(gen.props.language) === "javascript" ? "" : TS_SRC_FOLDER;

  // create the echo bot folder structure common to both languages
  for (let cnt = 0; cnt < folders.length; ++cnt) {
    mkdirp.sync(folders[cnt]);
  }
  // create a src directory if we are generating TypeScript
  if (_.toLower(gen.props.language) === "typescript") {
    mkdirp.sync(srcFolder);
  }

  // write out the deployment scripts
  let sourcePath = templatePath + folders[DEPLOYMENT_MSBOT];
  let destinationPath = gen.destinationPath() + '/' + folders[DEPLOYMENT_MSBOT];
  gen.fs.copy(sourcePath + 'bot.recipe', destinationPath + 'bot.recipe');

  // write out the index.js and bot.js
  destinationPath = gen.destinationPath() + '/' + srcFolder;

  // gen the main index file
  gen.fs.copyTpl(
    gen.templatePath(templatePath + `index.${extension}`),
    destinationPath + `index.${extension}`,
    {
      botName: gen.props.botName
    }
  );
  // gen the main bot activity router
  gen.fs.copy(gen.templatePath(templatePath + `bot.${extension}`), destinationPath + `bot.${extension}`);

  // write out the  AI resource(s)
  sourcePath = templatePath + folders[RESOURCES];
  destinationPath = gen.destinationPath() + '/' + folders[RESOURCES];
  gen.fs.copy(sourcePath + `echo.chat`, destinationPath + `echo.chat`);
}

/**
 * Write project files for Echo template
 *
 * @param {Generator} gen Yeoman's generator object
 */
module.exports.echoTemplateWriter = gen => {
  // do some simple sanity checking to ensure we're being
  // called correctly
  if (_.toLower(gen.props.template) !== TEMPLATE_NAME) {
    throw new Error(`writeEchoProjectFiles called for wrong template: ${gen.props.template}`)
  }
  const templatePath = gen.templatePath() + TEMPLATE_PATH;

  // write files common to all our template options
  commonFilesWriter(gen, templatePath);

  // write files specific to the echo bot template
  writeEchoTemplateFiles(gen, templatePath);
}


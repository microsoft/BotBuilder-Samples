// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

<<<<<<< HEAD
const path = require("path");
const _ = require("lodash");
const mkdirp = require("mkdirp");
=======
const path = require('path');
const _ = require('lodash');
const mkdirp = require('mkdirp');
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145

const pkg = require('../package.json');

/**
 * Create the folder for the generated bot code, if it doesn't exist
 *
 * @param {Generator} gen Yeoman's generator object
 * @param {String} directoryName root folder for the generated code
 */
const makeProjectDirectory = (gen, directoryName) => {
  gen.log(path.basename(gen.destinationPath()));
  gen.log(directoryName);
  if (path.basename(gen.destinationPath()) !== directoryName) {
    gen.log(`Your bot should be in a directory named ${directoryName}\nI'll automatically create this folder.`);
    mkdirp.sync(directoryName);
    gen.destinationRoot(gen.destinationPath(directoryName));
  }
}

/**
 * Based on the template, write the files common across the given template options
 *
 * @param {Generator} gen Yeoman's generator object
 * @param {String} templatePath file path to write the generated code
 */
// const writeCommonFiles = (gen, templatePath) => {
module.exports.commonFilesWriter = (gen, templatePath) => {
<<<<<<< HEAD
  const botName = gen.props.botName;
  const extension = _.toLower(gen.props.language) === "javascript" ? "js" : "ts";
  const npmMain = extension === 'js' ? `index.js` : `./lib/index.js`;
  const npmBuildCmd = extension === 'js' ? `exit 1` : `tsc`;
  const npmRunCmd = extension === 'js' ? `node ./index.js` : "tsc && node ./lib/index.js";
  const npmWatchCmd = extension === 'js' ? "nodemon ./index.js" : "tsc && node ./lib/index.js";

  // ensure our project directory exists before we start writing files into it
  makeProjectDirectory(gen, _.camelCase(gen.props.botName));
=======
  const botname = gen.props.botname;
  const extension = _.toLower(gen.props.language) === 'javascript' ? 'js' : 'ts';
  const npmMain = extension === 'js' ? `index.js` : `./lib/index.js`;


  // ensure our project directory exists before we start writing files into it
  makeProjectDirectory(gen, _.kebabCase(gen.props.botname));
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145

  // write the project files common to all templates
  // do any text token processing where required
  gen.fs.copyTpl(
<<<<<<< HEAD
    gen.templatePath(path.join(templatePath, "package.json." + extension)),
    gen.destinationPath("package.json"),
    {
      botName: gen.props.botName,
=======
    gen.templatePath(path.join(templatePath, 'package.json.' + extension)),
    gen.destinationPath('package.json'),
    {
      botname: gen.props.botname,
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
      botDescription: gen.props.description,
      version: pkg.version,
      npmMain: npmMain
    }
  );
  gen.fs.copy(
    gen.templatePath(path.join(templatePath, '_gitignore')),
    gen.destinationPath('.gitignore')
  );

  gen.fs.copy(
    gen.templatePath(path.join(templatePath, `botName.bot`)),
<<<<<<< HEAD
    gen.destinationPath(`${gen.props.botName}.bot`), {
    process: function (content) {
      var pattern = new RegExp('<%= botName %>', 'g');
      return content.toString().replace(pattern, botName.toString());
=======
    gen.destinationPath(`${gen.props.botname}.bot`), {
    process: function (content) {
      var pattern = new RegExp('<%= botname %>', 'g');
      return content.toString().replace(pattern, botname.toString());
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
    }
  });

  // gen a .env file that points to the botfile
  gen.fs.copyTpl(
    gen.templatePath(path.join(templatePath, '_env')),
    gen.destinationPath('.env'),
    {
<<<<<<< HEAD
      botFileName: gen.props.botName
=======
      botFileName: gen.props.botname
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
    }
  );

  // determine what language we are working in, TypeScript or JavaScript
  // and write language specific files now
  if (extension === 'ts') {
    gen.fs.copy(
      gen.templatePath(path.join(templatePath, 'tsconfig.json')),
      gen.destinationPath('tsconfig.json')
    );
<<<<<<< HEAD
    srcReadmePath = path.join(templatePath, "README.md.ts")
=======
    gen.fs.copy(
      gen.templatePath(path.join(templatePath, 'tslint.json')),
      gen.destinationPath('tslint.json')
    );
    srcReadmePath = path.join(templatePath, 'README.md.ts')
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
  } else {
    gen.fs.copy(
      gen.templatePath(path.join(templatePath, '_eslintrc.js')),
      gen.destinationPath('.eslintrc.js')
    );
<<<<<<< HEAD
    srcReadmePath = path.join(templatePath, "README.md.js")
=======
    srcReadmePath = path.join(templatePath, 'README.md.js')
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
  }

  // gen a readme with specifics to what was generated
  gen.fs.copyTpl(
    gen.templatePath(srcReadmePath),
<<<<<<< HEAD
    gen.destinationPath("README.md"),
    {
      botName: gen.props.botName,
      description: gen.props.description,
      runCmd: npmRunCmd,
      watchCmd: npmWatchCmd,
      extension: extension
=======
    gen.destinationPath('README.md'),
    {
      botname: gen.props.botname,
      description: gen.props.description
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
    }
  );
}

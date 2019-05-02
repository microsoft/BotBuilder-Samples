// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const path = require('path');
const _ = require('lodash');
const mkdirp = require('mkdirp');

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
  const botname = gen.props.botname;
  const extension = _.toLower(gen.props.language) === 'javascript' ? 'js' : 'ts';
  const npmMain = extension === 'js' ? `index.js` : `./lib/index.js`;


  // ensure our project directory exists before we start writing files into it
  makeProjectDirectory(gen, _.kebabCase(gen.props.botname));

  // write the project files common to all templates
  // do any text token processing where required
  gen.fs.copyTpl(
    gen.templatePath(path.join(templatePath, 'package.json.' + extension)),
    gen.destinationPath('package.json'),
    {
      botname: gen.props.botname,
      botDescription: gen.props.description,
      version: pkg.version,
      npmMain: npmMain
    }
  );
  gen.fs.copy(
    gen.templatePath(path.join(templatePath, '_gitignore')),
    gen.destinationPath('.gitignore')
  );

  // gen a .env file that points to the botfile
  gen.fs.copyTpl(
    gen.templatePath(path.join(templatePath, '_env')),
    gen.destinationPath('.env'),
    {
      botFileName: gen.props.botname
    }
  );

  // determine what language we are working in, TypeScript or JavaScript
  // and write language specific files now
  if (extension === 'ts') {
    gen.fs.copy(
      gen.templatePath(path.join(templatePath, 'tsconfig.json')),
      gen.destinationPath('tsconfig.json')
    );
    gen.fs.copy(
      gen.templatePath(path.join(templatePath, 'tslint.json')),
      gen.destinationPath('tslint.json')
    );
    srcReadmePath = path.join(templatePath, 'README.md.ts')
  } else {
    gen.fs.copy(
      gen.templatePath(path.join(templatePath, '_eslintrc.js')),
      gen.destinationPath('.eslintrc.js')
    );
    srcReadmePath = path.join(templatePath, 'README.md.js')
  }

  // gen a readme with specifics to what was generated
  gen.fs.copyTpl(
    gen.templatePath(srcReadmePath),
    gen.destinationPath('README.md'),
    {
      botname: gen.props.botname,
      description: gen.props.description
    }
  );

  // gen the deployment/Templates folder
  const deploymentFolder = 'deploymentTemplates';
  const deploymentFiles = [
    'template-with-new-rg.json',
    'template-with-preexisting-rg.json',
  ];
  mkdirp.sync(deploymentFolder);
  const sourcePath = path.join(templatePath, deploymentFolder);
  const destinationPath = path.join(gen.destinationPath(), deploymentFolder);
  for(let cnt = 0; cnt < deploymentFiles.length; ++cnt) {
    gen.fs.copy(
      path.join(sourcePath, deploymentFiles[cnt]),
      path.join(destinationPath, deploymentFiles[cnt]),
    );
  }
}

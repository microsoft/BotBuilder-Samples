// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const path = require('path');
const _ = require('lodash');
const mkdirp = require('mkdirp');

const pkg = require('../package.json');

/**
 * Create the folder for the generated bot code, if it doesn't exist
 *
 * @param {Generator} generator Yeoman's generator object
 * @param {String} directoryName root folder for the generated code
 */
const makeProjectDirectory = (generator, directoryName) => {
  generator.log(path.basename(generator.destinationPath()));
  generator.log(directoryName);
  if (path.basename(generator.destinationPath()) !== directoryName) {
    generator.log(`Your bot should be in a directory named ${directoryName}\nI'll automatically create this folder.`);
    mkdirp.sync(directoryName);
    generator.destinationRoot(generator.destinationPath(directoryName));
  }
}

/**
 * Based on the template, write the files common across the given template options
 *
 * @param {Generator} generator Yeoman's generator object
 * @param {String} templatePath file path to write the generated code
 */
// const writeCommonFiles = (generator, templatePath) => {
module.exports.commonFilesWriter = (generator, templatePath) => {
  const botname = generator.templateConfig.botname;
  const extension = _.toLower(generator.templateConfig.language) === 'javascript' ? 'js' : 'ts';
  const npmMain = extension === 'js' ? `index.js` : `./lib/index.js`;


  // ensure our project directory exists before we start writing files into it
  makeProjectDirectory(generator, _.kebabCase(generator.templateConfig.botname));

  // write the project files common to all templates
  // do any text token processing where required
  generator.fs.copyTpl(
    generator.templatePath(path.join(templatePath, 'package.json.' + extension)),
    generator.destinationPath('package.json'),
    {
      botname: generator.templateConfig.botname,
      botDescription: generator.templateConfig.description,
      version: pkg.version,
      npmMain: npmMain
    }
  );
  generator.fs.copy(
    generator.templatePath(path.join(templatePath, '_gitignore')),
    generator.destinationPath('.gitignore')
  );

  // gen a .env file that points to the botfile
  generator.fs.copyTpl(
    generator.templatePath(path.join(templatePath, '_env')),
    generator.destinationPath('.env'),
    {
      botFileName: generator.templateConfig.botname
    }
  );

  // determine what language we are working in, TypeScript or JavaScript
  // and write language specific files now
  if (extension === 'ts') {
    generator.fs.copy(
      generator.templatePath(path.join(templatePath, 'tsconfig.json')),
      generator.destinationPath('tsconfig.json')
    );
    generator.fs.copy(
      generator.templatePath(path.join(templatePath, 'tslint.json')),
      generator.destinationPath('tslint.json')
    );
    srcReadmePath = path.join(templatePath, 'README.md.ts')
  } else {
    generator.fs.copy(
      generator.templatePath(path.join(templatePath, '_eslintrc.js')),
      generator.destinationPath('.eslintrc.js')
    );
    srcReadmePath = path.join(templatePath, 'README.md.js')
  }

  // gen a readme with specifics to what was generated
  generator.fs.copyTpl(
    generator.templatePath(srcReadmePath),
    generator.destinationPath('README.md'),
    {
      botname: generator.templateConfig.botname,
      description: generator.templateConfig.description
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
  const destinationPath = path.join(generator.destinationPath(), deploymentFolder);
  for(let cnt = 0; cnt < deploymentFiles.length; ++cnt) {
    generator.fs.copy(
      path.join(sourcePath, deploymentFiles[cnt]),
      path.join(destinationPath, deploymentFiles[cnt]),
    );
  }
}

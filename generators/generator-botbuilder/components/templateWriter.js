// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const path = require("path");
const _ = require("lodash");
const mkdirp = require("mkdirp");

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
const writeCommonFiles = (gen, templatePath) => {
    const botName = gen.props.botName;
    const extension = _.toLower(gen.props.language) === "javascript" ? "js" : "ts";
    const npmMain = extension === 'js' ? `index.js` : `./lib/index.js`;
    const npmBuildCmd = extension === 'js' ? `echo "Error: no build specified" && exit 1` : `tsc`;
    const npmRunCmd = extension === 'js' ? `node ./index.js` : `tsc && node ./lib/index.js`;
    const npmWatchCmd = extension === 'js' ? `nodemon ./index.js` : `tsc && node ./lib/index.js`;

    // ensure our project directory exists before we start writing files into it
    makeProjectDirectory(gen, _.camelCase(gen.props.botName));

    // write the project files common to all templates
    // do any text token processing where required
    gen.fs.copyTpl(
        gen.templatePath(templatePath + "package.json"),
        gen.destinationPath("package.json"),
        {
            botName: gen.props.botName,
            botDescription: gen.props.description,
            npmMain: npmMain,
            npmBuildCmd: npmBuildCmd,
            npmRunCmd: npmRunCmd,
            npmWatchCmd: npmWatchCmd
        }
    );
    gen.fs.copy(gen.templatePath(templatePath + '_gitignore'), gen.destinationPath('.gitignore'));

    gen.fs.copy(gen.templatePath(templatePath + `botName.bot`), gen.destinationPath(`${gen.props.botName}.bot`), {
        process: function (content) {
            var pattern = new RegExp('<%= botName %>', 'g');
            return content.toString().replace(pattern, botName.toString());
        }
    });

    // gen a .env file that points to the botfile
    gen.fs.copyTpl(
        gen.templatePath(templatePath + '_env'),
        gen.destinationPath('.env'),
        {
            botFileName: gen.props.botName
        }
    );

    // determine what language we are working in, TypeScript or JavaScript
    // and write language specific files now
    if (extension === 'ts') {
        gen.fs.copy(gen.templatePath(templatePath + 'tsconfig.json'), gen.destinationPath('tsconfig.json'));
    } else {
        gen.fs.copy(gen.templatePath(templatePath + '_eslintrc.js'), gen.destinationPath('.eslintrc.js'));
    }

    // gen a readme with specifics to what was generated
    gen.fs.copyTpl(
        gen.templatePath(templatePath + "README.md"),
        gen.destinationPath("README.md"),
        {
            botName: gen.props.botName,
            description: gen.props.description,
            runCmd: npmRunCmd,
            watchCmd: npmWatchCmd,
            extension: extension
        }
    );
}

/**
 * Write the files that are specific to the basic bot template
 *
 * @param {Generator} gen Yeoman's generator object
 * @param {String} templatePath file path to write the generated code
 */
const writeBasicTemplateFiles = (gen, templatePath) => {
    const COGNITIVE_MODELS = 0;
    const DEPLOYMENT_SCRIPTS = 1;
    const DEPLOYMENT_MSBOT = 2;
    const DIALOGS_GREETING = 3;
    const DIALOGS_GREETING_RESOURCES = 4;
    const DIALOGS_WELCOME = 5;
    const DIALOGS_WELCOME_RESOURCES = 6;
    const TS_SRC_FOLDER = "src/";
    const folders = [
        'cognitiveModels/',
        'deploymentScripts/',
        'deploymentScripts/msbotClone/',
        'dialogs/greeting/',
        'dialogs/greeting/resources/',
        'dialogs/welcome/',
        'dialogs/welcome/resources/',
    ];
    const extension = _.toLower(gen.props.language) === "javascript" ? "js" : "ts";
    const srcFolder = _.toLower(gen.props.language) === "javascript" ? "" : TS_SRC_FOLDER;

    // create the basic bot folder structure
    for(let cnt = 0; cnt < folders.length; ++cnt) {
        mkdirp.sync(folders[cnt]);
    }
    // write out the LUIS model
    let sourcePath = templatePath + folders[COGNITIVE_MODELS];
    let destinationPath = gen.destinationPath() + '/' + folders[COGNITIVE_MODELS];
    gen.fs.copy(sourcePath + 'basicBot.luis',  destinationPath + 'basicBot.luis');

    // write out the deployment msbot recipe and docs
    sourcePath = templatePath + folders[DEPLOYMENT_SCRIPTS];
    destinationPath = gen.destinationPath() + '/' + folders[DEPLOYMENT_SCRIPTS];
    gen.fs.copy(sourcePath + 'DEPLOYMENT.md', destinationPath + 'DEPLOYMENT.md', {
        process: function (content) {
            var pattern = new RegExp('<%= botName %>', 'g');
            return content.toString().replace(pattern, gen.props.botName.toString());
        }
    });
    // write out deployment resources
    sourcePath = templatePath + folders[DEPLOYMENT_MSBOT];
    destinationPath = gen.destinationPath() + '/' + folders[DEPLOYMENT_MSBOT];
    gen.fs.copy(sourcePath + '34.luis', destinationPath + '34.luis');
    gen.fs.copy(sourcePath + 'bot.recipe', destinationPath + 'bot.recipe');

    // write out the greeting dialog
    sourcePath = templatePath + folders[DIALOGS_GREETING];
    destinationPath = gen.destinationPath() + '/' + srcFolder + folders[DIALOGS_GREETING];
    gen.fs.copyTpl(
        sourcePath + `greeting.${extension}`,
        destinationPath + `greeting.${extension}`,
        {
            botName: gen.props.botName
        }
    );
    gen.fs.copy(sourcePath + `index.${extension}`, destinationPath + `index.${extension}`);
    gen.fs.copy(sourcePath + `userProfile.${extension}`, destinationPath + `userProfile.${extension}`);

    // list the greeting dialog resources
    const greetingResources = [
        'cancel.lu',
        'greeting.chat',
        'greeting.lu',
        'help.lu',
        'main.lu',
        'none.lu',
    ];
    // write out greeting dialog resources
    sourcePath = templatePath + folders[DIALOGS_GREETING_RESOURCES];
    destinationPath = gen.destinationPath() + '/' + folders[DIALOGS_GREETING_RESOURCES];
    for (let cnt = 0; cnt < greetingResources.length; cnt++) {
        gen.fs.copy(sourcePath + greetingResources[cnt], destinationPath + greetingResources[cnt]);
    }

    // write out welcome named exports
    sourcePath = templatePath + folders[DIALOGS_WELCOME];
    destinationPath = gen.destinationPath() + '/' + srcFolder + folders[DIALOGS_WELCOME];
    gen.fs.copy(sourcePath + `index.${extension}`, destinationPath + `index.${extension}`);

    // write out welcome adaptive card
    sourcePath = templatePath + folders[DIALOGS_WELCOME_RESOURCES];
    destinationPath = gen.destinationPath() + '/' + folders[DIALOGS_WELCOME_RESOURCES];
    gen.fs.copy(sourcePath + 'welcomeCard.json', destinationPath + 'welcomeCard.json');

    // write out the index.js and bot.js
    destinationPath = gen.destinationPath() + '/' + srcFolder;

    // gen index and main dialog files
    gen.fs.copyTpl(
        gen.templatePath(templatePath + `index.${extension}`),
        destinationPath + `index.${extension}`,
        {
            botName: gen.props.botName
        }
    );

    // gen the main dialog file
    gen.fs.copy(gen.templatePath(templatePath + `bot.${extension}`), destinationPath + `bot.${extension}`);
}

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
    for(let cnt = 0; cnt < folders.length; ++cnt) {
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
module.exports.writeEchoProjectFiles = gen => {
    // do some simple sanity checking to ensure we're being
    // called correctly
    if (_.toLower(gen.props.template) !== "echo") {
        throw new Error(`writeEchoProjectFiles called for wrong template: ${gen.props.template}`)
    }
    const templatePath = gen.templatePath() + "/echo/";

    // write files common to all our template options
    writeCommonFiles(gen, templatePath);

    // write files specific to the echo bot template
    writeEchoTemplateFiles(gen, templatePath);
}

/**
 * Write project files for Basic template
 *
 * @param {Generator} gen Yeoman's generator object
 */
module.exports.writeBasicProjectFiles = gen => {
    // do some simple sanity checking to ensure we're being
    // called correctly
    if (_.toLower(gen.props.template) !== "basic") {
        throw new Error(`writeBasicProjectFiles called for wrong template: ${gen.props.template}`)
    }
    const templatePath = gen.templatePath() + "/basic/";

    // write files common to all our template options
    writeCommonFiles(gen, templatePath);

    // write files specific to the basic bot template
    writeBasicTemplateFiles(gen, templatePath);
}
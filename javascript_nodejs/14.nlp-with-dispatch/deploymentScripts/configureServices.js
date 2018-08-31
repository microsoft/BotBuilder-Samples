// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
const exec = require('child_process').exec;
const execSync = require('child_process').execSync;

const path = require('path');
// Import required bot confuguration.
const { BotConfiguration, ServiceTypes } = require('botframework-config');
const CONFIG_ERROR = 1;
// Read botFilePath and botFileSecret from .env file
// Note: Ensure you have a .env file and include botFilePath and botFileSecret.
const ENV_FILE = path.join(__dirname, '..', '.env');
const env = require('dotenv').config({ path: ENV_FILE });
const BOT_FILE = path.join(__dirname, '..', (process.env.botFilePath || ''));
// read bot configuration from .bot file. 
let botConfig;
try {
    botConfig = BotConfiguration.loadSync(BOT_FILE, process.env.botFileSecret);
} catch (err) {
    console.log(`Error reading bot file. Please ensure you have valid botFilePath and botFileSecret set for your environment`);
    process.exit(CONFIG_ERROR);
}

async function main(botConfig) {
    // see if we have a LUIS key, region and QnA Maker key in the .bot file if not, get from args
    let LUISAuthoringKey, LUISRegion, QnASubscriptionKey;
    const LUISModelInBotFile = botConfig.services.find(service => service.type === ServiceTypes.Luis);
    if (!LUISModelInBotFile) {
        // see if we have these passed in through args.
        if (process.argv.length < 4) {
            console.log(`Missing LUIS authoring key and region.`);
            console.log(`Try npm run configure <YOUR-LUIS-AUTHORING-KEY> <YOUR-LUIS-AUTHORING-REGION> <YOUR-QNA-MAKER-SUBSCRIPTION-KEY>`);
            process.exit();
        } else {
            LUISAuthoringKey = process.argv[2];
            LUISRegion = process.argv[3];
        }
    } else {
        LUISAuthoringKey = LUISModelInBotFile.authoringKey;
        LUISRegion = LUISModelInBotFile.region;
    }
    const QnAModelInBotFile = botConfig.services.find(service => service.type == ServiceTypes.QnA);
    if(!QnAModelInBotFile) {
        // see if we have these passed in through args.
        if (process.argv.length < 5) {
            console.log(`Missing QnA Maker subscription key`);
            console.log(`Try npm run configure <YOUR-LUIS-AUTHORING-KEY> <YOUR-LUIS-AUTHORING-REGION> <YOUR-QNA-MAKER-SUBSCRIPTION-KEY>`);
            process.exit();
        } else {
            QnASubscriptionKey = process.argv[4];
        }
    } else {
        QnASubscriptionKey = QnAModelInBotFile.subscriptionKey;
    }
    console.log('LUIS Authoring key: ' + LUISAuthoringKey);
    console.log('LUIS Region: ' + LUISRegion);
    console.log('QnA Maker subscription key: ' + QnASubscriptionKey);

    // parse lu files and generate home automation models
    const PATH_TO_HOME_AUTOMATION_LU = path.join(__dirname, '..', 'dialogs/homeAutomation/resources');
    const COGNITIVE_MODELS_PATH = path.join(__dirname, '..', 'cognitiveModels');
    const HOME_AUTOMATION_MODEL = 'homeautomation.luis';
    const HOME_AUTOMATION_LU_COMMAND = `ludown parse toluis -l ${PATH_TO_HOME_AUTOMATION_LU} -o ${COGNITIVE_MODELS_PATH} -n ${HOME_AUTOMATION_MODEL}`;
    exec(HOME_AUTOMATION_LU_COMMAND, (err, stdout, stderr) => {
        if (!err) {
            console.log('Successfully generated home automation luis model from .lu files...');
            // create/ update LUIS model
            createOrUpdateLUISModel(HOME_AUTOMATION_MODEL, LUISAuthoringKey, LUISRegion, COGNITIVE_MODELS_PATH, botConfig);
        }
        if (stdout) console.log('stdout:' + stdout);
        if (stderr) throw (`Error creating home automation LUIS model!\n` + stderr);
    });

    // parse lu files and generate weather models
    const PATH_TO_WEATHER_LU = path.join(__dirname, '..', 'dialogs/weather/resources');
    const WEATHER_MODEL = 'weather.luis';
    const WEATHER_LU_COMMAND = `ludown parse toluis -l ${PATH_TO_WEATHER_LU} -o ${COGNITIVE_MODELS_PATH} -n ${WEATHER_MODEL}`;
    exec(WEATHER_LU_COMMAND, (err, stdout, stderr) => {
        if (!err) {
            console.log('Successfully generated weather luis model from .lu files...');
            // create/ update LUIS model
            createOrUpdateLUISModel(WEATHER_MODEL, LUISAuthoringKey, LUISRegion, COGNITIVE_MODELS_PATH, botConfig);
        }
        if (stdout) console.log('stdout:' + stdout);
        if (stderr) throw (`Error creating weather LUIS model!\n` + stderr);
    });

    // parse lu files and generate QnA models
    const PATH_TO_QNA_LU = path.join(__dirname, '..', 'dialogs/qna/resources');
    const QNA_MODEL = 'sample.qna';
    const QNA_LU_COMMAND = `ludown parse toqna -l ${PATH_TO_QNA_LU} -o ${COGNITIVE_MODELS_PATH} -n ${QNA_MODEL}`;
    exec(QNA_LU_COMMAND, (err, stdout, stderr) => {
        if (!err) {
            console.log('Successfully generated QnA Maker model from .lu files...');
            // create/ update LUIS model
            createOrUpdateQNAModel(QNA_MODEL, QnASubscriptionKey, COGNITIVE_MODELS_PATH, botConfig);
        }
        if (stdout) console.log('stdout:' + stdout);
        if (stderr) throw (`Error creating sample QnA Maker model!\n` + stderr);
    });

}
function createOrUpdateQNAModel(appName, QnASubscriptionKey, COGNITIVE_MODELS_PATH, botConfig) {
    let qnaAppConfig = botConfig.findServiceByNameOrId(appName);
    if (!qnaAppConfig) {
        // see if a qna maker model with this name already exists
        let qnaApp = QnAAppExists(QnASubscriptionKey, appName);
        if(qnaApp) {
            console.log(`QnA Maker KB ${appName} already exists!`);
            updateQnAKB(qnaApp, path.join(COGNITIVE_MODELS_PATH, appName), QnASubscriptionKey, appName);
        } else {
            // create new qna maker kb
            qnaApp = createQnAKB(path.join(COGNITIVE_MODELS_PATH, appName), QnASubscriptionKey, appName);
            console.log(`New QnA Maker KB created for ${appName}`);
        }
        qnaAppConfig = {
            type: ServiceTypes.QnA,
            name: qnaApp.name,
            subscriptionKey: QnASubscriptionKey,
            kbId: qnaApp.kbId,
            hostname: qnaApp.hostname,
            endpointKey: qnaApp.endpointKey
        };
        // update bot file with configuration
        botConfig.connectService(qnaAppConfig);
        // save bot file
        botConfig.saveSync(process.env.botFileSecret);
    } else {
        // update QnA KB
        updateQnAKB(qnaAppConfig, path.join(COGNITIVE_MODELS_PATH, appName), QnASubscriptionKey, appName);
    }
    // train and publish QnA Maker KB
    trainAndPublishQnAKB(qnaAppConfig, QnASubscriptionKey, appName);
}
function QnAAppExists(QnASubscriptionKey, appName) {
    const QNA_LIST_APPLICATIONS_COMMAND = `qnamaker list kbs --subscriptionKey ${QnASubscriptionKey}`;
    const QNA_GET_APPLICATION_COMMAND = `qnamaker get kb --msbot --subscriptionKey ${QnASubscriptionKey}`;
    // get existing applications
    const existingQnAApplications = JSON.parse(execSync(QNA_LIST_APPLICATIONS_COMMAND).toString());
    if (existingQnAApplications) {
        let appExists = existingQnAApplications.knowledgebases.find(app => app.name == appName);
        if (appExists) {
            return JSON.parse(execSync(QNA_GET_APPLICATION_COMMAND + ` --kbId ${appExists.id}`).toString());
        }
    }
    return undefined;
}
function updateQnAKB(qnaConfig, modelFilePath, QnASubscriptionKey, appName) {
    console.log(`Updating QnA Maker application ${appName} ...`);
    const QNA_REPLACE_COMMAND = `qnamaker replace kb --in ${modelFilePath} --subscriptionKey ${QnASubscriptionKey} --kbId ${qnaConfig.kbId} --hostname ${qnaConfig.hostname} --endpointKey ${qnaConfig.endpointKey}`;
    execSync(QNA_REPLACE_COMMAND);
    console.log(`QnA Maker application ${appName} updated ...`);
}
function trainAndPublishQnAKB(qnaConfig, QnASubscriptionKey, appName) {
    console.log(`Publishing QnA Maker application ${appName} ...`);
    const QNA_PUBLISH_COMMAND = `qnamaker publish kb --subscriptionKey ${QnASubscriptionKey} --kbId ${qnaConfig.kbId} --hostname ${qnaConfig.hostname} --endpointKey ${qnaConfig.endpointKey}`;
    execSync(QNA_PUBLISH_COMMAND);
    console.log(`QnA Maker application ${appName} published ...`);
}
function createQnAKB(modelFilePath, QnASubscriptionKey, appName) {
    console.log(`Creating new QnA Maker application ${appName} ...`);
    const QNA_IMPORT_COMMAND = `qnamaker create kb --in ${modelFilePath} --msbot --name ${appName} --subscriptionKey ${QnASubscriptionKey}`;
    return JSON.parse(execSync(QNA_IMPORT_COMMAND).toString());
}
function createOrUpdateLUISModel(appName, LUISAuthoringKey, LUISRegion, COGNITIVE_MODELS_PATH, botConfig) {
    let luisAppConfig = botConfig.findServiceByNameOrId(appName);
    if (!luisAppConfig) {
        // see if a LUIS model with this name already exists
        let luisApp = LUISAppExists(LUISAuthoringKey, LUISRegion, appName);
        if (luisApp) {
            console.log(`LUIS application ${appName} already exists!`);
            // update LUIS model
            updateLUISModel(luisApp, path.join(COGNITIVE_MODELS_PATH, appName), LUISAuthoringKey, LUISRegion, appName);
        } else {
            // create new LUIS model
            luisApp = createLUISModel(path.join(COGNITIVE_MODELS_PATH, appName), LUISAuthoringKey, LUISRegion, appName);
            console.log(`New LUIS application created for ${appName}`);
        }
        luisAppConfig = {
            type: ServiceTypes.Luis,
            name: luisApp.name,
            appId: luisApp.appId,
            authoringKey: LUISAuthoringKey,
            subscriptionKey: LUISAuthoringKey,
            version: luisApp.version,
            region: LUISRegion
        };
        // update bot file with configuration
        botConfig.connectService(luisAppConfig);
        // save bot file
        botConfig.saveSync(process.env.botFileSecret);
    } else {
        // update LUIS model
        updateLUISModel(luisAppConfig, path.join(COGNITIVE_MODELS_PATH, appName), LUISAuthoringKey, LUISRegion, appName);
    }
    // train and publish LUIS model
    trainAndPublishLUISModel(luisAppConfig, LUISAuthoringKey, LUISRegion, appName)
        .then(res => res)
        .catch(err => {throw (err)});
}
function createLUISModel(modelFilePath, authoringKey, region, appName) {
    // create new luis model
    const LUIS_IMPORT_COMMAND = `luis import application --in ${modelFilePath}  --msbot --authoringKey ${authoringKey} --endpointBasePath https://${region}.api.cognitive.microsoft.com/luis/api/v2.0 --appName ${appName}`;
    return JSON.parse(execSync(LUIS_IMPORT_COMMAND).toString());
}
async function trainAndPublishLUISModel(luisConfig, authoringKey, region, appName) {
    const LUIS_TRAIN_COMMAND = `luis train version --appId ${luisConfig.appId} --versionId ${luisConfig.version} --authoringKey ${authoringKey} --endpointBasePath https://${region}.api.cognitive.microsoft.com/luis/api/v2.0`;
    execSync(LUIS_TRAIN_COMMAND);

    const LUIS_GET_TRAIN_STATUS = `luis get status --appId ${luisConfig.appId} --versionId ${luisConfig.version} --authoringKey ${authoringKey} --endpointBasePath https://${region}.api.cognitive.microsoft.com/luis/api/v2.0`;
    let inTraining = true;
    do {
        let result = JSON.parse(execSync(LUIS_GET_TRAIN_STATUS).toString());
        // get completed or up to date items
        let completedItems = result.filter(item => { return (item.details.status == "Success") || (item.details.status == "UpToDate") });
        if (completedItems.length == result.length) {
            inTraining = false;
        } else {
            let failedItems = result.filter(item => { return item.details.status == "Fail" });
            if (failedItems.length !== 0) throw (`Training failed for ${failedItems[0].modelId}: ${failedItems[0].details.failureReason}`);
            process.stderr.write(`Training LUIS app ${appName} - ${completedItems.length}/${result.length} complete.\n`);
            await Delay(1000);
        }
    } while (inTraining);

    const LUIS_PUBLISH_COMMAND = `luis publish version --region ${region} --versionId ${luisConfig.version} --appId ${luisConfig.appId} --authoringKey ${authoringKey} --endpointBasePath https://${region}.api.cognitive.microsoft.com/luis/api/v2.0`;
    execSync(LUIS_PUBLISH_COMMAND);

    console.log(`LUIS model ${appName} published ...`);
}
function updateLUISModel(luisConfig, modelFilePath, authoringKey, region, appName) {
    const currentLUISVersion = luisConfig.version;
    let date = new Date();
    const newLUISVersionId = date.getMonth() + '.' + date.getDate() + '.' + date.getHours() + date.getMinutes();
    // rename current LUIS version
    const LUIS_RENAME_VERSION_COMMAND = `luis rename version --appId ${luisConfig.appId} --versionId ${currentLUISVersion} --newVersionId ${newLUISVersionId} --authoringKey ${authoringKey} --endpointBasePath https://${region}.api.cognitive.microsoft.com/luis/api/v2.0`;
    execSync(LUIS_RENAME_VERSION_COMMAND);

    // import new version
    const LUIS_IMPORT_VERSION_COMMAND = `luis import version --in ${modelFilePath} --appId ${luisConfig.appId} --authoringKey ${authoringKey} --endpointBasePath https://${region}.api.cognitive.microsoft.com/luis/api/v2.0 --versionId ${currentLUISVersion}`;
    execSync(LUIS_IMPORT_VERSION_COMMAND);

    console.log(`LUIS model ${appName} updated ...`);
}
function LUISAppExists(authoringKey, region, appName) {
    const LUIS_LIST_APPLICATIONS_COMMAND = `luis list applications --take 500 --authoringKey ${authoringKey} --endpointBasePath https://${region}.api.cognitive.microsoft.com/luis/api/v2.0`;
    const LUIS_GET_APPLICATION_COMMAND = `luis get application --msbot --authoringKey ${authoringKey} --endpointBasePath https://${region}.api.cognitive.microsoft.com/luis/api/v2.0`;
    // get existing applications
    const existingLUISApplications = JSON.parse(execSync(LUIS_LIST_APPLICATIONS_COMMAND).toString());
    if (existingLUISApplications) {
        let appExists = existingLUISApplications.find(app => app.name == appName);
        if (appExists) {
            return JSON.parse(execSync(LUIS_GET_APPLICATION_COMMAND + ` --appId ${appExists.id}`).toString());
        }
    }
    return undefined;
}

function Delay (delay) {
    return new Promise(function (fulfill) {
        setTimeout(fulfill, delay)
    })
}

main(botConfig).then().catch(err => console.log(err));

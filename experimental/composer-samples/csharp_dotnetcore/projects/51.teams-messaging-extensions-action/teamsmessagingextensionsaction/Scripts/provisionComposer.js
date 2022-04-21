// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
const chalk = require('chalk');
const fs = require('fs-extra');
const msRestNodeAuth = require('@azure/ms-rest-nodeauth');
const argv = require('minimist')(process.argv.slice(2));
const path = require('path');
const rp = require('request-promise');
const { promisify } = require('util');
const { GraphRbacManagementClient } = require('@azure/graph');
const { ApplicationInsightsManagementClient } = require('@azure/arm-appinsights');
const { AzureBotService } = require('@azure/arm-botservice');
const { ResourceManagementClient } = require('@azure/arm-resources');
const readFile = promisify(fs.readFile);
const ora = require('ora');

const logger = (msg) => {
  if (msg.status === BotProjectDeployLoggerType.PROVISION_ERROR) {
    console.log(chalk.red(msg.message));
  } else if (msg.status === BotProjectDeployLoggerType.PROVISION_ERROR_DETAILS) {
    console.log(chalk.white(msg.message));
  } else {
    console.log(chalk.green(msg.message));
  }
};

const usage = () => {
  const options = [
    ['subscriptionId', 'Azure Subscription Id'],
    ['name', 'Project Name'],
    ['appPassword', '16 character password'],
    ['environment', 'Environment name (Defaults to dev)'],
    ['location', 'Azure Region (Defaults to westus)'],
    ['resourceGroup', 'Name of your resource group (Defaults to name-environment)'],
    ['appId', 'Microsoft App ID (Will create if absent)'],
    ['tenantId', 'ID of your tenant if required (will choose first in list by default)'],
    ['createLuisResource', 'Create a LUIS resource? Default true'],
    ['createLuisAuthoringResource', 'Create a LUIS authoring resource? Default true'],
    ['createCosmosDb', 'Create a CosmosDB? Default true'],
    ['createStorage', 'Create a storage account? Default true'],
    ['createAppInsights', 'Create an AppInsights resource? Default true'],
    ['createQnAResource', 'Create a QnA resource? Default true'],
    [
      'customArmTemplate',
      'Path to runtime ARM template. By default it will use an Azure WebApp template. Pass `DeploymentTemplates/function-template-with-preexisting-rg.json` for Azure Functions or your own template for a custom deployment.',
    ],
    ['qnaTemplate', 'Path to qna template. By default it will use `DeploymentTemplates/qna-template.json`'],
  ];

  const instructions = [
    ``,
    chalk.bold('Provision Azure resources for use with Bot Framework Composer bots'),
    `* This script will create a new resource group and the necessary Azure resources needed to operate a Bot Framework bot in the cloud.`,
    `* Use this to create a publishing profile used in Composer's "Publish" toolbar.`,
    ``,
    chalk.bold(`Basic Usage:`),
    chalk.greenBright(`node provisionComposer --subscriptionId=`) +
      chalk.yellow('<Azure Subscription Id>') +
      chalk.greenBright(' --name=') +
      chalk.yellow('<Name for your environment>') +
      chalk.greenBright(' --appPassword=') +
      chalk.yellow('<16 character password>'),
    ``,
    chalk.bold(`All options:`),
    ...options.map((option) => {
      return chalk.greenBright('--' + option[0]) + '\t' + chalk.yellow(option[1]);
    }),
  ];

  console.log(instructions.join('\n'));
};

// check for required parameters
if (Object.keys(argv).length === 0) {
  return usage();
}

if (!argv.name || !argv.subscriptionId || !argv.appPassword) {
  return usage();
}

// Get required fields from the arguments
const subId = argv.subscriptionId;
const name = argv.name.toString();
const appPassword = argv.appPassword;

// Get optional fields from the arguments
const environment = argv.environment || 'dev';
const location = argv.location || 'westus';
const appId = argv.appId; // MicrosoftAppId - generated if left blank

// Get option flags
const createLuisResource = argv.createLuisResource == 'false' ? false : true;
const createLuisAuthoringResource = argv.createLuisAuthoringResource == 'false' ? false : true;
const createCosmosDb = argv.createCosmosDb == 'false' ? false : true;
const createStorage = argv.createStorage == 'false' ? false : true;
const createAppInsights = argv.createAppInsights == 'false' ? false : true;
const createQnAResource = argv.createQnAResource == 'false' ? false : true;
var tenantId = argv.tenantId ? argv.tenantId : '';

const templatePath =
  argv.customArmTemplate || path.join(__dirname, 'DeploymentTemplates', 'template-with-preexisting-rg.json');
const qnaTemplatePath = argv.qnaTemplate || path.join(__dirname, 'DeploymentTemplates', 'qna-template.json');
const resourceGroup = argv.resourceGroup || `${name}-${environment}`;

const BotProjectDeployLoggerType = {
  // Logger Type for Provision
  PROVISION_INFO: 'PROVISION_INFO',
  PROVISION_ERROR: 'PROVISION_ERROR',
  PROVISION_WARNING: 'PROVISION_WARNING',
  PROVISION_SUCCESS: 'PROVISION_SUCCESS',
  PROVISION_ERROR_DETAILS: 'PROVISION_ERROR_DETAILS',
};

/**
 * Create a Bot Framework registration
 * @param {} graphClient
 * @param {*} displayName
 * @param {*} appPassword
 */
const createApp = async (graphClient, displayName, appPassword) => {
  try {
    const createRes = await graphClient.applications.create({
      displayName: displayName,
      passwordCredentials: [
        {
          value: appPassword,
          startDate: new Date(),
          endDate: new Date(new Date().setFullYear(new Date().getFullYear() + 2)),
        },
      ],
      availableToOtherTenants: true,
      replyUrls: ['https://token.botframework.com/.auth/web/redirect'],
    });
    return createRes;
  } catch (err) {
    logger({
      status: BotProjectDeployLoggerType.PROVISION_ERROR,
      message: err.body.message,
    });
    return false;
  }
};

/**
 * Create an Azure resources group
 * @param {} client
 * @param {*} location
 * @param {*} resourceGroupName
 */
const createResourceGroup = async (client, location, resourceGroupName) => {
  logger({
    status: BotProjectDeployLoggerType.PROVISION_INFO,
    message: `> Creating resource group ...`,
  });
  const param = {
    location: location,
  };

  return await client.resourceGroups.createOrUpdate(resourceGroupName, param);
};

/**
 * Format parameters
 * @param {} scope
 */
const pack = (scope) => {
  return {
    value: scope,
  };
};

const unpackObject = (output) => {
  const unpacked = {};
  for (const key in output) {
    const objValue = output[key];
    if (objValue.value) {
      unpacked[key] = objValue.value;
    }
  }
  return unpacked;
};

/**
 * For more information about this api, please refer to this doc: https://docs.microsoft.com/en-us/rest/api/resources/Tenants/List
 * @param {*} accessToken
 */
const getTenantId = async (accessToken) => {
  if (!accessToken) {
    throw new Error(
      'Error: Missing access token. Please provide a non-expired Azure access token. Tokens can be obtained by running az account get-access-token'
    );
  }
  if (!subId) {
    throw new Error(`Error: Missing subscription Id. Please provide a valid Azure subscription id.`);
  }
  try {
    const tenantUrl = `https://management.azure.com/subscriptions/${subId}?api-version=2020-01-01`;
    const options = {
      headers: { Authorization: `Bearer ${accessToken}` },
    };
    const response = await rp.get(tenantUrl, options);
    const jsonRes = JSON.parse(response);
    if (jsonRes.tenantId === undefined) {
      throw new Error(`No tenants found in the account.`);
    }
    return jsonRes.tenantId;
  } catch (err) {
    throw new Error(`Get Tenant Id Failed, details: ${getErrorMesssage(err)}`);
  }
};

/**
 *
 * @param {*} appId the appId of application registration
 * @param {*} appPwd the app password of application registration
 * @param {*} location the locaiton of all resources
 * @param {*} name the name of resource group
 * @param {*} shouldCreateAuthoringResource
 * @param {*} shouldCreateLuisResource
 * @param {*} useAppInsights
 * @param {*} useCosmosDb
 * @param {*} useStorage
 */
const getDeploymentTemplateParam = (
  appId,
  appPwd,
  location,
  name,
  shouldCreateAuthoringResource,
  shouldCreateLuisResource,
  useAppInsights,
  useCosmosDb,
  useStorage
) => {
  return {
    appId: pack(appId),
    appSecret: pack(appPwd),
    appServicePlanLocation: pack(location),
    botId: pack(name),
    shouldCreateAuthoringResource: pack(shouldCreateAuthoringResource),
    shouldCreateLuisResource: pack(shouldCreateLuisResource),
    useAppInsights: pack(useAppInsights),
    useCosmosDb: pack(useCosmosDb),
    useStorage: pack(useStorage),
  };
};

/**
 * Get QnA template param
 */
const getQnaTemplateParam = (location, name) => {
  return {
    appServicePlanLocation: pack(location),
    name: pack(name),
  };
};

/**
 * Validate the qna template and the qna template param
 */
const validateQnADeployment = async (client, resourceGroupName, deployName, templateParam) => {
  logger({
    status: BotProjectDeployLoggerType.PROVISION_INFO,
    message: '> Validating QnA deployment ...',
  });

  const templateFile = await readFile(qnaTemplatePath, { encoding: 'utf-8' });
  const deployParam = {
    properties: {
      template: JSON.parse(templateFile),
      parameters: templateParam,
      mode: 'Incremental',
    },
  };
  return await client.deployments.validate(resourceGroupName, deployName, deployParam);
};

/**
 * Create a QnA resource deployment
 * @param {*} client
 * @param {*} resourceGroupName
 * @param {*} deployName
 * @param {*} templateParam
 */
const createQnADeployment = async (client, resourceGroupName, deployName, templateParam) => {
  const templateFile = await readFile(qnaTemplatePath, { encoding: 'utf-8' });
  const deployParam = {
    properties: {
      template: JSON.parse(templateFile),
      parameters: templateParam,
      mode: 'Incremental',
    },
  };

  return await client.deployments.createOrUpdate(resourceGroupName, deployName, deployParam);
};

/**
 * Validate the deployment using the Azure API
 */
const validateDeployment = async (client, resourceGroupName, deployName, templateParam) => {
  logger({
    status: BotProjectDeployLoggerType.PROVISION_INFO,
    message: '> Validating Azure deployment ...',
  });

  const templateFile = await readFile(templatePath, { encoding: 'utf-8' });
  const deployParam = {
    properties: {
      template: JSON.parse(templateFile),
      parameters: templateParam,
      mode: 'Incremental',
    },
  };
  return await client.deployments.validate(resourceGroupName, deployName, deployParam);
};

/**
 * Using an ARM template, provision a bunch of resources
 */
const createDeployment = async (client, resourceGroupName, deployName, templateParam) => {
  const templateFile = await readFile(templatePath, { encoding: 'utf-8' });
  const deployParam = {
    properties: {
      template: JSON.parse(templateFile),
      parameters: templateParam,
      mode: 'Incremental',
    },
  };

  return await client.deployments.createOrUpdate(resourceGroupName, deployName, deployParam);
};

/**
 * Format the results into the expected shape
 */
const updateDeploymentJsonFile = async (client, resourceGroupName, deployName, appId, appPwd) => {
  const outputs = await client.deployments.get(resourceGroupName, deployName);
  if (outputs && outputs.properties && outputs.properties.outputs) {
    const outputResult = outputs.properties.outputs;
    const applicationResult = {
      MicrosoftAppId: appId,
      MicrosoftAppPassword: appPwd,
    };
    const outputObj = unpackObject(outputResult);

    if (!createAppInsights) {
      delete outputObj.applicationInsights;
    }
    if (!createCosmosDb) {
      delete outputObj.cosmosDb;
    }
    if (!createLuisAuthoringResource && !createLuisResource) {
      delete outputObj.luis;
    }
    if (!createStorage) {
      delete outputObj.blobStorage;
    }
    const result = {};
    Object.assign(result, outputObj, applicationResult);
    return result;
  } else {
    return null;
  }
};

const provisionFailed = (msg) => {
  logger({
    status: BotProjectDeployLoggerType.PROVISION_ERROR,
    message: chalk.bold('** Provision failed **'),
  });
};

const getErrorMesssage = (err) => {
  if (err.body) {
    if (err.body.error) {
      if (err.body.error.details) {
        const details = err.body.error.details;
        let errMsg = '';
        for (let detail of details) {
          errMsg += detail.message;
        }
        return errMsg;
      } else {
        return err.body.error.message;
      }
    } else {
      return JSON.stringify(err.body, null, 2);
    }
  } else {
    return JSON.stringify(err, null, 2);
  }
};

/**
 * Provision a set of Azure resources for use with a bot
 */
const create = async (
  creds,
  subId,
  name,
  location,
  environment,
  appId,
  appPassword,
  createLuisResource = true,
  createLuisAuthoringResource = true,
  createQnAResource = true,
  createCosmosDb = true,
  createStorage = true,
  createAppInsights = true
) => {
  // App insights is a dependency of QnA
  if (createQnAResource) {
    createAppInsights = true;
  }

  const resourceGroupName = resourceGroup;

  // If tenantId is empty string, get tenanId from API
  if (!tenantId) {
    const token = await creds.getToken();
    const accessToken = token.accessToken;
    // the returned access token will almost surely have a tenantId.
    // use this as the default if one isn't specified.
    if (token.tenantId) {
      tenantId = token.tenantId;
      logger({
        status: BotProjectDeployLoggerType.PROVISION_INFO,
        message: `> Using Tenant ID: ${tenantId}`,
      });
    } else {
      tenantId = await getTenantId(accessToken);
    }
  }

  const graphCreds = new msRestNodeAuth.DeviceTokenCredentials(
    creds.clientId,
    tenantId,
    creds.username,
    'graph',
    creds.environment,
    creds.tokenCache
  );
  const graphClient = new GraphRbacManagementClient(graphCreds, tenantId, {
    baseUri: 'https://graph.windows.net',
  });

  // If the appId is not specified, create one
  if (!appId) {
    logger({
      status: BotProjectDeployLoggerType.PROVISION_INFO,
      message: '> Creating App Registration ...',
    });

    // create the app registration
    const appCreated = await createApp(graphClient, name, appPassword);
    if (appCreated === false) {
      return provisionFailed();
    }

    // use the newly created app
    appId = appCreated.appId;
  }

  logger({
    status: BotProjectDeployLoggerType.PROVISION_INFO,
    message: `> Create App Id Success! ID: ${appId}`,
  });

  // timestamp will be used as deployment name
  const timeStamp = new Date().getTime().toString();
  const client = new ResourceManagementClient(creds, subId);

  // Create a resource group to contain the new resources
  try {
    const rpres = await createResourceGroup(client, location, resourceGroupName);
  } catch (err) {
    logger({
      status: BotProjectDeployLoggerType.PROVISION_ERROR,
      message: getErrorMesssage(err),
    });
    return provisionFailed();
  }

  // Caste the parameters into the right format
  const deploymentTemplateParam = getDeploymentTemplateParam(
    appId,
    appPassword,
    location,
    name,
    createLuisAuthoringResource,
    createLuisResource,
    createAppInsights,
    createCosmosDb,
    createStorage
  );

  // Validate the deployment using the Azure API
  const validation = await validateDeployment(client, resourceGroupName, timeStamp, deploymentTemplateParam);

  // Handle validation errors
  if (validation.error) {
    logger({
      status: BotProjectDeployLoggerType.PROVISION_ERROR,
      message: `! Error: ${validation.error.message}`,
    });
    if (validation.error.details) {
      logger({
        status: BotProjectDeployLoggerType.PROVISION_ERROR_DETAILS,
        message: JSON.stringify(validation.error.details, null, 2),
      });
    }
    logger({
      status: BotProjectDeployLoggerType.PROVISION_ERROR,
      message: `+ To delete this resource group, run 'az group delete -g ${resourceGroupName} --no-wait'`,
    });
    return provisionFailed();
  }

  // Create the entire stack of resources inside the new resource group
  // this is controlled by an ARM template identified in templatePath
  logger({
    status: BotProjectDeployLoggerType.PROVISION_INFO,
    message: `> Deploying Azure services (this could take a while)...`,
  });
  const spinner = ora().start();
  try {
    const deployment = await createDeployment(client, resourceGroupName, timeStamp, deploymentTemplateParam);
    // Handle errors
    if (deployment._response.status != 200) {
      spinner.fail();
      logger({
        status: BotProjectDeployLoggerType.PROVISION_ERROR,
        message: `! Template is not valid with provided parameters. Review the log for more information.`,
      });
      logger({
        status: BotProjectDeployLoggerType.PROVISION_ERROR,
        message: `! Error: ${validation.error}`,
      });
      logger({
        status: BotProjectDeployLoggerType.PROVISION_ERROR,
        message: `+ To delete this resource group, run 'az group delete -g ${resourceGroupName} --no-wait'`,
      });
      return provisionFailed();
    }
  } catch (err) {
    spinner.fail();
    logger({
      status: BotProjectDeployLoggerType.PROVISION_ERROR,
      message: getErrorMesssage(err),
    });
    return provisionFailed();
  }

  var qnaResult = null;

  // Create qna resources, the reason why seperate the qna resources from others: https://github.com/Azure/azure-sdk-for-js/issues/10186
  if (createQnAResource) {
    const qnaDeployName = new Date().getTime().toString();
    const qnaDeploymentTemplateParam = getQnaTemplateParam(location, name);
    const qnaValidation = await validateQnADeployment(
      client,
      resourceGroupName,
      qnaDeployName,
      qnaDeploymentTemplateParam
    );
    if (qnaValidation.error) {
      logger({
        status: BotProjectDeployLoggerType.PROVISION_ERROR,
        message: `! Error: ${qnaValidation.error.message}`,
      });
      if (qnaValidation.error.details) {
        logger({
          status: BotProjectDeployLoggerType.PROVISION_ERROR_DETAILS,
          message: JSON.stringify(qnaValidation.error.details, null, 2),
        });
      }
      logger({
        status: BotProjectDeployLoggerType.PROVISION_ERROR,
        message: `+ To delete this resource group, run 'az group delete -g ${resourceGroupName} --no-wait'`,
      });
      return provisionFailed();
    }

    // Create qna deloyment
    logger({
      status: BotProjectDeployLoggerType.PROVISION_INFO,
      message: `> Deploying QnA Resources (this could take a while)...`,
    });
    const spinner = ora().start();
    try {
      const qnaDeployment = await createQnADeployment(
        client,
        resourceGroupName,
        qnaDeployName,
        qnaDeploymentTemplateParam
      );
      // Handle errors
      if (qnaDeployment._response.status != 200) {
        spinner.fail();
        logger({
          status: BotProjectDeployLoggerType.PROVISION_ERROR,
          message: `! QnA Template is not valid with provided parameters. Review the log for more information.`,
        });
        logger({
          status: BotProjectDeployLoggerType.PROVISION_ERROR,
          message: `! Error: ${qnaValidation.error}`,
        });
        logger({
          status: BotProjectDeployLoggerType.PROVISION_ERROR,
          message: `+ To delete this resource group, run 'az group delete -g ${resourceGroupName} --no-wait'`,
        });
        return provisionFailed();
      }
    } catch (err) {
      spinner.fail();
      logger({
        status: BotProjectDeployLoggerType.PROVISION_ERROR,
        message: getErrorMesssage(err),
      });
      return provisionFailed();
    }

    const qnaDeploymentOutput = await client.deployments.get(resourceGroupName, qnaDeployName);
    if (qnaDeploymentOutput && qnaDeploymentOutput.properties && qnaDeploymentOutput.properties.outputs) {
      const qnaOutputResult = qnaDeploymentOutput.properties.outputs;
      qnaResult = unpackObject(qnaOutputResult);
    }
  }

  // If application insights created, update the application insights settings in azure bot service
  if (createAppInsights) {
    logger({
      status: BotProjectDeployLoggerType.PROVISION_INFO,
      message: `> Linking Application Insights settings to Bot Service ...`,
    });

    const appinsightsClient = new ApplicationInsightsManagementClient(creds, subId);
    const appComponents = await appinsightsClient.components.get(resourceGroupName, resourceGroupName);
    const appinsightsId = appComponents.appId;
    const appinsightsInstrumentationKey = appComponents.instrumentationKey;
    const apiKeyOptions = {
      name: `${resourceGroupName}-provision-${timeStamp}`,
      linkedReadProperties: [
        `/subscriptions/${subId}/resourceGroups/${resourceGroupName}/providers/microsoft.insights/components/${resourceGroupName}/api`,
        `/subscriptions/${subId}/resourceGroups/${resourceGroupName}/providers/microsoft.insights/components/${resourceGroupName}/agentconfig`,
      ],
      linkedWriteProperties: [
        `/subscriptions/${subId}/resourceGroups/${resourceGroupName}/providers/microsoft.insights/components/${resourceGroupName}/annotations`,
      ],
    };
    const appinsightsApiKeyResponse = await appinsightsClient.aPIKeys.create(
      resourceGroupName,
      resourceGroupName,
      apiKeyOptions
    );
    const appinsightsApiKey = appinsightsApiKeyResponse.apiKey;

    logger({
      status: BotProjectDeployLoggerType.PROVISION_INFO,
      message: `> AppInsights AppId: ${appinsightsId} ...`,
    });
    logger({
      status: BotProjectDeployLoggerType.PROVISION_INFO,
      message: `> AppInsights InstrumentationKey: ${appinsightsInstrumentationKey} ...`,
    });
    logger({
      status: BotProjectDeployLoggerType.PROVISION_INFO,
      message: `> AppInsights ApiKey: ${appinsightsApiKey} ...`,
    });

    if (appinsightsId && appinsightsInstrumentationKey && appinsightsApiKey) {
      const botServiceClient = new AzureBotService(creds, subId);
      const botCreated = await botServiceClient.bots.get(resourceGroupName, name);
      if (botCreated.properties) {
        botCreated.properties.developerAppInsightKey = appinsightsInstrumentationKey;
        botCreated.properties.developerAppInsightsApiKey = appinsightsApiKey;
        botCreated.properties.developerAppInsightsApplicationId = appinsightsId;
        const botUpdateResult = await botServiceClient.bots.update(resourceGroupName, name, botCreated);

        if (botUpdateResult._response.status != 200) {
          logger({
            status: BotProjectDeployLoggerType.PROVISION_ERROR,
            message: `! Something went wrong while trying to link Application Insights settings to Bot Service Result: ${JSON.stringify(
              botUpdateResult
            )}`,
          });
          throw new Error(`Linking Application Insights Failed.`);
        }
        logger({
          status: BotProjectDeployLoggerType.PROVISION_INFO,
          message: `> Linking Application Insights settings to Bot Service Success!`,
        });
      } else {
        logger({
          status: BotProjectDeployLoggerType.PROVISION_WARNING,
          message: `! The Bot doesn't have a keys properties to update.`,
        });
      }
    }
  }

  spinner.succeed('Success!');

  // Validate that everything was successfully created.
  // Then, update the settings file with information about the new resources
  const updateResult = await updateDeploymentJsonFile(client, resourceGroupName, timeStamp, appId, appPassword);

  // Handle errors
  if (!updateResult) {
    const operations = await client.deploymentOperations.list(resourceGroupName, timeStamp);
    if (operations) {
      const failedOperations = operations.filter(
        (value) => value && value.properties && value.properties.statusMessage.error !== null
      );
      if (failedOperations) {
        failedOperations.forEach((operation) => {
          switch (
            operation &&
            operation.properties &&
            operation.properties.statusMessage.error.code &&
            operation.properties.targetResource
          ) {
            case 'MissingRegistrationForLocation':
              logger({
                status: BotProjectDeployLoggerType.PROVISION_ERROR,
                message: `! Deployment failed for resource of type ${operation.properties.targetResource.resourceType}. This resource is not avaliable in the location provided.`,
              });
              break;
            default:
              logger({
                status: BotProjectDeployLoggerType.PROVISION_ERROR,
                message: `! Deployment failed for resource of type ${operation.properties.targetResource.resourceType}.`,
              });
              logger({
                status: BotProjectDeployLoggerType.PROVISION_ERROR,
                message: `! Code: ${operation.properties.statusMessage.error.code}.`,
              });
              logger({
                status: BotProjectDeployLoggerType.PROVISION_ERROR,
                message: `! Message: ${operation.properties.statusMessage.error.message}.`,
              });
              break;
          }
        });
      }
    } else {
      logger({
        status: BotProjectDeployLoggerType.PROVISION_ERROR,
        message: `! Deployment failed. Please refer to the log file for more information.`,
      });
    }
  }

  // Merge qna outputs with other resources' outputs
  if (createQnAResource) {
    if (qnaResult) {
      Object.assign(updateResult, qnaResult);
    }
  }

  return updateResult;
};

console.log(chalk.bold('Login to Azure:'));
msRestNodeAuth
  .interactiveLogin({ domain: tenantId })
  .then(async (creds) => {
    const createResult = await create(
      creds,
      subId,
      name,
      location,
      environment,
      appId,
      appPassword,
      createLuisResource,
      createLuisAuthoringResource,
      createQnAResource,
      createCosmosDb,
      createStorage,
      createAppInsights
    );

    if (createResult) {
      console.log('');
      console.log(
        chalk.bold(
          `Your Azure hosting environment has been created! Copy paste the following configuration into a new profile in Composer's Publishing tab.`
        )
      );
      console.log('');

      const token = await creds.getToken();
      const profile = {
        accessToken: token.accessToken,
        name: name,
        environment: environment,
        hostname: `${name}-${environment}`,
        luisResource: `${name}-${environment}-luis`,
        settings: createResult,
        runtimeIdentifier: 'win-x64',
        resourceGroup: resourceGroup,
        botName: name,
        region: location,
        subscriptionId: subId,
      };

      console.log(chalk.white(JSON.stringify(profile, null, 2)));

      console.log('');
    }
  })
  .catch((err) => {
    console.error(err);
  });

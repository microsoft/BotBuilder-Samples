# Prerequisites
- If you don't have an Azure subscription, create a [free account](https://portal.azure.com/) before you begin.
- An [Azure subscription](https://learn.microsoft.com/en-us/azure/guides/developer/azure-developer-guide#understanding-accounts-subscriptions-and-billing) to use for your bot.
- An [Azure DevOps project](https://learn.microsoft.com/en-us/devops/what-is-devops) in which to configure the Azure pipeline.
- An [Azure resource group](https://learn.microsoft.com/en-us/azure/azure-resource-manager/management/manage-resource-groups-portal#what-is-a-resource-group) with the following resources provisioned and configured:
    - An [Azure App Service plan](https://learn.microsoft.com/en-us/azure/app-service/overview-hosting-plans) for your web apps. ([Create resource](https://portal.azure.com/#create/Microsoft.AppServicePlanCreate)).
    - An [Azure App Service](https://learn.microsoft.com/en-us/azure/app-service/overview) to deploy your bot. ([Create resource](https://portal.azure.com/#create/Microsoft.WebSite)).
    - An [Azure Bot](https://learn.microsoft.com/en-us/azure/bot-service/abs-quickstart?view=azure-bot-service-4.0&tabs=userassigned) resource. ([Create resource](https://portal.azure.com/#create/Microsoft.AzureBot)).
    - A [Conversational Language Understanding (CLU)](https://learn.microsoft.com/en-us/azure/cognitive-services/language-service/conversational-language-understanding/quickstart?pivots=language-studio) service. ([Create resource](https://portal.azure.com/#create/Microsoft.CognitiveServicesTextAnalytics)).
    - An _optional_ [QnA Maker](https://learn.microsoft.com/en-us/azure/cognitive-services/QnAMaker/quickstarts/create-publish-knowledge-base) resource. ([Create resource](https://portal.azure.com/#create/Microsoft.CognitiveServicesQnAMaker)).

The sample YAML files referenced in this article can be found on GitHub at [microsoft/BotBuilder-Samples](./build/yaml).
| File | Description |
| ------ | ------ |
| [buildAndDeploy.yaml](./build/yaml/buildAndDeploy.yaml) | The main YAML file used when the Azure DevOps pipeline is triggered. It maps Azure DevOps variables (see [Define variables](https://learn.microsoft.com/en-us/azure/devops/pipelines/process/variables?view=azure-devops&tabs=yaml%2Cbatch) for additional information) into YAML [runtime parameters](https://learn.microsoft.com/en-us/azure/devops/pipelines/process/runtime-parameters?view=azure-devops&tabs=script) and then sequentially calls the YAML templates in the templates folder to build and deploy the bot. |
| [templates/installPrerequisites.yaml](./build/yaml/templates/installPrerequisites.yaml) | Installs the tools needed to run the pipeline, like npm, BF CLI, and .NET core. |
| [templates/buildAndDeployModels.yaml](./build/yaml/templates/buildAndDeployModels.yaml) | Builds, trains, and deploys the optional QnA Maker model. This template also creates a **generated** subfolder in your bot project's directory. The **generated** folder is needed by the bot. |
| [templates/buildAndDeployDotNetWebApp.yaml](./build/yaml/templates/buildAndDeployDotNetWebApp.yaml) | Builds the dotnet bot app, prepares the zip package, and deploys it to Azure. It also configures the app settings for the app in Azure once it's deployed. |

# Provision your Azure environment
You first need to provision Azure resources to publish your bot. Take note of the configuration settings so you can later configure the CI/CD pipeline. To make this easier, copy and paste the [Pipeline parameters table](#pipeline-parameters) provided in a separate document and use it as a template.

> **Note**: This is a good time to update the `BotProjectDirectory` and `BotProjectName` settings in your pipeline's parameters table with your `.csproj` name and the relative path (in forward slash format `/`) to its location in your source tree. 

# Configure your CLU prediction resources (first time only)
> **Note**: Because BF CLI is not compatible with CLU, the creation of the resources must be separated from the execution of this pipeline.
> [Here](https://learn.microsoft.com/en-us/azure/cognitive-services/language-service/conversational-language-understanding/how-to/create-project?tabs=language-studio%2CLanguage-Studio) you can find the guide to create a CLU resource.

> **Note**: When creating a new CLU project in the Language Studio, you can use the following [cognitive model](./BasicAssistantCLU/BasicAssistantCLU/CognitiveModels/BasicAssistantCLU.json) from the BasicAssistantCLU sample to import the project instead.

From the [CLU resource](https://language.cognitive.azure.com/), after a project has been created, you must take the following credentials from Language Studio to set them in the [pipeline's parameters table](#pipeline-parameters).
- The `CluDeploymentName` value can be found in the `Deploying a model` blade under `Deployments` inside `Language Studio`.

- The `CluEndpoint` value can be found in the `Deploying a model` blade under `Deployments` inside `Language Studio` by clicking on the `Get prediction URL` option. It can also be found in the `Keys & Endpoint` blade of your Language resource in the Azure Portal. The endpoint should take the format `https://<language-resource-name>.cognitiveservices.azure.com`.

- The `CluProjectName` and `CluEndpointKey` values can be found in your `Project Settings` blade under `Azure Language Resource` inside `Language Studio`.

See [CLU Recognizer](https://github.com/Microsoft/botframework-components/tree/main/packages/Recognizers/ConversationLanguageUnderstanding/dotnet) in Composer package for additional information.

> **Note**: Ensure that you have selected the correct values for each field. Using the wrong values can lead to errors when running the bot.

# Create and configure your CI/CD pipeline
## Configure an Azure Service Connection for your Azure DevOps project


> **Note**: **Important**. Ensure that your user has 'Owner' or 'User Access Administrator' permissions on the Azure Subscription before you try to create the service connection.

> **Note**: The name you use for the resource will be used as `AzureServiceConnection` in the [pipeline's parameters table](#pipeline-parameters).

The following steps will guide you on how to add a Service Connection to an Azure Resource Manager in your DevOps organization. This is a required step to connect your Azure Subscription to the pipelines and deploy resources to Azure.

### Steps

- From your Azure DevOps project, go to `Project settings -> Service Connections -> New Service connection`.

  ![image](https://github.com/microsoft/BotBuilder-Samples/assets/62260472/2de8b21b-faba-4bf7-accf-b5a4704e2760)

- Select `Azure Resource Manager -> Service principal (manual)` and fill out the form. Check the `Grant access permission to all pipelines` option. The name you use for the Subscription Name field will be your `AzureServiceConnection` variable in the pipelines. [More Info](https://docs.microsoft.com/en-us/azure/devops/pipelines/library/connect-to-azure?view=azure-devops#create-an-azure-resource-manager-service-connection-with-an-existing-service-principal).

  ![image](https://github.com/microsoft/BotBuilder-Samples/assets/62260472/7d212645-5db9-40f3-9781-dba57de118c7)

- Click on `Verify and save` when done.

See [Service connections](https://learn.microsoft.com/en-us/azure/devops/pipelines/library/service-endpoints?view=azure-devops&tabs=yaml) in the Azure DevOps documentation for additional information.

## Create the pipeline
In your Azure DevOps project pipelines section, select new pipeline.

Select your repository and source code provider in the first two tabs and scroll down to the bottom of the page and select **Existing Azure Pipelines YAML file**.

![image](https://github.com/microsoft/BotBuilder-Samples/assets/62260472/62bb1732-a3a8-4565-88e2-c120e8e9b5ab)
![image](https://github.com/microsoft/BotBuilder-Samples/assets/62260472/ba2f7da8-e831-4e27-ad27-f95745624b6e)
![image](https://github.com/microsoft/BotBuilder-Samples/assets/62260472/b3c56397-38fa-40bb-9912-332e7285fa63)

Then select the [buildAndDeploy.yaml](./build/yaml/buildAndDeploy.yaml) file.

![image](https://github.com/microsoft/BotBuilder-Samples/assets/62260472/40e01ef2-f055-4894-ab7c-6fd00054bfa9)

In the **Review** tab, select the **Variables** button and create the variables with the values defined in the [pipeline's parameters table](#pipeline-parameters). Make sure you check the **Keep this value** secret option for sensitive parameters.

![image](https://github.com/microsoft/BotBuilder-Samples/assets/62260472/ab16b423-9ccd-47dc-84b6-4701995c1cf0)


Then select **Run**. This saves and executes your pipeline for the fist time, seen below:

![image](https://github.com/microsoft/BotBuilder-Samples/assets/62260472/29bdb1b8-528f-4349-8a50-0134b0bdd5bc)

# Run and test your deployed bot
You can now run and test your bot from the configured channels.

# Additional information

## Pipeline parameters
| Configuration setting | Description | Example |
| ------ | ------ | ------ |
| AzureServiceConnection | The name of the Azure DevOps service connection used to deploy the bot to Azure | *MyCICDBotDeployment* |
| BotName | Your bot's name | *MyCICDBot* |
| BotProjectDirectory | The relative path from the repo root to your bot's *.csproj* file | *BasicAssistantCLU/BasicAssistantCLU* |
| BotProjectName | The name of your bot's *.csproj* file | *BasicAssistantCLU.csproj* |
| BuildYamlDirectory | The relative path from the repo root to your pipeline's *.yaml* files | *build/yaml* |
| CluDeploymentName | The name of the deployment model from the Language service | *BasicAssistantCLUDeployment* |
| CluEndpoint | The CLU endpoint where the Language service project has been deployed | *https://basicassistantclu.cognitiveservices.azure.com* |
| CluEndpointKey | The endpoint key used to access the published project | *11111111-1111-1111-1111-111111111111* |
| CluProjectName | The name of the project created in the Language service resource | *BasicAssistantCLU* |
| MicrosoftAppId | The Azure Active Directory app registration app ID for your bot | *11111111-1111-1111-1111-111111111111* |
| MicrosoftAppPassword | The Azure Active Directory app registration app password for your bot | *YourAppPassword* |
| QnAEndpointKey | The endpoint key used to access /QnA Maker/ (from QnA portal <your_knowledgebase>/Settings/Deployment Details/Authorization: EndpointKey) | *11111111-1111-1111-1111-111111111111* |
| QnAHostName | Your QnA Maker host URL (from QnA portal <your_knowledgebase>/Settings/Deployment Details/Host) | *https://mycicdbotapp-qna-qnahost.azurewebsites.net/qnamaker* |
| QnAKnowledgebaseId | 	Your QnA Maker knowledge base ID (from /QnA portal/ <your_knowledgebase>/Settings/Deployment Details/knowledgebases) | *11111111-1111-1111-1111-111111111111* |
| QnARegion | Your QnA Maker region (from the /Azure portal/ <qna_maker_resource>/Keys and Endpoint/Location) | *westus* |
| QnASubscriptionKey | The QnA Maker subscription key (from the /Azure portal/ <qna_maker_resource>/Keys and Endpoint/KEY 1) | *1234567890abcdef1234567890abcdef* |
| ResourceGroupName | The Azure resource group containing your bot's deployed resources | *MyTestBot-RG* |
| WebAppName | The Azure app service name for your bot | *MyCICDBotApp* |

## Portals
- [Azure portal](https://portal.azure.com/#home) - Access to your bot's deployment and Azure resources.
- [Language Studio portal](https://language.cognitive.azure.com/home) - Most LUIS pipeline parameter values are under <app_name>/Manage/Azure Resources.
- [QnA Maker portal](https://www.qnamaker.ai/) - Most QnA pipeline parameter values are under <your_knowledgebase>/Settings/Deployment Details.

## Submit Feedback or file an issue
1. Go to the [create new issue](https://github.com/microsoft/BotFramework-Composer/issues/new/choose) page for the BotFramework-Composer repository.
2. Select the appropriate option: **Bug, Feature Request, or Other**.
3. Fill out template and submit your issue.
4. Add the **Area: CICD** label to the issue.


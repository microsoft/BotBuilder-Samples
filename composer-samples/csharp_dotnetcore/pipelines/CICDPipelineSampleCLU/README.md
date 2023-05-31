# Prerequisites
- If you don't have an Azure subscription, create a free account before you begin.
- An [Azure subscription](https://learn.microsoft.com/en-us/azure/guides/developer/azure-developer-guide#understanding-accounts-subscriptions-and-billing) to use for your bot.
- An [Azure DevOps project](https://learn.microsoft.com/en-us/devops/what-is-devops) in which to configure the Azure pipeline.
- A git repository with the source for the bot you're trying to deploy.
- An [Azure resource group](https://learn.microsoft.com/en-us/azure/azure-resource-manager/management/manage-resource-groups-portal#what-is-a-resource-group) with the following resources provisioned and configured:
    - An [Azure App Service plan](https://learn.microsoft.com/en-us/azure/app-service/overview-hosting-plans) for your web apps.
    - An [Azure App Service](https://learn.microsoft.com/en-us/azure/app-service/overview) to deploy your bot.
    - An Azure Bot resource.
    - A Conversational Language Understanding (CLU) service.
    - An optional QnA Maker resource.

The sample YAML and PowerShell files referenced in this article can be found on GitHub at [microsoft/BotBuilder-Samples](https://github.com/microsoft/BotBuilder-Samples/tree/main/composer-samples/csharp_dotnetcore/pipelines/CICDPipelineSample/build/yaml).
| File | Description |
| ------ | ------ |
| [buildAndDeploy.yaml](https://github.com/microsoft/BotBuilder-Samples/blob/main/composer-samples/csharp_dotnetcore/pipelines/CICDPipelineSampleCLU/build/yaml/buildAndDeploy.yaml) | The main YAML file used when the Azure DevOps pipeline is triggered. It maps Azure DevOps variables (see [Define variables](https://learn.microsoft.com/en-us/azure/devops/pipelines/process/variables?view=azure-devops&tabs=yaml%2Cbatch) for additional information) into YAML [runtime parameters](https://learn.microsoft.com/en-us/azure/devops/pipelines/process/runtime-parameters?view=azure-devops&tabs=script) and then sequentially calls the YAML templates in the templates folder to build and deploy the bot. |
| [templates/installPrerequisites.yaml](https://github.com/microsoft/BotBuilder-Samples/blob/main/composer-samples/csharp_dotnetcore/pipelines/CICDPipelineSampleCLU/build/yaml/templates/installPrerequisites.yaml) | Installs the tools needed to run the pipeline, like npm, BF CLI, and .NET core. |
| [templates/buildAndDeployModels.yaml](https://github.com/microsoft/BotBuilder-Samples/blob/main/composer-samples/csharp_dotnetcore/pipelines/CICDPipelineSampleCLU/build/yaml/templates/buildAndDeployModels.yaml) | Builds, trains, and deploys the optional QnA Maker model. This template also creates a **generated** subfolder in your bot project's directory. The **generated** folder is needed by the bot. |
| [templates/buildAndDeployDotNetWebApp.yaml](https://github.com/microsoft/BotBuilder-Samples/blob/main/composer-samples/csharp_dotnetcore/pipelines/CICDPipelineSampleCLU/build/yaml/templates/buildAndDeployDotNetWebApp.yaml) | Builds the dotnet bot app, prepares the zip package, and deploys it to Azure. It also configures the app settings for the app in Azure once it's deployed. |

# Provision your Azure environment
You first need to provision Azure resources to publish your bot. Take note of the configuration settings so you can later configure the CI/CD pipeline. To make this easier, copy and paste the [Pipeline parameters table](#pipeline-parameters) provided in a separate document and use it as a template.

# Add YAML files to your bot project source code
Download the sample YAML and PowerShell files provided, and add them to your bot's source code in git under the build folder. Then merge the changes into your main branch.

![yaml-source-github](https://github.com/southworks/BotBuilder-Samples/assets/122501764/c456d2e5-f1a3-4d92-a17c-5e31357989ef)

> Tip
> 
> This is a good time to update the BotProjectDirectory and BotProjectName settings in your Pipeline parameters table with your .csproj name and the relative path to its location in your source tree.

# Create and configure your CI/CD pipeline
## Configure an Azure Service Connection for your Azure DevOps project

> Important
> 
> Ensure that your user has 'Owner' or 'User Access Administrator' permissions on the Azure Subscription before you try to create the service connection.

Your pipeline uses a service connection to interact with the resource group where the Azure resources for your bot are located. Create a service connection and note the name in the Pipeline parameters table under the *AzureServiceConnection* setting.

To configure a Service Connection, go to the **Project Settings** section in Azure DevOps, and select **Service connections**, as seen below:

![image](https://github.com/southworks/BotBuilder-Samples/assets/122501764/0c5b13f7-61b3-4385-8d7c-6acb4bc5e867)

Then select the **New Service Connection** button and create an **Azure Resource Manager** resource:

![image](https://github.com/southworks/BotBuilder-Samples/assets/122501764/6cf4f1ae-712c-47e2-8812-f1903fbd6553)


Select **Service principal (automatic)** and grant it permissions to the Azure resource group where you created your bot resources.

![image](https://github.com/southworks/BotBuilder-Samples/assets/122501764/3a80c251-c41e-4d0b-b9c1-68f602aa201e)

See [Service connections](https://learn.microsoft.com/en-us/azure/devops/pipelines/library/service-endpoints?view=azure-devops&tabs=yaml) in the Azure DevOps documentation for additional information.

## Create the pipeline
In your Azure DevOps project pipelines section, select new pipeline.

Select your source code provider and repository in the first two tabs and in the Configure steps above. Scroll down to the bottom of the page and select **Existing Azure Pipelines YAML file**:

![image](https://github.com/southworks/BotBuilder-Samples/assets/122501764/c2803656-efd8-45d3-b23c-6d12c76d94d7)

Then select the *buildAndDeploy.yaml* file you previously added to the repository:

![image](https://github.com/southworks/BotBuilder-Samples/assets/122501764/2467eb2c-5600-4a49-9f75-e3f63c7829d1)

In the **Review** tab, select the **Variables** button and create the variables with the values defined in the Pipeline parameters table. Make sure you check the **Keep this value** secret option for sensitive parameters.

![image](https://github.com/southworks/BotBuilder-Samples/assets/122501764/ead8241b-8524-4cf7-91c8-df4739d5825a)

Then select **Run**. This saves and executes your pipeline for the fist time, seen below:

![image](https://github.com/southworks/BotBuilder-Samples/assets/122501764/0b03c60e-b421-4726-960e-cc6704c27586)

# Configure your CLU prediction resources (first time only)
Because BF CLI is not compatible with CLU, then the creation of the resources must be separated from the execution of this pipeline.

[Here](https://learn.microsoft.com/en-us/azure/cognitive-services/language-service/conversational-language-understanding/how-to/create-project?tabs=language-studio%2CLanguage-Studio) you can find the guide to create a CLU resource.

After you create the resource, you must take the following credentials from Language Studio to set them in the bot configuration:
- Project Name
- Endpoint
- Endpoint Key
- Deployment Name

To use the CLU recognizer you must first install the /Microsoft.CluRecognizer/ package in the bot project.
Then, you must use the custom option of the recognizer and set the type as Microsoft.CluRecognizer.
Finally, you must add the JSON with the following format:
>{
>
>    "$kind": "Microsoft.CluRecognizer",
>    
>    "projectName": "",
>    
>    "endpoint": "",
>    
>    "endpointKey": "",
>    
>    "deploymentName": ""
>    
>  }

# Run and test your deployed bot
You can now run and test your bot from the configured channels.

# Additional information
> Note
> 
> Azure QnA Maker will be retired on 31 March 2025. Beginning 1 October 2022, you won't be able to create new QnA Maker resources or knowledge bases. A newer version of the question and answering capability is now available as part of Azure Cognitive Service for Language.
> 
> Custom question answering, a feature of Azure Cognitive Service for Language, is the updated version of the QnA Maker service. For more information about question-and-answer support in Composer, see Natural language processing.
> 

## Pipeline parameters
| Configuration setting | Description | Example |
| ------ | ------ | ------ |
| AzureServiceConnection | The name of the Azure DevOps service connection used to deploy the bot to Azure | *MyCICDBotDeployment* |
| BotName | Your bot's name | *MyCICDBot* |
| BotProjectDirectory | The relative path from the repo root to your bot's *.csproj* file | *BasicAssistantCLU/BasicAssistantCLU* |
| BotProjectName | The name of your bot's *.csproj* file | *BasicAssistantCLU.csproj* |
| BuildYamlDirectory | The relative path from the repo root to your pipeline's *.yaml* files | *build/yaml* |
| MicrosoftAppId | The Azure Active Directory app registration app ID for your bot | *11111111-1111-1111-1111-111111111111* |
| MicrosoftAppPassword | The Azure Active Directory app registration app password for your bot | *YourAppPassword* |
| QnAEndpointKey | The endpoint key used to access /QnA Maker/ (from QnA portal <your_knowledgebase>/Settings/Deployment Details/Authorization: EndpointKey) | *11111111-1111-1111-1111-111111111111* |
| QnAHostName | Your QnA Maker host URL (from QnA portal <your_knowledgebase>/Settings/Deployment Details/Host) | *https://mycicdbotapp-qna-qnahost.azurewebsites.net/qnamaker* |
| QnAKnowledgebaseId | 	Your QnA Maker knowledge base ID (from /QnA portal/ <your_knowledgebase>/Settings/Deployment Details/knowledgebases) | *11111111-1111-1111-1111-111111111111* |
| QnARegion | Your QnA Maker region (from the /Azure portal/ <qna_maker_resource>/Keys and Endpoint/Location) | *westus* |
| QnASubscriptionKey | The QnA Maker subscription key (from the /Azure portal/ <qna_maker_resource>/Keys and Endpoint/KEY 1) | *1234567890abcdef1234567890abcdef* |
| ResourceGroupName | The Azure resource group containing your bot's deployed resources | *MyTestBot-RG* |
| WebAppName | The Azure app service name for your bot | *MyCICDBotApp* |

Here's an example of how the pipeline variables would look in Azure DevOps:
![image](https://github.com/southworks/BotBuilder-Samples/assets/122501764/63e856fb-8821-4d23-896b-753e024bb6ad)

## Portals
- [Azure portal](https://portal.azure.com/#home) - Access to your bot's deployment and Azure resources.
- [Language Studio portal](https://language.cognitive.azure.com/home) - Most LUIS pipeline parameter values are under <app_name>/Manage/Azure Resources.
- [QnA Maker portal](https://www.qnamaker.ai/) - Most QnA pipeline parameter values are under <your_knowledgebase>/Settings/Deployment Details.

## Submit Feedback or file an issue
1. Go to the [create new issue](https://github.com/microsoft/BotFramework-Composer/issues/new/choose) page for the BotFramework-Composer repository.
2. Select the appropriate option: **Bug, Feature Request, or Other**.
3. Fill out template and submit your issue.
4. Add the **Area: CICD** label to the issue.


# Event Hubs logger for transcripts
This sample demonstrates how to use [Event Hubs][14], [Stream Analytics][15] and [Power BI][16] to create a real-time dashboard.
![Sample PoweBI Dashboard](images\sample_powerbi_dashboard.PNG)

## Prerequisites
This sample **requires** prerequisites in order to run.
- An Azure account
- An account for Power BI.
- A bot sending transcript data.

### Create a LUIS Application to enable language understanding
LUIS language model setup, training, and application configuration steps can be found [here][7].

### Create a Event Hubs to enable data ingestion
Event Hubs setup steps can be found [here][18]. 

### Create a Stream Analytics job to enable stream processing
Stream Analytics setup steps can be found [here][17].  See below for suggestions on naming.

## To try this sample
- Clone the repository
    ```bash
    git clone https://github.com/microsoft/botbuilder-samples.git
    ```
- In a terminal, navigate to `samples/javascript_nodejs/28.eventhubs-telemetry`
    ```bash
    cd samples/javascript_nodejs/28.eventhubs-telemetry
    ```
- Install modules
    ```bash
    npm install
    ```
- Setup LUIS

    The prerequisite outlined above contain the steps necessary to provision a language understanding model on www.luis.ai.  Refer to _Create a LUIS Application to enable language understanding_ above for directions to setup and configure LUIS.

- Set up a new Stream Analytics Job

  Create a new `Stream Analytics Job` (for example, name the new job **`sa-transcripts-job-demo`**).

  Create a new **output** called **`Transcript-PowerBI`** that points at your Power BI account.  Name the dataset (for example, **`sa-transcript-dataset`**).

  Create a new **input** called **`TranscriptStream`** that points at your Event Hubs deployment.

  Create a new **query** with the following query:

    ```sql
    SELECT
    COUNT(*) as count, channelId as channel, type as msgtype
    INTO
    [Transcript-PowerBI]
    FROM
    TranscriptStream TIMESTAMP by [timestamp]
    GROUP BY
    channelId,
    type,
    SlidingWindow(minute, 5) 
    ```

# Testing the bot using Bot Framework Emulator
[Bot Framework Emulator][5] is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator version 4.3.0 or greater from [here][6]

## Connect to the bot using Bot Framework Emulator
- Launch Bot Framework Emulator
- File -> Open Bot
- Enter a Bot URL of `http://localhost:3978/api/messages`



### Set up the dashboard with the PowerBI Service

   >**NOTE:** The PowerBI datasets won't be visible until data is pushed (from Stream Analytics).

- Log into your PowerBI account

- Within PowerBI, navigate to the `workspace` referenced in Stream Analytics **`output`** configured above.

- Click on **`Datasets`** and verify the data set is created (ie, **`sa-transcripts-job-demo`** from above).

- Click on **`Dashboards`** and create a new Dashboard.

- Click 'Add Tile', 'Custom Streaming Data' and select your dataset (ie, **`sa-transcript-dataset`**).

- Create a **`Clustered Column Chart`** and use the following *settings*:


Setting Name | Value
------------ | ------------
Visualization Type | Clustered column chart
Axis | msgtype
Legend | msgtype
Value | count
Time window: Last | 5 Minutes
Title | Number of Messages by Type
Subtitle | In the past 5 minutes (Sliding window)

- Create a **`Clustered bar chart`** and use the following *settings*:


Setting Name | Value
------------ | -------------
Visualization Type | Clustered bar chart
Axis | channel
Legend | channel
Value | count
Time window: Last | 5 Minutes
Title | Number of Messages by Channel
Subtitle | In the past 5 minutes (Sliding window)



# Deploy the bot to Azure
## Publishing Changes to Azure Bot Service

```bash
# build the TypeScript bot before you publish
npm run build
```

To learn more about deploying a bot to Azure, see [Deploy your bot to Azure][40] for a complete list of deployment instructions.


# Further reading
- [Bot Framework Documentation][20]
- [Bot Basics][32]
- [Azure Bot Service Introduction][21]
- [Azure Bot Service Documentation][22]
- [Deploying Your Bot to Azure][40]
- [Azure CLI][7]
- [Azure Portal][10]
- [Language Understanding using LUIS][11]
- [Add Natural Language Understanding to Your Bot][12]
- [TypeScript][2]
- [Restify][30]
- [dotenv][31]

[1]: https://dev.botframework.com
[2]: https://www.typescriptlang.org
[3]: https://www.typescriptlang.org/#download-links
[4]: https://nodejs.org
[5]: https://github.com/microsoft/botframework-emulator
[6]: https://github.com/Microsoft/BotFramework-Emulator/releases
[7]: https://docs.microsoft.com/cli/azure/?view=azure-cli-latest
[8]: https://docs.microsoft.com/cli/azure/install-azure-cli?view=azure-cli-latest
[10]: https://portal.azure.com
[11]: https://www.luis.ai
[12]: https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-v4-luis?view=azure-bot-service-4.0&tabs=js#configure-your-bot-to-use-your-luis-app
[14]: https://azure.microsoft.com/services/event-hubs/
[15]: https://azure.microsoft.com/services/stream-analytics/
[16]: https://powerbi.microsoft.com/
[17]: https://docs.microsoft.com/azure/stream-analytics/stream-analytics-quick-create-portal
[18]: https://docs.microsoft.com/azure/event-hubs/event-hubs-create
[20]: https://docs.botframework.com
[21]: https://docs.microsoft.com/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0
[22]: https://docs.microsoft.com/azure/bot-service/?view=azure-bot-service-4.0
[30]: https://www.npmjs.com/package/restify
[31]: https://www.npmjs.com/package/dotenv
[32]: https://docs.microsoft.com/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0
[40]: https://aka.ms/azuredeployment

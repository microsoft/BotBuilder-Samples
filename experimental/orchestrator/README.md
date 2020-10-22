# Orchestrator (PREVIEW)

Conversational AI applications today are built using disparate technologies to fulfill language understanding (LU) needs – e.g. [LUIS][1], [QnA Maker][2]. Often, conversational AI applications are also built by assembling different [skills][3] each of which fulfill a specific conversation topic and can be built using different LU technologies. Hence, conversational AI applications typically require LU arbitration/ decision making to route incoming user request to an appropriate skill or to dispatch to a specific sub-component. Orchestration refers to the ability to perform LU arbitration/ decision making for a conversational AI application.  

[Orchestrator][18] is a [transformer][4] based solution that is heavily optimized for conversational AI applications. It is built ground-up to run locally with your bot.

## Supported scenarios
1. **Dispatch**: Orchestrator is a successor to [dispatch][5]. You can use Orchestrator instead of the current [dispatch][5] solution to arbitrate across your [LUIS][1] and [QnA Maker][2] applications. With Orchestrator, you are likely to see  
    - improved classification accuracy
    - higher resilience to data imbalance across your LUIS and QnA Maker authoring data.
    - ability to correctly dispatch from relatively little authoring data.
2. **Recognizer**: You can use Orchestrator as a recognizer with [Adaptive dialogs][6]. 

## Authoring experience

Here's the end to end authoring experience.

<p align="center">
  <img width="350" src="./docs/media/authoring.png" />
</p>

### Using [BF CLI][7]

* Pre-requisite: Install [BF CLI Orchestrator plugin][11] first. 

1. [Export][8] your LUIS application and [convert][9] to .lu format or [export][10] your QnA Maker KB to .qna format 

2. Use `bf orchestrator:basemodel:get` to [download][15] Orchestrator base model.
<!-- TODO: missing docs for most CLI commands --> 
3. Use [`bf orchestrator:create`][16] or [`bf orchestrator:build`][17] to generate the snapshot content.

## Runtime integration

1. For use in dispatch scenario, you can create `OrchestratorRecognizer` and provide it the path to the model as well the snapshot. Use the `RecognizeAsync` (C#), `recognizeAsync` (JS) method to have Orchestrator recognize user input. 

**C#:**

- Add reference to `Microsoft.Bot.Builder.AI.Orchestrator` package.
- Set your project to target `x64` platform
- Install latest supported version of [Visual C++](https://support.microsoft.com/en-gb/help/2977003/the-latest-supported-visual-c-downloads)


```C# 
using Microsoft.Bot.Builder.AI.Orchestrator;

// Get Model and Snapshot path.
string modelPath = Path.GetFullPath(OrchestratorConfig.ModelPath);
string snapshotPath = Path.GetFullPath(OrchestratorConfig.SnapshotPath);

// Create OrchestratorRecognizer.
OrchestratorRecognizer orc = new OrchestratorRecognizer()
{
    ModelPath = modelPath,
    SnapshotPath = snapshotPath
};

// Recognize user input.
var recoResult = await orc.RecognizeAsync(turnContext, cancellationToken);
```

**JS:**

- Add `botbuilder-ai-orchestrator` package to your bot

```JS
const { OrchestratorRecognizer } = require('botbuilder-ai-orchestrator');

// Create OrchestratorRecognizer.
const dispatchRecognizer = new OrchestratorRecognizer().configure({
            modelPath: process.env.ModelPath, 
            snapshotPath: process.env.SnapShotPath
});
// To recognize user input
const recoResult = await dispatchRecognizer.recognize(context);
```

2. For use in adaptive dialogs, set the `recognizer` to `OrchestratorAdaptiveRecognizer`

**C#:**
- Add reference to `Microsoft.Bot.Builder.AI.Orchestrator` package.
- Set your project to target `x64` platform
- Install latest supported version of [Visual C++](https://support.microsoft.com/en-gb/help/2977003/the-latest-supported-visual-c-downloads)

```C#
using Microsoft.Bot.Builder.AI.Orchestrator;

// Get Model and Snapshot path.
string modelPath = Path.GetFullPath(OrchestratorConfig.ModelPath);
string snapshotPath = Path.GetFullPath(OrchestratorConfig.SnapshotPath);

// Create adaptive dialog
const myDialog = new AdaptiveDialog()
{
    // Set Recognizer to OrchestratorAdaptiveRecognizer.
    Recognizer = new OrchestratorAdaptiveRecognizer()
    {
        ModelPath = modelPath,
        SnapshotPath = snapshotPath
    }
}
```

**JS:**

- Add `botbuilder-ai-orchestrator` package to your bot.

```JS
const { OrchestratorAdaptiveRecognizer } = require('botbuilder-ai-orchestrator');

// Create adaptive dialog.
const myDialog = new AdaptiveDialog('myDialog').configure({
    // Set recognizer to OrchestratorAdaptiveRecognizer.
    recognizer: new OrchestratorAdaptiveRecognizer().configure(
    {
        modelPath: new StringExpression(process.env.ModelPath),
        snapshotPath: new StringExpression(process.env.RootDialogSnapshotPath),
    });
})
```

## Additional reading
- [Tech overview][18]
- [API reference][14]
- [Roadmap](./docs/Overview.md#Roadmap)
- [BF CLI Orchestrator plugin][11]
- [C# samples][12]
- [NodeJS samples][13]

[1]:https://luis.ai
[2]:https://qnamaker.ai
[3]:https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-skills-overview?view=azure-bot-service-4.0
[4]:https://en.wikipedia.org/wiki/Transformer_(machine_learning_model)
[5]:https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-tutorial-dispatch?view=azure-bot-service-4.0&tabs=cs
[6]:https://aka.ms/adaptive-dialogs
[7]:https://github.com/microsoft/botframework-cli
[8]:https://github.com/microsoft/botframework-cli/tree/master/packages/luis#bf-luisversionexport
[9]:https://github.com/microsoft/botframework-cli/tree/master/packages/luis#bf-luisconvert
[10]:https://github.com/microsoft/botframework-cli/tree/master/packages/qnamaker#bf-qnamakerkbexport
[11]:https://github.com/microsoft/botframework-cli/tree/beta/packages/orchestrator
[12]:./csharp_dotnetcore
[13]:./javascript_nodejs
[14]:./docs/API_reference.md
[15]:TBD
[16]:https://github.com/microsoft/botframework-cli/tree/beta/packages/orchestrator#bf-orchestratorcreate
[17]:TBD
[18]:./docs/Overview.md

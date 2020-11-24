# Orchestrator (PREVIEW)

Conversational AI applications today are built using disparate technologies to fulfill language understanding (LU) needs e.g. [LUIS][1], [QnA Maker][2]. Often, conversational AI applications are also built by assembling different [skills][3] each of which fulfill a specific conversation topic and can be built using different LU technologies. Hence, conversational AI applications typically require LU arbitration/ decision making to route incoming user request to an appropriate skill or to dispatch to a specific sub-component. Orchestration refers to the ability to perform LU arbitration/ decision making for a conversational AI application.  

[Orchestrator][18] is a [transformer][4] based solution that is optimized for conversational AI applications. It is built ground-up to run locally with your bot.

## Scenarios
**Dispatch**: Orchestrator is a successor to [dispatch][5]. You can use Orchestrator instead of the current dispatch solution to arbitrate across your [LUIS][1] and [QnA Maker][2] applications. With Orchestrator, you are likely to see:

- Improved classification accuracy
- Higher resilience to data imbalance across your LUIS and QnA Maker authoring data.
- Ability to correctly dispatch from relatively little authoring data.

**Intent Recognizer**: You can use Orchestrator as an intent recognizer with [Adaptive dialogs][6]. Using the same approach as in the dispatch scenario above in order to route to responses within your bot logic.

**Entity Extraction** is not supported yet.  It is on the planning roadmap to add entity extraction in the future.

## Authoring Experience

Orchestrator can be used in different development environments:

* **Code First**: Orchestrator can be integrated into your code project by replacing LUIS for intent recognition such as for skill delegation or dispatching to subsequent language understanding services.  See [Runtime Integration](#runtime-integration) section for more.
* [Bot Framework Composer][19]: Orchestrator can be selected as a recognizer within Bot Framework Composer. At this point there are limitations to using Orchestrator in Composer primarily around importing of existing models and tuning up recognition performance (* currently available only if the feature flag is enabled with Composer).

Thus, use of [BF command line tool][7] to prepare and optimize the model for your domain is required in most, if not all, use cases.   To illustrate the workflow, here is a sample the end to end authoring experience:

<p align="center">
  <img width="350" src="./docs/media/authoring.png" />
</p>

### Prepare

* Pre-requisite: Install [BF CLI Orchestrator plugin][11] first.

1. Author Intent-utterances example based .lu definition referred to as a *label file* using the Language Understanding practices as described in [Language Understanding][2] for dispatch (e.g. author .lu file or within the [Composer][3] GUI experience). 
   * Alternatively, [export][8] your LUIS application and [convert][9] to .lu format or [export][10] your QnA Maker KB to .qna format.
   * See also the [.lu file format][21] to author a .lu file from scratch. 
2. Download Natural Language Representation ([NLR][20]) base Model (will be referred to as the *basemodel*) using the `bf orchestrator:basemodel:get` command. 
   * See `bf orchestrator:basemodel:list` for alternate models. You may need to experiment with the different models to find which performs best for your language domain.
3. Combine the label file .lu from (1) with the base model from (2) to create a *snapshot* file with a .blu extension.
   * Use [`bf orchestrator:create`][16] to create just a single .blu snapshot file for all Lu/json/qna tsv files for dispatch scenario.

### Validate

* Create another test .lu file similar to (1) with utterances that are similar but are not identical to the ones specified in the example based .lu definition in (1). This is typically variations on end-user utterances. 
* Test quality of utterance to intent recognition. 
* Examine report to ensure that the recognition quality is satisfactory. See more in [Report Interpretation][21].
* If not, adjust the label file in (1) and repeat this cycle.

## Runtime Integration

For use in dispatch scenario, you can create `OrchestratorRecognizer` and provide it the path to the model as well the snapshot. Use the `RecognizeAsync` (C#), `recognizeAsync` (JS) method to have Orchestrator recognize user input. 

**C#:**

- Add reference to `Microsoft.Bot.Builder.AI.Orchestrator` package.
- Set your project to target `x64` platform
- Install latest supported version of [Visual C++ redistributable package](https://support.microsoft.com/en-gb/help/2977003/the-latest-supported-visual-c-downloads)


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



## Composer Integration

\<TBD: This section is FYI in preparation for upcoming Composer functionality. It will be updated once ready.>

Once the feature flag is enabled in Composer, it is possible to specify Orchestrator as a recognizer. For the most basic intent recognition cases, simply specify Orchestrator as the recognizer, and fill in the language data as you would for LUIS. For more advanced scenarios, such as dispatch orchestration, follow the steps above to import and tune up routing quality. 



## Additional Reading

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
[15]: TBD/AvailableIndex
[16]:https://github.com/microsoft/botframework-cli/tree/beta/packages/orchestrator#bf-orchestratorcreate
[17]:TBD/AvailableIndex
[18]:./docs/Overview.md
[19]: https://docs.microsoft.com/en-us/composer/introduction
[20]: https://aka.ms/NLRModels "Natural Language Representation Models"
[21]:https://docs.microsoft.com/en-us/azure/bot-service/file-format/bot-builder-lu-file-format?view=azure-bot-service-4.0	"LU file format"
[22]:https://github.com/microsoft/botframework-sdk/blob/R11/Orchestrator/docs/BFOrchestratorReport.md "Report Interpretation: Temp R11 Branch"

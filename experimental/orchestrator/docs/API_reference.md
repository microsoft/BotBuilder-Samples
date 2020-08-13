# Orchestrator (PREVIEW)

## C#

**OrchestratorRecognizer**
```C#
/// <summary>
/// Class that represents an adaptive Orchestrator recognizer.
/// </summary>
public class OrchestratorRecognizer : IRecognizer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OrchestratorRecognizer"/> class.
    /// </summary>
    [JsonConstructor]
    public OrchestratorRecognizer()
    {
    }

    /// <summary>
    /// Gets or sets the full path to the NLR model to use.
    /// </summary>
    /// <value>
    /// Model path.
    /// </value>
    [JsonProperty("modelPath")]
    public string ModelPath { get; set; }

    /// <summary>
    /// Gets or sets the full path to the snapshot to use.
    /// </summary>
    /// <value>
    /// Snapshot path.
    /// </value>
    [JsonProperty("snapshotPath")]
    public string SnapshotPath { get; set; }

    /// <summary>
    /// Runs an utterance through a recognizer and returns a generic recognizer result.
    /// </summary>
    /// <param name="turnContext">Turn context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Analysis of utterance.</returns>
    Task<RecognizerResult> RecognizeAsync(ITurnContext turnContext, CancellationToken cancellationToken);
}
```

**OrchestratorAdaptiveRecognizer**

```C#
/// <summary>
/// Class that represents an adaptive Orchestrator recognizer.
/// </summary>
public class OrchestratorAdaptiveRecognizer : Recognizer
{
    /// <summary>
    /// The Kind name for this recognizer.
    /// </summary>
    [JsonProperty("$kind")]
    public const string Kind = "Microsoft.OrchestratorRecognizer";

    /// <summary>
    /// Property key in RecognizerResult that holds the full recognition result from Orchestrator core.
    /// </summary>
    public const string ResultProperty = "result";

    /// <summary>
    /// Gets or sets the full path to the NLR model to use.
    /// </summary>
    /// <value>
    /// Model path.
    /// </value>
    [JsonProperty("modelPath")]
    public StringExpression ModelPath { get; set; } = "=settings.orchestrator.modelPath";

    /// <summary>
    /// Gets or sets the full path to the snapshot to use.
    /// </summary>
    /// <value>
    /// Snapshot path.
    /// </value>
    [JsonProperty("snapshotPath")]
    public StringExpression SnapshotPath { get; set; } = "=settings.orchestrator.snapshotPath";

    /// <summary>
    /// Gets or sets the entity recognizers.
    /// </summary>
    /// <value>
    /// The entity recognizers.
    /// </value>
    [JsonProperty("entityRecognizers")]
    public List<EntityRecognizer> EntityRecognizers { get; set; } = new List<EntityRecognizer>();

    /// <summary>
    /// Gets or sets the disambiguation score threshold.
    /// </summary>
    /// <value>
    /// Recognizer returns ChooseIntent (disambiguation) if other intents are classified within this score of the top scoring intent.
    /// </value>
    [JsonProperty("disambiguationScoreThreshold")]
    public NumberExpression DisambiguationScoreThreshold { get; set; } = 0.05F;

    /// <summary>
    /// Gets or sets detect ambiguous intents.
    /// </summary>
    /// <value>
    /// When true, recognizer will look for ambiguous intents (intents with close recognition scores from top scoring intent).
    /// </value>
    [JsonProperty("detectAmbiguousIntents")]
    public BoolExpression DetectAmbiguousIntents { get; set; } = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrchestratorAdaptiveRecognizer"/> class.
    /// </summary>
    /// <param name="modelPath">Path to NLR model.</param>
    /// <param name="snapshotPath">Path to snapshot.</param>
    /// <param name="resolver">Label resolver.</param>
    public OrchestratorAdaptiveRecognizer(string modelPath, string snapshotPath, ILabelResolver resolver = null){}

    /// <summary>
    /// Runs current DialogContext.TurnContext.Activity through a recognizer and returns a generic recognizer result.
    /// </summary>
    /// <param name="dialogContext">Dialog context.</param>
    /// <param name="activity">activity to recognize.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <param name="telemetryProperties">Additional properties to be logged to telemetry with the LuisResult event.</param>
    /// <param name="telemetryMetrics">Additional metrics to be logged to telemetry with the LuisResult event.</param>
    /// <returns>Analysis of utterance.</returns>
    Task<RecognizerResult> RecognizeAsync(DialogContext dialogContext, Activity activity, CancellationToken cancellationToken = default, Dictionary<string, string> telemetryProperties = null, Dictionary<string, double> telemetryMetrics = null) {}
}
```

## NodeJS

**OrchestratorRecognizer**

```JS
export class OrchestratorRecognizer extends Configurable {
    /**
     * Full recognition results are available under this property
     */
    public readonly resultProperty: string = 'result';

    /**
     * Path to the model to load.
     */
    public modelPath: string = null;

    /**
     * Path to the snapshot (.blu file) to load.
     */
    public snapshotPath: string = null;

    /**
     * Returns recognition result. Also sends trace activity with recognition result.
     * @param context Context for the current turn of conversation with the use.
     */
    public async recognize(context: TurnContext): Promise<RecognizerResult> {}
}
```

**OrchestratorAdaptiveRecognizer**

```JS
export class OrchestratorAdaptiveRecognizer extends Recognizer {
    /**
     * Recognizers unique ID.
     */
    public id: string;

    /**
     * Path to the model to load.
     */
    public modelPath: StringExpression = new StringExpression('');

    /**
     * Path to the snapshot (.blu file) to load.
     */
    public snapshotPath: StringExpression = new StringExpression('');

    /**
     * Threshold value to use for ambiguous intent detection. 
     * Any intents that are classified with a score that is within this value from the top scoring intent is determined to be ambiguous.
     */
    public disambiguationScoreThreshold: NumberExpression = new NumberExpression(0.05);

    /**
     * Enable ambiguous intent detection.
     */
    public detectAmbiguousIntents: BoolExpression = new BoolExpression(false);

    /**
     * The entity recognizers.
     */
    public entityRecognizers: EntityRecognizer[] = [];

    /**
     * Intent name if ambiguous intents are detected.
     */
    public readonly chooseIntent: string = 'ChooseIntent';

    /**
     * Property under which ambiguous intents are returned.
     */
    public readonly candidatesCollection: string = 'candidates';

    /**
     * Intent name when no intent matches.
     */
    public readonly noneIntent: string = 'None';

    /**
     * Full recognition results are available under this property
     */
    public readonly resultProperty: string = 'result';

    /**
     * Returns an OrchestratorAdaptiveRecognizer instance.
     * @param modelPath Path to NLR model.
     * @param snapshoPath Path to snapshot.
     * @param resolver Orchestrator resolver to use.
     */
    constructor(modelPath?: string, snapshoPath?: string, resolver?: any){}

    /**
     * Returns a new OrchestratorAdaptiveRecognizer instance.
     * @param dialogContext Context for the current dialog.
     * @param activity Current activity sent from user.
     */
    public async recognize(dialogContext: DialogContext, activity: Activity): Promise<RecognizerResult{}
}
```

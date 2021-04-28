// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Recognizers;
using Newtonsoft.Json;

namespace Microsoft.Bot.Builder.AI.Orchestrator
{
    /// <summary>
    /// Class that represents an adaptive Orchestrator recognizer.
    /// </summary>
    public class OrchestratorRecognizerHelper : IRecognizer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrchestratorRecognizerHelper"/> class.
        /// </summary>
        [JsonConstructor]
        public OrchestratorRecognizerHelper()
        {
        }

        /// <summary>
        /// Gets or sets the id for the recognizer.
        /// </summary>
        /// <value>
        /// The id for the recognizer.
        /// </value>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the folder path to Orchestrator base model to use.
        /// </summary>
        /// <value>
        /// Model folder.
        /// </value>
        [JsonProperty("modelFolder")]
        public string ModelFolder { get; set; }

        /// <summary>
        /// Gets or sets the full path to the snapshot to use.
        /// </summary>
        /// <value>
        /// Snapshot path.
        /// </value>
        [JsonProperty("snapshotFile")]
        public string SnapshotFile { get; set; }

        /// <summary>
        /// Gets or sets an external entity recognizer.
        /// </summary>
        /// <value>
        /// Recognizer.
        /// </value>
        [JsonProperty("externalEntityRecognizers")]
        public Recognizer ExternalEntityRecognizer { get; set; }

        /// <summary>
        /// Gets or sets the disambiguation score threshold.
        /// </summary>
        /// <value>
        /// Recognizer returns ChooseIntent (disambiguation) if other intents are classified within this score of the top scoring intent.
        /// </value>
        [JsonProperty("disambiguationScoreThreshold")]
        public float DisambiguationScoreThreshold { get; set; } = 0.05F;

        /// <summary>
        /// Gets or sets a value indicating whether detect ambiguous intents.
        /// </summary>
        /// <value>
        /// When true, recognizer will look for ambiguous intents (intents with close recognition scores from top scoring intent).
        /// </value>
        [JsonProperty("detectAmbiguousIntents")]
        public bool DetectAmbiguousIntents { get; set; } = false;

        /// <inheritdoc/>
        public async Task<RecognizerResult> RecognizeAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var rec = new OrchestratorRecognizer
            {
                Id = this.Id,
                DetectAmbiguousIntents = this.DetectAmbiguousIntents,
                ModelFolder = this.ModelFolder,
                SnapshotFile = this.SnapshotFile,
                DisambiguationScoreThreshold = this.DisambiguationScoreThreshold,
                ExternalEntityRecognizer = this.ExternalEntityRecognizer,
            };

            var dc = new DialogContext(new DialogSet(), turnContext, new DialogState());
            return await rec.RecognizeAsync(dc, turnContext.Activity, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<T> RecognizeAsync<T>(ITurnContext turnContext, CancellationToken cancellationToken)
            where T : IRecognizerConvert, new()
        {
            var result = await RecognizeAsync(turnContext, cancellationToken).ConfigureAwait(false);
            return ObjectPath.MapValueTo<T>(result);
        }
    }
}

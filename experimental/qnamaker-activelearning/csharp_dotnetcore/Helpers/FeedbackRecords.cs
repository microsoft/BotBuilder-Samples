namespace QnAMakerActiveLearningBot.Helpers
{
    using Newtonsoft.Json;

    /// <summary>
    /// Active learning feedback records
    /// </summary>
    public class FeedbackRecords
    {
        // <summary>
        /// List of feedback records
        /// </summary>
        [JsonProperty("feedbackRecords")]
        public FeedbackRecord[] Records { get; set; }
    }
}

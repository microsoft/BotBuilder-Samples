namespace QnAMakerActiveLearningBot.Helpers
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// Active learning feedback records
    /// </summary>
    public class FeedbackRecordsDTO
    {
        // <summary>
        /// List of feedback records
        /// </summary>
        public IEnumerable<FeedbackRecordDTO> FeedbackRecords { get; set; }
    }
}

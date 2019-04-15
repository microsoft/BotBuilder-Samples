namespace QnAMakerActiveLearningBot.Helpers
{
    /// <summary>
    /// Active learning feedback record
    /// </summary>
    public class FeedbackRecord
    {
        /// <summary>
        /// User id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// User question
        /// </summary>
        public string UserQuestion { get; set; }

        /// <summary>
        /// QnA Id
        /// </summary>
        public int QnaId { get; set; }
    }
}

namespace QnAMakerActiveLearningBot.Helpers
{
    /// <summary>
    /// Active learning feedback class
    /// </summary>
    public class FeedbackRecordDTO
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

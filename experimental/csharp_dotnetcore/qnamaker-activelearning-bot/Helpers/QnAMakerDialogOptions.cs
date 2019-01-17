using QnAMakerActiveLearningBot.QnAMakerServices;

namespace QnAMakerActiveLearningBot.Helpers
{
    /// <summary>
    /// QnA Maker dialog options
    /// </summary>
    public class QnAMakerDialogOptions
    {
        /// <summary>
        /// Max number of answers to be returned for the question.
        /// </summary>
        public int Top { get; set; }

        /// <summary>
        /// Confidence score threshold limit above which the answers are returned
        /// </summary>
        public double Threshold { get; set; }
    }
}

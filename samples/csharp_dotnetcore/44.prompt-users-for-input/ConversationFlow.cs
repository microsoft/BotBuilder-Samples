namespace Microsoft.BotBuilderSamples
{
    public class ConversationFlow
{
    // Identifies the last question asked.
    public enum Question
    {
        Name,
        Age,
        Date,
        None, // Our last action did not involve a question.
    }

    // The last question asked.
    public Question LastQuestionAsked { get; set; } = Question.None;
}
}

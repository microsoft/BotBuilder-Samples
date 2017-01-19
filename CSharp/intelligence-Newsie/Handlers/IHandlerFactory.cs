namespace NewsieBot.Handlers
{
    public interface IHandlerFactory
    {
        IIntentHandler CreateIntentHandler(string key);

        IRegexHandler CreateRegexHandler(string key);
    }
}

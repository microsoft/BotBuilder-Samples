namespace Newsie.Handlers
{
    public interface IHandlerFactory
    {
        IIntentHandler CreateIntentHandler(string key);

        IRegexHandler CreateRegexHandler(string key);
    }
}

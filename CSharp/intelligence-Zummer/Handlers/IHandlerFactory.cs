namespace Zummer.Handlers
{
    public interface IHandlerFactory
    {
        IIntentHandler CreateIntentHandler(string key);

        IRegexHandler CreateRegexHandler(string key);
    }
}

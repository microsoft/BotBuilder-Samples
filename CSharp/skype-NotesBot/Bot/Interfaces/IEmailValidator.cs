namespace Bot.Interfaces
{
    public interface IEmailValidator
    {
        bool OnCheckIsValidEmail(string emailString);
    }
}
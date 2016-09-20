namespace ContosoFlowers.BotAssets
{
    public interface IDialogFactory
    {
        T Create<T>();

        T Create<T, U>(U parameter);
    }
}
namespace ContosoFlowers.BotAssets
{
    using Autofac;
    using Microsoft.Bot.Builder.Internals.Fibers;

    public class DialogFactory : IDialogFactory
    {
        protected readonly IComponentContext Scope;

        public DialogFactory(IComponentContext scope)
        {
            SetField.NotNull(out this.Scope, nameof(scope), scope);
        }

        public T Create<T>()
        {
            return this.Scope.Resolve<T>();
        }

        public T Create<T, U>(U parameter)
        {
            return this.Scope.Resolve<T>(TypedParameter.From(parameter));
        }
    }
}
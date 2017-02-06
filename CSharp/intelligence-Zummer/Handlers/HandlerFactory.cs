using Autofac;
using Microsoft.Bot.Builder.Internals.Fibers;

namespace Zummer.Handlers
{
    internal sealed class HandlerFactory : IHandlerFactory
    {
        private readonly IComponentContext scope;

        public HandlerFactory(IComponentContext scope)
        {
            SetField.NotNull(out this.scope, nameof(scope), scope);
        }

        public IIntentHandler CreateIntentHandler(string key)
        {
            return this.scope.ResolveNamed<IIntentHandler>(key);
        }
    }
}

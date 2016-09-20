namespace ContosoFlowers.Dialogs
{
    using System.Collections.Generic;
    using Autofac;
    using BotAssets;
    using BotAssets.Dialogs;
    using Microsoft.Bot.Builder.Internals.Fibers;

    public class ContosoFlowersDialogFactory : DialogFactory, IContosoFlowersDialogFactory
    {
        public ContosoFlowersDialogFactory(IComponentContext scope)
            : base(scope)
        {
        }

        public SavedAddressDialog CreateSavedAddressDialog(
            string prompt,
            string useSavedAddressPrompt,
            string saveAddressPrompt,
            IDictionary<string, string> savedAddresses,
            IEnumerable<string> saveOptionNames)
        {
            return this.Scope.Resolve<SavedAddressDialog>(
                new NamedParameter("prompt", prompt),
                new NamedParameter("useSavedAddressPrompt", useSavedAddressPrompt),
                new NamedParameter("saveAddressPrompt", saveAddressPrompt),
                TypedParameter.From(savedAddresses),
                TypedParameter.From(saveOptionNames));
        }
    }
}
namespace ContosoFlowers.Dialogs
{
    using System.Collections.Generic;
    using BotAssets;
    using ContosoFlowers.BotAssets.Dialogs;

    public interface IContosoFlowersDialogFactory : IDialogFactory
    {
        SavedAddressDialog CreateSavedAddressDialog(
            string prompt,
            string useSavedAddressPrompt,
            string saveAddressPrompt,
            IDictionary<string, string> savedAddresses,
            IEnumerable<string> saveOptionNames);
    }
}
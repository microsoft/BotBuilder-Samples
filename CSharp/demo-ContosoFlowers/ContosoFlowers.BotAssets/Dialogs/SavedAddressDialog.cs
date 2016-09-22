namespace ContosoFlowers.BotAssets.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Properties;
    using Services;

    [Serializable]
    public class SavedAddressDialog : IDialog<SavedAddressDialog.SavedAddressResult>
    {
        private readonly IDictionary<string, string> savedAddresses;
        private readonly IEnumerable<string> saveOptionNames;
        private readonly string prompt;
        private readonly string useSavedAddressPrompt;
        private readonly string saveAddressPrompt;
        private readonly ILocationService locationService;
        private readonly IDialogFactory dialogFactory;

        private string currentAddress;

        public SavedAddressDialog(
            string prompt, 
            string useSavedAddressPrompt, 
            string saveAddressPrompt, 
            IDictionary<string, string> savedAddresses, 
            IEnumerable<string> saveOptionNames,
            ILocationService locationService,
            IDialogFactory dialogFactory)
        {
            this.savedAddresses = savedAddresses ?? new Dictionary<string, string>();
            this.saveOptionNames = saveOptionNames;
            this.prompt = prompt;
            this.useSavedAddressPrompt = useSavedAddressPrompt;
            this.saveAddressPrompt = saveAddressPrompt;
            this.locationService = locationService;
            this.dialogFactory = dialogFactory;
        }

        public async Task StartAsync(IDialogContext context)
        {
            if (this.savedAddresses.Any())
            {
                PromptDialog.Choice(context, this.AfterSelectSavedAddress, this.savedAddresses.Values.Concat(new[] { Resources.SavedAddressDialog_AddNewAddress }), this.useSavedAddressPrompt);
            }
            else
            {
                this.AddressPrompt(context);
            }
        }

        private void AddressPrompt(IDialogContext context)
        {
            var addressDialog = this.dialogFactory.Create<AddressDialog, string>(this.prompt);
            context.Call(addressDialog, this.AfterAddressPrompt);
        }

        private async Task AfterAddressPrompt(IDialogContext context, IAwaitable<string> result)
        {
            this.currentAddress = await result;
            PromptDialog.Choice(context, this.AfterSelectToSaveAddress, this.saveOptionNames.Concat(new[] { Resources.SavedAddressDialog_NotThisTime }), this.saveAddressPrompt);
        }

        private async Task AfterSelectToSaveAddress(IDialogContext context, IAwaitable<string> result)
        {
            var saveOptionName = await result;
            saveOptionName = saveOptionName == Resources.SavedAddressDialog_NotThisTime ? null : saveOptionName;
            context.Done(new SavedAddressResult { Value = this.currentAddress, SaveOptionName = saveOptionName });
        }

        private async Task AfterSelectSavedAddress(IDialogContext context, IAwaitable<string> result)
        {
            this.currentAddress = await result;
            if (this.currentAddress == Resources.SavedAddressDialog_AddNewAddress)
            {
                this.AddressPrompt(context);
            }
            else
            {
                context.Done(new SavedAddressResult { Value = this.currentAddress });
            }
        }

        public class SavedAddressResult
        {
            public string Value { get; set; }

            public string SaveOptionName { get; set; }
        }
    }
}
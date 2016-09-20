namespace ContosoFlowers.BotAssets.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Extensions;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using Properties;
    using Services;

    [Serializable]
    public class AddressDialog : IDialog<string>
    {
        private readonly string prompt;
        private readonly ILocationService locationService;

        private string currentAddress;

        public AddressDialog(string prompt, ILocationService locationService)
        {
            this.prompt = prompt;
            this.locationService = locationService;
        }

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync(this.prompt);
            context.Wait(this.MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            var addresses = await this.locationService.ParseAddressAsync(message.Text);
            if (addresses.Count() == 0)
            {

                await context.PostAsync(Resources.AddressDialog_EnterAddressAgain);
                context.Wait(this.MessageReceivedAsync);
            }
            else if (addresses.Count() == 1)
            {
                this.currentAddress = addresses.First();
                PromptDialog.Choice(context, this.AfterAddressChoice, new[] { Resources.AddressDialog_Confirm, Resources.AddressDialog_Edit }, this.currentAddress);
            }
            else
            {
                var reply = context.MakeMessage();
                reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;

                foreach (var address in addresses)
                {
                    reply.AddHeroCard(Resources.AddressDialog_DidYouMean, address, new[] { new KeyValuePair<string, string>(Resources.AddressDialog_UseThisAddress, address) });
                }

                await context.PostAsync(reply);
                context.Wait(this.MessageReceivedAsync);
            }
        }

        private async Task AfterAddressChoice(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                var choice = await result;

                if (choice == Resources.AddressDialog_Edit)
                {
                    await this.StartAsync(context);
                }
                else
                {
                    context.Done(this.currentAddress);
                }
            }
            catch (TooManyAttemptsException)
            {
                throw;
            }
        }
    }
}
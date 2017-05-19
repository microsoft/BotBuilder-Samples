namespace ContosoFlowers.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading.Tasks;
    using System.Web;
    using AutoMapper;
    using BotAssets.Dialogs;
    using BotAssets.Extensions;
    using Microsoft.Bot.Builder.ConnectorEx;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.FormFlow;
    using Microsoft.Bot.Builder.Location;
    using Microsoft.Bot.Connector;
    using Models;
    using Properties;
    using Services;
    using Services.Models;

    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private readonly string checkoutUriFormat;
        private readonly IContosoFlowersDialogFactory dialogFactory;
        private readonly IOrdersService ordersService;

        private Models.Order order;
        private ConversationReference conversationReference;
        
        public RootDialog(string checkoutUriFormat, IContosoFlowersDialogFactory dialogFactory, IOrdersService ordersService)
        {
            this.checkoutUriFormat = checkoutUriFormat;
            this.dialogFactory = dialogFactory;
            this.ordersService = ordersService;
        }

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            if (this.conversationReference == null)
            {
                this.conversationReference = message.ToConversationReference();
            }

            await this.WelcomeMessageAsync(context);
        }

        private async Task WelcomeMessageAsync(IDialogContext context)
        {
            var reply = context.MakeMessage();

            var options = new[]
            {
                Resources.RootDialog_Welcome_Orders,
                Resources.RootDialog_Welcome_Support
            };
            reply.AddHeroCard(
                Resources.RootDialog_Welcome_Title,
                Resources.RootDialog_Welcome_Subtitle,
                options,
                new[] { "https://placeholdit.imgix.net/~text?txtsize=56&txt=Contoso%20Flowers&w=640&h=330" });

            await context.PostAsync(reply);

            context.Wait(this.OnOptionSelected);
        }

        private async Task OnOptionSelected(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            if (message.Text == Resources.RootDialog_Welcome_Orders)
            {
                this.order = new Models.Order();

                // BotBuilder's LocationDialog
                // Leverage DI to inject other parameters
                var locationDialog = this.dialogFactory.Create<LocationDialog>(
                    new Dictionary<string, object>()
                    {
                        { "prompt", string.Format(CultureInfo.CurrentCulture, Resources.RootDialog_DeliveryAddress_Prompt, message.From.Name ?? "User") },
                        { "channelId", context.Activity.ChannelId }
                    });

                context.Call(locationDialog, this.AfterDeliveryAddress);
            }
            else if (message.Text == Resources.RootDialog_Welcome_Support)
            {
                await this.StartOverAsync(context, Resources.RootDialog_Support_Message);
            }
            else
            {
                await this.StartOverAsync(context, Resources.RootDialog_Welcome_Error);
            }
        }

        private async Task AfterDeliveryAddress(IDialogContext context, IAwaitable<Place> result)
        {
            try
            {
                var place = await result;
                var formattedAddress = place.GetPostalAddress().FormattedAddress;
                this.order.DeliveryAddress = formattedAddress;

                context.Call(this.dialogFactory.Create<FlowerCategoriesDialog>(), this.AfterFlowerCategorySelected);
            }
            catch (TooManyAttemptsException)
            {
                await this.StartOverAsync(context, Resources.RootDialog_TooManyAttempts);
            }
        }

        private async Task AfterFlowerCategorySelected(IDialogContext context, IAwaitable<string> result)
        {
            this.order.FlowerCategoryName = await result;

            context.Call(this.dialogFactory.Create<BouquetsDialog, string>(this.order.FlowerCategoryName), this.AfterBouquetSelected);
        }

        private async Task AfterBouquetSelected(IDialogContext context, IAwaitable<Bouquet> result)
        {
            var selectedBouquet = await result;

            this.order.Bouquet = selectedBouquet;

            await context.PostAsync(string.Format(CultureInfo.CurrentCulture, Resources.RootDialog_Bouquet_Selected, this.order.Bouquet.Name));

            PromptDialog.Choice(context, this.AfterDeliveryDateSelected, new[] { StringConstants.Today, StringConstants.Tomorrow }, Resources.RootDialog_DeliveryDate_Prompt);
        }

        private async Task AfterDeliveryDateSelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                this.order.DeliveryDate = (await result == StringConstants.Today) ? DateTime.Today : DateTime.Today.AddDays(1);

                await context.PostAsync(string.Format(CultureInfo.CurrentCulture, Resources.RootDialog_DeliveryDate_Selected, this.order.Bouquet.Name, this.order.DeliveryDate.ToShortDateString()));

                UserPreferences userPreferences;
                if (context.UserData.TryGetValue(StringConstants.UserPreferencesKey, out userPreferences))
                {
                    this.order.SenderEmail = userPreferences.SenderEmail;
                    this.order.SenderPhoneNumber = userPreferences.SenderPhoneNumber;

                    this.order.AskToUseSavedSenderInfo = !string.IsNullOrWhiteSpace(this.order.SenderEmail) && !string.IsNullOrWhiteSpace(this.order.SenderPhoneNumber);
                }

                var orderForm = new FormDialog<Models.Order>(this.order, Models.Order.BuildOrderForm, FormOptions.PromptInStart);
                context.Call(orderForm, this.AfterOrderForm);
            }
            catch (TooManyAttemptsException)
            {
                await this.StartOverAsync(context, Resources.RootDialog_TooManyAttempts);
            }
        }

        private async Task AfterOrderForm(IDialogContext context, IAwaitable<Models.Order> result)
        {
            try
            {
                await result;
                if (this.order.SaveSenderInfo)
                {
                    context.UserData.UpdateValue<UserPreferences>(
                        StringConstants.UserPreferencesKey,
                        userPreferences =>
                        {
                            userPreferences.SenderEmail = this.order.SenderEmail;
                            userPreferences.SenderPhoneNumber = this.order.SenderPhoneNumber;
                        });
                }

                var savedAddresses = new Dictionary<string, string>();
                UserPreferences preferences;

                if (context.UserData.TryGetValue(StringConstants.UserPreferencesKey, out preferences))
                {
                    savedAddresses = preferences.BillingAddresses;
                }

                var addressDialog = this.dialogFactory.CreateSavedAddressDialog(
                    Resources.RootDialog_BillingAddress_Prompt,
                    Resources.RootDialog_BillingAddress_SelectSaved,
                    Resources.RootDialog_BillingAddress_ShouldSave,
                    savedAddresses,
                    new[] { StringConstants.HomeBillingAddress, StringConstants.WorkBillingAddress });

                context.Call(addressDialog, this.AfterBillingAddress);
            }
            catch (FormCanceledException e)
            {
                string reply;

                if (e.InnerException == null)
                {
                    reply = Resources.RootDialog_Order_Cancelation;
                }
                else
                {
                    reply = string.Format(CultureInfo.CurrentCulture, Resources.RootDialog_Order_Error, e.InnerException.Message);
                }

                await this.StartOverAsync(context, reply);
            }
            catch (TooManyAttemptsException)
            {
                await this.StartOverAsync(context, Resources.RootDialog_TooManyAttempts);
            }
        }

        private async Task AfterBillingAddress(IDialogContext context, IAwaitable<SavedAddressDialog.SavedAddressResult> result)
        {
            try
            {
                var addressResult = await result;
                this.order.BillingAddress = addressResult.Value;

                if (!string.IsNullOrWhiteSpace(addressResult.SaveOptionName))
                {
                    context.UserData.UpdateValue<UserPreferences>(
                        StringConstants.UserPreferencesKey,
                        userPreferences =>
                        {
                            userPreferences.BillingAddresses = userPreferences.BillingAddresses ?? new Dictionary<string, string>();
                            userPreferences.BillingAddresses[addressResult.SaveOptionName.ToLower()] = this.order.BillingAddress;
                        });
                }

                await this.PaymentSelectionAsync(context);
            }
            catch (TooManyAttemptsException)
            {
                await this.StartOverAsync(context, Resources.RootDialog_TooManyAttempts);
            }
        }

        private async Task PaymentSelectionAsync(IDialogContext context)
        {
            var paymentReply = context.MakeMessage();

            var serviceModel = Mapper.Map<Services.Models.Order>(this.order);
            if (this.order.OrderID == null)
            {
                this.order.OrderID = this.ordersService.PlacePendingOrder(serviceModel);
            }

            var checkoutUrl = this.BuildCheckoutUrl(this.order.OrderID);
            paymentReply.Attachments = new List<Attachment>
                {
                    new HeroCard()
                    {
                        Text = string.Format(CultureInfo.CurrentCulture, Resources.RootDialog_Checkout_Prompt, this.order.Bouquet.Price.ToString("C")),
                        Buttons = new List<CardAction>
                        {
                            new CardAction(ActionTypes.OpenUrl, Resources.RootDialog_Checkout_Continue, value: checkoutUrl),
                            new CardAction(ActionTypes.ImBack, Resources.RootDialog_Checkout_Cancel, value: Resources.RootDialog_Checkout_Cancel)
                        }
                    }.ToAttachment()
                };

            await context.PostAsync(paymentReply);

            context.Wait(this.AfterPaymentSelection);
        }

        private string BuildCheckoutUrl(string orderID)
        {
            var uriBuilder = new UriBuilder(this.checkoutUriFormat);

            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["orderID"] = orderID;
            query["botId"] = this.conversationReference.Bot.Id;
            query["channelId"] = this.conversationReference.ChannelId;
            query["conversationId"] = this.conversationReference.Conversation.Id;
            query["serviceUrl"] = this.conversationReference.ServiceUrl;
            query["userId"] = this.conversationReference.User.Id;

            uriBuilder.Query = query.ToString();
            var checkoutUrl = uriBuilder.Uri.ToString();

            return checkoutUrl;
        }

        private async Task AfterPaymentSelection(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var selection = await result;

            if (selection.Text == Resources.RootDialog_Checkout_Cancel)
            {
                var options = new[] { Resources.RootDialog_Menu_StartOver, Resources.RootDialog_Menu_Cancel, Resources.RootDialog_Welcome_Support };
                PromptDialog.Choice(context, this.AfterChangedMyMind, options, Resources.RootDialog_Menu_Prompt);
            }
            else
            {
                var serviceOrder = this.ordersService.RetrieveOrder(selection.Text);
                if (serviceOrder == null || !serviceOrder.Payed)
                {
                    await context.PostAsync(string.Format(CultureInfo.CurrentCulture, Resources.RootDialog_Checkout_Error, selection.Text));
                    await this.PaymentSelectionAsync(context);
                    return;
                }

                var message = context.MakeMessage();
                message.Text = string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.RootDialog_Receipt_Text,
                   selection.Text,
                   this.order.Bouquet.Name,
                   this.order.RecipientFirstName,
                   this.order.RecipientLastName,
                   this.order.Note);
                message.Attachments.Add(this.GetReceiptCard());

                await this.StartOverAsync(context, message);
            }
        }

        private async Task AfterChangedMyMind(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                var option = await result;

                if (option == Resources.RootDialog_Menu_StartOver)
                {
                    await this.StartOverAsync(context, Resources.RootDialog_Welcome_Message);
                }
                else if (option == Resources.RootDialog_Menu_Cancel)
                {
                    await this.PaymentSelectionAsync(context);
                }
                else
                {
                    await this.StartOverAsync(context, Resources.RootDialog_Support_Message);
                }
            }
            catch (TooManyAttemptsException)
            {
                await this.StartOverAsync(context, Resources.RootDialog_TooManyAttempts);
            }
        }

        private Attachment GetReceiptCard()
        {
            var order = this.ordersService.RetrieveOrder(this.order.OrderID);
            var creditCardOffuscated = order.PaymentDetails.CreditCardNumber.Substring(0, 4) + "-****";
            var receiptCard = new ReceiptCard
            {
                Title = Resources.RootDialog_Receipt_Title,
                Facts = new List<Fact>
                {
                    new Fact(Resources.RootDialog_Receipt_OrderID, order.OrderID),
                    new Fact(Resources.RootDialog_Receipt_PaymentMethod, creditCardOffuscated)
                },
                Items = new List<ReceiptItem>
                {
                    new ReceiptItem(
                        title: order.FlowerCategoryName,
                        subtitle: order.Bouquet.Name,
                        price: order.Bouquet.Price.ToString("C"),
                        image: new CardImage(order.Bouquet.ImageUrl)),
                },
                Total = order.Bouquet.Price.ToString("C")
            };

            return receiptCard.ToAttachment();
        }

        private async Task StartOverAsync(IDialogContext context, string text)
        {
            var message = context.MakeMessage();
            message.Text = text;
            await this.StartOverAsync(context, message);
        }

        private async Task StartOverAsync(IDialogContext context, IMessageActivity message)
        {
            await context.PostAsync(message);
            this.order = new Models.Order();
            await this.WelcomeMessageAsync(context);
        }
    }
}
namespace PaymentsBot.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Helpers;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using Microsoft.Bot.Connector.Payments;
    using Models;
    using Properties;
    using Services;

    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public const string CARTKEY = "CART_ID";

        public async Task StartAsync(IDialogContext context)
        {
            await Task.FromResult(true);
            context.Wait(this.MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            await this.WelcomeMessageAsync(context, argument);
        }

        public async Task WelcomeMessageAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var reply = context.MakeMessage();

            reply.Text = string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.RootDialog_Welcome_Msg,
                    context.Activity.From.Name);

            await context.PostAsync(reply);

            var replyMessage = context.MakeMessage();

            replyMessage.Attachments = new List<Attachment>();

            var catalogItem = await new CatalogService().GetRandomItemAsync();

            // store cartId and userId in the conversationData
            var cartId = catalogItem.Id.ToString();
            context.ConversationData.SetValue(CARTKEY, cartId);
            context.ConversationData.SetValue(cartId, context.Activity.From.Id);

            replyMessage.Attachments.Add(await BuildBuyCardAsync(cartId, catalogItem));

            await context.PostAsync(replyMessage);

            context.Wait(this.AfterPurchaseAsync);
        }

        private static PaymentRequest BuildPaymentRequest(string cartId, CatalogItem item, MicrosoftPayMethodData methodData)
        {
            return new PaymentRequest
            {
                Id = cartId,
                Expires = TimeSpan.FromDays(1).ToString(),
                MethodData = new List<PaymentMethodData>
                {
                    methodData.ToPaymentMethodData()
                },
                Details = new PaymentDetails
                {
                    Total = new PaymentItem
                    {
                        Label = Resources.Wallet_Label_Total,
                        Amount = new PaymentCurrencyAmount
                        {
                            Currency = item.Currency,
                            Value = Convert.ToString(item.Price, CultureInfo.InvariantCulture)
                        },
                        Pending = true
                    },
                    DisplayItems = new List<PaymentItem>
                    {
                        new PaymentItem
                        {
                            Label = item.Title,
                            Amount = new PaymentCurrencyAmount
                            {
                                Currency = item.Currency,
                                Value = item.Price.ToString(CultureInfo.InvariantCulture)
                            }
                        },
                        new PaymentItem
                        {
                            Label = Resources.Wallet_Label_Shipping,
                            Amount = new PaymentCurrencyAmount
                            {
                                Currency = item.Currency,
                                Value = "0.00"
                            },
                            Pending = true
                        },
                        new PaymentItem
                        {
                            Label = Resources.Wallet_Label_Tax,
                            Amount = new PaymentCurrencyAmount
                            {
                                Currency = item.Currency,
                                Value = "0.00"
                            },
                            Pending = true
                        }
                    }
                },
                Options = new PaymentOptions
                {
                    RequestShipping = true,
                    RequestPayerEmail = true,
                    RequestPayerName = true,
                    RequestPayerPhone = true,
                    ShippingType = PaymentShippingTypes.Shipping
                }
            };
        }

        private static Task<Attachment> BuildBuyCardAsync(string cartId, CatalogItem item)
        {
            var heroCard = new HeroCard
            {
                Title = item.Title,
                Subtitle = $"{item.Currency} {item.Price.ToString("F")}",
                Text = item.Description,
                Images = new List<CardImage>
                {
                    new CardImage
                    {
                        Url = item.ImageUrl
                    }
                },
                Buttons = new List<CardAction>
                {
                    new CardAction
                    {
                        Title = "Buy",
                        Type = PaymentRequest.PaymentActionType,
                        Value = BuildPaymentRequest(cartId, item, PaymentService.GetAllowedPaymentMethods())
                    }
                }
            };

            return Task.FromResult(heroCard.ToAttachment());
        }

        private static ReceiptItem BuildReceiptItem(string title, string subtitle, string price, string imageUrl)
        {
            return new ReceiptItem(
                title: title,
                subtitle: subtitle,
                price: price,
                image: new CardImage(imageUrl));
        }

        private static async Task<Attachment> BuildReceiptCardAsync(PaymentRecord paymentRecord)
        {
            var shippingOption = await new ShippingService().GetShippingOptionAsync(paymentRecord.ShippingOption);

            var catalogItem = await new CatalogService().GetItemByIdAsync(paymentRecord.OrderId);

            var receiptItems = new List<ReceiptItem>();

            var facts = new List<Fact>
            {
                new Fact(Resources.RootDialog_Receipt_OrderID, paymentRecord.OrderId.ToString()),
                new Fact(Resources.RootDialog_Receipt_PaymentMethod, paymentRecord.MethodName),
                new Fact(Resources.RootDialog_Shipping_Address, paymentRecord.ShippingAddress.FullInline()),
                new Fact(Resources.RootDialog_Shipping_Option, shippingOption != null ? shippingOption.Label : "N/A")
            };

            receiptItems.AddRange(paymentRecord.Items.Select<PaymentItem, ReceiptItem>(item =>
            {
                if (catalogItem.Title.Equals(item.Label))
                {
                    return RootDialog.BuildReceiptItem(
                        catalogItem.Title,
                        catalogItem.Description,
                        $"{catalogItem.Currency} {catalogItem.Price.ToString("F")}",
                        catalogItem.ImageUrl);
                }
                else
                {
                    facts.Add(new Fact(item.Label, $"{item.Amount.Currency} {item.Amount.Value}"));
                    return null;
                }
            })
            .Where(item => item != null));

            var receiptCard = new ReceiptCard
            {
                Title = Resources.RootDialog_Receipt_Title,
                Facts = facts,
                Items = receiptItems,
                Tax = null, // Sales Tax is a displayed line item, leave this blank
                Total = $"{paymentRecord.Total.Amount.Currency} {paymentRecord.Total.Amount.Value}"
            };

            return receiptCard.ToAttachment();
        }

        private async Task AfterPurchaseAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            // clean up state store after completion
            var cartId = context.ConversationData.GetValue<string>(CARTKEY);
            context.ConversationData.RemoveValue(CARTKEY);
            context.ConversationData.RemoveValue(cartId);

            var activity = await argument as Activity;
            var paymentRecord = activity?.Value as PaymentRecord;

            if (paymentRecord == null)
            {
                // show error
                var errorMessage = activity.Text;
                var message = context.MakeMessage();
                message.Text = errorMessage;

                await this.StartOverAsync(context, argument, message);
            }
            else
            {
                // show receipt
                var message = context.MakeMessage();
                message.Text = string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.RootDialog_Receipt_Text,
                    paymentRecord.OrderId,
                    paymentRecord.PaymentProcessor);

                message.Attachments.Add(await BuildReceiptCardAsync(paymentRecord));

                await this.StartOverAsync(context, argument, message);
            }
        }

        private async Task StartOverAsync(IDialogContext context, IAwaitable<IMessageActivity> argument, IMessageActivity message)
        {
            await context.PostAsync(message);

            context.Wait(this.MessageReceivedAsync);
        }
    }
}
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Bot.Dialogs
{


    /*
     * 1. Bot requests a client token from server in order to initialize the client SDK
     * 2. Bot creates a payment order 
     * 
     * 3. Once the client SDK is initialized and the customer has submitted payment information, the SDK communicates that information to Braintree, which returns a payment method nonce
     * 4 You then send the payment nonce to your server
     * 5. Your server code receives the payment method nonce from your client and then uses the server SDK to create a transaction or perform other Braintree functions Credentials
     * 
     * 
     * 
     */
    [Serializable]
    public class PayFlow : IDialog<bool>
    {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task StartAsync(IDialogContext context)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            context.Wait<string>(InitialMessageReceived);
        }

        private async Task InitialMessageReceived(IDialogContext context, IAwaitable<string> result)
        {
            //1. Your app or web front-end requests a client token from your server in order to initialize the client SDK
            var apiContext = Bot.Utilities.Configuration.GetAPIContext();
            //2.Bot creates a payment order
            await CreatePayment(apiContext, context, await result);
        }

        private async Task CreatePayment(APIContext apiContext, IDialogContext context, string amount)
        {
            // ###Items
            // Items within a transaction.
            var itemList = new ItemList()
            {
                items = new List<Item>()
            };

            itemList.items.Add(new Item()
            {
                name = @"Payment",
                currency = @"USD",
                price = amount,
                quantity = "1"
            });

            //We're only doing one item so this is not really needed, but with multiple items you need to ensure the sum is correct
            double totalAmt = itemList.items.Sum( x => Convert.ToDouble(x.price));

            // ###Payer
            // A resource representing a Payer that funds a payment
            // Payment Method
            // as `paypal`
            var payer = new Payer() { payment_method = "paypal" };

            // ###Redirect URLS
            // These URLs will determine how the user is redirected from PayPal once they have either approved or canceled the payment.
            var baseURI = System.Configuration.ConfigurationManager.AppSettings["Payment_Redirect_base_URI"];
            ResumptionCookie _resumption = null;
            context.PrivateConversationData.TryGetValue("resumption", out _resumption);
            var redirectUrl = baseURI + "resume=" + UrlToken.Encode(_resumption);
            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl + "&cancel=true",
                return_url = redirectUrl
            };

            // ###Details
            // Let's you specify details of a payment amount.
            var details = new Details()
            {
                subtotal = totalAmt.ToString()
            };

            // ###Amount
            // Let's you specify a payment amount.
            var totalAmount = new PayPal.Api.Amount()
            {
                currency = "USD",
                total = totalAmt.ToString()// Total must be equal to sum of shipping, tax and subtotal.
                                           //, // Total must be equal to sum of shipping, tax and subtotal.
                                           //details = details
            };

            // ###Transaction
            // A transaction defines the contract of a
            // payment - what is the payment for and who
            // is fulfilling it. 
            var transactionList = new List<PayPal.Api.Transaction>();

            // The Payment creation API requires a list of
            // Transaction; add the created `Transaction`
            // to a List
            transactionList.Add(new PayPal.Api.Transaction()
            {
                description = "Transaction description.",
                invoice_number = GetRandomInvoiceNumber(),
                amount = totalAmount,
                item_list = itemList
            });

            // ###Payment
            // A Payment Resource; create one using
            // the above types and intent as `sale` or `authorize`
            var payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };

            // Create a payment using a valid APIContext
            var createdPayment = payment.Create(apiContext);
            // Using the `links` provided by the `createdPayment` object, we can give the user the option to redirect to PayPal to approve the payment.
            var links = createdPayment.links.GetEnumerator();
            var urllink = string.Empty;
            while (links.MoveNext())
            {
                var link = links.Current;
                if (link.rel.ToLower().Trim().Equals("approval_url"))
                {
                    urllink = link.href;
                    break;
                }
            }
            //var msg = context.MakeMessage();
            //msg.TextFormat = TextFormatTypes.Markdown;
            //msg.Text = $"Please click [here]({urllink}) to PayPal to approve the payment...";
            await context.PostAsync($"Please click [here]({urllink}) to PayPal to approve the payment...");
            context.Wait(StoreReceiptAndFinish);

        }

        /// <summary>
        /// Gets a random invoice number to be used with a sample request that requires an invoice number.
        /// </summary>
        /// <returns>A random invoice number in the range of 0 to 999999</returns>
        private static string GetRandomInvoiceNumber() => new Random().Next(999999).ToString();

        private async Task StoreReceiptAndFinish(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var receiptNumber = await result;

            // TODO: Validate entered receipt with Paypal
            var msg = context.MakeMessage();
            msg.Text = receiptNumber.Text;

            await context.PostAsync(msg, CancellationToken.None);
            context.ConversationData.SetValue("receiptNumber", receiptNumber.Text);
            context.Done(true);
        }
    }
}
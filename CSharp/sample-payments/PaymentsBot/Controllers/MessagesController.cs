namespace PaymentsBot
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Activities;
    using Dialogs;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Scorables;
    using Microsoft.Bot.Connector;
    using Microsoft.Bot.Connector.Payments;
    using Models;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Services;

    [BotAuthentication]
    public class MessagesController : ApiController
    {
        private readonly CatalogService catalogService;
        private readonly ShippingService shippingService;
        private readonly PaymentService paymentService;

        public MessagesController()
        {
            this.shippingService = new ShippingService();
            this.paymentService = new PaymentService(AppSettings.StripeApiKey);
            this.catalogService = new CatalogService();
        }

        private enum ShippingUpdateKind
        {
            Address,
            Options,
            Both
        }

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity, CancellationToken token)
        {
            var response = new HttpResponseMessage(HttpStatusCode.Accepted);

            // dispatch based on the activity to the private methods below
            IDispatcher dispatcher = new ActivityControllerDispatcher(this, activity, response);
            await dispatcher.TryPostAsync(token);

            return response;
        }

        /// <summary>
        /// Handle recieved messages
        /// </summary>
        [MethodBind]
        [ScorableGroup(1)]
        private async Task OnMessageActivity(IMessageActivity activity, CancellationToken token)
        {
            await Conversation.SendAsync(activity, () => new RootDialog(), token);
        }

        /// <summary>
        /// Handle Payment calls
        /// </summary>
        [MethodBind]
        [ScorableGroup(1)]
        private async Task OnInvoke(IInvokeActivity invoke, IConnectorClient connectorClient, IStateClient stateClient, HttpResponseMessage response, CancellationToken token)
        {
            MicrosoftAppCredentials.TrustServiceUrl(invoke.RelatesTo.ServiceUrl);

            var jobject = invoke.Value as JObject;
            if (jobject == null)
            {
                throw new ArgumentException("Request payload must be a valid json object.");
            }

            // This is a temporary workaround for the issue that the channelId for "webchat" is mapped to "directline" in the incoming RelatesTo object
            invoke.RelatesTo.ChannelId = (invoke.RelatesTo.ChannelId == "directline") ? "webchat" : invoke.RelatesTo.ChannelId;

            if (invoke.RelatesTo.User == null)
            {
                // Bot keeps the userId in context.ConversationData[cartId]
                var conversationData = await stateClient.BotState.GetConversationDataAsync(invoke.RelatesTo.ChannelId, invoke.RelatesTo.Conversation.Id, token);
                var cartId = conversationData.GetProperty<string>(RootDialog.CARTKEY);

                if (!string.IsNullOrEmpty(cartId))
                {
                    invoke.RelatesTo.User = new ChannelAccount
                    {
                        Id = conversationData.GetProperty<string>(cartId)
                    };
                }
            }

            var updateResponse = default(object);
            switch (invoke.Name)
            {
                case PaymentOperations.UpdateShippingAddressOperationName:
                    updateResponse = await this.ProcessShippingUpdate(jobject.ToObject<PaymentRequestUpdate>(), ShippingUpdateKind.Address, token);
                    break;

                case PaymentOperations.UpdateShippingOptionOperationName:
                    updateResponse = await this.ProcessShippingUpdate(jobject.ToObject<PaymentRequestUpdate>(), ShippingUpdateKind.Options, token);
                    break;

                case PaymentOperations.PaymentCompleteOperationName:
                    updateResponse = await this.ProcessPaymentComplete(invoke, jobject.ToObject<PaymentRequestComplete>(), token);
                    break;

                default:
                    throw new ArgumentException("Invoke activity name is not a supported request type.");
            }

            response.Content = new ObjectContent<object>(
              updateResponse,
              this.Configuration.Formatters.JsonFormatter,
              JsonMediaTypeFormatter.DefaultMediaType);

            response.StatusCode = HttpStatusCode.OK;
        }

        private async Task<PaymentRequestUpdateResult> ProcessShippingUpdate(PaymentRequestUpdate paymentRequestUpdate, ShippingUpdateKind updateKind, CancellationToken token = default(CancellationToken))
        {
            var catalogItem = await this.catalogService.GetItemByIdAsync(Guid.Parse(paymentRequestUpdate.Id));
            if (catalogItem == null)
            {
                throw new ArgumentException("Invalid cart identifier within payment request provided.");
            }

            var result = new PaymentRequestUpdateResult(paymentRequestUpdate.Details);
            if (ShippingUpdateKind.Both.Equals(updateKind) || ShippingUpdateKind.Address.Equals(updateKind))
            {
                result.Details.ShippingOptions = (await this.shippingService.GetShippingOptionsAsync(catalogItem, paymentRequestUpdate.ShippingAddress)).ToList();
            }

            if (ShippingUpdateKind.Both.Equals(updateKind) || ShippingUpdateKind.Options.Equals(updateKind))
            {
                foreach (var shippingOption in result.Details.ShippingOptions)
                {
                    shippingOption.Selected = shippingOption.Id.Equals(paymentRequestUpdate.ShippingOption, StringComparison.OrdinalIgnoreCase);
                }
            }

            if (result.Details.ShippingOptions.Count(option => option.Selected.HasValue && option.Selected.Value) > 1)
            {
                throw new ArgumentException("Expected exactly zero or one selected shipping option.");
            }

            // update payment details after shipping changed
            await this.shippingService.UpdatePaymentDetailsAsync(result.Details, paymentRequestUpdate.ShippingAddress, catalogItem);

            return result;
        }

        private async Task<PaymentRequestCompleteResult> ProcessPaymentComplete(IInvokeActivity invoke, PaymentRequestComplete paymentRequestComplete, CancellationToken token = default(CancellationToken))
        {
            var paymentRequest = paymentRequestComplete.PaymentRequest;
            var paymentResponse = paymentRequestComplete.PaymentResponse;

            paymentRequest.Details = (await this.ProcessShippingUpdate(
                new PaymentRequestUpdate()
                {
                    Id = paymentRequest.Id,
                    Details = paymentRequest.Details,
                    ShippingAddress = paymentResponse.ShippingAddress,
                    ShippingOption = paymentResponse.ShippingOption
                },
                ShippingUpdateKind.Both,
                token)).Details;

            PaymentRecord paymentRecord = null;
            PaymentRequestCompleteResult result = null;
            Exception paymentProcessingException = null;
            try
            {
                paymentRecord = await this.paymentService.ProcessPaymentAsync(paymentRequest, paymentResponse);
                result = new PaymentRequestCompleteResult("success");
            }
            catch (Exception ex)
            {
                paymentProcessingException = ex;
                // TODO: If payment is captured but not charged this would be considered "unknown" (charge the captured amount after shipping scenario).
                result = new PaymentRequestCompleteResult("failure");
            }

            try
            {
                var message = invoke.RelatesTo.GetPostToBotMessage();
                if (result.Result == "success")
                {
                    // Resume the conversation with the receipt to user
                    message.Text = paymentRequestComplete.Id;
                    message.Value = paymentRecord;
                }
                else
                {
                    // Resume the conversation with error message
                    message.Text = $"Failed to process payment with error: {paymentProcessingException?.Message}";
                }
                await Conversation.ResumeAsync(invoke.RelatesTo, message, token);
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Failed to resume the conversation using ConversationReference: {JsonConvert.SerializeObject(invoke.RelatesTo)} and exception: {ex.Message}");
            }

            return result;
        }
    }
}
namespace PaymentsBot.Services
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Microsoft.Bot.Connector.Payments;
    using Models;
    using Newtonsoft.Json.Linq;
    using Stripe;

    public class PaymentService
    {
        private static readonly MicrosoftPayMethodData MethodData = new MicrosoftPayMethodData(testMode: !AppSettings.LiveMode)
        {
            MerchantId = AppSettings.MerchantId,
            SupportedNetworks = new[] { "visa", "mastercard" },
            SupportedTypes = new[] { "credit" }
        };

        private readonly StripeChargeService chargeService;

        public PaymentService(string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new ArgumentNullException(nameof(apiKey));
            }

            this.chargeService = new StripeChargeService(apiKey);
        }

        public static MicrosoftPayMethodData GetAllowedPaymentMethods()
        {
            return MethodData;
        }

        public async Task<PaymentRecord> ProcessPaymentAsync(PaymentRequest paymentRequest, PaymentResponse paymentResponse)
        {
            if (paymentRequest == null)
            {
                throw new ArgumentNullException(nameof(paymentRequest));
            }

            if (paymentResponse == null)
            {
                throw new ArgumentNullException(nameof(paymentResponse));
            }

            if (!MicrosoftPayMethodData.MethodName.Equals(paymentResponse.MethodName, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentOutOfRangeException("Payment method is not supported.");
            }

            var details = JObject.FromObject(paymentResponse.Details);

            var paymentToken = PaymentToken.Parse(details.GetValue("paymentToken").Value<string>());

            if (string.IsNullOrWhiteSpace(paymentToken.Source))
            {
                throw new ArgumentNullException("Payment token source is empty.");
            }

            if (paymentToken.Format != PaymentTokenFormat.Stripe)
            {
                throw new ArgumentOutOfRangeException("Payment token format is not Stripe.");
            }

            if (!AppSettings.MerchantId.Equals(paymentToken.MerchantId, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentOutOfRangeException("Merchant id is not supported.");
            }

            if (!paymentToken.Amount.Currency.Equals(paymentRequest.Details.Total.Amount.Currency))
            {
                throw new ArgumentOutOfRangeException("Payment token amount currency mismatch.");
            }

            if (!paymentToken.Amount.Value.Equals(paymentRequest.Details.Total.Amount.Value))
            {
                throw new ArgumentOutOfRangeException("Payment token amount value mismatch.");
            }

            var result = new PaymentRecord()
            {
                OrderId = Guid.Parse(paymentRequest.Id),
                TransactionId = Guid.NewGuid(),
                MethodName = paymentResponse.MethodName,
                PaymentProcessor = paymentToken.Format.ToString(),
                ShippingAddress = paymentResponse.ShippingAddress,
                ShippingOption = paymentResponse.ShippingOption,
                Items = paymentRequest.Details.DisplayItems,
                Total = paymentRequest.Details.Total,
                LiveMode = !paymentToken.IsEmulated,
            };

            // If the payment token is microsoft emulated do not charge (as it will fail)
            if (paymentToken.IsEmulated)
            {
                return result;
            }

            var chargeOptions = new StripeChargeCreateOptions()
            {
                Currency = paymentToken.Amount.Currency,
                Amount = (int)(double.Parse(paymentToken.Amount.Value) * 100), // Amount in cents
                Description = paymentRequest.Id,
                SourceTokenOrExistingSourceId = paymentToken.Source,
            };

            try
            {
                var charge = await this.chargeService.CreateAsync(chargeOptions);

                if (charge.Status.Equals("succeeded", StringComparison.OrdinalIgnoreCase) && charge.Captured == true)
                {
                    // Charge succeeded, return payment paymentRecord
                    // Ideally, you should register the transaction using the PaymentRecord and charge.Id
                    return result;
                }

                // Other statuses may include processing "pending" or "success" with non captured funds. It is up to the merchant how to handle these cases.
                // If payment is captured but not charged this would be considered "unknown" (charge the captured amount after shipping scenario)
                // Merchant might choose to handle "pending" and "failed" status or handle "success" status with funds captured null or false
                // More information @ https://stripe.com/docs/api#charge_object-captured
                throw new PaymentException($"Could not process charge using Stripe with charge.status: {charge.Status} and charge.captured: {charge.Captured}");
            }
            catch (StripeException ex)
            {
                Debug.Write($"Error processing payment with Stripe: {ex.Message}");
                throw new PaymentException("Error processing payment with Stripe.", ex);
            }
        }
    }
}
namespace PaymentsBot.Models
{
    using System;
    using Microsoft.Bot.Connector.Payments;
    using Newtonsoft.Json;

    public class PaymentTokenHeader
    {
        /// <summary>
        /// Parse from header json
        /// </summary>
        /// <param name="headerJson">header json</param>
        internal PaymentTokenHeader(string headerJson)
        {
            JsonConvert.PopulateObject(headerJson, this, PaymentToken.DefaultJsonSerializerSettings);
        }

        /// <summary>
        /// Payment Token Format
        /// </summary>
        [JsonProperty(PropertyName = "format")]
        public PaymentTokenFormat Format { get; set; }

        /// <summary>
        /// Merchant Id
        /// </summary>
        [JsonProperty(PropertyName = "merchantId")]
        public string MerchantId { get; set; }

        /// <summary>
        /// Payment Request Id
        /// </summary>
        public string PaymentRequestId { get; set; }

        /// <summary>
        /// Amount
        /// </summary>
        [JsonProperty(PropertyName = "amount")]
        public PaymentCurrencyAmount Amount { get; set; }

        /// <summary>
        /// Expiry
        /// </summary>
        [JsonProperty(PropertyName = "expiry")]
        public DateTime Expiry { get; set; }

        /// <summary>
        /// TimeStamp
        /// </summary>
        [JsonProperty(PropertyName = "timeStamp")]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Error Code
        /// </summary>
        [JsonProperty(PropertyName = "errorCode")]
        public string ErrorCode { get; set; }

        /// <summary>
        /// Error Text
        /// </summary>
        [JsonProperty(PropertyName = "errorText")]
        public string ErrorText { get; set; }

        /// <summary>
        /// Error Source
        /// </summary>
        [JsonProperty(PropertyName = "errorSource")]
        public string ErrorSource { get; set; }
    }
}
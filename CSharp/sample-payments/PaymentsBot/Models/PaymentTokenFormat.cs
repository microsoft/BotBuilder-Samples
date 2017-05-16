namespace PaymentsBot.Models
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum PaymentTokenFormat
    {
        /// <summary>
        /// Invalid
        /// </summary>
        Invalid = 0,

        /// <summary>
        /// Error
        /// </summary>
        Error,

        /// <summary>
        /// Stripe
        /// </summary>
        Stripe,
    }
}
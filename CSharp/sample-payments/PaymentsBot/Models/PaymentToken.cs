namespace PaymentsBot.Models
{
    using System;
    using System.Text;
    using Helpers;
    using Microsoft.Bot.Connector.Payments;
    using Newtonsoft.Json;

    public class PaymentToken
    {
        internal static readonly JsonSerializerSettings DefaultJsonSerializerSettings = new JsonSerializerSettings
        {
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            NullValueHandling = NullValueHandling.Ignore
        };

        private const string MsPayEmulatedStripeTokenSource = "tok_18yWDMKVgMv7trmwyE21VqO";

        private readonly PaymentTokenHeader header;
        private readonly string source;
        private readonly byte[] signature;

        private PaymentToken(PaymentTokenHeader header, string source, byte[] signature)
        {
            this.header = header;
            this.source = source;
            this.signature = signature;
        }

        /// <summary>
        /// Token Format
        /// </summary>
        public PaymentTokenFormat Format => this.header.Format;

        /// <summary>
        /// Merchant Id
        /// </summary>
        public string MerchantId => this.header.MerchantId;

        /// <summary>
        /// Payment Request Id
        /// </summary>
        public string PaymentRequestId => this.header.PaymentRequestId;

        /// <summary>
        /// Amount
        /// </summary>
        public PaymentCurrencyAmount Amount => this.header.Amount;

        /// <summary>
        /// Expiry
        /// </summary>
        public DateTime Expiry => this.header.Expiry;

        /// <summary>
        /// Timestamp
        /// </summary>
        public DateTime Timestamp => this.header.Timestamp;

        /// <summary>
        /// Is Emulated
        /// </summary>
        public bool IsEmulated => this.Format == PaymentTokenFormat.Stripe && MsPayEmulatedStripeTokenSource.Equals(this.Source, StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Token Source
        /// </summary>
        public string Source => this.source;

        /// <summary>
        /// Parse the token string
        /// </summary>
        /// <param name="tokenstring">token string</param>
        /// <returns>payment token</returns>
        public static PaymentToken Parse(string tokenstring)
        {
            if (string.IsNullOrWhiteSpace(tokenstring))
            {
                throw new ArgumentNullException(nameof(tokenstring));
            }

            var tokenParts = tokenstring.Split('.');

            if (tokenParts.Length != 3)
            {
                throw new ArgumentException("Invalid payment token.");
            }

            return new PaymentToken(ParseHeader(tokenParts[0]), ParseSource(tokenParts[1]), ParseSignature(tokenParts[2]));
        }

        private static PaymentTokenHeader ParseHeader(string headerString)
        {
            var headerBytes = Base64Url.Decode(headerString);
            var headerJson = Encoding.UTF8.GetString(headerBytes);
            return new PaymentTokenHeader(headerJson);
        }

        private static string ParseSource(string sourceString)
        {
            var sourceBytes = Base64Url.Decode(sourceString);
            var sourceText = Encoding.UTF8.GetString(sourceBytes);
            return sourceText;
        }

        private static byte[] ParseSignature(string signatureString)
        {
            return Base64Url.Decode(signatureString);
        }
    }
}
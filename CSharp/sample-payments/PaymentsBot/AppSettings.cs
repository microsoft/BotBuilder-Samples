namespace PaymentsBot
{
    using System;
    using System.Configuration;

    /// <summary>
    /// App Settings
    /// </summary>
    public static class AppSettings
    {
        private static Lazy<string> invalidShippingCountry = new Lazy<string>(() => ConfigurationManager.AppSettings["InvalidShippingCountry"]);

        private static Lazy<bool> liveMode = new Lazy<bool>(() => bool.TrueString.Equals(ConfigurationManager.AppSettings["LiveMode"], StringComparison.OrdinalIgnoreCase));

        private static Lazy<string> merchantId = new Lazy<string>(() => ConfigurationManager.AppSettings["MerchantId"]);

        private static Lazy<string> stripeApiKey = new Lazy<string>(() => ConfigurationManager.AppSettings["StripeApiKey"]);

        /// <summary>
        /// Invalid shipping country
        /// </summary>
        public static string InvalidShippingCountry => invalidShippingCountry.Value;

        /// <summary>
        /// Live Mode value
        /// </summary>
        public static bool LiveMode => liveMode.Value;

        /// <summary>
        /// Merchant Id
        /// </summary>
        public static string MerchantId => merchantId.Value;

        /// <summary>
        /// Stripe Api Key
        /// </summary>
        public static string StripeApiKey => stripeApiKey.Value;
    }
}
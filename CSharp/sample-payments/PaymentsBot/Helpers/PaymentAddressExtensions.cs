namespace PaymentsBot.Helpers
{
    using System.Linq;
    using Microsoft.Bot.Connector.Payments;

    public static class PaymentAddressExtensions
    {
        public static string FullInline(this PaymentAddress address)
        {
            var addressLine = address.AddressLine.Count > 0 ? address.AddressLine.First() : "Address not provided";

            return $"{addressLine}, {address.City} {address.Region}, {address.Country}\n{address.PostalCode}";
        }
    }
}
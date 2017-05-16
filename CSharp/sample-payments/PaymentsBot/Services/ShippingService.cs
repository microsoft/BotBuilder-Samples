namespace PaymentsBot.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Helpers;
    using Microsoft.Bot.Connector.Payments;
    using Models;
    using Properties;

    public class ShippingService
    {
        private static readonly IEnumerable<PaymentShippingOption> FakeShippingOptionsRepository = new List<PaymentShippingOption>()
        {
            new PaymentShippingOption
            {
                Id = "STANDARD",
                Label = "Standard - (5-6 Business days)",
                Amount = PaymentItemBuilder.BuildPaymentAmount(0),
                Selected = true
            },
            new PaymentShippingOption
            {
                Id = "EXPEDITED",
                Label = "Expedited - (2 Business days)",
                Amount = PaymentItemBuilder.BuildPaymentAmount(2.50)
            },
            new PaymentShippingOption
            {
                Id = "INTERNATIONAL",
                Label = "International",
                Amount = PaymentItemBuilder.BuildPaymentAmount(25)
            }
        };


        private static readonly IEnumerable<PaymentShippingOption> FakeUSShippingOptions = FakeShippingOptionsRepository.Take(2);

        private static readonly IEnumerable<PaymentShippingOption> FakeInternationalShippingOptions = FakeShippingOptionsRepository.Skip(2);

        public async Task<PaymentShippingOption> GetShippingOptionAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            return await Task.FromResult(FakeShippingOptionsRepository.FirstOrDefault(option => id.Equals(option.Id, StringComparison.OrdinalIgnoreCase)));
        }

        public async Task<IEnumerable<PaymentShippingOption>> GetShippingOptionsAsync(CatalogItem item, PaymentAddress address)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (address == null)
            {
                throw new ArgumentNullException(nameof(address));
            }

            if (string.IsNullOrWhiteSpace(address.Country))
            {
                throw new ArgumentNullException(nameof(address.Country));
            }

            var shippingOptions = new List<PaymentShippingOption>();

            // if shipping country is not supported then throw
            if (AppSettings.InvalidShippingCountry.Equals(address.Country))
            {
                throw new Exception(Resources.RootDialog_Invalid_Address);
            }

            // shipping options within US are first two
            if (address.Country == "US")
            {
                shippingOptions.AddRange(FakeUSShippingOptions);
            }
            else
            {
                shippingOptions.AddRange(FakeInternationalShippingOptions);
            }

            return await Task.FromResult(shippingOptions);
        }

        public Task UpdatePaymentDetailsAsync(PaymentDetails details, PaymentAddress address, CatalogItem item)
        {
            var shippingOption = details.ShippingOptions.FirstOrDefault(o => o.Selected.HasValue && o.Selected.Value);
            var shippingTotal = shippingOption != null ? Convert.ToDouble(shippingOption.Amount.Value) : 0;

            var taxTotal = (item.Price + shippingTotal) * ShippingService.GetTaxPercentage(address);

            // update items
            var pending = shippingOption == null;
            details.DisplayItems = new List<PaymentItem>
            {
                PaymentItemBuilder.BuildPaymentItem(item.Title, item.Price),
                PaymentItemBuilder.BuildPaymentItem(Resources.Wallet_Label_Shipping, shippingTotal, pending),
                PaymentItemBuilder.BuildPaymentItem(Resources.Wallet_Label_Tax, taxTotal, pending)
            };

            // update total
            var total = item.Price + shippingTotal + taxTotal;
            details.Total = PaymentItemBuilder.BuildPaymentItem(Resources.Wallet_Label_Total, total, pending);

            return Task.FromResult(true);
        }

        private static double GetTaxPercentage(PaymentAddress address)
        {
            return address.Country == "US" ? 0.085 : 0;
        }
    }
}
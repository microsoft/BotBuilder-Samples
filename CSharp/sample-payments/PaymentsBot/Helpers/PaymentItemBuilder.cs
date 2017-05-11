using Microsoft.Bot.Connector.Payments;

namespace PaymentsBot.Helpers
{
    public class PaymentItemBuilder
    {
        public static PaymentCurrencyAmount BuildPaymentAmount(double amount)
        {
            return new PaymentCurrencyAmount
            {
                Currency = "USD",
                Value = amount.ToString("F")
            };
        }

        public static PaymentItem BuildPaymentItem(string label, double amount, bool pending = false)
        {
            return new PaymentItem
            {
                Amount = BuildPaymentAmount(amount),
                Label = label,
                Pending = pending
            };
        }
    }
}
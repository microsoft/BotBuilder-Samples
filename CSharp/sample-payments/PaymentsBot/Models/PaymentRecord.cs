namespace PaymentsBot.Models
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Bot.Connector.Payments;

    public class PaymentRecord
    {
        public Guid OrderId { get; set; }

        public Guid TransactionId { get; set; }

        public string MethodName { get; set; }

        public string PaymentProcessor { get; set; }

        public PaymentAddress ShippingAddress { get; set; }

        public string ShippingOption { get; set; }

        public IEnumerable<PaymentItem> Items { get; set; }

        public PaymentItem Total { get; set; }

        public bool LiveMode { get; set; }
    }
}
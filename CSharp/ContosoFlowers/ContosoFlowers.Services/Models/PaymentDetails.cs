namespace ContosoFlowers.Services.Models
{
    using System;

    [Serializable]
    public class PaymentDetails
    {
        public string CreditCardHolder { get; set; }

        public string CreditCardNumber { get; set; }
    }
}
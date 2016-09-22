namespace ContosoFlowers.Services.Models
{
    using System;

    [Serializable]
    public class Order
    {
        public string OrderID { get; set; }

        public string RecipientFirstName { get; set; }

        public string RecipientLastName { get; set; }

        public string RecipientPhoneNumber { get; set; }

        public string Note { get; set; }

        public string SenderEmail { get; set; }

        public string SenderPhoneNumber { get; set; }

        public string SenderFirstName { get; set; }

        public string SenderLastName { get; set; }

        public bool SaveSenderInfo { get; set; }

        public string DeliveryAddress { get; set; }

        public string FlowerCategoryName { get; set; }

        public Bouquet Bouquet { get; set; }

        public DateTime DeliveryDate { get; set; }

        public string BillingAddress { get; set; }

        public bool Payed { get; set; }

        public PaymentDetails PaymentDetails { get; set; }
    }
}

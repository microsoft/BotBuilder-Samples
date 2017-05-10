namespace PaymentsBot.Models
{
    using System;

    public class CatalogItem
    {
        public string Currency { get; set; }

        public string Description { get; set; }

        public Guid Id { get; set; }

        public string ImageUrl { get; set; }

        public double Price { get; set; }

        public string Title { get; set; }
    }
}
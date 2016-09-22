namespace ContosoFlowers.Services.Models
{
    using System;

    [Serializable]
    public class Bouquet
    {
        public string Name { get; set; }

        public string ImageUrl { get; set; }

        public double Price { get; set; }

        public string FlowerCategory { get; set; }
    }
}
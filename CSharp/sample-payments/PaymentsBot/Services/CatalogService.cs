namespace PaymentsBot.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Models;

    public class CatalogService
    {
        private static readonly IEnumerable<CatalogItem> FakeCatalogRepository = new List<CatalogItem>
        {
            new CatalogItem
            {
                Currency = "USD",
                Description = "Shiny red, ready to rock on Keynotes",
                Id = new Guid("bc861179-46a5-4645-a249-7eba2a4d9846"),
                ImageUrl = "https://pbs.twimg.com/profile_images/565139568/redshirt_400x400.jpg",
                Price = 1.99,
                Title = "Scott Gu - Favorite Shirt"
            }
        };

        public Task<CatalogItem> GetItemByIdAsync(Guid itemId)
        {
            return Task.FromResult(FakeCatalogRepository.FirstOrDefault(o => o.Id.Equals(itemId)));
        }

        public Task<CatalogItem> GetRandomItemAsync()
        {
            // getting a random item - currently we have only one choice :p
            return Task.FromResult(FakeCatalogRepository.First());
        }
    }
}
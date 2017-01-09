namespace ContosoFlowers.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Models;

    public class InMemoryBouquetRepository : InMemoryRepositoryBase<Bouquet>
    {
        private IEnumerable<Bouquet> bouquets;

        public InMemoryBouquetRepository()
        {
            this.bouquets = Enumerable.Range(1, 50)
                .Select(i => new Bouquet
                {
                    Name = $"Bouquet {i}\u2122",
                    ImageUrl = $"https://placeholdit.imgix.net/~text?txtsize=48&txt={HttpUtility.UrlEncode("Bouquet " + i)}&w=640&h=330",
                    Price = new Random(i).Next(10, 100) + .99,
                    
                    // randomizing the flower category but ensuring at least 1 bouquet for each of it.
                    FlowerCategory = (i <= 5) ? $"Flower {i}" : "Flower " + new Random(i).Next(1, 5) 
                });
        }

        public override Bouquet GetByName(string name)
        {
            return this.bouquets.SingleOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }

        protected override IEnumerable<Bouquet> Find(Func<Bouquet, bool> predicate)
        {
            return predicate != default(Func<Bouquet, bool>) ? this.bouquets.Where(predicate) : this.bouquets;
        }
    }
}
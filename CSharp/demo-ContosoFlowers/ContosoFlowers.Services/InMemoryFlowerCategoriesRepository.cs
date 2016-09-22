namespace ContosoFlowers.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Models;

    public class InMemoryFlowerCategoriesRepository : InMemoryRepositoryBase<FlowerCategory>
    {
        private IEnumerable<FlowerCategory> flowerCategories;

        public InMemoryFlowerCategoriesRepository()
        {
            this.flowerCategories = Enumerable.Range(1, 5)
                .Select(i => new FlowerCategory
                {
                    Name = $"Flower {i}",
                    ImageUrl = $"https://placeholdit.imgix.net/~text?txtsize=48&txt={HttpUtility.UrlEncode("Flower " + i)}&w=640&h=330"
                });
        }

        public override FlowerCategory GetByName(string name)
        {
            return this.flowerCategories.SingleOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }

        protected override IEnumerable<FlowerCategory> Find(Func<FlowerCategory, bool> predicate)
        {
            return predicate != default(Func<FlowerCategory, bool>) ? this.flowerCategories.Where(predicate) : this.flowerCategories;
        }
    }
}
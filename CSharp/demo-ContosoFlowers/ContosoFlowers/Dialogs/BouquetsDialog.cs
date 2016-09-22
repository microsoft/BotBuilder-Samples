namespace ContosoFlowers.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using BotAssets.Dialogs;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using Properties;
    using Services;
    using Services.Models;

    [Serializable]
    public class BouquetsDialog : PagedCarouselDialog<Bouquet>
    {
        private readonly string flowerCategory;

        private readonly IRepository<Bouquet> repository;

        public BouquetsDialog(string flowerCategory, IRepository<Bouquet> repository)
        {
            this.flowerCategory = flowerCategory;
            this.repository = repository;
        }

        public override string Prompt
        {
            get { return string.Format(CultureInfo.CurrentCulture, Resources.BouquetsDialog_Prompt, this.flowerCategory); }
        }

        public override PagedCarouselCards GetCarouselCards(int pageNumber, int pageSize)
        {
            var pagedResult = this.repository.RetrievePage(pageNumber, pageSize, (bouquet) => bouquet.FlowerCategory == this.flowerCategory);

            var carouselCards = pagedResult.Items.Select(it => new HeroCard
            {
                Title = it.Name,
                Subtitle = it.Price.ToString("C"),
                Images = new List<CardImage> { new CardImage(it.ImageUrl, it.Name) },
                Buttons = new List<CardAction> { new CardAction(ActionTypes.ImBack, Resources.BouquetsDialog_Select, value: it.Name) }
            });

            return new PagedCarouselCards
            {
                Cards = carouselCards,
                TotalCount = pagedResult.TotalCount
            };
        }

        public override async Task ProcessMessageReceived(IDialogContext context, string bouquetName)
        {
            var bouquet = this.repository.GetByName(bouquetName);

            if (bouquet != null)
            {
                context.Done(bouquet);
            }
            else
            {
                await context.PostAsync(string.Format(CultureInfo.CurrentCulture, Resources.BouquetsDialog_InvalidOption, bouquetName));
                await this.ShowProducts(context);
                context.Wait(this.MessageReceivedAsync);
            }
        }
    }
}
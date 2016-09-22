namespace Search.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Internals.Fibers;
    using Search.Models;
    using Search.Services;

    [Serializable]
    public class SearchRefineDialog : IDialog<string>
    {
        protected readonly string Refiner;
        protected readonly SearchQueryBuilder QueryBuilder;
        protected readonly PromptStyler PromptStyler;
        protected readonly string Prompt;
        protected readonly ISearchClient SearchClient;

        public SearchRefineDialog(ISearchClient searchClient, string refiner, SearchQueryBuilder queryBuilder = null, PromptStyler promptStyler = null, string prompt = null)
        {
            SetField.NotNull(out this.SearchClient, nameof(searchClient), searchClient);
            SetField.NotNull(out this.Refiner, nameof(refiner), refiner);

            this.QueryBuilder = queryBuilder ?? new SearchQueryBuilder();
            this.PromptStyler = promptStyler;
            this.Prompt = prompt ?? $"Here's what I found for {this.Refiner}.";
        }

        public async Task StartAsync(IDialogContext context)
        {
            var result = await this.SearchClient.SearchAsync(this.QueryBuilder, this.Refiner);

            IEnumerable<string> options = result.Facets[this.Refiner].Select(f => this.FormatRefinerOption((string)f.Value, f.Count));

            var promptOptions = new CancelablePromptOptions<string>(this.Prompt, cancelPrompt: "Type 'cancel' if you don't want to select any of these.", options: options.ToList(), promptStyler: this.PromptStyler);
            CancelablePromptChoice<string>.Choice(context, this.ApplyRefiner, promptOptions);
        }

        public async Task ApplyRefiner(IDialogContext context, IAwaitable<string> input)
        {
            string selection = await input;

            if (selection == null)
            {
                context.Done<string>(null);
            }
            else
            {
                string value = this.ParseRefinerValue(selection);

                if (this.QueryBuilder != null)
                {
                    await context.PostAsync($"Filtering by {this.Refiner}: {value}");
                    this.QueryBuilder.Refinements.Add(this.Refiner, new string[] { value });
                }

                context.Done(value);
            }
        }

        protected virtual string FormatRefinerOption(string value, long count)
        {
            return $"{value} ({count})";
        }

        protected virtual string ParseRefinerValue(string value)
        {
            return value.Substring(0, value.LastIndexOf('(') - 1);
        }
    }
}

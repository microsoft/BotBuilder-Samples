namespace Search.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Internals.Fibers;
    using Search.Models;

    [Serializable]
    public class SearchSelectRefinerDialog : IDialog<string>
    {
        protected readonly SearchQueryBuilder QueryBuilder;
        protected readonly IEnumerable<string> Refiners;
        protected readonly PromptStyler PromptStyler;

        public SearchSelectRefinerDialog(IEnumerable<string> refiners, SearchQueryBuilder queryBuilder = null, PromptStyler promptStyler = null)
        {
            SetField.NotNull(out this.Refiners, nameof(refiners), refiners);

            this.QueryBuilder = queryBuilder ?? new SearchQueryBuilder();
            this.PromptStyler = promptStyler;
        }

        public async Task StartAsync(IDialogContext context)
        {
            IEnumerable<string> unusedRefiners = this.Refiners;
            if (this.QueryBuilder != null)
            {
                unusedRefiners = unusedRefiners.Except(this.QueryBuilder.Refinements.Keys, StringComparer.OrdinalIgnoreCase);
            }

            if (unusedRefiners.Any())
            {
                var promptOptions = new CancelablePromptOptions<string>("What do you want to refine by?", cancelPrompt: "Type 'cancel' if you changed your mind.", options: unusedRefiners.ToList(), promptStyler: this.PromptStyler);
                CancelablePromptChoice<string>.Choice(context, this.ReturnSelection, promptOptions);
            }
            else
            {
                await context.PostAsync("Oops! You used all the available refiners and you cannot refine the results anymore.");
                context.Done<string>(null);
            }
        }

        protected virtual async Task ReturnSelection(IDialogContext context, IAwaitable<string> input)
        {
            var selection = await input;

            context.Done(selection);
        }
    }
}

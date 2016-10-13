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
    using System.Text.RegularExpressions;
    [Serializable]
    public class SearchRefineDialog : IDialog<FilterExpression>
    {
        protected readonly string Refiner;
        protected double RangeMin;
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
            this.QueryBuilder.Refinements.Remove(refiner);
        }

        public async Task StartAsync(IDialogContext context)
        {
            var result = await this.SearchClient.SearchAsync(this.QueryBuilder, this.Refiner);
            List<string> options = new List<string>();
            List<string> descriptions = new List<string>();
            var choices = (from facet in result.Facets[this.Refiner] orderby facet.Value ascending select facet);
            var schema = SearchClient.Schema[Refiner];
            if (schema.FilterPreference == PreferredFilter.None)
            {
                foreach (var choice in choices)
                {
                    options.Add((string) choice.Value);
                    descriptions.Add($"{choice.Value} ({choice.Count})");
                }
            }
            else if (schema.FilterPreference == PreferredFilter.MinValue)
            {
                var total = choices.Sum((choice) => choice.Count);
                foreach (var choice in choices)
                {
                    options.Add($"{choice.Value}+");
                    descriptions.Add($"{choice.Value}+ ({total})");
                    total -= choice.Count;
                }
            }
            else if (schema.FilterPreference == PreferredFilter.MaxValue)
            {
                long total = 0;
                foreach (var choice in choices)
                {
                    total += choice.Count;
                    options.Add($"<= {choice.Value}");
                    descriptions.Add($"<= {choice.Value} ({total})");
                }
            }
            if (options.Any())
            {
                options.Add("Any");
                descriptions.Add("Any");
                PromptOptions<string> promptOptions = new PromptOptions<string>(this.Prompt, retry: "I did not understand, try one of these choices:", options: options.ToList(), promptStyler: this.PromptStyler, descriptions:descriptions);
                PromptDialog.Choice(context, ApplyRefiner, promptOptions);
            }
            else if (schema.FilterPreference == PreferredFilter.RangeMin)
            {
                PromptDialog.Number(context, MinRefiner, $"What is the minimum {this.Refiner}?");
            }
            else if (schema.FilterPreference == PreferredFilter.RangeMax)
            {
                PromptDialog.Number(context, MinRefiner, $"What is the maximum {this.Refiner}?");
            }
            else if (schema.FilterPreference == PreferredFilter.Range)
            {
                PromptDialog.Number(context, GetRangeMin, $"What is the minimum {this.Refiner}?");
            }
        }

        public async Task MinRefiner(IDialogContext context, IAwaitable<double> number)
        {
            var expression = new FilterExpression(Operator.GreaterThanOrEqual, await number);
            this.QueryBuilder.Refinements.Add(this.Refiner, expression);
            context.Done<FilterExpression>(expression);
        }

        public async Task MaxRefiner(IDialogContext context, IAwaitable<double> number)
        {
            var expression = new FilterExpression(Operator.LessThanOrEqual, await number);
            this.QueryBuilder.Refinements.Add(this.Refiner, expression);
            context.Done<FilterExpression>(expression);
        }

        public async Task GetRangeMin(IDialogContext context, IAwaitable<double> min)
        {
            RangeMin = await min;
            PromptDialog.Number(context, GetRangeMax, $"What is the maximum {this.Refiner}?");
        }

        public async Task GetRangeMax(IDialogContext context, IAwaitable<double> max)
        {
            var expression = new FilterExpression(Operator.And, new FilterExpression(Operator.GreaterThanOrEqual, RangeMin),
                new FilterExpression(Operator.LessThanOrEqual, await max));
            this.QueryBuilder.Refinements.Add(this.Refiner, expression);
            context.Done(expression);
        }

        // Handles 3+, <=5 and 3-5.
        private static Regex extractValue = new Regex(@"(?<lt>\<\s*\=)?\s*(?<number1>[+-]?[0-9]+(.[0-9]+)?)\s*((?<gt>\+)|(-\s*(?<number2>[+-]?[0-9]+(.[0-9]+)?)))?", RegexOptions.Compiled);

        protected virtual FilterExpression ParseRefinerValue(string value)
        {
            var expression = new FilterExpression();
            var field = this.SearchClient.Schema[this.Refiner];
            if (field.Type == typeof(string) || field.Type == typeof(string[]))
            {
                expression = new FilterExpression(Operator.Equal, value);
            }
            else
            {
                var match = extractValue.Match(value);
                if (!match.Success)
                {
                    expression = new FilterExpression();
                }
                else
                {
                    var lt = match.Groups["lt"];
                    var gt = match.Groups["gt"];
                    var number1 = match.Groups["number1"];
                    var number2 = match.Groups["number2"];
                    if (number1.Success)
                    {
                        double num1;
                        if (double.TryParse(number1.Value, out num1))
                        {
                            if (lt.Success)
                            {
                                expression = new FilterExpression(Operator.LessThanOrEqual, num1);
                            }
                            else if (gt.Success)
                            {
                                expression = new FilterExpression(Operator.GreaterThanOrEqual, num1);
                            }
                            else if (number2.Success)
                            {
                                double num2;
                                if (double.TryParse(number2.Value, out num2) && num1 <= num2)
                                {
                                    expression = new FilterExpression(Operator.And,
                                            new FilterExpression(Operator.GreaterThanOrEqual, num1),
                                            new FilterExpression(Operator.LessThanOrEqual, num2));
                                }
                            }
                        }
                    }
                }
            }
            return expression;
        }

        public async Task ApplyRefiner(IDialogContext context, IAwaitable<string> input)
        {
            string selection = await input;

            if (selection != null)
            {
                if (selection.Trim().ToLowerInvariant() == "any")
                {
                    this.QueryBuilder.Refinements.Remove(this.Refiner);
                    context.Done<FilterExpression>(null);
                }
                else
                {
                    var expression = ParseRefinerValue(selection);
                    if (expression.Operator != Operator.None)
                    {
                        this.QueryBuilder.Refinements.Add(this.Refiner, expression);
                        context.Done(expression);
                    }
                }
            }
        }
    }
}

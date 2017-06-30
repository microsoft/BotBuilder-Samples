using Microsoft.Bot.Builder.Dialogs;
using Search.Azure;
using Search.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Search.Dialogs.UserInteraction
{
    public enum ResourceType
    {
        AddedToListPrompt,          // 0-ItemID
        AddKeywordPrompt,
        AddOrRemoveKeywordPrompt,
        ClearPrompt,
        FacetPrompt,
        FacetValuePrompt,           // 0-ItemID
        InitialPrompt,
        ListPrompt,
        NoResultsPrompt,            // 0-Filter
        NotAddedPrompt,
        NotUnderstoodPrompt,
        NoValuesPrompt,
        RefinePrompt,
        RemovedFromListPrompt,      // 0-ItemID
        UnknownItemPrompt,
        // Status messages
        Ascending,
        Count,                      // 0-Count
        Descending,
        Filter,                     // 0-Filter
        Keywords,                   // 0-Keywords
        Page,                       // 0-Page#
        Selected,                   // 0-Number selected
        Sort                        // 0-Sort
    };

    public enum ButtonType
    {
        // Buttons
        Add,                // 0-ItemID
        Any,                // 0-Field
        AnyNumber,          // 0-Field
        Clear,
        ContinueSearch,
        Finished,
        Keyword,
        List,
        NextPage,
        Refine,
        Remove,             // 0-ItemID
        RemoveKeyword,      // 0-Keyword
        Select,             // 0-ItemID
        StartOver
    };

    public enum FieldType
    {
        // 0-Field, 1-Typical, 2-Min, 3-Max, 4-Most common, 5-Example Min, 6-Example Max
        IntroHint,
        FilterPrompt,
    };

    public interface IResource
    {
        Button ButtonResource(ButtonType type, params object[] parameters);
        string Resource(ResourceType type, params object[] parameters);
        string FieldResource(FieldType type, string field, SearchSchema schema, IDialogContext context);
    }

    [Serializable]
    public class ResourceGenerator : IResource
    {
        private readonly Prompts _prompts;
        private readonly Random _random = new Random();
        [NonSerialized]
        private readonly HumanNumberFormatter _formatter;

        public ResourceGenerator(Prompts prompts)
        {
            _prompts = prompts;
            _formatter = new HumanNumberFormatter(prompts);
        }

        public Button ButtonResource(ButtonType type, params object[] parameters)
        {
            var button = (Button)_prompts.GetType().GetField(type.ToString()).GetValue(_prompts);
            if (parameters.Length > 0)
            {
                button = new Button(string.Format(button.Label, parameters),
                    button.Message != null ? string.Format(button.Message, parameters) : null);
            }
            return button;
        }

        private void Examples(SearchField field, out double typical, out double min, out double max)
        {
            var examples = field.Examples;
            typical = double.Parse(examples.First());
            min = double.MaxValue;
            max = double.MinValue;
            foreach (var example in examples.Skip(1))
            {
                double val;
                if (double.TryParse(example, out val))
                {
                    if (val < min) min = val;
                    if (val > max) max = val;
                }
            }
        }

        private string FieldHint(SearchField field, bool single = false)
        {
            string hint = null;
            if (field.ValueSynonyms.Any())
            {
                var prompt = _prompts.ValueHints[_random.Next(_prompts.ValueHints.Length)];
                var examples = new List<string>();
                foreach (var synonym in field.ValueSynonyms)
                {
                    if (field.Examples.Contains(synonym.Canonical))
                    {
                        examples.Add(synonym.Alternatives[_random.Next(synonym.Alternatives.Length)]);
                        if (single)
                        {
                            break;
                        }
                    }
                }
                var builder = new StringBuilder();
                var count = 0;
                foreach (var example in examples)
                {
                    if (count > 0)
                    {
                        builder.Append(", ");
                        if (count == examples.Count() - 1)
                        {
                            builder.Append(_prompts.Or);
                            builder.Append(' ');
                        }
                    }
                    if (!single) builder.Append('"');
                    builder.Append(example);
                    if (!single) builder.Append('"');   
                    ++count;
                }
                if (single)
                {
                    hint = builder.ToString();
                }
                else
                {
                    hint = string.Format(prompt, field.Description(), builder.ToString());
                }
            }
            else if (field.IsMoney)
            {
                double typical, min, max;
                Examples(field, out typical, out min, out max);
                var prompt = _prompts.MoneyHints[_random.Next(_prompts.MoneyHints.Length)];
                hint = string.Format(_formatter, prompt, field.Description(), typical, min, max);
            }
            else if (field.Type.IsNumeric())
            {
                double typical, min, max;
                Examples(field, out typical, out min, out max);
                var prompt = _prompts.NumberHints[_random.Next(_prompts.NumberHints.Length)];
                hint = string.Format(_formatter, prompt, field.Description(), typical, min, max);
            }
            return hint;
        }

        public string FieldResource(FieldType type, string fieldName, SearchSchema schema, IDialogContext context)
        {
            string prompt = null;
            if (type == FieldType.IntroHint)
            {
                // Randomly select field of each type
                var numbers = (from field in schema.Fields.Values where field.Type.IsNumeric() && !field.IsMoney select field).ToArray();
                var values = (from field in schema.Fields.Values where field.ValueSynonyms.Any() select field).ToArray();
                var money = (from field in schema.Fields.Values where field.IsMoney select field).ToArray();
                var singleBuilder = new StringBuilder();
                var compositeBuilder = new StringBuilder();
                if (numbers.Any())
                {
                    var field = numbers[_random.Next(numbers.Length)];
                    singleBuilder.Append("* ");
                    singleBuilder.AppendLine(FieldHint(field));
                    singleBuilder.AppendLine();
                    field = numbers[_random.Next(numbers.Length)];
                    compositeBuilder.Append(FieldHint(field));
                }
                if (values.Any())
                {
                    var field = values[_random.Next(values.Length)];
                    singleBuilder.Append("* ");
                    singleBuilder.AppendLine(FieldHint(field));
                    singleBuilder.AppendLine();
                    field = values[_random.Next(values.Length)];
                    if (compositeBuilder.Length > 0)
                    {
                        compositeBuilder.Append(' ');
                    }
                    compositeBuilder.Append(FieldHint(field, true));
                }
                if (money.Any())
                {
                    var field = money[_random.Next(money.Length)];
                    singleBuilder.Append("* ");
                    singleBuilder.AppendLine(FieldHint(field));
                    singleBuilder.AppendLine();
                    field = money[_random.Next(money.Length)];
                    if (compositeBuilder.Length > 0)
                    {
                        compositeBuilder.Append(' ');
                    }
                    compositeBuilder.Append(FieldHint(field, true));
                }
                prompt = string.Format(_prompts.IntroHint, 
                    Environment.NewLine + Environment.NewLine + singleBuilder.ToString(), 
                    Environment.NewLine + Environment.NewLine + "* " + compositeBuilder.ToString());
            }
            else
            {
                var field = schema.Field(fieldName);
                if (field.IsMoney)
                {
                    double typical, min, max;
                    prompt = (string)_prompts.GetType().GetField(type.ToString() + "Money").GetValue(_prompts);
                    Examples(field, out typical, out min, out max);
                    prompt = string.Format(prompt, field.Description(), field.Min, field.Max, typical, min, max);
                }
                else if (field.Type.IsNumeric())
                {
                    double typical, min, max;
                    prompt = (string)_prompts.GetType().GetField(type.ToString() + "Number").GetValue(_prompts);
                    Examples(field, out typical, out min, out max);
                    prompt = string.Format(prompt, field.Description(), field.Min, field.Max, typical, min, max);
                }
                else
                {
                    var typical = field.Examples.FirstOrDefault();
                    prompt = (string)_prompts.GetType().GetField(type.ToString() + "String").GetValue(_prompts);
                    prompt = string.Format(prompt, field.Description(), typical);
                }
            }
            return prompt;
        }

        public string Resource(ResourceType type, params object[] parameters)
        {
            var prompt = (string)_prompts.GetType().GetField(type.ToString()).GetValue(_prompts);
            if (parameters.Length > 0)
            {
                prompt = string.Format(prompt, parameters);
            }
            return prompt;
        }
    }

    public class HumanNumberFormatter : ICustomFormatter, IFormatProvider
    {
        private Prompts _prompts;
        public HumanNumberFormatter(Prompts prompts)
        {
            _prompts = prompts;
        }

        public object GetFormat(Type formatType)
        {
            return (formatType == typeof(ICustomFormatter)) ? this : null;
        }

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (format == null || !format.Trim().StartsWith("H"))
            {
                if (arg is IFormattable)
                {
                    return ((IFormattable)arg).ToString(format, formatProvider);
                }
                return arg.ToString();
            }

            int precision;
            int.TryParse(format.Trim().Substring(1), out precision);

            decimal value = Convert.ToDecimal(arg);
            string units = "";
            if (value >= 1000)
            {
                value = value / 1000;
                units = _prompts.Thousand;
            }
            else if (value >= 1000000)
            {
                value = value / 10000000;
                units = " " + _prompts.Million;
            }
            else if (value >= 1000000000)
            {
                value = (value / 10000000000);
                units = " " + _prompts.Billion;
            }
            else if (value >= 1000000000000)
            {
                value = (value / 10000000000000);
                units = " " + _prompts.Trillion;
            }
            else if (value >= 1000000000000000)
            {
                value = (value / 1000000000000000);
                units = " " + _prompts.Quadrillion;
            }
            return value.ToString("n" + precision.ToString()) + units;
        }
    }

    [Serializable]
    public class Prompts
    {
        // Prompts
        public string AddedToListPrompt = "{0} was added to your list.";
        public string AddKeywordPrompt = "Type a keyword phrase to search for.";
        public string AddOrRemoveKeywordPrompt = "Type a keyword phrase to search for, or select what you would like to remove.";
        public string ClearPrompt = "Cleared all selections from your list.";
        public string FacetPrompt = "What would you like to refine your search by?";
        public string FacetValuePrompt = "What value for {0} would you like to filter by?";
        public string FilterPromptNumber = "{0} can have values between {1:n0} and {2:n0}.  Enter a filter like \"between {4:n0} and {5:n0}\".";
        public string FilterPrompString = "Enter a value for {0} like \"{3}\".";
        public string FilterPromptMoney = "{0} can have values between ${1:H} and ${2:H}.  Enter a filter like \"no more than ${4:H}\".";
        public string InitialPrompt = "Please describe in your own words what you would like to find?";
        public string ListPrompt = "Here is what you have selected so far.";
        public string NoResultsPrompt = "{0}Found no results so I undid your last change.  You can refine again.";
        public string NotAddedPrompt = "You have not added anything yet.";
        public string NotUnderstoodPrompt = "I did not understand what you said.";
        public string NoValuesPrompt = "There are no values to filter by for {0}.";
        public string RefinePrompt = "Refine your search or select an operation.";
        public string RemovedFromListPrompt = "{0} was removed from your list.";
        public string UnknownItemPrompt = "That is not an item in the current results.";

        // Buttons
        public Button Add = new Button("Add to list", "ADD:{0}");
        public Button Any = new Button("Any", "Any {0}");
        public Button AnyNumber = new Button("Any", "Any number of {0}");
        public Button Clear = new Button("Clear list", "clear list");
        public Button ContinueSearch = new Button("Continue searching", "search");
        public Button Finished = new Button("Done searching", "done");
        public Button Keyword = new Button("Keyword", "keyword");
        public Button List = new Button("Show list", "list");
        public Button NextPage = new Button("More results", "more results");
        public Button Refine = new Button("Help me search", "help");
        public Button Remove = new Button("Remove from list", "REMOVE:{0}");
        public Button RemoveKeyword = new Button("{0}", "remove {0}");
        public Button Select = new Button("Select", "ADD:{0}");
        public Button StartOver = new Button("Clear search");

        // Status
        public string Ascending = "Ascending";
        public string Count = "**Total results**: {0}";
        public string Descending = "Descending";
        public string Filter = "**Filter**: {0}";
        public string Keywords = "**Keywords**: {0}";
        public string Page = "**Page**: {0}";
        public string Selected = "**Kept so far**: {0}";
        public string Sort = "**Sort**: {0}";

        // Hints
        // 0-hints one per-line, 1-composite hint
        public string IntroHint = "Some of the things I understand include: {0}You can also combine multiple restrictions: {1}";
        public string[] NumberHints = new string[]
        {
            // 0-Field, 1-Typical, 2-MinExample, 3-MaxExample
            "at least {1:n0} {0}",
            "no more than {1:n0} {0}",
            "between {2:n0} and {3:n0} {0}",
            "{1:n0}+ {0}",
            "{2:n0}-{3:n0} {0}",
            "any number of {0}"
        };

        public string[] MoneyHints = new string[]
        {
            // 0-Field, 1-Typical, 2-MinExample, 3-MaxExample
            "{0} is less than ${1:H}",
            "{0} is more than ${1:H}",
            "more than ${1:H}",
            "less than ${1:H}",
            "at least ${1:H}",
            "between ${2:H} and ${3:H}",
            "${2:H}-${3:H}",
            "any {0}"
        };

        public string[] ValueHints = new string[]
        {
            // 0-Field, 1-list of possible values
            "{1} for {0}"
        };

        public string Or = "or";

        // Units
        public string Thousand = "k";
        public string Million = "million";
        public string Billion = "billion";
        public string Trillion = "trillion";
        public string Quadrillion = "quadrillion";
    }
}
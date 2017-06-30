using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Bot.Builder.FormFlow.Advanced;
using Microsoft.Bot.Builder.Luis.Models;
using Search.Models;

namespace Search.Dialogs.Tools
{
    public static class Keywords
    {
        public class Range
        {
            public int Start;
            public int End;
            public Range(int start, int end)
            {
                Start = start;
                End = end;
            }
        }

        public static IEnumerable<Range> NonEntityRanges(IEnumerable<EntityRecommendation> entities, EntityRecommendation span)
        {
            var ranges = new List<Range>();
            ranges.Add(new Range(span.StartIndex.Value, span.EndIndex.Value));
            foreach (var entity in entities)
            {
                if (entity.StartIndex.HasValue)
                {
                    // Ignore entity that covers whole range
                    if (entity != span)
                    {

                        int i = 0;
                        while (i < ranges.Count)
                        {
                            var range = ranges[i];
                            if (range.Start > entity.EndIndex)
                            {
                                // No overlap and since ranges are in ascending order we are done
                                break;
                            }
                            else if (range.Start >= entity.StartIndex && range.End <= entity.EndIndex)
                            {
                                // Completely contained by entity, so remove
                                ranges.RemoveAt(i);
                            }
                            else if (range.End < entity.StartIndex)
                            {
                                // Range is before entity
                                ++i;
                            }
                            else
                            {
                                // We have overlap, so replace
                                ranges.RemoveAt(i);
                                if (range.Start < entity.StartIndex)
                                {
                                    ranges.Insert(i, new Range(range.Start, entity.StartIndex.Value - 1));
                                    ++i;
                                }
                                if (range.End > entity.EndIndex)
                                {
                                    ranges.Insert(i, new Range(entity.EndIndex.Value + 1, range.End));
                                    ++i;
                                }
                            }
                        }
                    }
                }
            }
            return ranges;
        }

        public static IEnumerable<string> ExtractPhrases(string originalText, IEnumerable<Range> ranges)
        {
            IEnumerable<string> substrings = new List<string>();
            foreach (var range in ranges)
            {
                var str = originalText.Substring(range.Start, range.End - range.Start + 1);
                substrings = substrings.Union(str.Phrases());
            }
            return substrings;
        }

        public static IEnumerable<string> ExtractPhrases(IEnumerable<EntityRecommendation> entities, string originalText)
        {
            var ranges = new[] { new { start = 0, end = originalText.Length } }.ToList();
            foreach (var entity in entities)
            {
                if (entity.StartIndex.HasValue)
                {
                    int i = 0;
                    while (i < ranges.Count)
                    {
                        var range = ranges[i];
                        if (range.start > entity.EndIndex)
                        {
                            break;
                        }
                        if (range.start == entity.StartIndex)
                        {
                            if (range.end <= entity.EndIndex)
                            {
                                // Completely contained 
                                ranges.RemoveAt(i);
                            }
                            else
                            {
                                // Remove from start
                                ranges.RemoveAt(i);
                                ranges.Insert(i, new { start = entity.EndIndex.Value + 1, end = range.end });
                                ++i;
                            }
                        }
                        else if (range.end == entity.EndIndex)
                        {
                            // Remove from end
                            ranges.RemoveAt(i);
                            ranges.Insert(i, new { start = range.start, end = entity.StartIndex.Value });
                            ++i;
                        }
                        else if (range.start < entity.StartIndex && range.end > entity.EndIndex)
                        {
                            // Split
                            ranges.RemoveAt(i);
                            ranges.Insert(i, new { start = range.start, end = entity.StartIndex.Value });
                            ranges.Insert(++i, new { start = entity.EndIndex.Value + 1, end = range.end });
                            ++i;
                        }
                        else if (range.start > entity.StartIndex && range.end < entity.EndIndex)
                        {
                            // Completely contained
                            ranges.RemoveAt(i);
                        }
                        else
                        {
                            ++i;
                        }
                    }
                }
            }
            IEnumerable<string> substrings = new List<string>();
            foreach (var range in ranges)
            {
                var str = originalText.Substring(range.start, range.end - range.start);
                substrings = substrings.Union(str.Phrases());
            }
            return substrings;
        }

        // Break string into phrases where noise words or punctuation are breaks
        public static IEnumerable<string> Phrases(this string str)
        {
            var phrase = new StringBuilder();
            var words = str.Split(' ');
            foreach (var word in words)
            {
                if (!string.IsNullOrEmpty(word))
                {
                    if (Language.NoiseResponse(word))
                    {
                        if (phrase.Length > 0)
                        {
                            yield return phrase.ToString();
                            phrase = new StringBuilder();
                        }
                    }
                    else if (char.IsPunctuation(word.Last()))
                    {
                        int lastPunc = 0;
                        for (int i = 0; i < word.Length; ++i)
                        {
                            if (char.IsPunctuation(word[i]))
                            {
                                lastPunc = i;
                            }
                        }
                        phrase.Append(word.Substring(0, lastPunc));
                        yield return phrase.ToString();
                        phrase = new StringBuilder();
                    }
                    else
                    {
                        if (phrase.Length > 0)
                        {
                            phrase.Append(' ');
                        }
                        phrase.Append(word);
                    }
                }
            }
            if (phrase.Length > 0)
            {
                yield return phrase.ToString();
            }
        }

        public static FilterExpression GenerateFilterExpression(this IEnumerable<FilterExpression> filters, FilterOperator connector = FilterOperator.And, FilterExpression soFar = null)
        {
            FilterExpression result = soFar;
            foreach (var filter in filters)
            {
                result = FilterExpression.Combine(result, filter, connector);
            }
            return result;
        }
    }
}
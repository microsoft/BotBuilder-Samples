using System.Collections.Generic;
using Search.Models;

namespace Search.Dialogs.Filter
{
    public static class FilterExpressionBuilder
    {
        public static FilterExpression Build(IEnumerable<Range> ranges, FilterOperator connector,
            FilterExpression soFar = null)
        {
            var filter = soFar;
            foreach (var range in ranges)
            {
                var lowercmp = range.IncludeLower ? FilterOperator.GreaterThanOrEqual : FilterOperator.GreaterThan;
                var uppercmp = range.IncludeUpper ? FilterOperator.LessThanOrEqual : FilterOperator.LessThan;
                if (range.Lower is double && double.IsNegativeInfinity((double)range.Lower))
                {
                    if (range.Upper is double && !double.IsPositiveInfinity((double)range.Upper))
                    {
                        filter = FilterExpression.Combine(filter,
                            new FilterExpression(range.Description, uppercmp, range.Property.Name, range.Upper), connector);
                    }
                }
                else if (range.Upper is double && double.IsPositiveInfinity((double)range.Upper))
                {
                    filter = FilterExpression.Combine(filter,
                        new FilterExpression(range.Description, lowercmp, range.Property.Name, range.Lower), connector);
                }
                else if (range.Lower == range.Upper)
                {
                    filter = FilterExpression.Combine(filter,
                        new FilterExpression(range.Description,
                            range.Lower is string && range.Property.IsSearchable ? FilterOperator.FullText : FilterOperator.Equal,
                            range.Property.Name, range.Lower), connector);
                }
                else
                {
                    //Only add the description to the combination to avoid description duplication and limit the tree traversal
                    var child = FilterExpression.Combine(new FilterExpression(lowercmp, range.Property.Name, range.Lower),
                        new FilterExpression(uppercmp, range.Property.Name, range.Upper), FilterOperator.And, range.Description);
                    filter = FilterExpression.Combine(filter, child, connector);
                }
            }
            return filter;
        }
    }
}

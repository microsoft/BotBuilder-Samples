using System.Runtime.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Search.Models
{
    public enum FilterOperator { None, LessThan, LessThanOrEqual, Equal, GreaterThanOrEqual, GreaterThan, And, Or, Not, FullText };

#if !NETSTANDARD1_6
    [Serializable]
#else
    [DataContract]
#endif
    public class FilterExpression
    {
        public FilterOperator Operator { get; }
        public object[] Values { get; }
        public string Description { get; }

        public FilterExpression(string description, FilterOperator op, params object[] values)
        {
            Description = description;
            Operator = op;
            Values = values;
        }

        public FilterExpression(FilterOperator op, params object[] values) : this(null, op, values) { }

        public FilterExpression DeepCopy()
        {
            var values = (from value in Values select value is FilterExpression ? (object)(value as FilterExpression).DeepCopy() : value).ToArray();
            return new FilterExpression(Description, Operator, values);
        }

        // Remove all references to the same field
        public FilterExpression Remove(FilterExpression expression)
        {
            FilterExpression filter = this;
            if (expression != null)
            {
                foreach (var field in expression.Fields())
                {
                    filter = filter.Remove(field);
                    if (filter == null)
                    {
                        break;
                    }
                }
            }
            return filter;
        }

        public IEnumerable<string> Fields()
        {
            return AllFields().Distinct();
        }

        public FilterExpression Remove(string field)
        {
            FilterExpression result = null;
            if (Operator == FilterOperator.And
                || Operator == FilterOperator.Or)
            {
                var child1 = (Values[0] as FilterExpression).Remove(field);
                var child2 = (Values[1] as FilterExpression).Remove(field);
                result = FilterExpression.Combine(child1, child2, Operator, Description);
            }
            else if (Operator == FilterOperator.Not)
            {
                var child = (Values[0] as FilterExpression).Remove(field);
                if (child != null)
                {
                    result = new FilterExpression(Description, FilterOperator.Not, child);
                }
            }
            else
            {
                if ((string) Values[0] != field)
                {
                    result = this;
                }
            }
            return result;
        }

        public static FilterExpression Combine(FilterExpression child1, FilterExpression child2, FilterOperator combination = FilterOperator.And, string description = null)
        {
            FilterExpression filter;
            if (child1 != null)
            {
                if (child2 != null)
                {
                    filter = new FilterExpression(description, combination, child1, child2);
                }
                else
                {
                    filter = child1;
                }
            }
            else
            {
                filter = child2;
            }
            return filter;
        }

        public override bool Equals(object obj)
        {
            var other = obj as FilterExpression;
            return other != null
                && Operator.Equals(other.Operator)
                && Values.SequenceEqual(other.Values)
                && (Description == other.Description || (Description != null && Description.Equals(other.Description)))
                ;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append(Operator.ToString());
            builder.Append("(");
            var seperator = "";
            foreach (var value in Values)
            {
                builder.Append(seperator);
                builder.Append(value.ToString());
                seperator = ", ";
            }
            builder.Append(")");
            return builder.ToString();
        }

        /// <summary>
        /// Obtains a user-friendly description of the filter.
        /// Traverses the expression tree collecting the descriptions from the nodes.
        /// </summary>
        public string ToUserFriendlyString()
        {
            const string spacePrefix = " ";
            var prefix = string.Empty;
            var builder = new StringBuilder();

            VisitExpressionTree(this, expression =>
            {
                //If the expression does not have a description, a descendant should.
                if (string.IsNullOrEmpty(expression.Description))
                    return true;

                builder.Append($"{prefix}\"{expression.Description}\"");

                if (prefix == string.Empty)
                {
                    prefix = spacePrefix;
                }
                //Since we found a description, descendants, if any, should not have a description. We stop traversing this branch.
                return false;
            });
            return builder.ToString();
        }

        private IEnumerable<string> AllFields()
        {
            if (Operator == FilterOperator.And
                || Operator == FilterOperator.Or
                || Operator == FilterOperator.Not)
            {
                foreach(FilterExpression child in Values)
                {
                    foreach(var field in child.Fields())
                    {
                        yield return field;
                    }
                }
            }
            else 
            {
                yield return (string) Values[0];
            }
        }

        /// <summary>
        /// Visits the nodes of the expression tree, and stops recursing a given branch when the visitor returns false.
        /// </summary>
        private static void VisitExpressionTree(FilterExpression node, Func<FilterExpression, bool> expressionVisitor)
        {
            if (node == null) return;

            //We execute the visitor function and keep recursing this branch or not depending on the visitor's result
            bool shouldKeepVisiting = expressionVisitor(node);

            if (shouldKeepVisiting)
            {
                foreach (var value in node.Values)
                {
                    var filter = value as FilterExpression;
                    if (filter != null)
                    {
                        VisitExpressionTree(filter, expressionVisitor);
                    }
                }
            }
        }
    }
}
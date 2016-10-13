using System;
using System.Text;

namespace Search.Models
{
    public enum Operator { None, LessThan, LessThanOrEqual, Equal, GreaterThanOrEqual, GreaterThan, And, Or };

    [Serializable]
    public class FilterExpression
    {
        public readonly Operator Operator;
        public readonly object[] Values;

        public FilterExpression()
        { }

        public FilterExpression(Operator op, params object[] values)
        {
            Operator = op;
            Values = values;
        }

        public static string ToFilter(SearchField field, FilterExpression expression)
        {
            string filter = "";
            if (expression.Values.Length > 0)
            {
                var constant = Constant(expression.Values[0]);
                string op = null;
                bool connective = false;
                switch (expression.Operator)
                {
                    case Operator.LessThan: op = "lt"; break;
                    case Operator.LessThanOrEqual: op = "le"; break;
                    case Operator.Equal: op = "eq"; break;
                    case Operator.GreaterThan: op = "gt"; break;
                    case Operator.GreaterThanOrEqual: op = "ge"; break;
                    case Operator.Or: op = "or"; connective = true; break;
                    case Operator.And: op = "and"; connective = true; break;
                }
                if (connective)
                {
                    var builder = new StringBuilder();
                    var seperator = string.Empty;
                    builder.Append('(');
                    foreach (var child in expression.Values)
                    {
                        builder.Append(seperator);
                        builder.Append(ToFilter(field, (FilterExpression)child));
                        seperator = $" {op} ";
                    }
                    builder.Append(')');
                    filter = builder.ToString();
                }
                else
                {
                    filter = $"{field.Name} {op} {constant}";
                }
            }
            return filter;
        }

        public static string Constant(object value)
        {
            string constant = null;
            if (value is string)
            {
                constant = $"'{EscapeFilterString(value as string)}'";
            }
            else
            {
                constant = value.ToString();
            }
            return constant;
        }

        private static string EscapeFilterString(string s)
        {
            return s.Replace("'", "''");
        }
    }
}
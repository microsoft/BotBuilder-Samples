namespace Microsoft.Cognitive.LUIS.ActionBinding
{
    using System;

    public static class BuiltInTypes
    {
        public const string Age = "builtin.age";

        public const string Dimension = "builtin.dimension";

        public const string Email = "builtin.email";

        public const string Money = "builtin.money";

        public const string Number = "builtin.number";

        public const string Ordinal = "builtin.ordinal";

        public const string Percentage = "builtin.percentage";

        public const string Phonenumber = "builtin.phonenumber";

        public const string Temperature = "builtin.temperature";

        public const string Url = "builtin.url";

        private const string SharedPrefix = "builtin.";

        public static BuiltInDatetimeTypes Datetime { get; } = new BuiltInDatetimeTypes();

        public static BuiltInEncyclopediaTypes Encyclopedia { get; } = new BuiltInEncyclopediaTypes();

        public static BuiltInGeographyTypes Geography { get; } = new BuiltInGeographyTypes();

        public static bool IsBuiltInType(string type)
        {
            return type.StartsWith(SharedPrefix, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}

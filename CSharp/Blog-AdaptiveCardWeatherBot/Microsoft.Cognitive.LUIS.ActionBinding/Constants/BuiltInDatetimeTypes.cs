namespace Microsoft.Cognitive.LUIS.ActionBinding
{
    public class BuiltInDatetimeTypes
    {
        public const string Date = "builtin.datetime.date";

        public const string Time = "builtin.datetime.time";

        public const string Duration = "builtin.datetime.duration";

        public const string Set = "builtin.datetime.set";

        public string DateType
        {
            get { return Date; }
        }

        public string DurationType
        {
            get { return Duration; }
        }

        public string SetType
        {
            get { return Set; }
        }

        public string TimeType
        {
            get { return Time; }
        }
    }
}

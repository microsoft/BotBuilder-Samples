namespace Microsoft.Cognitive.LUIS.ActionBinding
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class LuisActionBindingAttribute : Attribute
    {
        public LuisActionBindingAttribute(string intentName)
        {
            if (string.IsNullOrWhiteSpace(intentName))
            {
                throw new ArgumentException(nameof(intentName));
            }

            this.IntentName = intentName;

            // setting defaults
            this.CanExecuteWithNoContext = true;
            this.ConfirmOnSwitchingContext = true;

            this.FriendlyName = this.IntentName;
        }

        public bool CanExecuteWithNoContext { get; set; }

        public bool ConfirmOnSwitchingContext { get; set; }

        public string FriendlyName { get; set; }

        public string IntentName { get; private set; }
    }
}

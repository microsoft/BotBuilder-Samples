namespace LuisActions.Samples
{
    using System.ComponentModel.DataAnnotations;

    public class LocationAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            // TODO: Actually validate location using Bing
            return value == null || ((string)value).Length >= 3;
        }
    }
}

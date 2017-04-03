namespace LuisActions.Samples
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Threading.Tasks;
    using Microsoft.Cognitive.LUIS.ActionBinding;

    [Serializable]
    [LuisActionBinding("TimeInPlace", FriendlyName = "Get the Time in a location")]
    public class GetTimeInPlaceAction : GetDataFromPlaceBaseAction
    {
        /// <summary>
        /// Should convert to 24 hours format (Optional)
        /// </summary>
        [Display(Name = "Convert to 24 hours format?")]
        public bool ConvertTo24HoursFormat { get; set; }

        public override Task<object> FulfillAsync()
        {
            var result = string.Format("The time in {0} is 00:00", this.Place);
            return Task.FromResult((object)result);
        }
    }
}
namespace LuisActions.Samples
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Threading.Tasks;
    using Microsoft.Cognitive.LUIS.ActionBinding;

    [Serializable]
    [LuisActionBinding("FindHotels", FriendlyName = "Find Hotel Room")]
    public class FindHotelsAction : BaseLuisAction
    {
        public string Category { get; set; }

        [Required(ErrorMessage = "Please provide the check-in date")]
        [LuisActionBindingParam(BuiltinType = BuiltInDatetimeTypes.Date, Order = 2)]
        public DateTime? Checkin { get; set; }

        [Required(ErrorMessage = "Please provide the check-out date")]
        [LuisActionBindingParam(BuiltinType = BuiltInDatetimeTypes.Date, Order = 3)]
        public DateTime? Checkout { get; set; }

        [Required(ErrorMessage = "Please provide a location")]
        [Location(ErrorMessage = "Please provide a valid location")]
        [LuisActionBindingParam(BuiltinType = BuiltInGeographyTypes.City, Order = 1)]
        public string Place { get; set; }

        public string RoomType { get; set; }

        public override Task<object> FulfillAsync()
        {
            return Task.FromResult((object)$"Sorry, there are no {this.RoomType} rooms available at {this.Place} for your chosen dates ({this.Checkin.Value.ToShortDateString()} to {this.Checkout.Value.ToShortDateString()}), please try another search.");
        }
    }
}
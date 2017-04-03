namespace LuisActions.Samples
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Threading.Tasks;
    using Microsoft.Cognitive.LUIS.ActionBinding;

    [Serializable]
    [LuisActionBinding("FindHotels-ChangeLocation", FriendlyName = "Change the hotel location")]
    public class FindHotelsAction_ChangeLocation : BaseLuisContextualAction<FindHotelsAction>
    {
        [Required(ErrorMessage = "Please provide a new location for your hotel")]
        [Location(ErrorMessage = "Please provide a new valid location for your hotel")]
        [LuisActionBindingParam(BuiltinType = BuiltInGeographyTypes.City)]
        public string Place { get; set; }

        public override Task<object> FulfillAsync()
        {
            if (this.Context == null)
            {
                throw new InvalidOperationException("Action context not defined.");
            }

            // assign new location to FindHotelsAction
            this.Context.Place = this.Place;

            return Task.FromResult((object)$"Hotel location changed to {this.Place}");
        }
    }
}

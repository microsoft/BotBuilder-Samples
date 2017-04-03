namespace LuisActions.Samples
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Threading.Tasks;
    using Microsoft.Cognitive.LUIS.ActionBinding;

    [Serializable]
    [LuisActionBinding("FindHotels-ChangeCheckin", FriendlyName = "Change the hotel checkin date", CanExecuteWithNoContext = false)]
    public class FindHotelsAction_ChangeCheckin : BaseLuisContextualAction<FindHotelsAction>
    {
        [Required(ErrorMessage = "Please provide the new check-in date")]
        [LuisActionBindingParam(BuiltinType = BuiltInDatetimeTypes.Date)]
        public DateTime? Checkin { get; set; }

        public override Task<object> FulfillAsync()
        {
            if (this.Context == null)
            {
                throw new InvalidOperationException("Action context not defined.");
            }

            // assign new check-in date to FindHotelsAction
            this.Context.Checkin = this.Checkin;

            return Task.FromResult((object)$"Hotel checkin date changed to {this.Checkin?.ToShortDateString()}");
        }
    }
}

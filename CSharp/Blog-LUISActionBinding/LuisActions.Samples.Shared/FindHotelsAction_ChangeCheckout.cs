namespace LuisActions.Samples
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Threading.Tasks;
    using Microsoft.Cognitive.LUIS.ActionBinding;

    [Serializable]
    [LuisActionBinding("FindHotels-ChangeCheckout", FriendlyName = "Change the hotel checkout date", CanExecuteWithNoContext = false)]
    public class FindHotelsAction_ChangeCheckout : BaseLuisContextualAction<FindHotelsAction>
    {
        [Required(ErrorMessage = "Please provide the new check-out date")]
        [LuisActionBindingParam(BuiltinType = BuiltInDatetimeTypes.Date)]
        public DateTime? Checkout { get; set; }

        public override Task<object> FulfillAsync()
        {
            if (this.Context == null)
            {
                throw new InvalidOperationException("Action context not defined.");
            }

            // assign new check-out date to FindHotelsAction
            this.Context.Checkout = this.Checkout;

            return Task.FromResult((object)$"Hotel checkin date changed to {this.Checkout?.ToShortDateString()}");
        }
    }
}

namespace LuisActions.Samples.Web.Models
{
    using System.ComponentModel.DataAnnotations;
    using Microsoft.Cognitive.LUIS.ActionBinding;

    public class QueryViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please provide something to evaluate (E.g.: \"What is the time in Miami?\"")]
        public string Query { get; set; }

        public ILuisAction LuisAction { get; set; }

        // TODO: this is dangerous. This should be stored somewhere else, not in the client
        public string LuisActionType { get; set; }

        public bool HasIntent
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.LuisActionType);
            }
        }
    }
}
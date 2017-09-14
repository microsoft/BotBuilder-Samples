namespace LuisActions.Samples
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Threading.Tasks;
    using Microsoft.Cognitive.LUIS.ActionBinding;
    using Newtonsoft.Json;

    // This sample were added to showcase support to Custom List Entities

    // Refer to 'List Entities' @ https://docs.microsoft.com/en-us/azure/cognitive-services/luis/add-entities

    // You can create your own model with an 'AlternativeChoose' intent with utterances 
    // having a 'List Entity' called 'Alternatives' with the canonical forms defined by the
    // 'Alternative' enum - or just can simply create your custom list entity,
    // intent and update the intent binding here and custom name/type at fields

    // Note: The provided LUIS app @ LUIS_MODEL.json does not have an intent related to it

    public enum Alternative
    {
        // default (ie. empty choice)
        None            =   0,

        DomainOption1   =   11,
        DomainOption2   =   12,
        DomainOption3   =   13,

        ExternalOption1 =   101,
        ExternalOption2 =   102
    }

    public class RequiredEnumAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return (int)value > 0;
        }
    }

    [Serializable]
    [LuisActionBinding("AlternativeChoose", FriendlyName = "Alternatives Choosing Model Sample")]
    public class AlternativesAction : BaseLuisAction
    {
        [RequiredEnum(ErrorMessage = "Please provide an alternative")]
        [LuisActionBindingParam(CustomType = "Alternatives", Order = 1)]
        public Alternative FirstAlternative { get; set; }

        [Required(ErrorMessage = "Please provide one or more alternatives")]
        [LuisActionBindingParam(CustomType = "Alternatives", Order = 2)]
        public Alternative[] SecondaryAlternatives { get; set; }

        public override Task<object> FulfillAsync()
        {
            return Task.FromResult((object)JsonConvert.SerializeObject(this));
        }
    }
}

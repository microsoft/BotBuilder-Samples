namespace Microsoft.Cognitive.LUIS.ActionBinding
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading.Tasks;

    [Serializable]
    public abstract class BaseLuisAction : ILuisAction
    {
        public abstract Task<object> FulfillAsync();

        public virtual bool IsValid(out ICollection<ValidationResult> validationResults)
        {
            var context = new ValidationContext(this, null, null);

            validationResults = new List<ValidationResult>();
            var result = Validator.TryValidateObject(this, context, validationResults, true);

            // do order properties
            validationResults = validationResults
                .OrderBy(r =>
                {
                    var paramAttrib = LuisActionResolver.GetParameterDefinition(this, r.MemberNames.First());

                    return paramAttrib != null ? paramAttrib.Order : int.MaxValue;
                })
                .ThenBy(r => r.MemberNames.First())
                .ToList();

            return result;
        }
    }
}

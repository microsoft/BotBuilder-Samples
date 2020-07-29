using System;

namespace Microsoft.Botframework.AdaptiveCards.Converter.LINE.Models
{
    public interface ITemplateAction
    {
        string Type { get; }

        /// <summary>
        /// Max: 20 characters.
        /// </summary>
        string Label { get; set; }
    }
}

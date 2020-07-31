using System;

namespace Microsoft.Botframework.AdaptiveCards.Converter.LINE.Models
{
    public static class BoxComponentExtensions
    {
        /// <summary>
        /// Add a flex component to the Box component.
        /// </summary>
        /// <param name="self">BoxComponent.</param>
        /// <param name="component">Flex Component.</param>
        /// <returns>BoxComponent.</returns>
        public static BoxComponent AddContents(this BoxComponent self, IFlexComponent component)
        {
            if (component.IsValid())
            {
                self.Contents.Add(component);
            }

            return self;
        }

    }
}

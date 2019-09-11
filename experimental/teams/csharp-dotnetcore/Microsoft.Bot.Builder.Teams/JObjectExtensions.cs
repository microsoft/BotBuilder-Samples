// <copyright file="JObjectExtensions.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Builder.Teams
{
    using Newtonsoft.Json.Linq;
    /// <summary>
    /// JObject extensions.
    /// </summary>
    internal static class JObjectExtensions
    {
        /// <summary>
        /// Converts <see cref="object"/> into <see cref="JObject"/>.
        /// </summary>
        /// <param name="objectInstance">The object instance.</param>
        /// <returns><see cref="JObject"/> instance.</returns>
        public static JObject AsJObject(this object objectInstance)
        {
            if (objectInstance as JObject == null)
            {
                return JObject.FromObject(objectInstance);
            }
            else
            {
                return objectInstance as JObject;
            }
        }
    }
}

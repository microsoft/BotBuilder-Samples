// <copyright file="TeamsChannelAccount.Ex.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Schema.Teams
{
    using Newtonsoft.Json;

    /// <summary>
    /// Teams Channel Account extensions.
    /// </summary>
    public partial class TeamsChannelAccount
    {
        /// <summary>
        /// Gets or sets the AAD Object Id.
        /// </summary>
        [JsonProperty(PropertyName = "objectId")]
        private string ObjectId
        {
            get
            {
                return this.AadObjectId;
            }

            set
            {
                this.AadObjectId = value;
            }
        }
    }
}

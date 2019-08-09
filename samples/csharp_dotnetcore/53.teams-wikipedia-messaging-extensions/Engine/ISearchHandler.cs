// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Bot.Builder.Teams.WikipediaMessagingExtension.Engine
{
    using System.Threading.Tasks;
    using Microsoft.Bot.Schema.Teams;

    /// <summary>
    /// Search handler.
    /// </summary>
    public interface ISearchHandler
    {
        /// <summary>
        /// Gets the search result asynchronously.
        /// </summary>
        /// <param name="query">The invoke query object</param>
        /// <returns>Messaging extension result.</returns>
        Task<MessagingExtensionResult> GetSearchResultAsync(MessagingExtensionQuery query);
    }
}

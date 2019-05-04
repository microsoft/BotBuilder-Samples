// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Bot.Builder.AI.Translation.PostProcessor
{
    /// <summary>
    /// Abstraction for post processor.
    /// </summary>
    public interface IPostProcessor
    {
        /// <summary>
        /// Process the specific logic of the implemented post processor, and represents the abstraction for any post processor to be created
        /// so that all post processors follow the same pattern implementing Process function, and all the custom logic being wrapped inside the postprocessor implementation .
        /// </summary>
        /// <param name="translatedDocument">Translated document.</param>
        /// <param name="languageId">Current source language id.</param>
        /// <returns><see cref="PostProcessedDocument"/> that holds the original document and the newly post processed message/phrease.</returns>
        PostProcessedDocument Process(ITranslatedDocument translatedDocument, string languageId);
    }
}

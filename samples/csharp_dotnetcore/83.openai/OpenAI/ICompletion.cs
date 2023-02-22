// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.BotBuilderSamples
{
    public interface ICompletion
    {
        Task<string> GenerateCompletionAsync(string prompt);
    }
}

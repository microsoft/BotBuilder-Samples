// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Bot.Builder.AI.Translation
{
    public interface ILocaleConverter
    {
        bool IsLocaleAvailable(string locale);

        string Convert(string message, string fromLocale, string toLocale);

        string[] GetAvailableLocales();
    }
}

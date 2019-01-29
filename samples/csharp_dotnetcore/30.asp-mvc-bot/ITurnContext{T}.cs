// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder;

namespace Asp_Mvc_Bot
{
    public interface ITurnContext<T> : ITurnContext
    {
        new T Activity { get; }
    }
}

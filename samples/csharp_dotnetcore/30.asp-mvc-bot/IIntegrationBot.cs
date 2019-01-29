// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace Asp_Mvc_Bot
{
    public interface IIntegrationBot
    {
        Task<InvokeResponse> ProcessAsync(string authHeader, Activity activity);
    }
}

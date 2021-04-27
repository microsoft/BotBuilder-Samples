// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs.Declarative;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.BotFramework.Runtime.CustomAction
{
    public class MultiplyDialogBotComponent : BotComponent
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<DeclarativeType>(sp => new DeclarativeType<MultiplyDialog>(MultiplyDialog.Kind));
        }
    }
}

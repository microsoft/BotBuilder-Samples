// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
using AdaptiveExpressions.Converters;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs.Declarative;
using Microsoft.Bot.Builder.Dialogs.Declarative.Converters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Form.Extensions
{
    /// <summary>
    /// Configure form extensions.
    /// </summary>
    public class FormExtensionsBotComponent : BotComponent
    {
        /// <summary>
        /// <see cref="BotComponent"/> for adaptive components.
        /// </summary>
        /// <inheritdoc/>
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Declarative types.
            services.AddSingleton<DeclarativeType>(sp => new DeclarativeType<ControlForm>(ControlForm.Kind));
            services.AddSingleton<JsonConverterFactory, JsonConverterFactory<ArrayExpressionConverter<string>>>();
        }
    }
}

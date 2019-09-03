// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Connector.Authentication;

namespace Microsoft.Bot.Builder.Integration.AspNet.WebApi
{
    public static class BotFrameworkConfigurationBuilderExtensions
    {
        /// <summary>
        /// Configures the bot with the a single identity that will be used to authenticate requests made to the Bot Framework.
        /// </summary>
        /// <param name="builder">The <see cref="BotFrameworkConfigurationBuilder"/>.</param>
        /// <param name="applicationId">The application id that should be used to authenticate requests made to the Bot Framework.</param>
        /// <param name="applicationPassword">The application password that should be used to authenticate requests made to the Bot Framework.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="ICredentialProvider"/>
        /// <seealso cref="SimpleCredentialProvider"/>
        public static BotFrameworkConfigurationBuilder UseMicrosoftApplicationIdentity(this BotFrameworkConfigurationBuilder builder, string applicationId, string applicationPassword) =>
            builder.UseCredentialProvider(new SimpleCredentialProvider(applicationId, applicationPassword));
    }
}

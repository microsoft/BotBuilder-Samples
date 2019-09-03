// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Rest.TransientFaultHandling;

namespace Microsoft.Bot.Builder.Integration.AspNet.WebApi
{
    public class BotFrameworkConfigurationBuilder
    {
        public BotFrameworkConfigurationBuilder(BotFrameworkOptions botFrameworkOptions)
        {
            BotFrameworkOptions = botFrameworkOptions;
        }

        public BotFrameworkOptions BotFrameworkOptions { get; }

        /// <summary>
        /// Configures an <see cref="ICredentialProvider"/> that should be used to store and retrieve credentials used during authentication with the Bot Framework.
        /// </summary>
        /// <param name="credentialProvider">An <see cref="ICredentialProvider"/> that the bot framework will use to authenticate requests.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="ICredentialProvider" />
        public BotFrameworkConfigurationBuilder UseCredentialProvider(ICredentialProvider credentialProvider)
        {
            BotFrameworkOptions.CredentialProvider = credentialProvider;
            return this;
        }

        /// <summary>
        /// Adds an Error Handler the bot.
        /// </summary>
        /// <param name="errorHandler">An instance of <see cref="IMiddleware">middleware</see> that should be added to the bot's middleware pipeline.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="IMiddleware"/>
        public BotFrameworkConfigurationBuilder UseMiddleware(Func<ITurnContext, Exception, Task> errorHandler)
        {
            BotFrameworkOptions.OnTurnError = errorHandler;
            return this;
        }

        /// <summary>
        /// Adds a piece of <see cref="IMiddleware"/> to the bot's middleware pipeline.
        /// </summary>
        /// <param name="middleware">An instance of <see cref="IMiddleware">middleware</see> that should be added to the bot's middleware pipeline.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="IMiddleware"/>
        public BotFrameworkConfigurationBuilder UseMiddleware(IMiddleware middleware)
        {
            BotFrameworkOptions.Middleware.Add(middleware);
            return this;
        }

        /// <summary>
        /// Adds retry policy on failure for Bot Framework Service calls.
        /// </summary>
        /// <param name="retryPolicy">The retry policy.</param>
        /// <returns><see cref="BotFrameworkConfigurationBuilder"/> instance with the retry policy set.</returns>
        public BotFrameworkConfigurationBuilder UseRetryPolicy(RetryPolicy retryPolicy)
        {
            BotFrameworkOptions.ConnectorClientRetryPolicy = retryPolicy;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="HttpClient"/> instance that will be used to make Bot Framework Service calls.
        /// </summary>
        /// <param name="httpClient">The <see cref="HttpClient"/> to be used when calling the Bot Framework Service.</param>
        /// <returns><see cref="BotFrameworkConfigurationBuilder"/> instance with the <see cref="HttpClient"/> set.</returns>
        public BotFrameworkConfigurationBuilder UseHttpClient(HttpClient httpClient)
        {
            BotFrameworkOptions.HttpClient = httpClient;
            return this;
        }

        /// <summary>
        /// Configures which paths should be used to expose the various endpoints of the bot.
        /// </summary>
        /// <param name="configurePaths">A callback to configure the paths that determine where the endpoints of the bot will be exposed.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="BotFrameworkPaths"/>
        public BotFrameworkConfigurationBuilder UsePaths(Action<BotFrameworkPaths> configurePaths)
        {
            configurePaths(BotFrameworkOptions.Paths);

            return this;
        }
    }
}

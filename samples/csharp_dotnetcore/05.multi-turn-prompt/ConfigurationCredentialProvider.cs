// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading.Tasks;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;

namespace Microsoft.BotBuilderSamples
{
    public class ConfigurationCredentialProvider : ICredentialProvider
    {
        // Encapsulate the provider, so we can more easily change implementation in the future
        // without breaking compatability. 
        private readonly SimpleCredentialProvider _provider;

        public ConfigurationCredentialProvider(IConfiguration configuration)
        {
            _provider = new SimpleCredentialProvider(
                configuration["BotFramework:AppId"],
                configuration["BotFramework:Password"]);
        }

        public Task<string> GetAppPasswordAsync(string appId) => _provider.GetAppPasswordAsync(appId);

        public Task<bool> IsAuthenticationDisabledAsync() => _provider.IsAuthenticationDisabledAsync();

        public Task<bool> IsValidAppIdAsync(string appId) => _provider.IsValidAppIdAsync(appId);
    }
}

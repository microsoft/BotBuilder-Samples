// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Web.Http;

#pragma warning disable SA1649  // File name should match first type name

namespace QnA_Bot
{
    /// <summary>
    /// Main Web Application.
    /// </summary>
    /// <seealso cref="https://docs.microsoft.com/en-us/aspnet/web-api/overview/advanced/configuring-aspnet-web-api"/>
    public class WebApiApplication : System.Web.HttpApplication
#pragma warning restore SA1649 // File name should match first type name
    {
        /// <summary>
        /// Initialization after the ASP.Net framework is loaded by IIS.
        /// </summary>
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(config =>
            {
                BotConfig.Register(config);
            });
        }
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Web.Http;

#pragma warning disable SA1649  // File name should match first type name

namespace AspNetWebApi_QnA_Bot
{
    /// <summary>
    /// Main Web Application.
    /// </summary>
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

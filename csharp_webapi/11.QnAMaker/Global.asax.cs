// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace AspNetWebApi_QnA_Bot
{
    using System.Web.Http;

    /// <summary>
    /// Main Web Application.
    /// </summary>
    public class WebApiApplication : System.Web.HttpApplication
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

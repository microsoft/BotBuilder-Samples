namespace AppInsightsBot
{
    using System.Web.Http;

    public class WebApiApplication : System.Web.HttpApplication
    {
        public static Microsoft.ApplicationInsights.TelemetryClient Telemetry { get; } = new Microsoft.ApplicationInsights.TelemetryClient();

        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}

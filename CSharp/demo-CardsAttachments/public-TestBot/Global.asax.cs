namespace TestBot
{
    using System.Web.Http;
    using System.Web.Routing;
    using Autofac;
    using Helpers;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Dialogs.Internals;
    using Microsoft.Bot.Builder.History;

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            this.RegisterBotDependencies();

            RouteConfig.RegisterRoutes(RouteTable.Routes);

            GlobalConfiguration.Configure(WebApiConfig.Register);
        }

        private void RegisterBotDependencies()
        {
            Conversation.UpdateContainer(builder =>
            {
                builder
                    .Register(c => new ActivityLogger(c.Resolve<IBotData>()))
                    .As<IActivityLogger>()
                    .InstancePerLifetimeScope();

                foreach (var commandType in CommandsHelper.GetRegistrableTypes())
                {
                    builder
                        .RegisterType(commandType)
                        .Keyed(commandType.Name, commandType)
                        .AsImplementedInterfaces()
                        .InstancePerMatchingLifetimeScope(DialogModule.LifetimeScopeTag);
                }
            });
        }
    }
}
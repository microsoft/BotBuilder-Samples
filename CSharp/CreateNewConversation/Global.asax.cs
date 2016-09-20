namespace CreateNewConversationBot
{
    using System.Reflection;
    using System.Web.Http;
    using Autofac;
    using Autofac.Integration.WebApi;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Dialogs.Internals;
    using Microsoft.Bot.Builder.Internals.Fibers;

    public class WebApiApplication : System.Web.HttpApplication
    {
        public static ILifetimeScope FindContainer()
        {
            var config = GlobalConfiguration.Configuration;
            var resolver = (AutofacWebApiDependencyResolver)config.DependencyResolver;
            return resolver.Container;
        }

        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            var builder = new ContainerBuilder();

            // temporary registration until change in DialogModule currently in Dev branch that registers ResumptionCookie is released
            builder.RegisterType<ResumptionCookie>().AsSelf().InstancePerMatchingLifetimeScope(DialogModule.LifetimeScopeTag);

            builder.RegisterModule(new DialogModule());

            builder.RegisterModule(new SurveyModule());

            // Register your Web API controllers.
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            var config = GlobalConfiguration.Configuration;

            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}

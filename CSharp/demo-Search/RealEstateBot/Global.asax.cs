namespace RealEstateBot
{
    using System.Web;
    using System.Web.Http;
    using Autofac;
    using Microsoft.Azure.Search.Models;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Internals.Fibers;
    using RealEstateBot.Dialogs;
    using Search.Azure.Services;
    using Search.Models;
    using Search.Services;

    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            Conversation.UpdateContainer(builder =>
            {
                builder.RegisterType<IntroDialog>()
                  .As<IDialog<object>>()
                  .InstancePerDependency();

                builder.RegisterType<RealEstateMapper>()
                   .Keyed<IMapper<DocumentSearchResult, GenericSearchResult>>(FiberModule.Key_DoNotSerialize)
                   .AsImplementedInterfaces()
                   .SingleInstance();

                builder.RegisterType<AzureSearchClient>()
                    .Keyed<ISearchClient>(FiberModule.Key_DoNotSerialize)
                    .AsImplementedInterfaces()
                    .SingleInstance();
            });

            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
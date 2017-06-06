namespace LuisActions.Samples.Bot
{
    using Autofac;
    using Microsoft.Bot.Builder.Dialogs;
    using System.Web.Http;

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            ContainerBuilder builder = new ContainerBuilder();

            builder.RegisterType<RootDialog>()
              .As<IDialog<object>>()
              .InstancePerDependency();

            //builder.RegisterType<JobsMapper>()
            //    .Keyed<IMapper<DocumentSearchResult, GenericSearchResult>>(FiberModule.Key_DoNotSerialize)
            //    .AsImplementedInterfaces()
            //    .SingleInstance();

            //builder.RegisterType<AzureSearchClient>()
            //    .Keyed<ISearchClient>(FiberModule.Key_DoNotSerialize)
            //    .AsImplementedInterfaces()
            //    .SingleInstance();

            builder.Update(Conversation.Container);
        }
    }
}
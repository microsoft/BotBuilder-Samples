using Autofac;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Luis;
using Newsie.Dialogs;
using Newsie.Handlers;
using Newsie.Services;
using Newsie.Utilities;

namespace Newsie.Modules
{
    internal sealed class MainModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.Register(c => new LuisModelAttribute("6ef53edb-ea47-48f1-92f9-7405e8a4dc46", "a6d628faa2404cd799f2a291245eb135")).AsSelf().AsImplementedInterfaces().SingleInstance();

            // Top Level Dialog
            builder.RegisterType<MainLuisDialog>().As<IDialog<object>>().InstancePerDependency();

            // Singlton services
            builder.RegisterType<LuisService>().Keyed<ILuisService>(FiberModule.Key_DoNotSerialize).AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<TinyUrlRequestHandler>().Keyed<IUrlShorteningService>(FiberModule.Key_DoNotSerialize).AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<BingNewsService>().Keyed<INewsService>(FiberModule.Key_DoNotSerialize).AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<BingSummarizeService>().Keyed<ISummarizeService>(FiberModule.Key_DoNotSerialize).AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<ApiHandler>().Keyed<IApiHandler>(FiberModule.Key_DoNotSerialize).AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<CacheManager>().Keyed<ICacheManager>(FiberModule.Key_DoNotSerialize).AsImplementedInterfaces().SingleInstance();

            // Objects depending on incoming messages
            builder.RegisterType<NewsCategoryScorable>().Keyed<NewsCategoryScorable>(FiberModule.Key_DoNotSerialize).AsImplementedInterfaces().InstancePerMatchingLifetimeScope(DialogModule.LifetimeScopeTag);
            builder.RegisterType<HandlerFactory>().Keyed<IHandlerFactory>(FiberModule.Key_DoNotSerialize).AsImplementedInterfaces().InstancePerMatchingLifetimeScope(DialogModule.LifetimeScopeTag);
            builder.RegisterType<NewsIntentHandler>().Keyed<IIntentHandler>(FiberModule.Key_DoNotSerialize).Named<IIntentHandler>(NewsieStrings.NewsIntentName).AsImplementedInterfaces().InstancePerMatchingLifetimeScope(DialogModule.LifetimeScopeTag);
            builder.RegisterType<GreetingIntentHandler>().Keyed<IIntentHandler>(FiberModule.Key_DoNotSerialize).Named<IIntentHandler>(NewsieStrings.GreetingIntentName).AsImplementedInterfaces().InstancePerMatchingLifetimeScope(DialogModule.LifetimeScopeTag);
            builder.RegisterType<SummaryRegexHandler>().Keyed<IRegexHandler>(FiberModule.Key_DoNotSerialize).Named<IRegexHandler>(NewsieStrings.SummarizeActionName).AsImplementedInterfaces().InstancePerMatchingLifetimeScope(DialogModule.LifetimeScopeTag);
        }
    }
}
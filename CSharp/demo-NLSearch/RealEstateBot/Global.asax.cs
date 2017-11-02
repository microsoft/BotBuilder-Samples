using System.Web.Http;
using Autofac;
using Microsoft.Bot.Builder.Dialogs;
using RealEstateBot.Dialogs;
using Search.Dialogs;
using System.Configuration;
using System;
using System.Web;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs.Internals;
using System.Text.RegularExpressions;
using Microsoft.Bot.Builder.Scorables;
using Microsoft.Bot.Builder.History;
using Microsoft.Bot.Builder.Scorables.Internals;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Autofac.Base;

namespace RealEstateBot
{
    public sealed class MessWithActivity : ScorableBase<IActivity, object, double>
    {
        public MessWithActivity(IBotData data)
        {
            string vlaue;
            data.UserData.TryGetValue("hello", out vlaue);
        }

        protected override Task DoneAsync(IActivity item, object state, CancellationToken token)
        {
            return Task.CompletedTask;
        }

        protected override double GetScore(IActivity item, object state)
        {
            throw new NotImplementedException();
        }

        protected override bool HasScore(IActivity item, object state)
        {
            return false;
        }

        protected override Task PostAsync(IActivity item, object state, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        protected override async Task<object> PrepareAsync(IActivity item, CancellationToken token)
        {
            return null;
        }
    }

    public class WebApiApplication : HttpApplication
    {

        public static readonly IContainer Container;

        static WebApiApplication()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new DialogModule());
            builder
                // Change the order so that data is loaded before actitivity logger
                .RegisterAdapterChain<IPostToBot>
                (
                    typeof(EventLoopDialogTask),
                    typeof(SetAmbientThreadCulture),
                    typeof(QueueDrainingDialogTask),
                    typeof(LogPostToBot),
                    typeof(PersistentDialogTask),
                    typeof(ExceptionTranslationDialogTask),
                    typeof(SerializeByConversation),
                    typeof(PostUnhandledExceptionToUser)
                )
                .InstancePerLifetimeScope();

            // Add a global scorable to change language to make it easier to do
            var scorable = Actions
                .Bind(async (IActivityLogger ilogger, IBotToUser botToUser, IBotData data, IMessageActivity message) =>
                     {
                         var logger = ilogger as SearchTranslator;
                         var lang = message.Text.Substring(1);
                         logger.UserLanguage = lang;
                         data.UserData.SetValue("UserLanguage", lang);
                         await botToUser.PostAsync($"Switched language to <literal>{lang}</literal> (ar-Arabic, de-German, es-Spanish, en-English, fr-French, it-Italian, ja-Japanese, ko-Korean, pt-Portuguese, ru-Russion, zh-Chinese)", "en");
                     })
                // TODO: This is the current set of generalnn translation languages
                .When(new Regex(@"^\#"))
                .Normalize();
            builder.RegisterInstance(scorable).AsImplementedInterfaces().SingleInstance();

            var translationKey = ConfigurationManager.AppSettings["TranslationKey"];
            if (string.IsNullOrWhiteSpace(translationKey))
            {
                translationKey = Environment.GetEnvironmentVariable("TranslationKey");
            }

            builder.Register((c) => new SearchTranslator(c.Resolve<ConversationReference>(), c.Resolve<IBotData>(), "en", translationKey))
                   .AsImplementedInterfaces()
                   .InstancePerMatchingLifetimeScope(DialogModule.LifetimeScopeTag);

            builder.RegisterType<RealEstateDialog>()
                .As<IDialog<object>>()
                // .InstancePerDependency();
                .InstancePerMatchingLifetimeScope(DialogModule.LifetimeScopeTag);

            Container = builder.Build();
        }

        protected void Application_Start()
        {

            Conversation.UpdateContainer(builder =>
            {
            });

            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
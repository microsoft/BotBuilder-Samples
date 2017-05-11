namespace CreateNewConversationBot
{
    using Autofac;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Dialogs.Internals;
    using Microsoft.Bot.Builder.Internals.Fibers;
    using Microsoft.Bot.Connector;

    public class SurveyModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<CreateNewConversationDialog>().As<IDialog<object>>().InstancePerDependency();

            builder.Register((c, p) => new SurveyDialog()).AsSelf().InstancePerDependency();
            builder.RegisterType<SurveyScheduler>().Keyed<ISurveyScheduler>(FiberModule.Key_DoNotSerialize).AsImplementedInterfaces().SingleInstance();

            builder.Register(c => new SurveyService(c.Resolve<ISurveyScheduler>(), c.Resolve<ConversationReference>())).Keyed<ISurveyService>(FiberModule.Key_DoNotSerialize).AsImplementedInterfaces().InstancePerMatchingLifetimeScope(DialogModule.LifetimeScopeTag);
        }
    }
}
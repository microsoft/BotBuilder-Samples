namespace ContosoFlowers
{
    using System.Configuration;
    using Autofac;
    using BotAssets;
    using BotAssets.Dialogs;
    using Dialogs;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Internals.Fibers;
    using Microsoft.Bot.Builder.Location;
    using Microsoft.Bot.Builder.Scorables;
    using Microsoft.Bot.Connector;
    using Services.Models;

    public class ContosoFlowersModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<ContosoFlowersDialogFactory>()
                .Keyed<IContosoFlowersDialogFactory>(FiberModule.Key_DoNotSerialize)
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterType<RootDialog>()
                .As<IDialog<object>>()
                .InstancePerDependency();

            builder.RegisterType<SettingsScorable>()
                .As<IScorable<IActivity, double>>()
                .InstancePerLifetimeScope();

            builder.RegisterType<FlowerCategoriesDialog>()
                .InstancePerDependency();

            builder.RegisterType<BouquetsDialog>()
                .InstancePerDependency();

            builder.RegisterType<SavedAddressDialog>()
              .InstancePerDependency();

            builder.RegisterType<SettingsDialog>()
             .InstancePerDependency();

            // Location Dialog
            // ctor signature: LocationDialog(string apiKey, string channelId, string prompt, LocationOptions options = LocationOptions.None, LocationRequiredFields requiredFields = LocationRequiredFields.None, LocationResourceManager resourceManager = null);
            builder.RegisterType<LocationDialog>()
                .WithParameter("apiKey", ConfigurationManager.AppSettings["MicrosoftBingMapsKey"])
                .WithParameter("options", LocationOptions.UseNativeControl | LocationOptions.ReverseGeocode)
                .WithParameter("requiredFields", LocationRequiredFields.StreetAddress | LocationRequiredFields.Locality | LocationRequiredFields.Country)
                .WithParameter("resourceManager", new ContosoLocationResourceManager())
                .InstancePerDependency();

            // Service dependencies
            builder.RegisterType<Services.InMemoryOrdersService>()
                .Keyed<Services.IOrdersService>(FiberModule.Key_DoNotSerialize)
                .AsImplementedInterfaces()
                .SingleInstance();

            builder.RegisterType<Services.InMemoryBouquetRepository>()
                .Keyed<Services.IRepository<Bouquet>>(FiberModule.Key_DoNotSerialize)
                .AsImplementedInterfaces()
                .SingleInstance();

            builder.RegisterType<Services.InMemoryFlowerCategoriesRepository>()
                .Keyed<Services.IRepository<FlowerCategory>>(FiberModule.Key_DoNotSerialize)
                .AsImplementedInterfaces()
                .SingleInstance();
        }
    }
}
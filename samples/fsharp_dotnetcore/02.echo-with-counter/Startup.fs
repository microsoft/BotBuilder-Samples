// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
namespace Microsoft.BotBuilderSamples
open System
open System.Linq
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Bot.Builder
open Microsoft.Bot.Builder.Integration
open Microsoft.Bot.Builder.Integration.AspNet.Core
open Microsoft.Bot.Configuration
open Microsoft.Bot.Connector.Authentication
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging
open Microsoft.Extensions.Options
open System.Threading.Tasks



/// <summary>
/// The Startup class configures services and the request pipeline.
/// </summary>
type Startup private () =
    new (env : IHostingEnvironment) as this =
        Startup() then

        this._isProduction <- env.IsProduction()
        let builder = new ConfigurationBuilder()
        builder.SetBasePath(env.ContentRootPath) |> ignore
        builder.AddJsonFile("appsettings.json", optional= true, reloadOnChange= true) |> ignore
        builder.AddJsonFile("appsettings." + env.EnvironmentName + ".json", optional= true) |> ignore
        builder.AddEnvironmentVariables() |> ignore
        let config = builder.Build() :> IConfiguration
        this.Configuration <- config

        /// <summary>
        /// Gets the configuration that represents a set of key/value application configuration properties.
        /// </summary>
        /// <value>
        /// The <see cref="IConfiguration"/> that represents a set of key/value application configuration properties.
        /// </value>
    member val Configuration : IConfiguration = null with get, set

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> specifies the contract for a collection of service descriptors.</param>
        /// <seealso cref="IStatePropertyAccessor{T}"/>
        /// <seealso cref="https://docs.microsoft.com/en-us/aspnet/web-api/overview/advanced/dependency-injection"/>
        /// <seealso cref="https://docs.microsoft.com/en-us/azure/bot-service/bot-service-manage-channels?view=azure-bot-service-4.0"/>
    member this.ConfigureServices(services: IServiceCollection) =
        services.AddBot<EchoWithCounterBot>(fun options ->
            // Creates a logger for the application to use.
            let logger = this._loggerFactory.CreateLogger<EchoWithCounterBot>()

            let secretKey = this.Configuration.["botFileSecret"]
            let botFilePath = this.Configuration.["botFilePath"]

            // Loads .bot configuration file and adds a singleton that your Bot can access through dependency injection.
            let mutable botConfig = null 
            try
                botConfig <-BotConfiguration.Load(botFilePath , secretKey) 
                ()
            with 
            | :? Exception ->
                let msg = @"Error reading bot file. Please ensure you have valid botFilePath and botFileSecret set for your environment.
- You can find the botFilePath and botFileSecret in the Azure App Service application settings.
- If you are running this bot locally, consider adding a appsettings.json file with botFilePath and botFileSecret.
- See https://aka.ms/about-bot-file to learn more about .bot file its use and bot configuration.
"
                logger.LogError(msg)
                invalidOp msg
        

            services.AddSingleton<BotConfiguration>(botConfig) |> ignore
            // Retrieve current endpoint.
            let environment = if this._isProduction then "production" else "development"
            let mutable service = botConfig.Services.Where(fun s -> s.Type = "endpoint" && s.Name = environment).FirstOrDefault() :?> EndpointService
            if (service = null && this._isProduction) then
                // Attempt to load development environment
                service <- botConfig.Services.Where(fun s -> s.Type = "endpoint" && s.Name = "development").FirstOrDefault() :?> EndpointService
                logger.LogWarning("Attempting to load development endpoint in production environment.")

            if (service = null) then 
                invalidOp (sprintf "The .bot file does not contain an endpoint with name '%s'." environment)             

            options.CredentialProvider <- new SimpleCredentialProvider(service.AppId, service.AppPassword)

            // Catches any errors that occur during a conversation turn and logs them.
            options.OnTurnError <- fun context ex -> 
                logger.LogError("Exception caught : " + ex.Message) 
                context.SendActivityAsync("Sorry, it looks like something went wrong.") :> Task

            // The Memory Storage used here is for local bot debugging only. When the bot
            // is restarted, everything stored in memory will be gone.
            let  dataStore = new MemoryStorage() :> IStorage

            // For production bots use the Azure Blob or
            // Azure CosmosDB storage providers. For the Azure
            // based storage providers, add the Microsoft.Bot.Builder.Azure
            // Nuget package to your solution. That package is found at:
            // https://www.nuget.org/packages/Microsoft.Bot.Builder.Azure/
            // Uncomment the following lines to use Azure Blob Storage
            // Storage configuration name or ID from the .bot file.
            // const string StorageConfigurationId = "<STORAGE-NAME-OR-ID-FROM-BOT-FILE>";
            // var blobConfig = botConfig.FindServiceByNameOrId(StorageConfigurationId);
            // if (!(blobConfig is BlobStorageService blobStorageConfig))
            // {
            //    throw new InvalidOperationException($"The .bot file does not contain an blob storage with name '{StorageConfigurationId}'.");
            // }
            // // Default container name.
            // const string DefaultBotContainer = "<DEFAULT-CONTAINER>";
            // var storageContainer = string.IsNullOrWhiteSpace(blobStorageConfig.Container) ? DefaultBotContainer : blobStorageConfig.Container;
            // IStorage dataStore = new Microsoft.Bot.Builder.Azure.AzureBlobStorage(blobStorageConfig.ConnectionString, storageContainer);

            // Create Conversation State object.
            // The Conversation State object is where we persist anything at the conversation-scope.
            let conversationState = new ConversationState(dataStore)
            options.State.Add(conversationState)
            ()
        ) |> ignore

        // Create and register state accessors.
        // Accessors created here are passed into the IBot-derived class on every turn.
        services.AddSingleton<EchoBotAccessors>(fun sp ->
            let options = sp.GetRequiredService<IOptions<BotFrameworkOptions>>().Value
            if (options = null) then 
                invalidOp "BotFrameworkOptions must be configured prior to setting up the state accessors"
            
            let conversationState = options.State.OfType<ConversationState>().FirstOrDefault()
            if (conversationState = null) then
                invalidOp "ConversationState must be defined and added before adding conversation-scoped state accessors."

            // Create the custom state accessor.
            // State accessors enable other components to read and write individual properties of state.
            let accessors = new EchoBotAccessors(conversationState)             
            accessors.CounterState <- conversationState.CreateProperty<CounterState>(EchoBotAccessors.CounterStateName)
            accessors) |> ignore
    member this.Configure(app: IApplicationBuilder, env: IHostingEnvironment, loggerFactory: ILoggerFactory ) =

        this._loggerFactory <- loggerFactory

        app.UseDefaultFiles()
            .UseStaticFiles()
            .UseBotFramework() |> ignore
    member val private _loggerFactory : ILoggerFactory = null with get, set   
    member val private _isProduction: bool = false with get, set     
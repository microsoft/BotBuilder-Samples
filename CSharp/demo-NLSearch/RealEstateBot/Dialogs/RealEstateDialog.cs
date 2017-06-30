using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Internals.Fibers;
using Search.Dialogs;
using Search.Dialogs.UserInteraction;
using Search.Models;
using Search.Services;
using Search.Utilities;
using Microsoft.Bot.Connector;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.LUIS.API;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Azure.Search.Models;
using Search.Azure.Services;

namespace RealEstateBot.Dialogs
{
    [Serializable]
    public class RealEstateSearchDialog : AzureSearchDialog
    {
        public RealEstateSearchDialog(
            AzureSearchConfiguration configuration,
            LuisModelAttribute luis,
            SearchSpec query = null,
            bool multipleSelection = false,
            bool includeCount = true,
            IEnumerable<string> refiners = null
            )
            : base(configuration, luis, query, multipleSelection, includeCount, refiners)
        {
        }

        public override IMapper<DocumentSearchResult, GenericSearchResult> SearchResultMapper()
        {
            return new RealEstateMapper();
        }
    }

    [Serializable]
    public class RealEstateDialog : IDialog
    {
        private const string NameKey = "Name";
        private const string QueryKey = "LastQuery";
        private LuisModelAttribute LUISConfiguration;
        private AzureSearchConfiguration SearchConfiguration;
        private SearchSpec Query = new SearchSpec();
        private SearchSpec LastQuery;

        public RealEstateDialog()
        {
        }

        public async Task StartAsync(IDialogContext context)
        {
            var rootPath = HttpContext.Current.Server.MapPath("/");
            SearchConfiguration = new AzureSearchConfiguration
            {
                ServiceName = ConfigurationManager.AppSettings["SearchDialogsServiceName"],
                IndexName = ConfigurationManager.AppSettings["SearchDialogsIndexName"],
                ServiceKey = ConfigurationManager.AppSettings["SearchDialogsServiceKey"],
                SchemaPath = Path.Combine(rootPath, @"dialogs\RealEstate.json")
            };
            var key = ConfigurationManager.AppSettings["LUISSubscriptionKey"];
            var spelling = ConfigurationManager.AppSettings["LUISSpellingKey"];
            var domain = ConfigurationManager.AppSettings["LUISDomain"];
            var staging = bool.Parse(ConfigurationManager.AppSettings["LUISStaging"]);

            // For local debugging without checking in keys
            if (string.IsNullOrWhiteSpace(key))
            {
                key = Environment.GetEnvironmentVariable("LUISSubscriptionKey");
            }
            if (string.IsNullOrWhiteSpace(spelling))
            {
                spelling = Environment.GetEnvironmentVariable("LUISSpellingKey");
            }

            var subscription = new Subscription(domain, key);
            var application = await subscription.GetOrImportApplicationAsync(
                        Path.Combine(rootPath, @"dialogs\RealEstateModel.json"),
                        context.CancellationToken, spelling);
            LUISConfiguration = new LuisModelAttribute(application.ApplicationID, key, domain: domain, spellCheck: spelling != null, staging: staging);
            context.Wait(IgnoreFirstMessage);
        }

        private async Task IgnoreFirstMessage(IDialogContext context, IAwaitable<IMessageActivity> msg)
        {
            string name;
            if (context.UserData.TryGetValue(NameKey, out name))
            {
                await context.PostAsync($"Welcome back to the real estate bot {name}!");
                try
                {
                    byte[] lastQuery;
                    if (context.UserData.TryGetValue(QueryKey, out lastQuery))
                    {
                        using (var stream = new MemoryStream(lastQuery))
                        {
                            var formatter = new BinaryFormatter();
                            this.LastQuery = (SearchSpec)formatter.Deserialize(stream);
                        }
                        await context.PostAsync($@"**Last Search**

{this.LastQuery.Description(RealEstateSearchDialog.DefaultResources)}");
                        context.Call(new PromptDialog.PromptConfirm("Do you want to start from your last search?", null, 1, promptStyle: PromptStyle.Keyboard), UseLastSearch);
                    }
                    else
                    {
                        Search(context);
                    }
                }
                catch (Exception)
                {
                    context.UserData.RemoveValue(QueryKey);
                    Search(context);
                }
            }
            else
            {
                await context.PostAsync("Welcome to the real estate search bot!");
                context.Call(
                    new PromptDialog.PromptString("What is your name?", "What is your name?", 2),
                    GotName);
            }
        }

        public async Task GotName(IDialogContext context, IAwaitable<string> name)
        {
            var newName = await name;
            await context.PostAsync($"Good to meet you {newName}!");
            context.UserData.SetValue(NameKey, newName);
            Search(context);
        }

        private async Task UseLastSearch(IDialogContext context, IAwaitable<bool> answer)
        {
            if (await answer)
            {
                this.Query = this.LastQuery;
            }
            else
            {
                this.Query = new SearchSpec();
            }
            this.LastQuery = null;
            Search(context);
        }

        private void Search(IDialogContext context)
        {
            context.Call(new RealEstateSearchDialog(
                SearchConfiguration, LUISConfiguration,
                multipleSelection: true,
                query: this.Query,
                refiners: new string[]
                {
                    "type", "beds", "baths", "sqft", "price",
                    "Keyword", "city", "district", "region",
                    "daysOnMarket", "status"
                }), Done);
        }

        private async Task Done(IDialogContext context, IAwaitable<IList<SearchHit>> input)
        {
            var selection = await input;

            if (selection != null && selection.Any())
            {
                var list = string.Join("\n\n", selection.Select(s => $"* {s.Title} ({s.Key})"));
                await context.PostAsync($"Done! For future reference, you selected these properties:\n\n{list}");
            }
            else
            {
                await context.PostAsync($"Sorry you could not find anything you liked--maybe next time!");
            }
            if (this.Query.HasNoConstraints)
            {
                // Reset name and query if no query
                context.UserData.RemoveValue(NameKey);
                context.UserData.RemoveValue(QueryKey);
            }
            else
            {
                this.Query.PageNumber = 0;
                using (var stream = new MemoryStream())
                {
                    var formatter = new BinaryFormatter();
                    formatter.Serialize(stream, this.Query);
                    context.UserData.SetValue(QueryKey, stream.ToArray());
                }
            }
            context.Done<object>(null);
        }
    }
}
Building a MS Teams bot using Microsoft AI (C\#)
================================================

In this tutorial we will cover how to build a Search and Summary Bot - Zummer
using Microsoft Cognitive Services - [Bing Web Search
API](https://www.microsoft.com/cognitive-services/en-us/bing-web-search-apihttps://www.microsoft.com/cognitive-services/en-us/bing-news-search-api),
[Language Understanding Intelligent Services
(LUIS)](https://www.microsoft.com/cognitive-services/en-us/language-understanding-intelligent-service-luis)
and [Bing Summarizer
API](https://www.microsoft.com/cognitive-services/en-us/labs).

<https://azuredeploy.net?repository=https://github.com/microsoft/BotBuilder-Samples/tree/master/CSharp/intelligence-Zummer>

Bot Recipe/Prerequisites:
-------------------------

-   **Microsoft Bot Framework** to host and publish to multiple platforms. You
    can download Bot Framework Emulator from
    [here](https://aka.ms/bf-bc-emulator). More details in [this documentation
    article](https://docs.botframework.com/en-us/csharp/builder/sdkreference/gettingstarted.html) 

-   **Bing Web Search API** to fetch most relevant Wikipedia article on any
    given topic.

-   **Bing Summarizer API** to summarize the Wikipedia article

-   **Luis.ai** to understand client’s query intent

-   **Azure Account** you can create a free account
    [here](https://azure.microsoft.com/en-gb/free/)

-   **Microsoft Teams Account**

 

Let's get started, Shall we?
----------------------------

This tutorial would help you understand how to string together various Cognitive
APIs -  Bing Web Search, Bing Summarizer and LUIS to build a productivity bot

Here is a simple flowchart of what the Zummer bot logic will be:

 

![](images/0.png)

Creating a LUIS application and training it to understand the query intent
--------------------------------------------------------------------------

Zummer bot is trained to understand the following intents:

-   Greeting

-   Article Search by topic

 

1.  **Sign in and Create an application** on [www.luis.ai](http://www.luis.ai/)  
    **Note:** You can either import the LUIS application JSON file
    “ZummerLuisApp.json” found in the sample folder

    ![](images/1.PNG)

2.  **Create intent, entities and train LUIS**

    1.  Add an intent for Greeting and Search each by clicking on '+'

        ![](images/2.PNG)

    2.  Add utterances like Hi, Hello, etc. and assign it to the Greeting intent

    3.  For Search Intent: Create “ArticleTopic” Entity

        ![](images/3.PNG)

    4.  Add utterances for queries that contain "ArticleTopic" entity

        ![](images/4.PNG)

3.  **Train your models** by clicking “Train”

4.  **Publish your application**

5.  **Save your published endpoint URL** to be used when creating your bot using
    the bot framework

    ![](images/5.PNG)

### Calling the LUIS application from MS bot framework project

1.  Create a LuisDialog as in MainDialog.cs

    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ c#
    [Serializable]
    internal sealed class MainDialog : LuisDialog<object>
    {
    private readonly IHandlerFactory handlerFactory;

    public MainDialog(ILuisService luis, IHandlerFactory handlerFactory)
        : base(luis)
    {
        SetField.NotNull(out this.handlerFactory, nameof(handlerFactory), handlerFactory);
    }
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

2.  Register both MainDialog and LuisModelAttribute (use the modelId and
    subscriptionKey from the published LUIS URL) in the dependency container, in
    MainModule.cs

    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ c#
    internal sealed class MainModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.Register(c => new LuisModelAttribute("b550e80a-74ec-4bb4-bcbc-fe35f5b1fce4", "a6d628faa2404cd799f2a291245eb135")).AsSelf().AsImplementedInterfaces().SingleInstance();

            // Top Level Dialog
            builder.RegisterType<MainDialog>().As<IDialog<object>>().InstancePerDependency();
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

3.  Create methods in this dialog to handle your LUIS intents. In this tutorial
    a factory is created that returns the needed object that is responsible
    handling certain intent and  responding to the user

    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ c#
    [LuisIntent(ZummerStrings.GreetingIntentName)]
    public async Task GreetingIntentHandlerAsync(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
    {
        await this.handlerFactory.CreateIntentHandler(ZummerStrings.GreetingIntentName).Respond(activity, result);
        context.Wait(this.MessageReceived);
    }

    [LuisIntent(ZummerStrings.SearchIntentName)]
    public async Task FindArticlesIntentHandlerAsync(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
    {
        await this.handlerFactory.CreateIntentHandler(ZummerStrings.SearchIntentName).Respond(activity, result);
        context.Wait(this.MessageReceived);
    }
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

 

Fetching Wikipedia articles on a topic using Bing Web Search API
----------------------------------------------------------------

1.  The model classes that represent the Bing Web Search API JSON response can
    be found in folder “Models\\Search*”*

2.  Create “Key” that will be used for calling the Bing APIs on [Microsoft
    Cognitive Service
    subscriptions](https://www.microsoft.com/cognitive-services/en-US/subscriptions)
    using the free tier

3.  Bing Web Search API request format details could be found at [Bing Web
    Search API
    reference](https://dev.cognitive.microsoft.com/docs/services/56b43eeccf5ff8098cef3807/operations/56b4447dcf5ff8098cef380d)
    page  
    This tutorial implements communication with Bing Web Search API service and
    manipulating the user's query to get response with only Wikipedia
    articles through “FindArticles”  in BingSearchServices.cs

    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ c#
    namespace Zummer.Services 
    { 
        /// <summary> 
        /// Responsible for calling Bing Web Search API 
        /// </summary> 
        internal sealed class BingSearchService : ISearchService 
        { 
            private const string BingSearchEndpoint = "https://api.cognitive.microsoft.com/bing/v5.0/search/"; 
     
            private static readonly Dictionary<string, string> Headers = new Dictionary<string, string> 
            { 
                { "Ocp-Apim-Subscription-Key", ConfigurationManager.AppSettings["BingSearchServiceKey"] } 
            };  
     
            private readonly IApiHandler apiHandler; 
     
            public BingSearchService(IApiHandler apiHandler) 
            { 
                SetField.NotNull(out this.apiHandler, nameof(apiHandler), apiHandler); 
            } 
     
            public async Task<BingSearch> FindArticles(string query) 
            { 
                var requestParameters = new Dictionary<string, string> 
                { 
                    { "q", $"{query} site:wikipedia.org" } 
                }; 
     
                return await this.apiHandler.GetJsonAsync<BingSearch>(BingSearchEndpoint, requestParameters, Headers); 
            } 
        } 
    }
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    **Note:** ApiHandler is the class implementing IApiHandler. It is a
    singleton wrapper for HttpClient, to send Http requests. Code can be found
    in “Services\\ApiHandler.cs”

4.  SearchIntentHandler.cs “Respond” method contains

    1.  Calling *“*FindArticles” methods to receive the BingSearch response

    2.  Fetching first result and extracting from it the needed information to
        be used afterwards in the summarization logic by "PrepareZummerResult"
        method

    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ c#
    public async Task Respond(IAwaitable<IMessageActivity> activity, LuisResult result) 
    { 
        EntityRecommendation entityRecommendation; 

        var query = result.TryFindEntity(ZummerStrings.ArticlesEntityTopic, out entityRecommendation) 
            ? entityRecommendation.Entity 
            : result.Query; 

        await this.botToUser.PostAsync(string.Format(Strings.SearchTopicTypeMessage)); 

        var bingSearch = await this.bingSearchService.FindArticles(query); 

        var zummerResult = this.PrepareZummerResult(query, bingSearch.webPages.value[0]); 

       ... 
    }
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ c#
    private ZummerSearchResult PrepareZummerResult(string query, Value page) 
        { 
            string url; 
            var myUri = new Uri(page.url); 
     
            if (myUri.Host == "www.bing.com" && myUri.AbsolutePath == "/cr") 
            { 
                url = HttpUtility.ParseQueryString(myUri.Query).Get("r"); 
            } 
            else 
            { 
                url = page.url; 
            } 
     
            var zummerResult = new ZummerSearchResult 
            { 
                Url = url, 
                Query = query, 
                Tile = page.name 
            }; 
     
            return zummerResult; 
        }
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

 

Summarizing the Wikipedia article
---------------------------------

1.  Create “Key” that will be used for calling the Bing Summarizer APIs on
    [Microsoft Cognitive Service
    subscriptions](https://www.microsoft.com/cognitive-services/en-US/subscriptions)
    using the labs tab** *** *

2.  The model classes that represent the [Bing Summarizer
    API](https://cognitivegarage.portal.azure-api.net/docs/services/582ba037f1038311cc8b8ce8/operations/582ba057f1038311cc8b8ce9)
    JSON response can be found in the folder “Models\\Summarize”

3.  Bing Summarizer API is still being worked upon and hence is available in the
    [Microsoft Cognitive
    Labs](https://www.microsoft.com/cognitive-services/en-us/labs). In this
    tutorial summarization functionality is exposed in the bot code through
    “GetSummary” function in BingSummarizeService.cs

    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ c#
    namespace Newsie.Services 
    { 
        /// <summary> 
        /// Responsible for calling Bing Summarizer API 
        /// </summary> 
        internal sealed class BingSummarizeService : ISummarizeService 
        { 
            private const string BingSummarizeEndpoint = "https://cognitivegarage.azure-api.net/bingSummarizer/summary"; 
     
            private static readonly Dictionary<string, string> Headers = new Diction-ary<string, string> 
            { 
                { "Ocp-Apim-Subscription-Key", ConfigurationManag-er.AppSettings["BingSummarizeSerivceKey"] } 
            }; 
     
            private readonly IApiHandler apiHandler; 
     
            public BingSummarizeService(IApiHandler apiHandler) 
            { 
                SetField.NotNull(out this.apiHandler, nameof(apiHandler), apiHandler); 
            } 
     
            public async Task<BingSummarize> GetSummary(string url) 
            { 
                var requestParameters = new Dictionary<string, string> 
                { 
                    { "url", url } 
                }; 
     
                return await this.apiHandler.GetJsonAsync<BingSummarize>(BingSummarizeEndpoint, requestParameters, Headers); 
            } 
        } 
    } 
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

4.  Add to the  SearchIntentHandler.cs “Respond” method the following logic:

    1.  Invoke the “GetSummary” method to receive the BingSummarize response.

    2.  [Format](https://docs.botframework.com/en-us/csharp/builder/sdkreference/activities.html#message)
        the summary and text response that will be sent to the client and send
        the response

    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ c#
    public async Task Respond(IAwaitable<IMessageActivity> activity, LuisResult result) 
    { 
         ... 
     
         var bingSummary = await this.bingSummarizeService.GetSummary(zummerResult.Url); 
         if (bingSummary?.Data != null && bingSummary.Data.Length != 0) 
         { 
             var summaryText = bingSummary.Data.Aggregate( 
                 $"### [{zummerResult.Tile}]({zummerResult.Url})" 
                 + "\n" + 
                 $"**{Strings.SummaryString}**" 
                 + "\n\n", 
                 (current, datum) => current + (datum.Text + "\n\n")); 

             summaryText += 
                 $"*{string.Format(Strings.PowerBy, "[Bing™](https://www.bing.com/search/?q={zummerResult.Query} site:wikipedia.org)")}*"; 
     
             await this.botToUser.PostAsync(summaryText); 
         } 
         else 
         { 
             await this.botToUser.PostAsync(Strings.SummaryErrorMessage); 
         } 
    }
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

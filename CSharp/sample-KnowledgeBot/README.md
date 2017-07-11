# Document for C# code
If you are new for developing C# bot, please see [this document](https://docs.microsoft.com/en-us/bot-framework/dotnet/bot-builder-dotnet-quickstart) and try it.

In this document, we first describe how the code works. Then we explain how to try this sample.

[![Deploy to Azure][Deploy Button]][Deploy CSharp/KnowledgeBot]

[Deploy Button]: https://azuredeploy.net/deploybutton.png
[Deploy CSharp/KnowledgeBot]: https://azuredeploy.net

## How the code works
All messages get routed into the MessagesController.cs. From here, we replace the dialog with the PromptButtonsDialog dialog.

```cs
public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
{
    if (activity.Type == ActivityTypes.Message)
    {
        ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
        await Conversation.SendAsync(activity, () => new PromptButtonsDialog());
    }
    else
    {
        HandleSystemMessage(activity);
    }
    var response = Request.CreateResponse(HttpStatusCode.OK);
    return response;
}
```

In the PromptButtonsDialog we use PromptDialog.Choice method to prompt the user with our choices (in this case Musician Explorer and Musician Search) defined in the class variable.
Once the user answers, we move into the next function which uses a switch statement to decide which dialog to route us to. Note that the MusicianExplorerDialog and MusicianSearchDialog dialogs each have their own .cs file in the 'Dialogs' folder.
```cs
public virtual async Task MessageRecievedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
{
    //Show options whatever users chat
    PromptDialog.Choice(context, this.AfterMenuSelection, new List<string>() {ExplorerOption , SearchOption}, "How would you like to explore the classical music bot?");
}

//After users select option, Bot call other dialogs
private async Task AfterMenuSelection(IDialogContext context, IAwaitable<string> result)
{
    var optionSelected = await result;
    switch(optionSelected)
    {
        case ExplorerOption:
            context.Call(new MusicianExplorerDialog(), ResumeAfterOptionDialog);
            break;
        case SearchOption:
            context.Call(new MusicianSearchDialog(), ResumeAfterOptionDialog);
            break;
    }
}
```

The musician search dialog first prompts the user to type in the name of the musician that he/she is looking for:

```cs
public async Task StartAsync(IDialogContext context)
{
    await context.PostAsync("Type in the name of the musician you are searching for:");
    context.Wait(MessageRecievedAsync);
}
```

It then gets the name the user typed in and passes in searchService.SearchByName to generate a basic search against our index.
If it gets results from the query it routes us to CardUtil.showHeroCard, which we can think of as a view layer.
```cs
public virtual async Task MessageRecievedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
{
    var message = await result;
    try
    {
        SearchResult searchResult = await searchService.SearchByName(message.Text);
        if(searchResult.value.Length != 0)
        {
            CardUtil.showHeroCard(message, searchResult);
        }
        else{
            await context.PostAsync($"No musicians by the name {message.Text} found");
        }
    }
    catch(Exception e)
    {
        Debug.WriteLine($"Error when searching for musician: {e.Message}");
    }
    context.Done<object>(null);
}
```

Note that our error handling for this example simply logs the error to console - in a real world bot we would want to be more involved in 
our error handling. 

Our musician explorer is a bit more involved. First it gathers our era facets and prompts the user to choose which one he/she is interested in. 
Again we create a queryString (this time passing 'facet=Era') in searchService.FetchFacets() function and perform our search query, which gives us a JSON response of all of the eras of musicians that are represented in our index:
```cs
public async Task StartAsync(IDialogContext context)
{
    try
    {
        FacetResult facetResult = await searchService.FetchFacets();
        if (facetResult.searchfacets.Era.Length != 0)
        {
            List<string> eras = new List<string>();
            foreach (Era era in facetResult.searchfacets.Era)
            {
                eras.Add($"{era.value} ({era.count})");
            }
            PromptDialog.Choice(context, AfterMenuSelection, eras, "Which era of music are you interested in?");
        }
        else
        {
            await context.PostAsync("I couldn't find any genres to show you");
        }
    }
    catch (Exception e)
    {
        Debug.WriteLine($"Error when faceting by era: {e}");
    }
}
```

Once the user selects the era that they are interested in we perform a filter query, passing $filter=Era eq selectedEra
```cs
private async Task AfterMenuSelection(IDialogContext context, IAwaitable<string> result)
{
    var optionSelected = await result;
    string selectedEra = optionSelected.Split(' ')[0];

    try
    {
        SearchResult searchResult = await searchService.SearchByEra(selectedEra);
        if(searchResult.value.Length != 0)
        {
            CardUtil.showHeroCard((IMessageActivity)context.Activity, searchResult);
        }
        else
        {
            await context.PostAsync($"I couldn't find any musicians in that era :0");
        }
    }
    catch(Exception e)
    {
        Debug.WriteLine($"Error when filtering by genre: {e}");
    }
    context.Done<object>(null);
}
```

This gives us all of the musicians that map to the era the user chose. Once we have results, we again send them to our CardUtil.showHeroCard function.

Each functions in AzureSearchService performs a basic query using the httpClient.
Note, we're performing a GET for this example, but for production bots/apps you may consider using a POST so that you can place you api key in the POST header

```cs
[Serializable]
public class AzureSearchService
{
    private static readonly string QueryString = $"https://{WebConfigurationManager.AppSettings["SearchName"]}.search.windows.net/indexes/{WebConfigurationManager.AppSettings["IndexName"]}/docs?api-key={WebConfigurationManager.AppSettings["SearchKey"]}&api-version=2015-02-28&";

    public async Task<SearchResult> SearchByName(string name)
    {
        using (var httpClient = new HttpClient())
        {
            string nameQuey = $"{QueryString}search={name}";
            string response = await httpClient.GetStringAsync(nameQuey);
            return JsonConvert.DeserializeObject<SearchResult>(response);
        }
    }

    public async Task<FacetResult> FetchFacets()
    {
        using (var httpClient = new HttpClient())
        {
            string facetQuey = $"{QueryString}facet=Era";
            string response = await httpClient.GetStringAsync(facetQuey);
            return JsonConvert.DeserializeObject<FacetResult>(response);
        }
    }

    public async Task<SearchResult> SearchByEra(string era)
    {
        using (var httpClient = new HttpClient())
        {
            string nameQuey = $"{QueryString}$filter=Era eq '{era}'";
            string response = await httpClient.GetStringAsync(nameQuey);
            return JsonConvert.DeserializeObject<SearchResult>(response);
        }
    }
}
```

The CardUtil receives the results from the MusicianExplorerDialog and MusicianSearchDialog dialogs as properties of the `searchResult` parameter.
It then creates a new message with a carousel layout, and adds a hero card attachment with the name, era, search score, description and image for each musician.
```cs
public static class CardUtil
{
    public static async void showHeroCard(IMessageActivity message, SearchResult searchResult)
    {
        //Make reply activity and set layout
        Activity reply = ((Activity)message).CreateReply();
        reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;

        //Make each Card for each musician
        foreach (Value musician in searchResult.value)
        {
            List<CardImage> cardImages = new List<CardImage>();
            cardImages.Add(new CardImage(url: musician.imageURL));
            HeroCard card = new HeroCard()
            {
                Title = musician.Name,
                Subtitle = $"Era: {musician.Era } | Search Score: {musician.searchscore}",
                Text = musician.Description,
                Images = cardImages
            };
            reply.Attachments.Add(card.ToAttachment());
        }

        //make connector and reply message
        ConnectorClient connector = new ConnectorClient(new Uri(reply.ServiceUrl));
        await connector.Conversations.SendToConversationAsync(reply);
    }
}
```

## How to try this sample
Finally, let's test our bot out. You can tyr it with emulator after setting Web.config with search credentials.
I will demonstrate the bot working in the bot framework emulator, but if deployed to Azure Web Apps, this bot could be enabled on several different channels. 

### How to set search credentials in Web.Config
We should set `SearchName`,`IndexName`,`SearchKey` in Web.config.

1. Access Azure portal and see Azure Search you created. 
2. You can check `SearchName` in Url (in this window, `masotabot`), so copy&paste it in Web.config.
3. You can see `IndexName` in Indexes area (like `temp`). Please copy and paste it in Web.config.
4. Click [All settings]-[Keys]-[Manage query keys] and you can check Key. Please copy it and paste it in Web.config.

After setting web.config, you can start debugging. 

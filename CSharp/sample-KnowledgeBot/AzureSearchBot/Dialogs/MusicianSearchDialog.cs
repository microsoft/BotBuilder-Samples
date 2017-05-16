using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using AzureSearchBot.Services;
using AzureSearchBot.Model;
using System.Diagnostics;
using AzureSearchBot.Util;

namespace AzureSearchBot.Dialogs
{
    [Serializable]
    public class MusicianSearchDialog : IDialog<object>
    {
        private readonly AzureSearchService searchService = new AzureSearchService();
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("Type in the name of the musician you are searching for:");
            context.Wait(MessageRecievedAsync);
        }

        public virtual async Task MessageRecievedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            try
            {
                SearchResult searchResult = await searchService.SearchByName(message.Text);
                if(searchResult.value.Length != 0)
                {
                    CardUtil.ShowHeroCard(message, searchResult);
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
    }
}
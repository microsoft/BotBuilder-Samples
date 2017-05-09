using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using AzureSearchBot.Services;
using Microsoft.Bot.Connector;
using System.Diagnostics;
using AzureSearchBot.Model;
using System.Collections.Generic;
using AzureSearchBot.Util;

namespace AzureSearchBot.Dialogs
{
    [Serializable]
    public class MusicianExplorerDialog : IDialog<object>
    {
        private readonly AzureSearchService searchService = new AzureSearchService();
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
                context.Done <object>(null);
            }
        }

        private async Task AfterMenuSelection(IDialogContext context, IAwaitable<string> result)
        {
            var optionSelected = await result;
            string selectedEra = optionSelected.Split(' ')[0];

            try
            {
                SearchResult searchResult = await searchService.SearchByEra(selectedEra);
                if(searchResult.value.Length != 0)
                {
                    CardUtil.ShowHeroCard((IMessageActivity)context.Activity, searchResult);
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
    }
}
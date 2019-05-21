// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using ContosoHelpdeskChatBot.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ContosoHelpdeskChatBot.Dialogs
{
    public class InstallAppDialog : ComponentDialog
    {
        // Set up keys for managing collected information.
        private const string InstallInfo = "installInfo";

        public InstallAppDialog()
            : base(nameof(InstallAppDialog))
        {
            // Initialize our dialogs and prompts.
            InitialDialogId = nameof(WaterfallDialog);
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[] {
                GetSearchTermAsync,
                ResolveAppNameAsync,
                GetMachineNameAsync,
                SubmitRequestAsync,
            }));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
        }

        private async Task<DialogTurnResult> GetSearchTermAsync(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // Create an object in dialog state in which to track our collected information.
            stepContext.Values[InstallInfo] = new InstallApp();

            // Ask for the search term.
            return await stepContext.PromptAsync(
                nameof(TextPrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Ok let's get started. What is the name of the application? "),
                },
                cancellationToken);
        }

        private async Task<DialogTurnResult> ResolveAppNameAsync(
    WaterfallStepContext stepContext,
    CancellationToken cancellationToken = default(CancellationToken))
        {
            // Get the result from the text prompt.
            var appname = stepContext.Result as string;

            // Query the database for matches.
            var names = await this.GetAppsAsync(appname);

            if (names.Count == 1)
            {
                // Get our tracking information from dialog state and add the app name.
                var install = stepContext.Values[InstallInfo] as InstallApp;
                install.AppName = names.First();

                return await stepContext.NextAsync();
            }
            else if (names.Count > 1)
            {
                // Ask the user to choose from the list of matches.
                return await stepContext.PromptAsync(
                    nameof(ChoicePrompt),
                    new PromptOptions
                    {
                        Prompt = MessageFactory.Text("I found the following applications. Please choose one:"),
                        Choices = ChoiceFactory.ToChoices(names),
                    },
                    cancellationToken);
            }
            else
            {
                // If no matches, exit this dialog.
                await stepContext.Context.SendActivityAsync(
                    $"Sorry, I did not find any application with the name '{appname}'.",
                    cancellationToken: cancellationToken);

                return await stepContext.EndDialogAsync(null, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> GetMachineNameAsync(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // Get the tracking info. If we don't already have an app name,
            // Then we used the choice prompt to get it in the previous step.
            var install = stepContext.Values[InstallInfo] as InstallApp;
            if (install.AppName is null)
            {
                install.AppName = (stepContext.Result as FoundChoice).Value;
            }

            // We now need the machine name, so prompt for it.
            return await stepContext.PromptAsync(
                nameof(TextPrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text(
                        $"Found {install.AppName}. What is the name of the machine to install application?"),
                },
                cancellationToken);
        }

        private async Task<DialogTurnResult> SubmitRequestAsync(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var install = default(InstallApp);
            if (stepContext.Reason != DialogReason.CancelCalled)
            {
                // Get the tracking info and add the machine name.
                install = stepContext.Values[InstallInfo] as InstallApp;
                install.MachineName = stepContext.Context.Activity.Text;

                //TODO: Save to this information to the database.
            }

            await stepContext.Context.SendActivityAsync(
                $"Great, your request to install {install.AppName} on {install.MachineName} has been scheduled.",
                cancellationToken: cancellationToken);

            return await stepContext.EndDialogAsync(null, cancellationToken);
        }

        private async Task<List<string>> GetAppsAsync(string Name)
        {
            var names = new List<string>();

            // Simulate querying the database for applications that match.
            return (from app in AppMsis
                    where app.ToLower().Contains(Name.ToLower())
                    select app).ToList();
        }

        // Example list of app names in the database.
        private static readonly List<string> AppMsis = new List<string>
        {
            "µTorrent 3.5.0.44178",
            "7-Zip 17.1",
            "Ad-Aware 9.0",
            "Adobe AIR 2.5.1.17730",
            "Adobe Flash Player (IE) 28.0.0.105",
            "Adobe Flash Player (Non-IE) 27.0.0.130",
            "Adobe Reader 11.0.14",
            "Adobe Shockwave Player 12.3.1.201",
            "Advanced SystemCare Personal 11.0.3",
            "Auslogics Disk Defrag 3.6",
            "avast! 4 Home Edition 4.8.1351",
            "AVG Anti-Virus Free Edition 9.0.0.698",
            "Bonjour 3.1.0.1",
            "CCleaner 5.24.5839",
            "Chmod Calculator 20132.4",
            "CyberLink PowerDVD 17.0.2101.62",
            "DAEMON Tools Lite 4.46.1.328",
            "FileZilla Client 3.5",
            "Firefox 57.0",
            "Foxit Reader 4.1.1.805",
            "Google Chrome 66.143.49260",
            "Google Earth 7.3.0.3832",
            "Google Toolbar (IE) 7.5.8231.2252",
            "GSpot 2701.0",
            "Internet Explorer 903235.0",
            "iTunes 12.7.0.166",
            "Java Runtime Environment 6 Update 17",
            "K-Lite Codec Pack 12.1",
            "Malwarebytes Anti-Malware 2.2.1.1043",
            "Media Player Classic 6.4.9.0",
            "Microsoft Silverlight 5.1.50907",
            "Mozilla Thunderbird 57.0",
            "Nero Burning ROM 19.1.1005",
            "OpenOffice.org 3.1.1 Build 9420",
            "Opera 12.18.1873",
            "Paint.NET 4.0.19",
            "Picasa 3.9.141.259",
            "QuickTime 7.79.80.95",
            "RealPlayer SP 12.0.0.319",
            "Revo Uninstaller 1.95",
            "Skype 7.40.151",
            "Spybot - Search & Destroy 1.6.2.46",
            "SpywareBlaster 4.6",
            "TuneUp Utilities 2009 14.0.1000.353",
            "Unlocker 1.9.2",
            "VLC media player 1.1.6",
            "Winamp 5.56 Build 2512",
            "Windows Live Messenger 2009 16.4.3528.331",
            "WinPatrol 2010 31.0.2014",
            "WinRAR 5.0",
        };
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveExpressions.Properties;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Actions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Conditions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Generators;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Input;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Templates;
using Microsoft.Recognizers.Text;
using Newtonsoft.Json.Linq;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// This is an example root dialog. Replace this with your applications.
    /// </summary>
    public class RootDialog : ComponentDialog
    {
        private IStatePropertyAccessor<JObject> _userStateAccessor;

        public RootDialog(UserState userState)
            : base("root")
        {
            _userStateAccessor = userState.CreateProperty<JObject>("result");

            // Rather than explicitly coding a Waterfall we have only to declare what properties we want collected.
            // In this example we will want two text prompts to run, one for the first name and one for the last.
            var fullname_slots = new List<SlotDetails>
            {
                new SlotDetails("first", "text", "Please enter your first name."),
                new SlotDetails("last", "text", "Please enter your last name."),
            };

            // This defines an address dialog that collects street, city and zip properties.
            var address_slots = new List<SlotDetails>
            {
                new SlotDetails("street", "text", "Please enter the street."),
                new SlotDetails("city", "text", "Please enter the city."),
                new SlotDetails("zip", "text", "Please enter the zip."),
            };

            // Dialogs can be nested and the slot filling dialog makes use of that. In this example some of the child
            // dialogs are slot filling dialogs themselves.
            var slots = new List<SlotDetails>
            {
                new SlotDetails("fullname", "fullname"),
                new SlotDetails("age", "number", "Please enter your age."),
                new SlotDetails("shoesize", "shoesize", "Please enter your shoe size.", "You must enter a size between 0 and 16. Half sizes are acceptable."),
                new SlotDetails("address", "address"),
            };

            // define adaptive dialog
            var adaptiveSlotFillingDialog = new AdaptiveDialog();
            adaptiveSlotFillingDialog.Id = nameof(AdaptiveDialog);

            // Set a language generator
            // You can see other adaptive dialog samples to learn how to externalize generation resources into .lg files.
            adaptiveSlotFillingDialog.Generator = new TemplateEngineLanguageGenerator();

            // add set of actions to perform when the adaptive dialog begins.
            adaptiveSlotFillingDialog.Triggers.Add(new OnBeginDialog()
            {
                Actions = new List<Dialog>()
                {
                    // any options passed into adaptive dialog is automatically available under dialog.xxx
                    // get user age
                    new NumberInput()
                    {
                        Property = "dialog.userage",

                        // use information passed in to the adaptive dialog.
                        Prompt = new ActivityTemplate("Hello ${dialog.fullname.first}, what is your age?"),
                        Validations = new List<BoolExpression>()
                        {
                            "int(this.value) >= 1",
                            "int(this.value) <= 150"
                        },
                        InvalidPrompt = new ActivityTemplate("Sorry, ${this.value} does not work. Looking for age to be between 1-150. What is your age?"),
                        UnrecognizedPrompt = new ActivityTemplate("Sorry, I did not understand ${this.value}. What is your age?"),
                        MaxTurnCount = 3,
                        DefaultValue = "=30",
                        DefaultValueResponse = new ActivityTemplate("Sorry, this is not working. For now, I'm setting your age to ${this.defaultValue}"),
                        AllowInterruptions = false
                    },
                    new NumberInput()
                    {
                        Property = "dialog.shoesize",
                        Prompt = new ActivityTemplate("Please enter your shoe size."),
                        InvalidPrompt = new ActivityTemplate("Sorry ${this.value} does not work. You must enter a size between 0 and 16. Half sizes are acceptable."),
                        Validations = new List<BoolExpression>()
                        {
                            // size can only between 0-16
                            "int(this.value) >= 0 && int(this.value) <= 16",
                            // can only full or half size
                            "isMatch(string(this.value), '^[0-9]+(\\.5)*$')"
                        },
                        AllowInterruptions = false
                    },
                    // get address - adaptive is calling the custom slot filling dialog here.
                    new BeginDialog()
                    {
                        Dialog = "address",
                        ResultProperty = "dialog.address"
                    },
                    // return everything under dialog scope. 
                    new EndDialog()
                    {
                        Value = "=dialog"
                    }
                }
            }) ;

            // Add the various dialogs that will be used to the DialogSet.
            AddDialog(new SlotFillingDialog("address", address_slots));
            AddDialog(new SlotFillingDialog("fullname", fullname_slots));
            AddDialog(new TextPrompt("text"));
            AddDialog(new NumberPrompt<int>("number", defaultLocale: Culture.English));
            AddDialog(new NumberPrompt<float>("shoesize", ShoeSizeAsync, defaultLocale: Culture.English));

            // We will instead have adaptive dialog do the slot filling by invoking the custom dialog
            // AddDialog(new SlotFillingDialog("slot-dialog", slots));

            // Add adaptive dialog
            AddDialog(adaptiveSlotFillingDialog);

            // Defines a simple two step Waterfall to test the slot dialog.
            AddDialog(new WaterfallDialog("waterfall", new WaterfallStep[] { StartDialogAsync, DoAdaptiveDialog, ProcessResultsAsync }));

            // The initial child Dialog to run.
            InitialDialogId = "waterfall";
        }

        private Task<bool> ShoeSizeAsync(PromptValidatorContext<float> promptContext, CancellationToken cancellationToken)
        {
            var shoesize = promptContext.Recognized.Value;

            // show sizes can range from 0 to 16
            if (shoesize >= 0 && shoesize <= 16)
            {
                // we only accept round numbers or half sizes
                if (Math.Floor(shoesize) == shoesize || Math.Floor(shoesize * 2) == shoesize * 2)
                {
                    // indicate success by returning the value
                    return Task.FromResult(true);
                }
            }

            return Task.FromResult(false);
        }
        
        private async Task<DialogTurnResult> StartDialogAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Start the child dialog. This will just get the user's first and last name.
            return await stepContext.BeginDialogAsync("fullname", null, cancellationToken);
        }

        private async Task<DialogTurnResult> DoAdaptiveDialog(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            object adaptiveOptions = null;
            if (stepContext.Result is IDictionary<string, object> result && result.Count > 0)
            {
                adaptiveOptions = new { fullname = result };
            }
            // begin the adaptive dialog. This in-turn will get user's age, shoe-size using adaptive inputs and subsequently
            // call the custom slot filling dialog to fill user address.
            return await stepContext.BeginDialogAsync(nameof(AdaptiveDialog), adaptiveOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> ProcessResultsAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // To demonstrate that the slot dialog collected all the properties we will echo them back to the user.
            if (stepContext.Result is IDictionary<string, object> result && result.Count > 0)
            {
                // Now the waterfall is complete, save the data we have gathered into UserState.
                // This includes data returned by the adaptive dialog.
                var obj = await _userStateAccessor.GetAsync(stepContext.Context, () => new JObject());
                obj["data"] = new JObject
                {
                    { "fullname",  $"{result["fullname"]}" },
                    { "shoesize", $"{result["shoesize"]}" },
                    { "address", $"{result["address"]}" },
                };

                await stepContext.Context.SendActivityAsync(MessageFactory.Text(obj["data"]["fullname"].Value<string>()), cancellationToken);
                await stepContext.Context.SendActivityAsync(MessageFactory.Text(obj["data"]["shoesize"].Value<string>()), cancellationToken);
                await stepContext.Context.SendActivityAsync(MessageFactory.Text(obj["data"]["address"].Value<string>()), cancellationToken);
            }

            // Remember to call EndAsync to indicate to the runtime that this is the end of our waterfall.
            return await stepContext.EndDialogAsync();
        }
    }
}

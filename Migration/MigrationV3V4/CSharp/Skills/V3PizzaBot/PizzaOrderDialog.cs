// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using Newtonsoft.Json;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;

namespace Microsoft.Bot.Sample.PizzaBot
{
    [LuisModel("4311ccf1-5ed1-44fe-9f10-a6adbad05c14", "6d0966209c6e4f6b835ce34492f3e6d9", LuisApiVersion.V2)]
    [Serializable]
    class PizzaOrderDialog : LuisDialog<PizzaOrder>
    {
        private readonly BuildFormDelegate<PizzaOrder> MakePizzaForm;

        internal PizzaOrderDialog(BuildFormDelegate<PizzaOrder> makePizzaForm)
        {
            this.MakePizzaForm = makePizzaForm;
        }

        public override async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("Welcome to the Pizza Order Bot. Let me know if you would like to order a pizza, or know our store hours.");
            await context.PostAsync("Say 'end' or 'stop' and I'll end the conversation and back to the parent.");

            await base.StartAsync(context);
        }

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("I'm sorry. I didn't understand you.");
            context.Wait(MessageReceived);
        }

        [LuisIntent("OrderPizza")]
        [LuisIntent("UseCoupon")]
        public async Task ProcessPizzaForm(IDialogContext context, LuisResult result)
        {
            var entities = new List<EntityRecommendation>(result.Entities);
            if (!entities.Any((entity) => entity.Type == "Kind"))
            {
                // Infer kind
                foreach (var entity in result.Entities)
                {
                    string kind = null;
                    switch (entity.Type)
                    {
                        case "Signature": kind = "Signature"; break;
                        case "GourmetDelite": kind = "Gourmet delite"; break;
                        case "Stuffed": kind = "stuffed"; break;
                        default:
                            if (entity.Type.StartsWith("BYO")) kind = "byo";
                            break;
                    }
                    if (kind != null)
                    {
                        entities.Add(new EntityRecommendation(type: "Kind") { Entity = kind });
                        break;
                    }
                }
            }

            var pizzaForm = new FormDialog<PizzaOrder>(new PizzaOrder(), this.MakePizzaForm, FormOptions.PromptInStart, entities);
            context.Call<PizzaOrder>(pizzaForm, PizzaFormComplete);
        }

        private async Task PizzaFormComplete(IDialogContext context, IAwaitable<PizzaOrder> result)
        {
            PizzaOrder order = null;
            try
            {
                order = await result;
            }
            catch (OperationCanceledException)
            {
                await context.PostAsync("You canceled the form!");

                // If the user cancels the skill, send an `endOfConversation` activity to the skill consumer.
                await ConversationHelper.EndConversation(context.Activity as Activity, endOfConversationCode: EndOfConversationCodes.UserCancelled);
                return;
            }

            if (order != null)
            {
                await context.PostAsync("Your Pizza Order: " + order.ToString());
            }
            else
            {
                await context.PostAsync("Form returned empty response!");
            }

            // When the skill completes, send an `endOfConversation` activity and include the finished order.
            await ConversationHelper.EndConversation(context.Activity as Activity, order);
            context.Wait(MessageReceived);
        }

        enum Days { Saturday, Sunday, Monday, Tuesday, Wednesday, Thursday, Friday };

        [LuisIntent("StoreHours")]
        public async Task ProcessStoreHours(IDialogContext context, LuisResult result)
        {
            // Figuring out if the action is triggered or not
            var bestIntent = BestIntentFrom(result);

            // extracting day parameter value from Entities, if present
            var entity = result.Entities.FirstOrDefault(e => e.Type.ToLower() == "day");
            if (entity != null)
            {
                var dayParam = entity.Entity;
                Days day;
                if (Enum.TryParse(dayParam, true, out day))
                {
                    await this.StoreHoursResult(context, Awaitable.FromItem(day));
                    return;
                }
            }

            var days = (IEnumerable<Days>)Enum.GetValues(typeof(Days));
            PromptDialog.Choice(context, StoreHoursResult, days, "Which day of the week?",
                descriptions: from day in days
                              select (day == Days.Saturday || day == Days.Sunday) ? day.ToString() + "(no holidays)" : day.ToString());
        }

        private async Task StoreHoursResult(IDialogContext context, IAwaitable<Days> day)
        {
            var hours = string.Empty;
            switch (await day)
            {
                case Days.Saturday:
                case Days.Sunday:
                    hours = "5pm to 11pm";
                    break;
                default:
                    hours = "11am to 10pm";
                    break;
            }

            var text = $"Store hours {(await day).ToString()} are {hours}";
            await context.PostAsync(text);

            context.Wait(MessageReceived);
        }
    }
}

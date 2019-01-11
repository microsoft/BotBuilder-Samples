// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace Microsoft.BotBuilderSamples
{
    public class GetLocationDateTimePartySizePrompt : TextPrompt
    {
        private const string ContinuePromptIntent = "GetLocationDateTimePartySize";
        private const string HelpIntent = "Help";
        private const string CancelIntent = "Cancel";
        private const string InterruptionsIntent = "Interruptions";
        private const string NoChangeIntent = "noChange";
        private const string InterruptionDispatcher = "interruptionDispatcherDialog";
        private const string ConfirmCancelPrompt = "confirmCancelPrompt";

        // LUIS service type entry for turn.n book table LUIS model in the .bot file.
        private static readonly string LuisConfiguration = $"cafeBotBookTableTurnNModel";

        private readonly BotServices _botServices;
        private readonly IStatePropertyAccessor<UserProfile> _userProfileAccessor;
        private readonly IStatePropertyAccessor<OnTurnProperty> _onTurnAccessor;
        private readonly IStatePropertyAccessor<ReservationProperty> _reservationsAccessor;

        public GetLocationDateTimePartySizePrompt(
                    string dialogId,
                    BotServices botServices,
                    IStatePropertyAccessor<ReservationProperty> reservationsAccessor,
                    IStatePropertyAccessor<OnTurnProperty> onTurnAccessor,
                    IStatePropertyAccessor<UserProfile> userProfileAccessor,
                    PromptValidator<string> validator = null)
            : base(dialogId, validator)
        {
            _botServices = botServices ?? throw new ArgumentNullException(nameof(botServices));
            _userProfileAccessor = userProfileAccessor ?? throw new ArgumentNullException(nameof(botServices));
            _onTurnAccessor = onTurnAccessor ?? throw new ArgumentNullException(nameof(onTurnAccessor));
            _reservationsAccessor = reservationsAccessor ?? throw new ArgumentNullException(nameof(reservationsAccessor));
        }

        public async override Task<DialogTurnResult> ContinueDialogAsync(DialogContext dc, CancellationToken cancellationToken = default(CancellationToken))
        {
            var turnContext = dc.Context;
            var step = dc.ActiveDialog.State;

            // Get reservation property.
            var newReservation = await _reservationsAccessor.GetAsync(turnContext, () => new ReservationProperty());

            // Get on turn property. This has any entities that mainDispatcher,
            // or Bot might have captured in its LUIS model.
            var onTurnProperties = await _onTurnAccessor.GetAsync(turnContext, () => new OnTurnProperty("None", new List<EntityProperty>()));

            // If on turn property has entities..
            ReservationResult updateResult = null;
            if (onTurnProperties.Entities.Count > 0)
            {
                // ...update reservation property with on turn property results.
                updateResult = newReservation.UpdateProperties(onTurnProperties);
            }

            // See if updates to reservation resulted in errors, if so, report them to user.
            if (updateResult != null &&
                updateResult.Status == ReservationStatus.Incomplete &&
                updateResult.Outcome != null &&
                updateResult.Outcome.Count > 0)
            {
                await _reservationsAccessor.SetAsync(turnContext, updateResult.NewReservation);

                // Return and do not continue if there is an error.
                await turnContext.SendActivityAsync(updateResult.Outcome[0].Message);
                return await base.ContinueDialogAsync(dc);
            }

            // Call LUIS and get results.
            var luisResults = await _botServices.LuisServices[LuisConfiguration].RecognizeAsync(turnContext, cancellationToken);
            var topLuisIntent = luisResults.GetTopScoringIntent();
            var topIntent = topLuisIntent.intent;

            // If we don't have an intent match from LUIS, go with the intent available via
            // the on turn property (parent's LUIS model).
            if (luisResults.Intents.Count <= 0)
            {
                // Go with intent in onTurnProperty.
                topIntent = string.IsNullOrWhiteSpace(onTurnProperties.Intent) ? "None" : onTurnProperties.Intent;
            }

            // Update object with LUIS result.
            updateResult = newReservation.UpdateProperties(OnTurnProperty.FromLuisResults(luisResults));

            // See if update reservation resulted in errors, if so, report them to user.
            if (updateResult != null &&
                updateResult.Status == ReservationStatus.Incomplete &&
                updateResult.Outcome != null &&
                updateResult.Outcome.Count > 0)
            {
                // Set reservation property.
                await _reservationsAccessor.SetAsync(turnContext, updateResult.NewReservation);

                // Return and do not continue if there is an error.
                await turnContext.SendActivityAsync(updateResult.Outcome[0].Message);
                return await base.ContinueDialogAsync(dc);
            }

            // Did user ask for help or said cancel or continuing the conversation?
            switch (topIntent)
            {
                case ContinuePromptIntent:
                    // User does not want to make any change.
                    updateResult.NewReservation.NeedsChange = false;
                    break;
                case NoChangeIntent:
                    // User does not want to make any change.
                    updateResult.NewReservation.NeedsChange = false;
                    break;
                case HelpIntent:
                    // Come back with contextual help.
                    var helpReadOut = updateResult.NewReservation.HelpReadOut();
                    await turnContext.SendActivityAsync(helpReadOut);
                    break;
                case CancelIntent:
                    // Start confirmation prompt.
                    var opts = new PromptOptions
                    {
                        Prompt = new Activity
                        {
                            Type = ActivityTypes.Message,
                            Text = "Are you sure you want to cancel?",
                        },
                    };

                    return await dc.PromptAsync(ConfirmCancelPrompt, opts);
                case InterruptionsIntent:
                default:
                    // If we picked up new entity values, do not treat this as an interruption.
                    if (onTurnProperties.Entities.Count != 0 || luisResults.Entities.Count > 1)
                    {
                        break;
                    }

                    // Handle interruption.
                    var onTurnProperty = await _onTurnAccessor.GetAsync(dc.Context);
                    return await dc.BeginDialogAsync(InterruptionDispatcher, onTurnProperty);
            }

            // Set reservation property based on OnTurn properties.
            await _reservationsAccessor.SetAsync(turnContext, updateResult.NewReservation);
            return await base.ContinueDialogAsync(dc);
        }

        public async override Task<DialogTurnResult> ResumeDialogAsync(DialogContext dc, DialogReason reason, object result, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (result is bool && (bool)result)
            {
                // User said yes to cancel prompt.
                await dc.Context.SendActivityAsync("Sure. I've canceled that!");
                return await dc.CancelAllDialogsAsync();
            }
            else
            {
                // User said no to cancel.
                return await base.ResumeDialogAsync(dc, reason, result, cancellationToken);
            }
        }
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace Microsoft.BotBuilderSamples
{
    public class GetLocationDateTimePartySizePrompt : TextPrompt
    {

        private const string CONTINUE_PROMPT_INTENT = "GetLocationDateTimePartySize";
        private const string HELP_INTENT = "Help";
        private const string CANCEL_INTENT = "Cancel";
        private const string INTERRUPTIONS_INTENT = "Interruptions";
        private const string NOCHANGE_INTENT = "noChange";
        private const string INTERRUPTION_DISPATCHER = "interruptionDispatcherDialog";
        private const string CONFIRM_CANCEL_PROMPT = "confirmCancelPrompt";

        // LUIS service type entry for turn.n book table LUIS model in the .bot file.
        private readonly string LUIS_CONFIGURATION = "cafeBotBookTableTurnNModel";

        private readonly BotServices _botServices;
        private readonly IStatePropertyAccessor<UserProfile> _userProfileAccessor;
        private readonly IStatePropertyAccessor<OnTurnProperty> _onTurnAccessor;
        private readonly IStatePropertyAccessor<ReservationProperty> _reservationsAccessor;

        /**
         * Constructor.
         * @param {String} dialog id
         * @param {BotConfiguration} .bot file configuration
         * @param {StateAccessor} accessor for the reservation property
         * @param {StateAccessor} accessor for on turn property
         * @param {StateAccessor} accessor for user profile property
         */
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

        /// <summary>
        /// Override continueDialog
        /// - The override enables:
        ///    * Interruption to be kicked off from right within this dialog.
        ///    * Ability to leverage a dedicated LUIS model to provide flexible entity filling,
        ///     * corrections and contextual help.
        /// </summary>
        /// <param name="innerDc"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async override Task<DialogTurnResult> ContinueDialogAsync(DialogContext dc, CancellationToken cancellationToken = default(CancellationToken))
        {
            var turnContext = dc.Context;
            var step = dc.ActiveDialog.State;

            // get reservation property
            var newReservation = await _reservationsAccessor.GetAsync(turnContext, () => new ReservationProperty());

            // Get on turn property. This has any entities that mainDispatcher,
            //  or Bot might have captured in its LUIS model
            var onTurnProperties = await _onTurnAccessor.GetAsync(turnContext, () => null);

            // if on turn property has entities
            ReservationResult updateResult = null;
            if (onTurnProperties != null && onTurnProperties.Entities != null && onTurnProperties.Entities.Count > 0)
            {
                // update reservation property with on turn property results
                updateResult = newReservation.UpdateProperties(onTurnProperties);
            }

            // see if updates to reservation resulted in errors, if so, report them to user.
            if (updateResult != null &&
                updateResult.Status == ReservationStatus.Incomplete &&
                updateResult.Outcome != null &&
                updateResult.Outcome.Count > 0)
            {
                // set reservation property
                await _reservationsAccessor.SetAsync(turnContext, updateResult.NewReservation);

                // return and do not continue if there is an error
                await turnContext.SendActivityAsync(updateResult.Outcome[0].Message);
                return await ContinueDialogAsync(dc);
            }

            // call LUIS and get results
            var LUISResults = await _botServices.LuisServices[LUIS_CONFIGURATION].RecognizeAsync(turnContext, cancellationToken);
            var topLuisIntent = LUISResults.GetTopScoringIntent();
            var topIntent = topLuisIntent.intent;

            // If we dont have an intent match from LUIS, go with the intent available via
            // the on turn property (parent's LUIS model)
            if (LUISResults.Intents.Count <= 0)
            {
                // go with intent in onTurnProperty
                topIntent = string.IsNullOrWhiteSpace(onTurnProperties.Intent) ? "None" : onTurnProperties.Intent;
            }

            // update object with LUIS result
            updateResult = newReservation.UpdateProperties(OnTurnProperty.FromLuisResults(LUISResults));

            // see if update reservation resulted in errors, if so, report them to user.
            if (updateResult != null &&
                updateResult.Status == ReservationStatus.Incomplete &&
                updateResult.Outcome != null &&
                updateResult.Outcome.Count > 0)
            {
                // set reservation property
                await _reservationsAccessor.SetAsync(turnContext, updateResult.NewReservation);

                // return and do not continue if there is an error.
                await turnContext.SendActivityAsync(updateResult.Outcome[0].Message);
                return await ContinueDialogAsync(dc);
            }

            // Did user ask for help or said cancel or continuing the conversation?
            switch (topIntent)
            {
                case CONTINUE_PROMPT_INTENT:
                    // user does not want to make any change.
                    updateResult.NewReservation.NeedsChange = false;
                    break;
                case NOCHANGE_INTENT:
                    // user does not want to make any change.
                    updateResult.NewReservation.NeedsChange = false;
                    break;
                case HELP_INTENT:
                    // come back with contextual help
                    var helpReadOut = updateResult.NewReservation.HelpReadOut();
                    await turnContext.SendActivityAsync(helpReadOut);
                    break;
                case CANCEL_INTENT:
                    // start confirmation prompt
                    var opts = new PromptOptions
                    {
                        Prompt = new Activity
                        {
                            Type = ActivityTypes.Message,
                            Text = "Are you sure you want to cancel ?",
                        },
                    };

                    return await dc.PromptAsync(CONFIRM_CANCEL_PROMPT, opts);
                case INTERRUPTIONS_INTENT:
                default:
                    // if we picked up new entity values, do not treat this as an interruption
                    if (onTurnProperties.Entities.Count != 0 || LUISResults.Entities.Count > 1)
                    {
                        break;
                    }

                    // Handle interruption.
                    var onTurnProperty = await _onTurnAccessor.GetAsync(dc.Context);
                    return await dc.BeginDialogAsync(INTERRUPTION_DISPATCHER, onTurnProperty);
            }

            // set reservation property based on OnTurn properties
            await _reservationsAccessor.SetAsync(turnContext, updateResult.NewReservation);
            return await ContinueDialogAsync(dc);
        }

        /// <summary>
        /// Override resumeDialog. This is used to handle user's response to confirm cancel prompt.
        /// </summary>
        /// <param name="dc">The dialog context.</param>
        /// <param name="reason">The <see cref="DialogReason"/>.</param>
        /// <param name="result">True </param>
        /// <returns></returns>
        public async override Task<DialogTurnResult> ResumeDialogAsync(DialogContext dc, DialogReason reason, object result, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (result != null)
            {
                // User said yes to cancel prompt.
                await dc.Context.SendActivityAsync("Sure. I've cancelled that!");
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

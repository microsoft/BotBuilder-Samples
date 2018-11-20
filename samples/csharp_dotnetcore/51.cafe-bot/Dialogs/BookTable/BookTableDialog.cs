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
    public class BookTableDialog : ComponentDialog
    {
        // This dialog's name. Also matches the name of the intent from ../Dispatcher/Resources/cafeDispatchModel.lu
        // LUIS recognizer replaces spaces ' ' with '_'. So intent name 'Who are you' is recognized as 'Who_are_you'.
        public const string Name = "Book_Table";

        public const string BookTableWaterfall = "bookTableWaterfall";
        public const string GetLocationDialogState = "getLocDialogState";
        public const string ConfirmDialogState = "confirmDialogState";

        public const string ConfirmCancelPrompt = "confirmCancelPrompt";

        // Turn.N here refers to all back and forth conversations beyond the initial trigger until the book table dialog is completed or canceled.
        public const string GetLocationDateTimePartySizePrompt = "getLocationDateTimePartySize";

        private readonly BotServices _services;

        public BookTableDialog(BotServices services, IStatePropertyAccessor<ReservationProperty> reservationsAccessor, IStatePropertyAccessor<OnTurnProperty> onTurnAccessor, IStatePropertyAccessor<UserProfile> userProfileAccessor, ConversationState conversationState)
            : base(Name)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
            ReservationsAccessor = reservationsAccessor ?? throw new ArgumentNullException(nameof(reservationsAccessor));
            OnTurnAccessor = onTurnAccessor ?? throw new ArgumentNullException(nameof(onTurnAccessor));
            UserProfileAccessor = userProfileAccessor ?? throw new ArgumentNullException(nameof(userProfileAccessor));

            // Create accessors for child dialogs
            GetLocDialogAccessor = conversationState.CreateProperty<DialogState>(GetLocationDialogState);
            ConfirmDialogAccessor = conversationState.CreateProperty<DialogState>(ConfirmDialogState);

            // Add dialogs
            // Water fall book table dialog
            var waterfallSteps = new WaterfallStep[]
            {
                GetAllRequiredPropertiesAsync,
                BookTableAsync,
            };
            AddDialog(new WaterfallDialog(BookTableWaterfall, waterfallSteps));

            // Get location, date, time & party size prompt.
            AddDialog(new GetLocationDateTimePartySizePrompt(
                                GetLocationDateTimePartySizePrompt,
                                services,
                                reservationsAccessor,
                                onTurnAccessor,
                                userProfileAccessor,
                                async (promptValidatorContext, cancellationToken) =>
                                {
                                    // Validation and prompting logic.
                                    // Get reservation property.
                                    var newReservation = await reservationsAccessor.GetAsync(promptValidatorContext.Context, () => new ReservationProperty());

                                    // If we have a valid reservation, end this prompt.
                                    // Otherwise, get LG based on what's available in reservation property
                                    if (newReservation.HaveCompleteReservation())
                                    {
                                        if (!newReservation.ReservationConfirmed)
                                        {
                                            if (newReservation.NeedsChange == true)
                                            {
                                                await promptValidatorContext.Context.SendActivityAsync("What would you like to change ?");
                                            }
                                            else
                                            {
                                                // Greet user with name if we have the user profile set.
                                                var userProfile = await userProfileAccessor.GetAsync(promptValidatorContext.Context, () => null);

                                                if (userProfile != null && !string.IsNullOrWhiteSpace(userProfile.UserName))
                                                {
                                                    await promptValidatorContext.Context.SendActivityAsync($"Alright {userProfile.UserName} I have a table for {newReservation.ConfirmationReadOut()}");
                                                }
                                                else
                                                {
                                                    await promptValidatorContext.Context.SendActivityAsync($"Ok. I have a table for {newReservation.ConfirmationReadOut()}");
                                                }

                                                await promptValidatorContext.Context.SendActivityAsync(MessageFactory.SuggestedActions(new List<string> { "Yes", "No" }, "Should I go ahead and book the table?"));
                                            }
                                        }
                                        else
                                        {
                                            // Have complete reservation.
                                            return true;
                                        }
                                    }
                                    else
                                    {
                                        // Readout what has been understood already.
                                        var groundedPropertiesReadout = newReservation.GetGroundedPropertiesReadOut();
                                        if (!string.IsNullOrWhiteSpace(groundedPropertiesReadout))
                                        {
                                            await promptValidatorContext.Context.SendActivityAsync(groundedPropertiesReadout);
                                        }
                                    }

                                    // Ask user for missing information
                                    var prompt = newReservation.GetMissingPropertyReadOut();
                                    if (!string.IsNullOrWhiteSpace(prompt))
                                    {
                                        await promptValidatorContext.Context.SendActivityAsync(prompt);
                                    }

                                    return false;
                                }));

            // This dialog is interruptable. So add interruptionDispatcherDialog.
            AddDialog(new InterruptionDispatcher(onTurnAccessor, conversationState, userProfileAccessor, services));

            // When user decides to abandon this dialog, we need to confirm user action. Add confirmation prompt.
            AddDialog(new ConfirmPrompt(ConfirmCancelPrompt));
        }

        public IStatePropertyAccessor<ReservationProperty> ReservationsAccessor { get; }

        public IStatePropertyAccessor<OnTurnProperty> OnTurnAccessor { get; }

        public object UserProfileAccessor { get; }

        public IStatePropertyAccessor<DialogState> GetLocDialogAccessor { get; }

        public IStatePropertyAccessor<DialogState> ConfirmDialogAccessor { get; }

        private async Task<DialogTurnResult> GetAllRequiredPropertiesAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var context = stepContext.Context;

            // Get current reservation from accessor.
            var newReservation = await ReservationsAccessor.GetAsync(stepContext.Context, () => new ReservationProperty());

            // Get on turn (includes LUIS entities captured by parent).
            var onTurnProperty = await OnTurnAccessor.GetAsync(context);
            ReservationResult reservationResult = null;
            if (onTurnProperty != null)
            {
                if (newReservation != null)
                {
                    // Update reservation object and gather results.
                    reservationResult = newReservation.UpdateProperties(onTurnProperty);
                }
                else
                {
                    reservationResult = ReservationProperty.FromOnTurnProperty(onTurnProperty);
                }
            }

            // Set the reservation.
            await ReservationsAccessor.SetAsync(context, reservationResult.NewReservation);

            // see if update reservation resulted in errors, if so, report them to user.
            if (reservationResult != null &&
                reservationResult.Status == ReservationStatus.Incomplete &&
                reservationResult.Outcome != null &&
                reservationResult.Outcome.Count > 0)
            {
                // Start the prompt with the initial feedback based on update results.
                var options = new PromptOptions()
                {
                    Prompt = MessageFactory.Text(reservationResult.Outcome[0].Message),
                };
                return await stepContext.PromptAsync(GetLocationDateTimePartySizePrompt, options);
            }
            else
            {
                if (reservationResult.NewReservation.HaveCompleteReservation())
                {
                    await context.SendActivityAsync($"Ok. I have a table for {reservationResult.NewReservation.ConfirmationReadOut()}");
                    await context.SendActivityAsync(MessageFactory.SuggestedActions(new List<string> { "Yes", "No" }, "Should I go ahead and book the table ?"));
                }

                var options = new PromptOptions()
                {
                    Prompt = MessageFactory.Text(reservationResult.NewReservation.GetMissingPropertyReadOut()),
                };

                // Start the prompt with the first missing piece of information.
                return await stepContext.PromptAsync(GetLocationDateTimePartySizePrompt, options);
            }
        }

        private async Task<DialogTurnResult> BookTableAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var context = stepContext.Context;

            // Report table booking based on confirmation outcome.
            if (stepContext.Result != null)
            {
                // User confirmed.
                // Get current reservation from accessor.
                var reservationFromState = await ReservationsAccessor.GetAsync(context);
                await context.SendActivityAsync($"Sure. I've booked the table for {reservationFromState.ConfirmationReadOut()}");

                // Clear out the reservation property since this is a successful reservation completion.
                reservationFromState.Date = null;
                reservationFromState.Location = null;
                reservationFromState.PartySize = 0;
                await ReservationsAccessor.SetAsync(context, reservationFromState);

                await stepContext.CancelAllDialogsAsync();

                return await stepContext.EndDialogAsync();
            }
            else
            {
                // User rejected cancellation.
                // Clear out state.
                var reservationFromState = await ReservationsAccessor.GetAsync(context);
                reservationFromState.Date = null;
                reservationFromState.Location = null;
                reservationFromState.PartySize = 0;
                await ReservationsAccessor.SetAsync(context, reservationFromState);
                await context.SendActivityAsync("Ok... I've canceled the reservation.");
                return await stepContext.EndDialogAsync();
            }
        }
    }
}

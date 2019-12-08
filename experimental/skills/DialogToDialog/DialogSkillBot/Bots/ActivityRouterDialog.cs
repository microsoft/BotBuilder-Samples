// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.BotBuilderSamples.DialogSkillBot.Dialogs;
using Microsoft.BotBuilderSamples.DialogSkillBot.Extensions;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Microsoft.BotBuilderSamples.DialogSkillBot.Bots
{
    /// <summary>
    /// A root dialog that can route activities sent to the skill to different dialogs.
    /// </summary>
    public class ActivityRouterDialog : ComponentDialog
    {
        private readonly DialogSkillBotRecognizer _luisRecognizer;

        public ActivityRouterDialog(DialogSkillBotRecognizer luisRecognizer, IConfiguration configuration)
            : base(nameof(ActivityRouterDialog))
        {
            _luisRecognizer = luisRecognizer;

            AddDialog(new BookingDialog());
            AddDialog(new OAuthTestDialog(configuration));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[] { ProcessActivityAsync }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> ProcessActivityAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // A skill can send trace activities if needed :)
            await stepContext.Context.SendTraceActivityAsync($"{GetType().Name}.ProcessActivityAsync()", value: $"Got ActivityType: {stepContext.Context.Activity.Type}", cancellationToken: cancellationToken);

            switch (stepContext.Context.Activity.Type)
            {
                case ActivityTypes.Message:
                    return await OnMessageActivityAsync(stepContext, cancellationToken);

                case ActivityTypes.Invoke:
                    return await OnInvokeActivityAsync(stepContext, cancellationToken);

                case ActivityTypes.Event:
                    return await OnEventActivityAsync(stepContext, cancellationToken);

                default:
                    // We didn't get an activity type we can handle.
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Unrecognized ActivityType: \"{stepContext.Context.Activity.Type}\".", inputHint: InputHints.IgnoringInput), cancellationToken);
                    return new DialogTurnResult(DialogTurnStatus.Complete);
            }
        }

        // This method performs different tasks based on the event name.
        private async Task<DialogTurnResult> OnEventActivityAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var activity = stepContext.Context.Activity;
            await stepContext.Context.SendTraceActivityAsync($"{GetType().Name}.OnEventActivityAsync().\r\nName: {activity.Name}. Value: {GetObjectAsJsonString(activity.Value)}", cancellationToken: cancellationToken);

            // Resolve what to execute based on the event name.
            switch (activity.Name)
            {
                case "BookFlight":
                    var bookingDetails = new BookingDetails();
                    if (activity.Value != null)
                    {
                        bookingDetails = JsonConvert.DeserializeObject<BookingDetails>(JsonConvert.SerializeObject(activity.Value));
                    }

                    // Start the booking dialog
                    var bookingDialog = FindDialog(nameof(BookingDialog));
                    return await stepContext.BeginDialogAsync(bookingDialog.Id, bookingDetails, cancellationToken);

                case "OAuthTest":
                    // Start the OAuthTestDialog
                    var oAuthDialog = FindDialog(nameof(OAuthTestDialog));
                    return await stepContext.BeginDialogAsync(oAuthDialog.Id, null, cancellationToken);

                default:
                    // We didn't get an event name we can handle.
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Unrecognized EventName: \"{activity.Name}\".", inputHint: InputHints.IgnoringInput), cancellationToken);
                    return new DialogTurnResult(DialogTurnStatus.Complete);
            }
        }

        // This method responds right away using an invokeResponse based on the activity name property.
        private async Task<DialogTurnResult> OnInvokeActivityAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var activity = stepContext.Context.Activity;
            await stepContext.Context.SendTraceActivityAsync($"{GetType().Name}.OnInvokeActivityAsync().\r\nName: {activity.Name}. Value: {GetObjectAsJsonString(activity.Value)}", cancellationToken: cancellationToken);

            // Resolve what to execute based on the invoke name.
            switch (activity.Name)
            {
                case "GetWeather":
                    var location = new Location();
                    if (activity.Value != null)
                    {
                        location = JsonConvert.DeserializeObject<Location>(JsonConvert.SerializeObject(activity.Value));
                    }

                    var lookingIntoItMessage = "Getting your weather forecast...";
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text( $"{lookingIntoItMessage} \n\nValue parameters: {JsonConvert.SerializeObject(location)}", lookingIntoItMessage, inputHint: InputHints.IgnoringInput), cancellationToken);

                    // Create and return an invoke activity with the weather results.
                    var invokeResponseActivity = new Activity(type: "invokeResponse")
                    {
                        Value = new InvokeResponse
                        {
                            Body = new[]
                            {
                                "New York, NY, Clear, 56 F",
                                "Bellevue, WA, Mostly Cloudy, 48 F"
                            },
                            Status = (int)HttpStatusCode.OK
                        }
                    };
                    await stepContext.Context.SendActivityAsync(invokeResponseActivity, cancellationToken);
                    break;

                default:
                    // We didn't get an invoke name we can handle.
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Unrecognized InvokeName: \"{activity.Name}\".", inputHint: InputHints.IgnoringInput), cancellationToken);
                    break;
            }

            return new DialogTurnResult(DialogTurnStatus.Complete);
        }

        // This method just gets a message activity and runs it through LUIS. 
        // A developer can chose to start a dialog based on the LUIS results (not implemented here).
        private async Task<DialogTurnResult> OnMessageActivityAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var activity = stepContext.Context.Activity;
            await stepContext.Context.SendTraceActivityAsync($"{GetType().Name}.OnMessageActivityAsync().\r\nText: \"{activity.Text}\". Value: {GetObjectAsJsonString(activity.Value)}", cancellationToken: cancellationToken);

            if (!_luisRecognizer.IsConfigured)
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("NOTE: LUIS is not configured. To enable all capabilities, add 'LuisAppId', 'LuisAPIKey' and 'LuisAPIHostName' to the appsettings.json file.", inputHint: InputHints.IgnoringInput), cancellationToken);
            }
            else
            {
                // Call LUIS with the utterance.
                var luisResult = await _luisRecognizer.RecognizeAsync(stepContext.Context, cancellationToken);

                // Create a message showing the LUIS results.
                var sb = new StringBuilder();
                sb.AppendLine($"LUIS results for \"{activity.Text}\":");
                var (intent, intentScore) = luisResult.Intents.FirstOrDefault(x => x.Value.Equals(luisResult.Intents.Values.Max()));
                sb.AppendLine($"Intent: \"{intent}\" Score: {intentScore.Score}");
                sb.AppendLine($"Entities found: {luisResult.Entities.Count - 1}");
                foreach (var luisResultEntity in luisResult.Entities)
                {
                    if (!luisResultEntity.Key.Equals("$instance"))
                    {
                        sb.AppendLine($"* {luisResultEntity.Key}");
                    }
                }

                await stepContext.Context.SendActivityAsync(MessageFactory.Text(sb.ToString(), inputHint: InputHints.IgnoringInput), cancellationToken);
            }

            return new DialogTurnResult(DialogTurnStatus.Complete);
        }

        private string GetObjectAsJsonString(object value)
        {
            return value == null ? string.Empty : JsonConvert.SerializeObject(value);
        }
    }
}

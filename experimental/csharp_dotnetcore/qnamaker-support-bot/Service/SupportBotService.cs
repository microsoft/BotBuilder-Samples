// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace SupportBot.Service
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.AI.QnA;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Dialogs.Choices;
    using Microsoft.Bot.Schema;
    using Microsoft.Recognizers.Text;
    using SupportBot.Dialogs.ShowQnAResult;
    using SupportBot.Providers.PersonalityChat;
    using SupportBot.Providers.QnAMaker;
    using Constants = SupportBot.Models.Constants;

    /// <summary>
    /// For each interaction from the user, an instance of this class is created and
    /// the OnTurnAsync method is called.
    /// This is a Transient lifetime service. Transient lifetime services are created
    /// each time they're requested. For each Activity received, a new instance of this
    /// class is created. Objects that are expensive to construct, or have a lifetime
    /// beyond the single Turn, should be carefully managed.
    /// </summary>
    public class SupportBotService : IBot
    {
        /// <summary>
        /// Key in the bot config (.bot file) for the QnA Maker instance.
        /// In the ".bot" file, multiple instances of QnA Maker can be configured.
        /// </summary>
        public static readonly string QnAMakerKey = "QnABot";
        public static readonly string LuisKey = "LuisBot";

        /// <summary>
        /// Services configured from the ".bot" file.
        /// </summary>
        private readonly BotServices _services;
        private readonly ShowQnAResultAccessor _accessors;
        private DialogSet dialogs;

        /// <summary>
        /// Initializes a new instance of the <see cref="SupportBotService"/> class.
        /// </summary>
        /// <param name="services">A <see cref="BotServices"/> configured from the ".bot" file.</param>
        /// <param name="accessor">Show QnA Result Accessor.</param>
        public SupportBotService(BotServices services, ShowQnAResultAccessor accessor)
        {
            _services = services ?? throw new System.ArgumentNullException(nameof(services));
            if (!_services.QnAServices.ContainsKey(QnAMakerKey))
            {
                throw new System.ArgumentException($"Invalid configuration. Please check your '.bot' file for a QnA service named '{QnAMakerKey}'.");
            }

            if (Constants.EnableLuis && !_services.LuisServices.ContainsKey(LuisKey))
            {
                throw new System.ArgumentException($"Invalid configuration. Please check your '.bot' file for a Luis service named '{LuisKey}'.");
            }

            _accessors = accessor ?? throw new System.ArgumentNullException(nameof(accessor));
            dialogs = new DialogSet(_accessors.ConversationDialogState);
            var choicePrompt = new ChoicePrompt("choicePrompt");
            choicePrompt.Style = ListStyle.SuggestedAction;
            choicePrompt.DefaultLocale = Culture.English;
            dialogs.Add(choicePrompt);
            dialogs.Add(new ChoicePrompt("cardPrompt"));
        }

        /// <summary>
        /// Every conversation turn for our QnA bot will call this method.
        /// There are no dialogs used, the sample only uses "single turn" processing,
        /// meaning a single request and response, with no stateful conversation.
        /// </summary>
        /// <param name="turnContext">A <see cref="ITurnContext"/> containing all the data needed
        /// for processing this conversation turn. </param>
        /// <param name="cancellationToken">(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> that represents the work queued to execute.</returns>
        /// <seealso cref="BotStateSet"/>
        /// <seealso cref="ConversationState"/>
        /// <seealso cref="IMiddleware"/>
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Handle Message activity type, which is the main activity type for shown within a conversational interface
            // Message activities may contain text, speech, interactive cards, and binary or unknown attachments.
            // see https://aka.ms/about-bot-activity-message to learn more about the message and other activity types
            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                var dialogContext = await dialogs.CreateContextAsync(turnContext, cancellationToken);
                var qnastatus = await _accessors.QnAResultState.GetAsync(turnContext, () => new ShowQnAResultState());
                if (qnastatus.IsFeedback)
                {
                    await HandleFeedbackFlow(qnastatus, turnContext, cancellationToken);
                }

                string requery = null;
                if (qnastatus.ConsiderState)
                {
                    // Call Active Learning Train API
                    if (qnastatus.ActiveLearningAnswer)
                    {
                        _services.QnAServices.TryGetValue(QnAMakerKey, out var qnaservice);
                        var activeLearningData = new ActiveLearningDTO()
                        {
                            hostName = qnaservice.Endpoint.Host,
                            endpointKey = qnaservice.Endpoint.EndpointKey,
                            kbid = qnaservice.Endpoint.KnowledgeBaseId,
                            userId = turnContext.Activity.From.Id,
                            userQuestion = qnastatus.ActiveLearningUserQuestion,
                        };

                        requery = Utils.GetRequery(qnastatus, turnContext.Activity.Text, activeLearningData);
                        if (requery != null)
                        {
                            TelemetryUtils.LogTrainApiResponse(this._services.TelemetryClient, TelemetryConstants.TrainApiEvent, turnContext.Activity, activeLearningData, requery);
                        }
                    }
                    else
                    {
                        requery = Utils.GetRequery(qnastatus, turnContext.Activity.Text);
                    }

                    if (requery != null)
                    {
                        turnContext.Activity.Text = requery;
                    }
                }

                QueryResult responseGeneral = null, responseLuis = null;

                // Get QnA Answer for multiturn
                var responseMultiturn = await GetMultiturnResponseFromKB(turnContext, qnastatus.QnaAnswer);
                if (responseMultiturn == null)
                {
                    // Get general response from KB
                    responseGeneral = await GetGeneralResponseFromKB(turnContext, qnastatus, true);
                    if (responseGeneral == null)
                    {
                        // Get LUIS intent
                        var luisIntent = await GetLuisIntent(turnContext, Constants.EnableLuis);
                        if (!luisIntent.Equals("None"))
                        {
                            // Get Luis response
                            responseLuis = await GetResponseWithLuisIntent(turnContext, luisIntent);
                        }
                    }
                }

                // choose response
                var qnaresponse = Utils.SelectResponse(responseMultiturn, responseGeneral, responseLuis, qnastatus.QnaAnswer);
                if (qnaresponse != null)
                {
                    if (qnaresponse.Options != null && qnaresponse.Options.Count != 0)
                    {
                        await ShowQnAResponseWithOptions(turnContext, qnaresponse, qnastatus);
                    }
                    else
                    {
                        await ShowQnAResponseWithText(turnContext, qnaresponse, qnastatus);
                    }

                    if (qnastatus.QnaAnswer.Name == Constants.MetadataValue.Redirection)
                    {
                        await ShowRedirection(Constants.RedirectionType.GuidedFlow, turnContext, cancellationToken);
                    }

                    if (qnastatus.NeuroconCount >= Constants.ConsecutiveNeuroconAnswersAllowed)
                    {
                        await ShowRedirection(Constants.RedirectionType.Neurocon, turnContext, cancellationToken);
                    }
                }
                else
                {
                    // get answer from Nurocon
                    var personalitychatanswer = await SendCognitiveServicesResponse(turnContext, cancellationToken, Constants.EnablePersonalityChat);
                    if (personalitychatanswer == string.Empty || personalitychatanswer.Equals(@"Sorry, I don't have a response for that.", StringComparison.OrdinalIgnoreCase))
                    {
                        var msg = @"I don't have an answer for that. Try typing MENU to go back to the main menu";
                        TelemetryUtils.LogNoAnswerEvent(this._services.TelemetryClient, turnContext.Activity);
                        qnastatus.NeuroconCount++;
                        await _accessors.QnAResultState.SetAsync(turnContext, qnastatus);
                        await _accessors.ConversationState.SaveChangesAsync(turnContext);
                        await turnContext.SendActivityAsync(msg, cancellationToken: cancellationToken);
                        if (qnastatus.NeuroconCount >= Constants.ConsecutiveNeuroconAnswersAllowed)
                        {
                            await ShowRedirection(Constants.RedirectionType.Neurocon, turnContext, cancellationToken);
                        }
                    }
                    else
                    {
                        personalitychatanswer = personalitychatanswer + " [&#x1f6c8;](https://labs.cognitive.microsoft.com/en-us/project-personality-chat \"Auto generated answer using Personality chat. Click to know more\")";
                        qnastatus.ConsiderState = false;
                        qnastatus.QnaAnswer = null;
                        qnastatus.NeuroconCount++;
                        TelemetryUtils.LogNeuroconResponse(this._services.TelemetryClient, TelemetryConstants.NeuroconEvent, turnContext.Activity, qnastatus, personalitychatanswer);
                        await _accessors.QnAResultState.SetAsync(turnContext, qnastatus);
                        await _accessors.ConversationState.SaveChangesAsync(turnContext);
                        await turnContext.SendActivityAsync(personalitychatanswer, cancellationToken: cancellationToken);
                        if (qnastatus.NeuroconCount >= Constants.ConsecutiveNeuroconAnswersAllowed)
                        {
                            await ShowRedirection(Constants.RedirectionType.Neurocon, turnContext, cancellationToken);
                        }
                    }
                }
            }
            else if (turnContext.Activity.Type == ActivityTypes.ConversationUpdate)
            {
                return;
            }
            else if (turnContext.Activity.Type == ActivityTypes.Event)
            {
                // Send a welcome message to the user.
                var eventActivity = turnContext.Activity.AsEventActivity();
                if (eventActivity.Name == Constants.EventWelcomeMessage)
                {
                    TelemetryUtils.LogWelcomeResponse(this._services.TelemetryClient, "Welcome Event Detected", turnContext.Activity);
                    var dialogContext = await dialogs.CreateContextAsync(turnContext, cancellationToken);
                    var qnastatus = await _accessors.QnAResultState.GetAsync(turnContext, () => new ShowQnAResultState());
                    qnastatus.ConsiderState = false;
                    qnastatus.QnaAnswer.Text = Constants.WelcomeQuestion;
                    qnastatus.QnaAnswer.Name = Constants.MetadataValue.Welcome;
                    await _accessors.QnAResultState.SetAsync(turnContext, qnastatus);
                    await _accessors.ConversationState.SaveChangesAsync(turnContext);
                    await SendWelcomeMessageAsync(turnContext, cancellationToken);
                }
            }
            else
            {
                await turnContext.SendActivityAsync($"{turnContext.Activity.Type} event detected", cancellationToken: cancellationToken);
            }
        }

        /// <summary>
        /// Greet new users as they are added to the conversation.
        /// </summary>
        /// <param name="turnContext">A <see cref="ITurnContext"/> containing all the data needed
        /// for processing this conversation turn. </param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> that represents the work queued to execute.</returns>
        private async Task SendWelcomeMessageAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var cardButtons = new List<CardAction>();
            foreach (var promptoption in Constants.WelcomeOptions)
            {
                var card = new CardAction()
                {
                    Value = promptoption,
                    Type = ActionTypes.ImBack,
                    Title = promptoption,
                };
                cardButtons.Add(card);
            }

            var heroCard = new HeroCard
            {
                Title = Constants.HeroCardTitle,
                Text = Constants.WelcomeQuestion,
                Buttons = cardButtons,
            };
            var cardActivity = Activity.CreateMessageActivity();
            cardActivity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            var cardAttachment = heroCard.ToAttachment();
            cardActivity.Attachments.Add(cardAttachment);
            await turnContext.SendActivityAsync(cardActivity, cancellationToken);
        }

        private async Task<string> SendCognitiveServicesResponse(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken), bool enableNeurocon = false)
        {
            var personalilitychatanswer = string.Empty;
            if (!enableNeurocon)
            {
                return personalilitychatanswer;
            }
            else
            {
                var personalityChatProvider = new PersonalityChatProvider();
                var personalityChatResponse = await personalityChatProvider.GetResponse(turnContext.Activity.Text);
                if (personalityChatResponse.Responses.Count > 0)
                {
                    var personaResponse = personalityChatResponse.Responses[0];
                    personalilitychatanswer = personaResponse.Text;
                }
            }

            return personalilitychatanswer;
        }

        private async Task<QueryResult> GetMultiturnResponseFromKB(ITurnContext turnContext, QnAMakerAnswer previousAnswer, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (previousAnswer == null || string.IsNullOrEmpty(previousAnswer.Name))
            {
                return null;
            }

            var qnaOptions = new QnAMakerOptions();
            var metadata = new Microsoft.Bot.Builder.AI.QnA.Metadata();

            metadata.Name = Constants.MetadataName.Parent;
            metadata.Value = previousAnswer.Name;
            qnaOptions.StrictFilters = new Microsoft.Bot.Builder.AI.QnA.Metadata[] { metadata };
            qnaOptions.Top = Constants.DefaultTop;
            qnaOptions.ScoreThreshold = Constants.MultiturnThreshold;
            var response = await _services.QnAServices[QnAMakerKey].GetAnswersAsync(turnContext, qnaOptions);
            if (response != null && response.GetLength(0) != 0 && response[0].Score >= Constants.MultiturnThreshold)
            {
                return response[0];
            }

            return null;
        }

        private async Task<QueryResult> GetGeneralResponseFromKB(ITurnContext turnContext, ShowQnAResultState qnaStatus, bool considerActiveLearning = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            var qnaOptions = new QnAMakerOptions();
            if (considerActiveLearning)
            {
                qnaOptions.Top = Constants.ActiveLearningTop;
                qnaOptions.ScoreThreshold = Constants.ActiveLearningThreshold;
            }
            else
            {
                qnaOptions.Top = Constants.DefaultTop;
                qnaOptions.ScoreThreshold = Constants.DefaultThreshold;
            }

            var response = await _services.QnAServices[QnAMakerKey].GetAnswersAsync(turnContext, qnaOptions);
            if (response != null && response.GetLength(0) != 0 && response[0].Score >= Constants.DefaultThreshold)
            {
                if (response.GetLength(0) == 1 || !considerActiveLearning || response[0].Score > 0.95F)
                {
                    return response[0];
                }

                var activeLearningResponse = HandleActiveLearning(response, qnaStatus, turnContext.Activity.Text);
                TelemetryUtils.LogActiveLearningResponse(this._services.TelemetryClient, TelemetryConstants.ActiveLearningEvent, turnContext.Activity, activeLearningResponse);
                return activeLearningResponse;
            }

            return null;
        }

        private async Task HandleFeedbackFlow(ShowQnAResultState qnastatus, ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            var isValidFeedback = await this.IsValidFeedback(turnContext);
            if (isValidFeedback)
            {
                TelemetryUtils.LogFeedbackResponse(this._services.TelemetryClient, TelemetryConstants.FeedbackEvent, turnContext.Activity, qnastatus);
                turnContext.Activity.Text = qnastatus.QnaAnswer.Requery ?? null;
            }

            qnastatus.ConsiderState = false;
            qnastatus.IsFeedback = false;
            qnastatus.NeuroconCount = 0;
        }

        private async Task<bool> IsValidFeedback(ITurnContext turnContext)
        {
            var qnaOptions = new QnAMakerOptions();
            qnaOptions.Top = Constants.DefaultTop;
            qnaOptions.ScoreThreshold = Constants.DefaultThreshold;
            var response = await _services.QnAServices[QnAMakerKey].GetAnswersAsync(turnContext, qnaOptions);

            // if there is an answer with greater threshold from KB, it is not a feedback
            if (response != null && response.GetLength(0) != 0 && response[0].Score >= 0.95F)
            {
                return false;
            }

            return true;
        }

        private QueryResult HandleActiveLearning(QueryResult[] response, ShowQnAResultState qnaStatus, string userQuestion)
        {
            var filteredResponse = response.Where(answer => answer.Score > Constants.DefaultThreshold).ToList();
            var responseCandidates = ActiveLearning.GetLowScoreVariation(filteredResponse);
            if (responseCandidates.Count > 1)
            {
                var activeLearningResponse = ActiveLearning.GenerateResponse(responseCandidates);
                qnaStatus.ActiveLearningAnswer = true;
                qnaStatus.ActiveLearningUserQuestion = userQuestion;
                return activeLearningResponse;
            }

            return responseCandidates[0];
        }

        private async Task<string> GetLuisIntent(ITurnContext turnContext, bool enableLuis, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!enableLuis)
            {
                return "None";
            }

            try
            {
                var luisResult = await _services.LuisServices[LuisKey].RecognizeAsync(turnContext, cancellationToken);
                var topIntent = luisResult?.GetTopScoringIntent();
                TelemetryUtils.LogLuisResponse(this._services.TelemetryClient, TelemetryConstants.LuisEvent, turnContext.Activity, topIntent.Value.intent + " score : " + topIntent.Value.score.ToString());
                return topIntent.Value.intent;
            }
            catch (Exception e)
            {
                TelemetryUtils.LogException(this._services.TelemetryClient, turnContext.Activity, e, "luis", "expection while getting luis intent");
                return "None";
            }
        }

        private async Task<QueryResult> GetResponseWithLuisIntent(ITurnContext turnContext, string topIntent, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (topIntent != null && topIntent != "None")
            {
                var qnaOptions = new QnAMakerOptions();
                var metadata = new Microsoft.Bot.Builder.AI.QnA.Metadata();

                metadata.Name = Constants.MetadataName.Intent;
                metadata.Value = topIntent;
                qnaOptions.StrictFilters = new Microsoft.Bot.Builder.AI.QnA.Metadata[] { metadata };
                qnaOptions.Top = Constants.DefaultTop;
                qnaOptions.ScoreThreshold = 0.3F;
                var activityText = turnContext.Activity.Text;
                turnContext.Activity.Text = Constants.MetadataName.Intent + "=" + topIntent;
                var response = await _services.QnAServices[QnAMakerKey].GetAnswersAsync(turnContext, qnaOptions);
                turnContext.Activity.Text = activityText;
                if (response != null && response.GetLength(0) != 0 && response[0].Score >= Constants.DefaultThreshold)
                {
                    return response[0];
                }
            }

            return null;
        }

        private async Task ShowQnAResponseWithOptions(ITurnContext turnContext, QnAMakerAnswer qnaresponse, ShowQnAResultState qnastatus, CancellationToken cancellationToken = default(CancellationToken))
        {
            var cardActivity = Utils.GenerateResponseOptions(qnaresponse);
            qnastatus.ConsiderState = true;
            qnastatus.QnaAnswer = qnaresponse;
            qnastatus.NeuroconCount = 0;
            if (qnaresponse.Flowtype == Constants.MetadataValue.Feedback)
            {
                qnastatus.IsFeedback = true;
            }
            else
            {
                qnastatus.IsFeedback = false;
            }

            await _accessors.QnAResultState.SetAsync(turnContext, qnastatus);
            await _accessors.ConversationState.SaveChangesAsync(turnContext);
            await turnContext.SendActivityAsync(cardActivity, cancellationToken);
        }

        private async Task ShowQnAResponseWithText(ITurnContext turnContext, QnAMakerAnswer qnaresponse, ShowQnAResultState qnastatus, CancellationToken cancellationToken = default(CancellationToken))
        {
            qnastatus.ConsiderState = true;
            if (!qnaresponse.IsChitChat)
            {
                qnastatus.NeuroconCount = 0;
            }
            else
            {
                qnastatus.NeuroconCount++;
            }

            qnastatus.QnaAnswer = qnaresponse;
            if (qnaresponse.Flowtype == Constants.MetadataValue.Feedback)
            {
                qnastatus.IsFeedback = true;
            }
            else
            {
                qnastatus.IsFeedback = false;
            }

            await _accessors.QnAResultState.SetAsync(turnContext, qnastatus);
            await _accessors.ConversationState.SaveChangesAsync(turnContext);
            await turnContext.SendActivityAsync(qnaresponse.Text);
            if (!string.IsNullOrEmpty(qnaresponse.Requery) && !qnastatus.IsFeedback)
            {
                var showNextResponseText = true;
                var counter = 0;
                while (showNextResponseText)
                {
                    turnContext.Activity.Text = qnaresponse.Requery;
                    qnastatus = await _accessors.QnAResultState.GetAsync(turnContext, () => new ShowQnAResultState());
                    var response = await GetMultiturnResponseFromKB(turnContext, qnastatus.QnaAnswer);
                    if (response == null)
                    {
                        showNextResponseText = false;
                    }
                    else
                    {
                        qnaresponse = Utils.GetQnAAnswerFromResponse(response);
                        if (qnaresponse == qnastatus.QnaAnswer)
                        {
                            break;
                        }

                        if (qnaresponse.Options != null && qnaresponse.Options.Count != 0)
                        {
                            qnastatus.ConsiderState = true;
                            qnastatus.QnaAnswer = qnaresponse;
                            qnastatus.NeuroconCount = 0;
                            if (qnaresponse.Flowtype == Constants.MetadataValue.Feedback)
                            {
                                qnastatus.IsFeedback = true;
                            }
                            else
                            {
                                qnastatus.IsFeedback = false;
                            }
                            await _accessors.QnAResultState.SetAsync(turnContext, qnastatus);
                            await _accessors.ConversationState.SaveChangesAsync(turnContext);
                            var cardActivity = Utils.GenerateResponseOptions(qnaresponse);
                            await turnContext.SendActivityAsync(cardActivity, cancellationToken);
                            showNextResponseText = false;
                        }
                        else
                        {
                            qnastatus.ConsiderState = true;
                            qnastatus.QnaAnswer = qnaresponse;
                            if (!qnaresponse.IsChitChat)
                            {
                                qnastatus.NeuroconCount = 0;
                            }
                            else
                            {
                                qnastatus.NeuroconCount++;
                            }

                            if (qnaresponse.Flowtype == Constants.MetadataValue.Feedback)
                            {
                                qnastatus.IsFeedback = true;
                            }
                            else
                            {
                                qnastatus.IsFeedback = false;
                            }

                            await _accessors.QnAResultState.SetAsync(turnContext, qnastatus);
                            await _accessors.ConversationState.SaveChangesAsync(turnContext);
                            await turnContext.SendActivityAsync(qnaresponse.Text);
                        }

                        counter++;
                        if (counter > Constants.MaxContinuousTextDialogs || qnastatus.IsFeedback || qnastatus.QnaAnswer.Requery == null || qnastatus.NeuroconCount > Constants.ConsecutiveNeuroconAnswersAllowed)
                        {
                            showNextResponseText = false;
                        }
                    }
                }
            }
        }

        private async Task ShowRedirection(Constants.RedirectionType redirectionType, ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            var dialogContext = await dialogs.CreateContextAsync(turnContext, cancellationToken);
            var qnastatus = await _accessors.QnAResultState.GetAsync(turnContext, () => new ShowQnAResultState());
            if (redirectionType == Constants.RedirectionType.Neurocon)
            {
                turnContext.Activity.Text = Constants.NeuroconRedirectionQuestion;
            }
            else if (redirectionType == Constants.RedirectionType.GuidedFlow)
            {
                turnContext.Activity.Text = Constants.GuidedFlowRedirectionQuestion;
            }

            var responseGeneral = await GetGeneralResponseFromKB(turnContext, qnastatus);
            var qnaresponse = Utils.GetQnAAnswerFromResponse(responseGeneral);
            if (qnaresponse != null)
            {
                qnastatus.NeuroconCount = 0;
                if (qnaresponse.Options != null && qnaresponse.Options.Count != 0)
                {
                    await ShowQnAResponseWithOptions(turnContext, qnaresponse, qnastatus);
                }
                else
                {
                    await ShowQnAResponseWithText(turnContext, qnaresponse, qnastatus);
                }
            }
        }
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace SupportBot.Service
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Bot.Builder.AI.QnA;
    using Microsoft.Bot.Schema;
    using SupportBot.Dialogs.ShowQnAResult;
    using SupportBot.Models;
    using SupportBot.Providers.QnAMaker;
    using Constants = SupportBot.Models.Constants;

    /// <summary>
    /// Utility functions.
    /// </summary>
    public class Utils
    {
        /// <summary>
        /// get requery text.
        /// </summary>
        /// <param name="qnastatus">qnastatus.</param>
        /// <param name="activityText">activity text.</param>
        /// <param name="activeLearningdata">activeleraning data.</param>
        /// <returns></returns>
        public static string GetRequery(ShowQnAResultState qnastatus, string activityText, ActiveLearningDTO activeLearningdata = null)
        {
            string requery = null;
            if (qnastatus.QnaAnswer.Options != null && qnastatus.QnaAnswer.Options.Count != 0)
            {
                foreach (var promptoption in qnastatus.QnaAnswer.Options)
                {
                    if (promptoption.Option.Equals(activityText, StringComparison.CurrentCultureIgnoreCase))
                    {
                        requery = string.IsNullOrEmpty(promptoption.Requery) ? promptoption.Option : promptoption.Requery;

                        // call train API if active learning
                        if (qnastatus.ActiveLearningAnswer == true && activeLearningdata != null)
                        {
                            qnastatus.ActiveLearningAnswer = false;
                            activeLearningdata.qnaId = promptoption.QnAId;
                            ActiveLearning.CallTrainApi(activeLearningdata);
                            qnastatus.ActiveLearningUserQuestion = null;
                        }

                        break;
                    }
                }
            }
            else
            {
                requery = string.IsNullOrEmpty(qnastatus.QnaAnswer.Requery) ? null : qnastatus.QnaAnswer.Requery;
            }

            return requery;
        }

        /// <summary>
        /// Get QnAAnswer from the query result.
        /// </summary>
        /// <param name="response">query result response.</param>
        /// <returns>QnAMaker answer.</returns>
        public static QnAMakerAnswer GetQnAAnswerFromResponse(Microsoft.Bot.Builder.AI.QnA.QueryResult response)
        {
            if (response == null)
            {
                return null;
            }

            var promptOptionDictionary = new Dictionary<int, PromptOption>();
            var selectedResponse = response;
            var qnaAnswer = new QnAMakerAnswer();
            qnaAnswer.Text = selectedResponse.Answer ?? null;
            foreach (var metadata in selectedResponse.Metadata)
            {
                if (metadata.Name.Contains(Constants.MetadataName.OptionRequery))
                {
                    var index = metadata.Name.Substring(13);
                    if (int.TryParse(index, out var result))
                    {
                        if (promptOptionDictionary.ContainsKey(result))
                        {
                            promptOptionDictionary[result].Requery = metadata.Value;
                        }
                        else
                        {
                            var optionPrompt = new PromptOption();
                            optionPrompt.Requery = metadata.Value;
                            promptOptionDictionary.Add(result, optionPrompt);
                        }
                    }
                }
                else if (metadata.Name.Contains(Constants.MetadataName.Option))
                {
                    var index = metadata.Name.Substring(6);
                    if (int.TryParse(index, out var result))
                    {
                        if (promptOptionDictionary.ContainsKey(result))
                        {
                            promptOptionDictionary[result].Option = metadata.Value;
                        }
                        else
                        {
                            var optionPrompt = new PromptOption();
                            optionPrompt.Option = metadata.Value;
                            promptOptionDictionary.Add(result, optionPrompt);
                        }
                    }
                }
                else if (metadata.Name.Contains(Constants.MetadataName.QnAId))
                {
                    var index = metadata.Name.Substring(5);
                    if (int.TryParse(index, out var result))
                    {
                        int.TryParse(metadata.Value, out var id);
                        if (promptOptionDictionary.ContainsKey(result))
                        {
                            promptOptionDictionary[result].QnAId = id;
                        }
                        else
                        {
                            var optionPrompt = new PromptOption();
                            optionPrompt.QnAId = id;
                            promptOptionDictionary.Add(result, optionPrompt);
                        }
                    }
                }
                else if (metadata.Name == Constants.MetadataName.Requery)
                {
                    qnaAnswer.Requery = metadata.Value ?? null;
                }
                else if (metadata.Name.Contains(Constants.MetadataName.Name))
                {
                    qnaAnswer.Name = metadata.Value ?? null;
                }
                else if (metadata.Name.Contains(Constants.MetadataName.Parent))
                {
                    qnaAnswer.Parent = metadata.Value ?? null;
                }
                else if (metadata.Name.Contains(Constants.MetadataName.Isroot))
                {
                    qnaAnswer.IsRoot = metadata.Value ?? null;
                }
                else if (metadata.Name.Contains(Constants.MetadataName.Flowtype))
                {
                    qnaAnswer.Flowtype = metadata.Value ?? null;
                }
            }

            foreach (var promptOption in promptOptionDictionary)
            {
                qnaAnswer.Options.Add(promptOption.Value);
            }

            qnaAnswer.IsChitChat = IsResponseChitChat(response);
            return qnaAnswer;
        }

        /// <summary>
        /// Generate response with options.
        /// </summary>
        /// <param name="qnaresponse"QnAMaker answer.></param>
        /// <returns>message activity with hero card.</returns>
        public static IMessageActivity GenerateResponseOptions(QnAMakerAnswer qnaresponse)
        {
            var cardButtons = new List<CardAction>();
            foreach (var promptoption in qnaresponse.Options)
            {
                var title = FormatOptionText(promptoption.Option);
                var card = new CardAction()
                {
                    Value = title,
                    Type = ActionTypes.ImBack,
                    Title = title,
                };
                cardButtons.Add(card);
            }

            var heroCard = new HeroCard
            {
                Title = Constants.HeroCardTitle,
                Text = qnaresponse.Text,
                Buttons = cardButtons,
            };
            var cardActivity = Activity.CreateMessageActivity();
            cardActivity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            var cardAttachment = heroCard.ToAttachment();
            cardActivity.Attachments.Add(cardAttachment);
            return cardActivity;
        }

        /// <summary>
        /// Handle capitalization and format the text before displaying.
        /// </summary>
        /// <param name="inputText">text to be formated.</param>
        /// <returns>formated text.</returns>
        public static string FormatOptionText(string inputText)
        {
            if (string.IsNullOrEmpty(inputText))
            {
                return string.Empty;
            }

            foreach (var word in HandleCaptitalization.formattedWords)
            {
                if (inputText.Contains(word.Original))
                {
                    inputText = inputText.Replace(word.Original, word.ConvertTo);
                }
            }

            inputText = char.ToUpper(inputText[0]) + inputText.Substring(1);
            return inputText;
        }

        /// <summary>
        /// Select final response.
        /// </summary>
        /// <param name="responseMultiturn">multiturn response.</param>
        /// <param name="responseGeneral">general response.</param>
        /// <param name="luisresponse">luis response.</param>
        /// <param name="previousAnswer">answer shoen previously.</param>
        /// <returns>selected QnAMaker answer.</returns>

        public static QnAMakerAnswer SelectResponse(QueryResult responseMultiturn, QueryResult responseGeneral, QueryResult luisresponse, QnAMakerAnswer previousAnswer)
        {
            if (responseMultiturn != null)
            {
                var qnaresponse = Utils.GetQnAAnswerFromResponse(responseMultiturn);
                if (previousAnswer == null || !qnaresponse.IsEqual(previousAnswer))
                {
                    return qnaresponse;
                }
            }

            if (responseGeneral != null)
            {
                var qnaresponse = Utils.GetQnAAnswerFromResponse(responseGeneral);
                if (qnaresponse.IsRoot == null || !Constants.MetadataValue.ExcludeIsroot.Equals(qnaresponse.IsRoot))
                {
                    return qnaresponse;
                }
            }

            if (luisresponse != null)
            {
                var qnaresponse = Utils.GetQnAAnswerFromResponse(luisresponse);
                if (previousAnswer == null || !qnaresponse.IsEqual(previousAnswer))
                {
                    return qnaresponse;
                }
            }

            return null;
        }

        /// <summary>
        /// check if the response has metadata chitchat.
        /// </summary>
        /// <param name="response">response.</param>
        /// <returns>true if it is chitchat response else false.</returns>

        public static bool IsResponseChitChat(QueryResult response)
        {
            if (response != null)
            {
                foreach (var metadata in response.Metadata)
                {
                    if (metadata.Name == Constants.MetadataName.Editorial && metadata.Value == Constants.MetadataValue.ChitChat)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}

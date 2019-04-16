// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading.Tasks;
using Designer.Dialogs;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using QnAPrompting.Helpers;
using QnAPrompting.Models;

namespace QnAPrompting.Dialogs
{
    public class QnADialog : FunctionDialogBase
    {
        private IQnAService _qnaService;

        public QnADialog(IQnAService qnaService)
            : base(nameof(QnADialog))
        {
            _qnaService = qnaService;
        }

        protected override async Task<(object newState, IEnumerable<Activity> output, object result)> ProcessAsync(object oldState, Activity inputActivity)
        {
            Activity outputActivity = null;
            QnABotState newState = null;
            
            var query = inputActivity.Text;           
            var qnaResult = await _qnaService.QueryQnAServiceAsync(query, (QnABotState)oldState);
            var qnaAnswer = qnaResult[0].Answer;
            var prompts = qnaResult[0].Context?.Prompts;
            
            if (prompts == null || prompts.Length < 1)
            {
                outputActivity = MessageFactory.Text(qnaAnswer);
            }
            else
            {
                // Set bot state only if prompts are found in QnA result
                newState = new QnABotState
                {
                    PreviousQnaId = qnaResult[0].Id,
                    PreviousUserQuery = query
                };

                outputActivity = CardHelper.GetHeroCard(qnaAnswer, prompts);
            }

            return (newState, new Activity[] { outputActivity }, null);
        }
    }
}

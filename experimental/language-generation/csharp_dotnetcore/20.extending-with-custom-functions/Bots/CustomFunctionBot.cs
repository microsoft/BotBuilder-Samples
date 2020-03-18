// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Builder.LanguageGeneration;
using AdaptiveExpressions;

using System.IO;
using System;

namespace Microsoft.BotBuilderSamples.Bots
{
    public class CustomFunctionBot : ActivityHandler
    {
        protected Templates _lgTemplates;

        public CustomFunctionBot()
        {
            var lgFilePath = Path.Join(".", "resources", "main.lg");
            const string mySqrtFnName = "contoso.sqrt";

            ExpressionEvaluator sqrtDelegate = new ExpressionEvaluator(
                    mySqrtFnName,
                    ExpressionFunctions.Apply(
                        args =>
                        {
                            object retValue = null;
                            double dblValue;
                            if(double.TryParse(args[0], out dblValue))
                            {
                                retValue = Math.Sqrt(dblValue);
                            }
                            return retValue;
                        }, ExpressionFunctions.VerifyContainer),
                    ReturnType.Number,
                    ExpressionFunctions.ValidateUnary);

            Expression.Functions.Add(mySqrtFnName, sqrtDelegate);
            _lgTemplates = Templates.ParseFile(lgFilePath);
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var replyText = _lgTemplates.Evaluate("sqrtReadBack");
            await turnContext.SendActivityAsync(ActivityFactory.FromObject(replyText), cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var welcomeText = "Hello and welcome! I will return a square root if you typed a number. Will echo everything back.";
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
                }
            }
        }
    }
}

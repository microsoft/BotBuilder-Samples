// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.IO;
using Microsoft.Bot.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Builder.LanguageGeneration;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using ActivityBuilder = Microsoft.Bot.Builder.Dialogs.Adaptive.Generators.ActivityGenerator;
using System.Collections.Generic;

namespace Microsoft.BotBuilderSamples
{
    public class AdapterWithErrorHandler : BotFrameworkHttpAdapter
    {
        private MultiLingualTemplateEngine _lgManager;
        public AdapterWithErrorHandler(ICredentialProvider credentialProvider, ILogger<BotFrameworkHttpAdapter> logger, ConversationState conversationState = null)
            : base(credentialProvider)
        {
            Dictionary<string, List<string>> lgFilesPerLocale = new Dictionary<string, List<string>>() 
            {
                {"", new List<string>() {Path.Combine(".", "Resources", "AdapterWithErrorHandler.lg")}},
                {"fr", new List<string>() {Path.Combine(".", "Resources", "AdapterWithErrorHandler.fr-fr.lg")}}
            };
            _lgManager = new MultiLingualTemplateEngine(lgFilesPerLocale);
            OnTurnError = async (turnContext, exception) =>
            {
                // Log any leaked exception from the application.
                logger.LogError($"Exception caught : {exception.Message}");

                // Send a catch-all apology to the user.
                await turnContext.SendActivityAsync(_lgManager.GenerateActivity("SomethingWentWrong", exception, turnContext));

                if (conversationState != null)
                {
                    try
                    {
                        // Delete the conversationState for the current conversation to prevent the
                        // bot from getting stuck in a error-loop caused by being in a bad state.
                        // ConversationState should be thought of as similar to "cookie-state" in a Web pages.
                        await conversationState.DeleteAsync(turnContext);
                    }
                    catch (Exception e)
                    {
                        logger.LogError($"Exception caught on attempting to Delete ConversationState : {e.Message}");
                    }
                }
            };
        }
    }
}

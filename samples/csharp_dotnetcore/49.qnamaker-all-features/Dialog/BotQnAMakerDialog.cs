// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder.AI.QnA.Dialogs;
using Microsoft.Extensions.Configuration;

namespace Microsoft.BotBuilderSamples.Dialog
{
    /// <summary>
    /// QnAMaker action builder class.  This class exists so that the base QnAMakerDialog can be supplied with
    /// constructor arguments from configuration.
    /// </summary>
    public class BotQnAMakerDialog : QnAMakerDialog
    {
        public BotQnAMakerDialog(IConfiguration configuration)
            :base(configuration["QnAKnowledgebaseId"], configuration["QnAEndpointKey"], BotQnAMakerDialog.GetHostname(configuration["QnAEndpointHostName"]))
        {

        }

        private static string GetHostname(string hostname)
        {
            if (!hostname.StartsWith("http"))
            {
                hostname = string.Concat("https://", hostname);
            }

            if (!hostname.EndsWith("/qnamaker"))
            {
                hostname = string.Concat(hostname, "/qnamaker");
            }

            return hostname;
        }
    }
}

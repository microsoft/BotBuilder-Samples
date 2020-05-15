// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
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
            :base(configuration["QnAKnowledgebaseId"], configuration["QnAEndpointKey"], BotQnAMakerDialog.GetHostPath(configuration["QnAEndpointHostName"]))
        {

        }

        // validates and creates (if needed) a full QnA maker url.
        // if the value already contains a Scheme and Path no changes are made, otherwise the
        // Scheme is set to HTTPS and the Path to "/qnamaker"
        private static string GetHostPath(string hostname)
        {
            // we'll take any Scheme, otherwise https
            var result = hostname;
            var protoSep = hostname.IndexOf("://");
            if (protoSep == -1)
            {
                result = "https://" + hostname;
            }

            // we'll take any path, including root, othwerwise add the default qnamaker path
            var pathSep = hostname.IndexOf("/", protoSep != -1 ? protoSep + 3 : 0);
            if (pathSep == -1)
            {
                result += "/qnamaker";
            }

            return (new Uri(result)).ToString();
        }
    }
}

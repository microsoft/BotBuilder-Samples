// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace SupportBot.Models
{
    using System.Collections.Generic;
    using static SupportBot.Models.Constants;

    /// <summary>
    /// Since metadata name is lower case only, handle captilalization here.
    /// </summary>
    public static class HandleCaptitalization
    {
        public static List<FormattedWord> formattedWords = new List<FormattedWord>()
        {
            { new FormattedWord { Original = "qna maker", ConvertTo = "QnAMaker" } },
            { new FormattedWord { Original = "qnamaker", ConvertTo = "QnAMaker" } },
            { new FormattedWord {Original = "qna", ConvertTo = "QnA" } },
            { new FormattedWord {Original = "rest api", ConvertTo = "REST API" } },
            { new FormattedWord {Original = "api", ConvertTo = "API" } },
            { new FormattedWord {Original = "azure bot service", ConvertTo = "Azure Bot Service" } },
            { new FormattedWord {Original = "curl", ConvertTo = "CURL" } },
            { new FormattedWord {Original = "azure search", ConvertTo = "Azure Search" } },
            { new FormattedWord {Original = @"java/C#/python/node.js/go", ConvertTo = @"Java/C#/Python/Node.js/Go" } },
            { new FormattedWord {Original = "crud", ConvertTo = "CRUD" } },
            { new FormattedWord {Original = "cors", ConvertTo = "CORS" } },
            { new FormattedWord {Original = "luis", ConvertTo = "LUIS" } },
        };
    }
}

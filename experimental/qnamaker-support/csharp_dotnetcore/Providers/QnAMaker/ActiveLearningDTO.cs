// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace SupportBot.Providers.QnAMaker
{
    /// <summary>
    /// DTO for Active Learning.
    /// </summary>
    public class ActiveLearningDTO
    {
        public string userQuestion { get; set; }

        public int qnaId { get; set; }

        public string hostName { get; set; }

        public string kbid { get; set; }

        public string endpointKey { get; set; }

        public string userId { get; set; }
    }
}

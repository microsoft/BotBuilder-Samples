// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Asp_Mvc_Bot.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}

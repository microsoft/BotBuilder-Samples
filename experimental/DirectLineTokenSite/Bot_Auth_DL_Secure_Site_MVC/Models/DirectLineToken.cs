// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

﻿namespace Bot_Auth_DL_Secure_Site_MVC.Models
{
    public class DirectLineToken
    {
        public string conversationId { get; set; }
        public string token { get; set; }
        public int expires_in { get; set; }
    }
}

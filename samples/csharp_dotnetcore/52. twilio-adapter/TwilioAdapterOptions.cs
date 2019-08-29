// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder.Adapters.Twilio;
using Microsoft.Extensions.Configuration;

namespace TwilioAdapterBot
{
    public class TwilioAdapterOptions : ITwilioAdapterOptions
    {
        public TwilioAdapterOptions(IConfiguration configuration)
        {
            TwilioNumber = configuration["TwilioNumber"];
            AccountSid = configuration["AccountSid"];
            AuthToken = configuration["AuthToken"];
            ValidationUrl = configuration["ValidationUrl"];
        }

        public string TwilioNumber { get; set; }

        public string AccountSid { get; set; }

        public string AuthToken { get; set; }

        public string ValidationUrl { get; set; }
    }
}

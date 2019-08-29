// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.3.0

namespace Microsoft.Bot.Builder.Adapters.Twilio.TestBot
{
    public class SimpleTwilioAdapterOptions : ITwilioAdapterOptions
    {
        public SimpleTwilioAdapterOptions(string twilioNumber, string accountSid, string authToken, string validationUrl)
        {
            TwilioNumber = twilioNumber;
            AccountSid = accountSid;
            AuthToken = authToken;
            ValidationUrl = validationUrl;
        }

        public string TwilioNumber { get; set; }

        public string AccountSid { get; set; }

        public string AuthToken { get; set; }

        public string ValidationUrl { get; set; }
    }
}
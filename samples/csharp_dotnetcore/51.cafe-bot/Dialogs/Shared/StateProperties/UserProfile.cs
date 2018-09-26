// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.BotBuilderSamples
{
    public class UserProfile
    {
        public UserProfile(string name, string location = null)
        {
            UserName = name;
            Location = location;
        }

        public string UserName { get; set; }

        public string Location { get; set;  }
    }
}

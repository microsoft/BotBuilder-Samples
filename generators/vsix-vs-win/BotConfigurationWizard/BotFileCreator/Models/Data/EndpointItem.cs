// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace BotFileCreator
{
    public class EndpointItem : BaseViewModel
    {
        public EndpointItem()
        {
            this.Name = string.Empty;
            this.AppId = string.Empty;
            this.AppPassword = string.Empty;
        }

        public string Name { get; set; }

        public string Endpoint { get; set; }

        public string AppId { get; set; }

        public string AppPassword { get; set; }
    }
}

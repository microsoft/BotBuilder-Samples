// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace BotFileCreator
{
    public class MSBotCommandEndpoint : MSBotCommandWrapper
    {
        private string endpoint;

        public MSBotCommandEndpoint(MSBotCommandManager commandManager, string endpoint)
        {
            this.commandManager = commandManager;
            this.endpoint = string.IsNullOrWhiteSpace(endpoint) ? string.Empty : endpoint;
        }

        public override string GetMSBotCommand()
        {
            if (!string.IsNullOrWhiteSpace(this.endpoint))
            {
                return string.Concat(base.GetMSBotCommand(), $" --endpoint {this.endpoint}");
            }

            return base.GetMSBotCommand();
        }
    }
}
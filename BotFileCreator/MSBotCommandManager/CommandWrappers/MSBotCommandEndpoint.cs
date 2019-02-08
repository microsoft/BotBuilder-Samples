// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace BotFileCreator
{
    public class MSBotCommandEndpoint : MSBotCommandWrapper
    {
        private string endpoint;
        private string appID;
        private string appPassword;

        public MSBotCommandEndpoint(MSBotCommandManager commandManager, string endpoint)
        {
            this.commandManager = commandManager;
            this.endpoint = string.IsNullOrWhiteSpace(endpoint) ? string.Empty : endpoint;
            this.appID = string.Empty;
            this.appPassword = string.Empty;
        }

        public MSBotCommandEndpoint(MSBotCommandManager commandManager, string endpoint, string appID, string appPassword)
        {
            this.commandManager = commandManager;
            this.endpoint = string.IsNullOrWhiteSpace(endpoint) ? string.Empty : endpoint;
            this.appID = appID;
            this.appPassword = appPassword;
        }

        public override string GetMSBotCommand()
        {
            if (!string.IsNullOrWhiteSpace(this.endpoint))
            {
                var command = string.Concat(base.GetMSBotCommand(), $" --endpoint {this.endpoint}");

                if (!string.IsNullOrWhiteSpace(this.appID))
                {
                    command = string.Concat(command, $" --appId {this.appID}");
                }

                if (!string.IsNullOrWhiteSpace(this.appPassword))
                {
                    command = string.Concat(command, $" --appPassword {this.appPassword}");
                }

                return command;
            }

            return base.GetMSBotCommand();
        }
    }
}
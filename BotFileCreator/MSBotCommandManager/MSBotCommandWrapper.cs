// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace BotFileCreator
{
    public abstract class MSBotCommandWrapper : MSBotCommandManager
    {
        protected MSBotCommandManager commandManager;

        public override string GetMSBotCommand()
        {
            if (commandManager != null)
            {
                return commandManager.GetMSBotCommand();
            }

            return string.Empty;
        }
    }
}

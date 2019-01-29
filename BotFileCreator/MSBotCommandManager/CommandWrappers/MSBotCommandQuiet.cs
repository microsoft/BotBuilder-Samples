// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace BotFileCreator
{
    public class MSBotCommandQuiet : MSBotCommandWrapper
    {
        public MSBotCommandQuiet(MSBotCommandManager commandManager)
        {
            this.commandManager = commandManager;
        }

        public override string GetMSBotCommand()
        {
            return string.Concat(base.GetMSBotCommand(), " --quiet");
        }
    }
}

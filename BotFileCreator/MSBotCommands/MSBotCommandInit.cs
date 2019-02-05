// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace BotFileCreator
{
    public class MSBotCommandInit : MSBotCommandManager
    {
        private string projectFolderPath;

        private string botFileName;

        public MSBotCommandInit(string projectFolderPath, string botFileName)
        {
            this.projectFolderPath = projectFolderPath;
            this.botFileName = botFileName;
        }

        public override string GetMSBotCommand()
        {
            return $"/C cd {this.projectFolderPath} & msbot init --name {botFileName}";
        }
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace BotFileCreator
{
    public class BotFileNameManager
    {
        public BotFileNameManager(string botFileName, string projectName)
        {
            this.BotFileName = botFileName;
            this.ProjectName = projectName;
            this.ProjectDirectoryPath = this.GetProjectDirectoryPath();
        }

        public string BotFileName { get; set; }

        public string ProjectName { get; set; }

        public string ProjectDirectoryPath { get; set; }

        /// <summary>
        /// Returns the Working Project Directory
        /// </summary>
        /// <returns>Project Directory</returns>
        private string GetProjectDirectoryPath()
        {
            return this.ProjectName.Substring(0, this.ProjectName.LastIndexOf('\\'));
        }
    }
}

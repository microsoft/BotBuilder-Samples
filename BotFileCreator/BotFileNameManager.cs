namespace BotFileCreator
{
    public class BotFileNameManager
    {
        public BotFileNameManager(string botFileName, string projectName)
        {
            this.BotFileName = botFileName;
            this.ProjectName = projectName;
            this.BotFileNameWithExtension = this.GetBotFileName();
            this.ProjectDirectoryPath = this.GetProjectDirectoryPath();
        }

        public string BotFileName { get; set; }

        // Bot's name, with the '.bot' extension
        public string BotFileNameWithExtension { get; set; }

        public string ProjectName { get; set; }

        public string ProjectDirectoryPath { get; set; }

        /// <summary>
        /// Add the '.bot' extension to the bot file's name if it doesn't have it.
        /// </summary>
        /// <returns>Bot File name</returns>
        private string GetBotFileName()
        {
            return this.BotFileName.EndsWith(".bot") ? this.BotFileName : string.Concat(this.BotFileName, ".bot");
        }

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

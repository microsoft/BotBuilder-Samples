namespace BotFileCreator.CLIExecutor
{
    public static class CLIExecutor
    {
        /// <summary>
        /// Executes a specific command
        /// </summary>
        /// <param name="command">Command to execute</param>
        public static void RunCommand(string command)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = command;
            process.StartInfo = startInfo;
            process.Start();
        }
    }
}

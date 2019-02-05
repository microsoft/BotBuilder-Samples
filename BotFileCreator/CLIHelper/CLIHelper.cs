namespace BotFileCreator.CLIHelper
{
    using System;
    using System.IO;

    public static class CLIHelper
    {
        /// <summary>
        /// Executes a specific command
        /// </summary>
        /// <param name="command">Command to execute</param>
        /// <param name="output">result of the command output</param>
        public static void RunCommand(string command, out string output)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = command;
            process.StartInfo = startInfo;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.Start();
            output = process.StandardOutput.ReadToEnd();
        }

        /// <summary>
        /// Checks if an specific command exists on the Path variable
        /// </summary>
        /// <param name="commandName">Command to validate</param>
        /// <returns>Boolean</returns>
        public static bool ExistsOnPath(string commandName)
        {
            return GetFullPath(commandName) != null;
        }

        /// <summary>
        /// Gets the full path of a specific Command
        /// </summary>
        /// <param name="commandName">Command to validate</param>
        /// <returns>The full path</returns>
        public static string GetFullPath(string commandName)
        {
            if (File.Exists(commandName))
            {
                return Path.GetFullPath(commandName);
            }

            var values = Environment.GetEnvironmentVariable("PATH");
            foreach (var path in values.Split(';'))
            {
                var fullPath = Path.Combine(path, commandName);
                if (File.Exists(fullPath))
                {
                    return fullPath;
                }
            }

            return null;
        }
    }
}

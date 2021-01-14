using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Xunit;

namespace Test
{
    public class Generate
    {

        static string Execute(string cmd, bool useCmd = true)
        {
            var cwd = Path.Join(Path.GetTempPath(), "TestBot");
            var startInfo = useCmd ? new ProcessStartInfo("cmd.exe", $"/C {cmd}") : new ProcessStartInfo(cmd);
            startInfo.WorkingDirectory = cwd;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            Directory.CreateDirectory(cwd);
            var process = Process.Start(startInfo);
            var std = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();
            process.WaitForExit();
            return std + error;
        }

        [Fact]
        public void GenerateAndRun()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var projectPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\.."));
                var testBot = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "TestBot.exe"));
                var schemaPath = Path.Combine(projectPath, "sandwich.schema");
                var transcriptPath = Path.Combine(projectPath, "sandwich.transcript");
                var output = Execute("bf config:show:luis");
                Assert.Contains("authoringKey", output);

                output = Execute($"bf dialog:generate {schemaPath}");
                Debug.WriteLine(output);
                Assert.True(!output.Contains("Error"));

                output = Execute($"bf dialog:generate:test {transcriptPath} sandwich -o test");
                Assert.Contains("Generated", output);

                output = Execute("build.cmd");
                Debug.WriteLine(output);
                Assert.True(output.Contains("No changes") || output.Contains("Successfully wrote settings file"));

                output = Execute(testBot, false);
                Debug.WriteLine(output);
                Assert.Contains("Passed", output);
            }
        }
    }
}

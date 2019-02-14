// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace BotFileCreator
{
    using System.Linq;

    public class FileSystemService
    {
        private string _projectName;

        public FileSystemService()
        {
            this._projectName = GeneralSettings.Default.ProjectName;
        }

        public void AddFileToProject(string botFileName)
        {
            // Load a specific project. Also, avoids several problems for re-loading the same project more than once
            var project = Microsoft.Build.Evaluation.ProjectCollection.GlobalProjectCollection.LoadedProjects.FirstOrDefault(pr => pr.FullPath == _projectName);
            project = project == null ? new Microsoft.Build.Evaluation.Project(this._projectName) : project;

            if (project != null)
            {
                // Reevaluates the project to add any change
                project.ReevaluateIfNecessary();

                // Checks if the project has a file with the same name. If it doesn't, it will be added to the project
                if (project.Items.FirstOrDefault(item => item.EvaluatedInclude == botFileName) == null)
                {
                    project.AddItem("Compile", botFileName);
                    project.Save();
                }
            }
        }

        public string GetProjectDirectoryPath()
        {
            return _projectName.Substring(0, _projectName.LastIndexOf('\\'));
        }
    }
}

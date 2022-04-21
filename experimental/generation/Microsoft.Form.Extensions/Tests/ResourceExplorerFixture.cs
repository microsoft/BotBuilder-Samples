// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.IO;
using Microsoft.Bot.Builder.Dialogs.Declarative.Resources;

namespace Microsoft.Bot.Builder.Dialogs.Adaptive.Tests
{
    public class ResourceExplorerFixture : IDisposable
    {
        private string _folderPath = string.Empty;

        public ResourceExplorerFixture()
        {
            ResourceExplorer = new ResourceExplorer();
        }

        public ResourceExplorer ResourceExplorer { get; private set; }

        public ResourceExplorerFixture Initialize(string resourceFolder)
        {
            if (_folderPath.Length == 0)
            {
                _folderPath = Path.Combine(TestUtils.GetProjectPath(), "Tests", resourceFolder);
                ResourceExplorer = ResourceExplorer.AddFolder(_folderPath, monitorChanges: false);
            }

            return this;
        }

        public void Dispose()
        {
            _folderPath = string.Empty;
            ResourceExplorer.Dispose();
        }
    }
}

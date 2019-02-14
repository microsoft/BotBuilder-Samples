// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace BotFileCreator
{
    using System;

    /// <summary>
    /// Interaction logic for BotFileCreationWizard.xaml
    /// </summary>
    public partial class BotFileCreationWizard : BaseDialogWindow
    {
        public BotFileCreationWizard()
        {
            var botConfiguration = new BotConfigurationViewModel();
            botConfiguration.CloseAction = new Action(() => this.Close());
            DataContext = botConfiguration;
            InitializeComponent();
        }
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
using System;

namespace BotFileCreator
{
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

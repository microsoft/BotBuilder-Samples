// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Windows;
using System.Windows.Controls;

namespace BotManagerVSIX
{
    /// <summary>
    /// Interaction logic for ChannelRegistrationView.xaml
    /// </summary>
    public partial class ChannelRegistrationView : UserControl
    {
        public ChannelRegistrationView()
        {
            InitializeComponent();
        }

        private void ResourceGroupButton_Click(object sender, RoutedEventArgs e)
        {
            ResourceGroupView.OpenAndSyncResourceGroupView(ResourceGroupCombo);
        }
    }
}

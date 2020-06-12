// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections;
using System.Windows.Controls;
using BotManagerVSIX.Models;

namespace BotManagerVSIX
{
    /// <summary>
    /// Interaction logic for SummaryView.xaml
    /// </summary>
    public partial class SummaryView : UserControl
    {

        public ArrayList data = new ArrayList();
        public SummaryView()
        {
            InitializeComponent();
            LoadSummaryDataGrid();
        }

        private void LoadSummaryDataGrid()
        {
            data.Add(new AzureResourceItem() { Type = "Resource Group", Name = "test-resource-group", Link = "https://portal.azure.com/#@testing.com/subscriptions/xxx/resourceGroups/test-resource-group/overview" });
            data.Add(new AzureResourceItem() { Type = "QnA App Service", Name = "testing-qna", Link = "https://portal.azure.com/#@testing.com/subscriptions/xxx/resourceGroups/test-resource-group/AppServices" });
            data.Add(new AzureResourceItem() { Type = "QnA Knowledge Base", Name = "testing-KB", Link = "https://portal.azure.com/#@testing.com/subscriptions/xxx/resourceGroups/test-resource-group/KnowledgeBases" });
            data.Add(new AzureResourceItem() { Type = "App Service", Name = "testing-bot", Link = "https://portal.azure.com/#@testing.com/subscriptions/xxx/resourceGroups/test-resource-group/AppServices" });
            data.Add(new AzureResourceItem() { Type = "Channel Registration", Name = "testing-channel", Link = "https://portal.azure.com/#@testing.com/subscriptions/xxx/resourceGroups/test-resource-group/ChannelRegistrations" });

            summaryDataGrid.DataContext = data;
        }
    }
}

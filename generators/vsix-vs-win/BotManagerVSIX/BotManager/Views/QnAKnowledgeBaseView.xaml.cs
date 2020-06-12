// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Windows;
using System.Windows.Controls;

namespace BotManagerVSIX
{
    /// <summary>
    /// Interaction logic for QnAKnowledgeBaseView.xaml
    /// </summary>
    public partial class QnAKnowledgeBaseView : UserControl
    {
        public QnAKnowledgeBaseView()
        {
            InitializeComponent();
        }

        private void PickFileClick(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
            {
                // Set filter for file extension and default file extension 
                DefaultExt = ".tsv",
                Filter = "QnA Files (*.qna)|*.qna|DTO Files (*.dto)|*.dto|TSV Files (*.tsv)|*.tsv"
            };

            // Display OpenFileDialog by calling ShowDialog method 
            bool? result = dlg.ShowDialog();

            // Get the selected file name and display it in the TextBox 
            if (result == true)
            {
                string filename = dlg.FileName;
                FileTextBox.Text = filename;
            }
        }
    }
}

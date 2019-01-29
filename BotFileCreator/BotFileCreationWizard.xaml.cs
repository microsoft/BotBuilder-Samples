// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace BotFileCreator
{
    using System;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for BotFileCreationWizard.xaml
    /// </summary>
    public partial class BotFileCreationWizard : BaseDialogWindow
    {
        private string botFileName;
        private string botFileFullPath;

        public BotFileCreationWizard()
        {
            InitializeComponent();
            this.botFileFullPath = GeneralSettings.Default.ProjectName;
        }

        /// <summary>
        /// When the BotFileName textbox changes, its value is stored in the GeneralSettings file to use it later.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event</param>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            this.botFileName = textBox.Text ?? string.Empty;
        }

        /// <summary>
        /// When the Create button is clicked, it will execute a process to create a .bot file in the project where the VS Extension is being executed.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event</param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BotFileNameManager botFileNameManager = new BotFileNameManager(this.botFileName, this.botFileFullPath);

                BotFileCreatorManager fileCreator = new BotFileCreatorManager(botFileNameManager);

                Tuple<bool, string> fileCreatorResult = fileCreator.CreateBotFile();

                // If the fileCreator returns a tuple with a FALSE value, will show the error message (Item2) in the Wizard.
                if (!fileCreatorResult.Item1)
                {
                    MessageBox.Show(fileCreatorResult.Item2, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    // If the file was successfully created, the Wizard will be closed.
                    MessageBox.Show("Bot file successfully created", "Bot file successfully created", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                // If there is an exception, it will be prompted.
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

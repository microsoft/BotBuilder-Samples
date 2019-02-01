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
        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BotFileNameManager botFileNameManager = new BotFileNameManager(this.botFileName, this.botFileFullPath);
                MSBotCommandManager commandManager = CreateMSBotCommandManager(botFileNameManager);

                BotFileCreatorManager fileCreator = new BotFileCreatorManager(botFileNameManager, commandManager);

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

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Returns a MSBotCommandManager which contains the command to execute for creating the .bot file
        /// </summary>
        /// <param name="botFileNameManager">File Name Manager</param>
        /// <returns>MSBotCommandManager</returns>
        private MSBotCommandManager CreateMSBotCommandManager(BotFileNameManager botFileNameManager)
        {
            MSBotCommandInit init = new MSBotCommandInit(botFileNameManager.ProjectDirectoryPath, botFileNameManager.BotFileName);

            // Does not prompt any message after executing `msbot init` command
            MSBotCommandQuiet quiet = new MSBotCommandQuiet(init);

            return quiet;
        }

        private void BotName_Click(object sender, RoutedEventArgs e)
        {
            this.BotEndpointPanel.Visibility = Visibility.Collapsed;
            this.BotServicesPanel.Visibility = Visibility.Collapsed;
            this.BotEncryptPanel.Visibility = Visibility.Collapsed;

            if (!this.BotNamePanel.IsVisible)
            {
                this.BotNamePanel.Visibility = Visibility.Visible;
            }
        }

        private void BotEndpoint_Click(object sender, RoutedEventArgs e)
        {
            this.BotNamePanel.Visibility = Visibility.Collapsed;
            this.BotServicesPanel.Visibility = Visibility.Collapsed;
            this.BotEncryptPanel.Visibility = Visibility.Collapsed;

            if (!this.BotEndpointPanel.IsVisible)
            {
                this.BotEndpointPanel.Visibility = Visibility.Visible;
            }
        }

        private void BotServices_Click(object sender, RoutedEventArgs e)
        {
            this.BotNamePanel.Visibility = Visibility.Collapsed;
            this.BotEndpointPanel.Visibility = Visibility.Collapsed;
            this.BotEncryptPanel.Visibility = Visibility.Collapsed;

            if (!this.BotServicesPanel.IsVisible)
            {
                this.BotServicesPanel.Visibility = Visibility.Visible;
            }
        }

        private void BotEncrypt_ClicK(object sender, RoutedEventArgs e)
        {
            this.BotNamePanel.Visibility = Visibility.Collapsed;
            this.BotEndpointPanel.Visibility = Visibility.Collapsed;
            this.BotServicesPanel.Visibility = Visibility.Collapsed;

            if (!this.BotEncryptPanel.IsVisible)
            {
                this.BotEncryptPanel.Visibility = Visibility.Visible;
            }
        }
    }
}

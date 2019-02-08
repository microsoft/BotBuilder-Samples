// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
using System;
using System.Windows;
using System.Windows.Controls;

namespace BotFileCreator
{
    /// <summary>
    /// Interaction logic for BotFileCreationWizard.xaml
    /// </summary>
    public partial class BotFileCreationWizard : BaseDialogWindow
    {
        private string botFileFullPath; // This should be in the model class or where the logic run the command to store the file
        private BotConfigurationViewModel botConfiguration;

        public BotFileCreationWizard()
        {
            botConfiguration = new BotConfigurationViewModel();
            DataContext = botConfiguration;

            InitializeComponent();
            this.botFileFullPath = GeneralSettings.Default.ProjectName;  // This should be in the model class or where the logic run the command to store the file
            ValidatePrerequisites();
        }

        public bool PrerequisitesInstalled { get; set; }

        /// <summary>
        /// When the Create button is clicked, it will execute a process to create a .bot file in the project where the VS Extension is being executed.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event</param>
        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BotFileNameManager botFileNameManager = new BotFileNameManager(this.botConfiguration.BotFileName, this.botFileFullPath);
                MSBotCommandManager commandManager = CreateMSBotCommandManager(botFileNameManager);

                // Checks if the bot configuration is valid
                Tuple<bool, string> configIsValid = BotFileConfigurationIsValid(botFileNameManager);

                // If the bot's configuration is not valid, it will show an error
                if (!configIsValid.Item1)
                {
                    MessageBox.Show(configIsValid.Item2, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Class that will create the .bot file
                BotFileCreatorManager fileCreator = new BotFileCreatorManager(botFileNameManager, commandManager, botConfiguration);

                // Creates the .bot file
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

                    if (!(bool)this.EncryptCheckBox.IsChecked)
                    {
                        this.Close();
                    }

                    this.BotEncrypt_ClicK(sender, e);
                    DisableAfterEncryption(this.gripPanel);
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
        /// Checks if all the pre-requisites are installed/present
        /// </summary>
        private void ValidatePrerequisites()
        {
            this.PrerequisitesInstalled = false;

            Tuple<bool, string> result;

            result = ValidateMSBotInstallation();

            // If MSBot is not installed, shows an error message and close the wizard
            if (!result.Item1)
            {
                ShowErrorMsgAndClose("MSBot is not installed", result.Item2);
                return;
            }

            this.PrerequisitesInstalled = true;
        }

        /// <summary>
        /// Validates the installation of MSBot command
        /// </summary>
        /// <returns>Boolean that indicates if the MSBot command is installed in the computer</returns>
        private Tuple<bool, string> ValidateMSBotInstallation()
        {
            var msbotExists = CLIHelper.CLIHelper.ExistsOnPath("msbot");
            var msgError = msbotExists ? string.Empty : "MSBot is not installed on your computer. Read the README.md file to check how to install it.";

            return new Tuple<bool, string>(msbotExists, msgError);
        }

        /// <summary>
        /// Returns a tuple with a bool which specifies if the bot file configuration is valid, and a string with the error message (empty string if there isn't any error).
        /// </summary>
        /// <param name="botFileNameManager">.</param>
        /// <returns>Tuple</returns>
        private Tuple<bool, string> BotFileConfigurationIsValid(BotFileNameManager botFileNameManager)
        {
            // If the .bot file name is Null or WhiteSpace, returns an error.
            if (string.IsNullOrWhiteSpace(botFileNameManager.BotFileName))
            {
                return new Tuple<bool, string>(false, "Bot file name can't be null.");
            }

            // If the .bot file name contains any whitespace, the method will return an error.
            if (botFileNameManager.BotFileName.Contains(" "))
            {
                return new Tuple<bool, string>(false, "Bot file name can't have whitespaces.");
            }

            // A tuple with True and Empty string will be returned if there are no errors.
            return new Tuple<bool, string>(true, string.Empty);
        }

        /// <summary>
        /// Returns a MSBotCommandManager which contains the command to execute for creating the .bot file
        /// </summary>
        /// <param name="botFileNameManager">File Name Manager</param>
        /// <returns>MSBotCommandManager</returns>
        private MSBotCommandManager CreateMSBotCommandManager(BotFileNameManager botFileNameManager)
        {
            MSBotCommandManager commandManager;

            MSBotCommandInit init = new MSBotCommandInit(botFileNameManager.ProjectDirectoryPath, botFileNameManager.BotFileName);

            commandManager = init;

            if (!string.IsNullOrWhiteSpace(botConfiguration.Endpoint))
            {
                // Adds Endpoint to the bot file
                MSBotCommandEndpoint endpoint = new MSBotCommandEndpoint(init, botConfiguration.Endpoint);
                commandManager = endpoint;
            }

            if (this.EncryptCheckBox.IsChecked == true)
            {
                var encrypt = new MSBotCommandEncrypt(commandManager);
                commandManager = encrypt;
            }


            // Does not prompt any message after executing `msbot init` command
            MSBotCommandQuiet quiet = new MSBotCommandQuiet(commandManager);

            return quiet;
        }

        /// <summary>
        /// Shows an error message and close the wizard
        /// </summary>
        /// <param name="title">Title of the error message window</param>
        /// <param name="msgError">Error message description</param>
        private void ShowErrorMsgAndClose(string title, string msgError)
        {
            MessageBox.Show(msgError, title, MessageBoxButton.OK, MessageBoxImage.Error);
            this.Close();
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

        private void EncryptCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (!this.EncryptNote.IsVisible)
            {
                this.EncryptNote.Visibility = Visibility.Visible;
            }
        }

        private void EncryptCheckBox_UnChecked(object sender, RoutedEventArgs e)
        {
            if (this.EncryptNote.IsVisible)
            {
                this.EncryptNote.Visibility = Visibility.Collapsed;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(botConfiguration.SecretKey);
        }

        private void DisableAfterEncryption(Grid gridPanel)
        {
            this.CancelButton.Content = "Close";
            this.leftMenu.IsEnabled = false;
            this.CreateButton.IsEnabled = false;
            this.EncryptCheckBox.IsEnabled = false;
        }
    }
}

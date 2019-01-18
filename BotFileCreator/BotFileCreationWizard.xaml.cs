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
        public BotFileCreationWizard()
        {
            InitializeComponent();
        }

        /// <summary>
        /// When the BotFileName textbox changes, its value is stored in the GeneralSettings file to use it later.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            GeneralSettings.Default.BotFileName = textBox.Text ?? string.Empty;
        }

        /// <summary>
        /// When the Create button is clicked, it will execute a process to create a .bot file in the project where the VS Extension is being executed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BotFileCreatorManager fileCreator = new BotFileCreatorManager(GeneralSettings.Default.BotFileName, GeneralSettings.Default.ProjectName);

                Tuple<bool, string> fileCreatorResult = fileCreator.CreateBotFile();

                // If the fileCreator returns a tuple with a FALSE value, will show the error message (Item2) in the Wizard.
                if (!fileCreatorResult.Item1)
                {
                    ErrorLabel.Content = fileCreatorResult.Item2;
                }
                else
                {
                    // If the file was successfully created, it will close the Wizard.
                    ErrorLabel.Content = string.Empty;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                // If there is an exception, it will be shown in the Wizard.
                ErrorLabel.Content = ex.Message;
            }
        }
    }
}

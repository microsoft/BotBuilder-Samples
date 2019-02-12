using System;
using System.Windows;
using System.Windows.Input;

namespace BotFileCreator
{
    public class BotConfigurationViewModel : BaseViewModel
    {
        private readonly ICommand _cancelCommand;

        private readonly ICommand _createCommand;

        private readonly ICommand _botNameCommand;

        private readonly ICommand _botEndpointCommmand;

        private readonly ICommand _botServicesCommand;

        private readonly ICommand _botEncryptCommand;

        private readonly ICommand _isCheckedEncryptCheckBox;

        private readonly ICommand _copyCommand;

        private bool _encryptNoteIsVisible;

        private EndpointItem _endpointItem;

        private string _secretKey;

        private string _botFileName = string.Empty;

        private string _panelToShow = "BotName";

        public BotConfigurationViewModel()
        {
            _encryptNoteIsVisible = false;
            _copyCommand = new RelayCommand(param => this.CopySecretKey(), null);
            _isCheckedEncryptCheckBox = new RelayCommand(param => this.CheckEncryptCheckBox(), null);
            _createCommand = new RelayCommand(param => this.CreateBotFile(), null);
            _cancelCommand = new RelayCommand(param => this.CloseAction(), null);
            _botNameCommand = new RelayCommand(param => this.CollapsePanels("BotName"), null);
            _botEndpointCommmand = new RelayCommand(param => this.CollapsePanels("BotEndpoint"), null);
            _botServicesCommand = new RelayCommand(param => this.CollapsePanels("BotServices"), null);
            _botEncryptCommand = new RelayCommand(param => this.CollapsePanels("BotEncrypt"), null);
        }

        public Visibility EncryptNoteVisibility
        {
            get => EncryptNoteIsVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        public bool EncryptNoteIsVisible
        {
            get => _encryptNoteIsVisible;
            set
            {
                _encryptNoteIsVisible = value;
                NotifyPropertyChanged("EncryptNoteVisibility");
            }
        }

        public string PanelToShow
        {
            get => _panelToShow;
            set
            {
                _panelToShow = value;
                NotifyPropertyChanged("BotNameVisibility");
                NotifyPropertyChanged("BotEndpointVisibility");
                NotifyPropertyChanged("BotServicesVisibility");
                NotifyPropertyChanged("BotEncrypVisibility");
            }
        }

        public Visibility BotNameVisibility
        {
            get => _panelToShow == "BotName" ? Visibility.Visible : Visibility.Collapsed;
        }

        public Visibility BotEndpointVisibility
        {
            get => _panelToShow == "BotEndpoint" ? Visibility.Visible : Visibility.Collapsed;
        }

        public Visibility BotServicesVisibility
        {
            get => _panelToShow == "BotServices" ? Visibility.Visible : Visibility.Collapsed;
        }

        public Visibility BotEncrypVisibility
        {
            get => _panelToShow == "BotEncrypt" ? Visibility.Visible : Visibility.Collapsed;
        }

        public string SecretKey { get => _secretKey; set => SetProperty(ref _secretKey, value); }

        public string BotFileName { get => _botFileName; set => SetProperty(ref _botFileName, value); }

        public EndpointItem EndpointItem
        {
            get
            {
                if (_endpointItem == null)
                {
                    _endpointItem = new EndpointItem();
                }

                return _endpointItem;
            }

            set => SetProperty(ref _endpointItem, value);
        }

        public bool EncryptCheckBoxIsChecked { get; set; }

        public ICommand CopyCommand { get => _copyCommand; }

        public ICommand CreateCommand { get => _createCommand; }

        public ICommand CancelCommand { get => _cancelCommand; }

        public ICommand BotNameCommand { get => _botNameCommand; }

        public ICommand BotEndpointCommand { get => _botEndpointCommmand; }

        public ICommand BotServicesCommand { get => _botServicesCommand; }

        public ICommand BotEncryptCommand { get => _botEncryptCommand; }

        public ICommand IsCheckedEncryptCheckBox { get => _isCheckedEncryptCheckBox; }

        public Action CloseAction { get; set; }

        public void CreateBotFile()
        {
            var botFileFullPath = GeneralSettings.Default.ProjectName;
            BotFileNameManager botFileNameManager = new BotFileNameManager(BotFileName, botFileFullPath);
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
            BotFileCreatorManager fileCreator = new BotFileCreatorManager(botFileNameManager, commandManager);

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

                if (!EncryptCheckBoxIsChecked)
                {
                    CloseAction();
                }
            }
        }

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

        private MSBotCommandManager CreateMSBotCommandManager(BotFileNameManager botFileNameManager)
        {
            MSBotCommandManager commandManager;

            MSBotCommandInit init = new MSBotCommandInit(botFileNameManager.ProjectDirectoryPath, botFileNameManager.BotFileName);

            commandManager = init;

            if (!string.IsNullOrWhiteSpace(EndpointItem.Endpoint))
            {
                // Adds Endpoint to the bot file
                MSBotCommandEndpoint endpoint = new MSBotCommandEndpoint(init, EndpointItem.Endpoint);

                if (!string.IsNullOrWhiteSpace(EndpointItem.AppId) && !string.IsNullOrWhiteSpace(EndpointItem.AppPassword))
                {
                    endpoint = new MSBotCommandEndpoint(init, EndpointItem.Endpoint, EndpointItem.AppId, EndpointItem.AppPassword);
                }
                else
                {
                    endpoint = new MSBotCommandEndpoint(init, EndpointItem.Endpoint);
                }

                commandManager = endpoint;
            }

            if (this.EncryptCheckBoxIsChecked == true)
            {
                var encrypt = new MSBotCommandEncrypt(commandManager);
                commandManager = encrypt;
            }

            // Does not prompt any message after executing `msbot init` command
            MSBotCommandQuiet quiet = new MSBotCommandQuiet(commandManager);

            return quiet;
        }

        private void CollapsePanels(string panelToShow)
        {
            this.PanelToShow = panelToShow;
        }

        private void CloseWindow(Window window)
        {
            if (window != null)
            {
                window.Close();
            }
        }

        private void CheckEncryptCheckBox()
        {
            EncryptNoteIsVisible = !EncryptNoteIsVisible;
        }

        private void CopySecretKey()
        {
            if (!string.IsNullOrWhiteSpace(SecretKey))
            {
                Clipboard.SetText(SecretKey);
            }
        }
    }
}

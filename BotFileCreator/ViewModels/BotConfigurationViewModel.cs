using BotFileCreator.Repository;
using Microsoft.Bot.Configuration;
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

            // Checks if the bot configuration is valid
            Tuple<bool, string> configIsValid = BotFileConfigurationIsValid(botFileNameManager);

            // If the bot's configuration is not valid, it will show an error
            if (!configIsValid.Item1)
            {
                MessageBox.Show(configIsValid.Item2, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Repository for creating bot files
            BotFileRepository repository = new BotFileRepository(botFileNameManager.BotFileName, botFileNameManager.ProjectDirectoryPath);

            // Adds the only endpoint (if it's not null) to the bot configuration
            if (!string.IsNullOrWhiteSpace(EndpointItem.Endpoint))
            {
                EndpointService endpoint = new EndpointService() { Name = this.EndpointItem.Name, Endpoint = this.EndpointItem.Endpoint, AppId = this.EndpointItem.AppId, AppPassword = this.EndpointItem.AppPassword };
                repository.ConnectService(endpoint);
            }

            // If the "encrypt" checkbox is checked, the bot configuration is save after encrypting it
            if (EncryptCheckBoxIsChecked)
            {
                repository.Save(this.SecretKey);
            }
            else
            {
                // Save the bot configuration without encryption
                repository.Save();
            }

            // If the file was successfully created, the Wizard will be closed.
            MessageBox.Show("Bot file successfully created", "Bot file successfully created", MessageBoxButton.OK, MessageBoxImage.Exclamation);

            if (!EncryptCheckBoxIsChecked)
            {
                CloseAction();
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
            EncryptCheckBoxIsChecked = !EncryptCheckBoxIsChecked;
            EncryptNoteIsVisible = !EncryptNoteIsVisible;
            this.SecretKey = BotFileRepository.GenerateKey();
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

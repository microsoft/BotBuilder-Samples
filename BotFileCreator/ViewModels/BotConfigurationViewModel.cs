using System.ComponentModel;

namespace BotFileCreator
{
    public class BotConfigurationViewModel : BaseViewModel
    {
        public string SecretKey { get; set; }

        public string Endpoint { get; set; }

        public string BotFileName { get; set; } = string.Empty;
    }
}

namespace BotFileCreator
{
    public class BotConfigurationViewModel : BaseViewModel
    {
        public BotConfigurationViewModel()
        {
            this.EndpointItem = new EndpointItem();
        }

        public string SecretKey { get; set; }

        public EndpointItem EndpointItem { get; set; }

        public string BotFileName { get; set; } = string.Empty;
    }
}

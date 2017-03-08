namespace TestBot.Scorables
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Hosting;
    using System.Xml.Linq;
    using Microsoft.Bot.Builder.Dialogs.Internals;
    using Microsoft.Bot.Connector;

    public abstract class ExtractCodeScorable : TriggerScorable
    {
        private const string TypeAttrib = "type";

        public ExtractCodeScorable(IBotToUser botToUser, IBotData botData) : base(botToUser, botData)
        {
        }

        protected override Task PostAsync(IActivity item, bool state, CancellationToken token)
        {
            if (state)
            {
                this.SaveCode("/Assets/CSharpCode.xml", Constants.LastCSharpKey);
                this.SaveCode("/Assets/NodeJsCode.xml", Constants.LastNodeJsKey);
            }

            return Task.CompletedTask;
        }

        private void SaveCode(string sourceFile, string key)
        {
            var commandsCode = XElement.Load(HostingEnvironment.MapPath(sourceFile));
            var element = (XElement)commandsCode
                .Nodes()
                .FirstOrDefault(n => (n as XElement).Attribute(TypeAttrib).Value.Equals(this.GetType().Name));

            if (element != null)
            {
                this.BotData.PrivateConversationData.SetValue(key, element.Value);
            }
            else
            {
                this.BotData.PrivateConversationData.RemoveValue(key);
            }
        }
    }
}
using System.Collections.Generic;

namespace OmniChannel.Models
{
    public class Conversation
    {
        public string WelcomeMessage { get; set; }

        public Dictionary<string, string> ConversationDictionary { get; set; }

        public Dictionary<string, Dictionary<string, string>> EscalationDictionary { get; set; }

        public Dictionary<string, Dictionary<string, string>> EndConversationDictionary { get; set; }
    }
}

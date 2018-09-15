using Microsoft.Bot.Builder.Azure;

namespace Microsoft.BotBuilderSamples
{
    public class TranscriptStore
    {
        public TranscriptStore(AzureBlobTranscriptStore store)
        {
            Store = store;
        }

        public AzureBlobTranscriptStore Store { get; }
    }
}

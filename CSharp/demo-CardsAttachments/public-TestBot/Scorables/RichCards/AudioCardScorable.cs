namespace TestBot.Scorables
{
    using System.Collections.Generic;
    using Microsoft.Bot.Builder.Dialogs.Internals;
    using Microsoft.Bot.Connector;

    public class AudioCardScorable : RichCardScorable
    {
        public AudioCardScorable(IBotToUser botToUser, IBotData botData) : base(botToUser, botData)
        {
        }

        public override string Trigger
        {
            get
            {
                return "Audio";
            }
        }

        protected override IList<Attachment> GetCardAttachments()
        {
            return new List<Attachment>
            {
                new AudioCard
                {
                    Title = "I am your father",
                    Subtitle = "Star Wars: Episode V - The Empire Strikes Back",
                    Text = "The Empire Strikes Back (also known as Star Wars: Episode V – The Empire Strikes Back) is a 1980 American epic space opera film directed by Irvin Kershner. Leigh Brackett and Lawrence Kasdan wrote the screenplay, with George Lucas writing the film's story and serving as executive producer. The second installment in the original Star Wars trilogy, it was produced by Gary Kurtz for Lucasfilm Ltd. and stars Mark Hamill, Harrison Ford, Carrie Fisher, Billy Dee Williams, Anthony Daniels, David Prowse, Kenny Baker, Peter Mayhew and Frank Oz.",
                    Image = new ThumbnailUrl
                    {
                        Url = "https://upload.wikimedia.org/wikipedia/en/3/3c/SW_-_Empire_Strikes_Back.jpg"
                    },
                    Media = new List<MediaUrl>
                    {
                        new MediaUrl()
                        {
                            Url = "http://www.wavlist.com/movies/004/father.wav"
                        }
                    },
                    Buttons = new List<CardAction>
                    {
                        new CardAction()
                        {
                            Title = "Read More",
                            Type = ActionTypes.OpenUrl,
                            Value = "https://en.wikipedia.org/wiki/The_Empire_Strikes_Back"
                        }
                    }
                }.ToAttachment()
            };
        }
    }
}
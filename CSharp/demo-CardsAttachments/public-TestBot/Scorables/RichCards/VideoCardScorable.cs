namespace TestBot.Scorables
{
    using System.Collections.Generic;
    using Microsoft.Bot.Builder.Dialogs.Internals;
    using Microsoft.Bot.Connector;

    public class VideoCardScorable : RichCardScorable
    {
        public VideoCardScorable(IBotToUser botToUser, IBotData botData) : base(botToUser, botData)
        {
        }

        public override string Trigger
        {
            get
            {
                return "Video";
            }
        }

        protected override IList<Attachment> GetCardAttachments()
        {
            return new List<Attachment>
            {
                new VideoCard
                {
                    Title = "Big Buck Bunny",
                    Subtitle = "by the Blender Institute",
                    Text = "Big Buck Bunny (code-named Peach) is a short computer-animated comedy film by the Blender Institute, part of the Blender Foundation. Like the foundation's previous film Elephants Dream, the film was made using Blender, a free software application for animation made by the same foundation. It was released as an open-source film under Creative Commons License Attribution 3.0.",
                    Image = new ThumbnailUrl
                    {
                        Url = "https://upload.wikimedia.org/wikipedia/commons/thumb/c/c5/Big_buck_bunny_poster_big.jpg/220px-Big_buck_bunny_poster_big.jpg"
                    },
                    Media = new List<MediaUrl>
                    {
                        new MediaUrl()
                        {
                            Url = "http://download.blender.org/peach/bigbuckbunny_movies/BigBuckBunny_320x180.mp4"
                        }
                    },
                    Buttons = new List<CardAction>
                    {
                        new CardAction()
                        {
                            Title = "Learn More",
                            Type = ActionTypes.OpenUrl,
                            Value = "https://peach.blender.org/"
                        }
                    }
                }.ToAttachment()
            };
        }
    }
}
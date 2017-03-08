namespace TestBot.Scorables
{
    using System.Collections.Generic;
    using Microsoft.Bot.Builder.Dialogs.Internals;
    using Microsoft.Bot.Connector;

    public class AnimationCardScorable : RichCardScorable
    {
        public AnimationCardScorable(IBotToUser botToUser, IBotData botData) : base(botToUser, botData)
        {
        }

        public override string Trigger
        {
            get
            {
                return "Animation";
            }
        }

        protected override IList<Attachment> GetCardAttachments()
        {
            return new List<Attachment>
            {
                new AnimationCard
                {
                    Title = "Microsoft Bot Framework",
                    Subtitle = "Animation Card",
                    Image = new ThumbnailUrl
                    {
                        Url = "https://docs.botframework.com/en-us/images/faq-overview/botframework_overview_july.png"
                    },
                    Media = new List<MediaUrl>
                    {
                        new MediaUrl()
                        {
                            Url = "http://i.giphy.com/Ki55RUbOV5njy.gif"
                        }
                    }
                }.ToAttachment()
            };
        }
    }
}
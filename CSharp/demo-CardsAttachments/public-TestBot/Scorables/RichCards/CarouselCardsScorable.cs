namespace TestBot.Scorables
{
    using System.Collections.Generic;
    using Microsoft.Bot.Builder.Dialogs.Internals;
    using Microsoft.Bot.Connector;

    public class CarouselCardsScorable : RichCardScorable
    {
        public CarouselCardsScorable(IBotToUser botToUser, IBotData botData) : base(botToUser, botData)
        {
        }

        public override string Trigger
        {
            get
            {
                return "Carousel";
            }
        }

        protected override IList<Attachment> GetCardAttachments()
        {
            return new List<Attachment>
            {
                new HeroCard
                {
                    Title = "BotFramework Hero Card",
                    Subtitle = "Your bots — wherever your users are talking",
                    Text = "Build and connect intelligent bots to interact with your users naturally wherever they are, from text/sms to Skype, Slack, Office 365 mail and other popular services.",
                    Images = new List<CardImage> { new CardImage("https://sec.ch9.ms/ch9/7ff5/e07cfef0-aa3b-40bb-9baa-7c9ef8ff7ff5/buildreactionbotframework_960.jpg") },
                    Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Get Started", value: "https://docs.microsoft.com/bot-framework") }
                }.ToAttachment(),
                new ThumbnailCard
                {
                    Title = "BotFramework Thumbnail Card",
                    Subtitle = "Your bots — wherever your users are talking",
                    Text = "Build and connect intelligent bots to interact with your users naturally wherever they are, from text/sms to Skype, Slack, Office 365 mail and other popular services.",
                    Images = new List<CardImage> { new CardImage("https://sec.ch9.ms/ch9/7ff5/e07cfef0-aa3b-40bb-9baa-7c9ef8ff7ff5/buildreactionbotframework_960.jpg") },
                    Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Get Started", value: "https://docs.microsoft.com/bot-framework") }
                }.ToAttachment(),
                new AnimationCard
                {
                    Title = "Microsoft Bot Framework",
                    Subtitle = "Animation Card",
                    Image = new ThumbnailUrl
                    {
                        Url = "https://docs.microsoft.com/en-us/bot-framework/media/how-it-works/architecture-resize.png"
                    },
                    Media = new List<MediaUrl>
                    {
                        new MediaUrl()
                        {
                            Url = "http://i.giphy.com/Ki55RUbOV5njy.gif"
                        }
                    }
                }.ToAttachment(),
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

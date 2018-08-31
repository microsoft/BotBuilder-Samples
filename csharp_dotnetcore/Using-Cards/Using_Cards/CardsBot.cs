using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;

namespace Using_Cards
{
    public class CardsBot : IBot
    {
        private readonly CardsBotAccessors _accessors;
        public DialogSet Dialogs;

        public CardsBot(CardsBotAccessors accessors)
        {
            _accessors = accessors;

            Dialogs = new DialogSet(accessors.ConversationDialogState);
            Dialogs.Add(new WaterfallDialog("cardSelector", new WaterfallStep[] { ChoiceCardStep, ShowCardStep }));
            Dialogs.Add(new ChoicePrompt("cardPrompt"));
        }

        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = new CancellationToken())
        {
            switch (turnContext.Activity.Type)
            {
                case ActivityTypes.Message:
                    try
                    {
                        var dc = await Dialogs.CreateContextAsync(turnContext, cancellationToken);
                        await dc.ContinueAsync(cancellationToken);

                        if (!dc.Context.Responded)
                        {
                            await dc.BeginAsync("cardSelector", cancellationToken: cancellationToken);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }

                    break;
                case ActivityTypes.ConversationUpdate:

                    // Send a welcome message to the user and tell them what actions they need to perform to use this bot
                    if (turnContext.Activity.MembersAdded.Any())
                    {
                        {
                            foreach (var member in turnContext.Activity.MembersAdded)
                            {
                                var newUserName = member.Name;
                                if (member.Id != turnContext.Activity.Recipient.Id)
                                {
                                    await turnContext.SendActivityAsync($"Welcome to CardBot {newUserName}. This bot will show you different types of Rich Cards.  Please type anything to get started.", cancellationToken: cancellationToken);
                                }
                            }
                        }
                    }
                    break;
                default:
                    // There is no code in this bot to deal with ActivityTypes other than conversationUpdate or message
                    await turnContext.SendActivityAsync("This type of activity is not handled in this bot", cancellationToken: cancellationToken);
                    break;
            }
        }

        private Task<DialogTurnResult> ChoiceCardStep(DialogContext dc, WaterfallStepContext step, CancellationToken cancellationToken)
        {
            return dc.PromptAsync("cardPrompt", GenerateOptions(dc.Context.Activity), cancellationToken);
        }

        public static PromptOptions GenerateOptions(Activity activity)
        {
            return new PromptOptions()
            {
                Prompt = activity.CreateReply("What card would you like to see? You can click or type the card name"),
                Choices = new List<Choice>()
                {
                    new Choice()
                    {
                        Value = "Animation card",
                    },
                    new Choice()
                    {
                        Value = "Audio card",
                    },
                    new Choice()
                    {
                        Value = "Hero card",
                    },
                    new Choice()
                    {
                        Value = "Receipt card",
                    },
                    new Choice()
                    {
                        Value = "Signin card",
                    },
                    new Choice()
                    {
                        Value = "Thumbnail card",
                    },
                    new Choice()
                    {
                        Value = "Video card",
                    },
                    new Choice()
                    {
                        Value = "All cards",
                    }
                }
            };
        }

        private static async Task<DialogTurnResult> ShowCardStep(DialogContext dc, WaterfallStepContext step, CancellationToken cancellationToken)
        {
            var activity = dc.Context.Activity;

            // Get the text from the activity to use to show the correct card
            var text = activity.Text.ToLowerInvariant().Split(' ')[0];

            // Replay to the activity we received with an activity 
            var reply = activity.CreateReply();

            // Cards are sent as Attackments in the Bot Framework.
            // So we need to create a list of attachments on the activity.
            reply.Attachments = new List<Attachment>();

            // Decide which type of card(s) we are going to show the user
            switch (text)
            {
                case "herocard":
                case "hero":
                    reply.Attachments.Add(GetHeroCard().ToAttachment());
                    break;
                case "thumbnailcard":
                case "thumbnail":
                    reply.Attachments.Add(GetThumbnailCard().ToAttachment());
                    break;
                case "receiptcard":
                case "receipt":
                    reply.Attachments.Add(GetReceiptCard().ToAttachment());
                    break;
                case "signincard":
                case "signin":
                case "sign":
                    reply.Attachments.Add(GetSigninCard().ToAttachment());
                    break;
                case "animationcard":
                case "animation":
                    reply.Attachments.Add(GetAnimationCard().ToAttachment());
                    break;
                case "videocard":
                case "video":
                    reply.Attachments.Add(GetVideoCard().ToAttachment());
                    break;
                case "audiocard":
                case "audio":
                    reply.Attachments.Add(GetAudioCard().ToAttachment());
                    break;
                default:
                    // Send all cards in a carousel if the user wants to see all cards.
                    reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                    reply.Attachments.Add(GetHeroCard().ToAttachment());
                    reply.Attachments.Add(GetThumbnailCard().ToAttachment());
                    reply.Attachments.Add(GetReceiptCard().ToAttachment());
                    reply.Attachments.Add(GetSigninCard().ToAttachment());
                    reply.Attachments.Add(GetAnimationCard().ToAttachment());
                    reply.Attachments.Add(GetVideoCard().ToAttachment());
                    reply.Attachments.Add(GetAudioCard().ToAttachment());
                    break;

            }

            // Send the card(s) to the user as an attachment to the activity
            await dc.Context.SendActivityAsync(reply, cancellationToken);

            // Give the user instructions about what to do next
            await dc.Context.SendActivityAsync("Type anything to see another card.", cancellationToken: cancellationToken);

            return await dc.EndAsync(cancellationToken: cancellationToken);
        }
        private static HeroCard GetHeroCard()
        {
            var heroCard = new HeroCard
            {
                Title = "BotFramework Hero Card",
                Subtitle = "Your bots — wherever your users are talking",
                Text = "Build and connect intelligent bots to interact with your users naturally wherever they are, from text/sms to Skype, Slack, Office 365 mail and other popular services.",
                Images = new List<CardImage> { new CardImage("https://sec.ch9.ms/ch9/7ff5/e07cfef0-aa3b-40bb-9baa-7c9ef8ff7ff5/buildreactionbotframework_960.jpg") },
                Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Get Started", value: "https://docs.microsoft.com/bot-framework") }
            };

            return heroCard;
        }
        private static ThumbnailCard GetThumbnailCard()
        {
            var heroCard = new ThumbnailCard
            {
                Title = "BotFramework Thumbnail Card",
                Subtitle = "Your bots — wherever your users are talking",
                Text = "Build and connect intelligent bots to interact with your users naturally wherever they are, from text/sms to Skype, Slack, Office 365 mail and other popular services.",
                Images = new List<CardImage> { new CardImage("https://sec.ch9.ms/ch9/7ff5/e07cfef0-aa3b-40bb-9baa-7c9ef8ff7ff5/buildreactionbotframework_960.jpg") },
                Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Get Started", value: "https://docs.microsoft.com/bot-framework") }
            };

            return heroCard;
        }

        private static ReceiptCard GetReceiptCard()
        {
            var receiptCard = new ReceiptCard
            {
                Title = "John Doe",
                Facts = new List<Fact> { new Fact("Order Number", "1234"), new Fact("Payment Method", "VISA 5555-****") },
                Items = new List<ReceiptItem>
                {
                    new ReceiptItem("Data Transfer", price: "$ 38.45", quantity: "368", image: new CardImage(url: "https://github.com/amido/azure-vector-icons/raw/master/renders/traffic-manager.png")),
                    new ReceiptItem("App Service", price: "$ 45.00", quantity: "720", image: new CardImage(url: "https://github.com/amido/azure-vector-icons/raw/master/renders/cloud-service.png")),
                },
                Tax = "$ 7.50",
                Total = "$ 90.95",
                Buttons = new List<CardAction>
                {
                    new CardAction(
                        ActionTypes.OpenUrl,
                        "More information",
                        "https://account.windowsazure.com/content/6.10.1.38-.8225.160809-1618/aux-pre/images/offer-icon-freetrial.png",
                        "https://azure.microsoft.com/en-us/pricing/")
                }
            };

            return receiptCard;
        }

        private static SigninCard GetSigninCard()
        {
            var signinCard = new SigninCard
            {
                Text = "BotFramework Sign-in Card",
                Buttons = new List<CardAction> { new CardAction(ActionTypes.Signin, "Sign-in", value: "https://login.microsoftonline.com/") }
            };

            return signinCard;
        }

        private static AnimationCard GetAnimationCard()
        {
            var animationCard = new AnimationCard
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
            };

            return animationCard;
        }
        private static VideoCard GetVideoCard()
        {
            var videoCard = new VideoCard
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
            };

            return videoCard;
        }

        private static AudioCard GetAudioCard()
        {
            var audioCard = new AudioCard
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
            };

            return audioCard;
        }
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Using_Cards
{
    /// <summary>
    /// This bot will respond to the user's input with rich card content.
    /// </summary>
    public class CardsBot : IBot
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CardsBot"/> class.
        /// In the constructor for the bot we are instantiating our dialog set, giving our field a value,
        /// and adding our waterfall and prompts to the dialog set.
        /// </summary>
        /// <param name="accessors">State accessors for the bot.</param>
        public CardsBot(CardsBotAccessors accessors)
        {
            this.Accessors = accessors ?? throw new ArgumentNullException(nameof(accessors));
            this.Dialogs = new DialogSet(this.Accessors.ConversationDialogState);
            this.Dialogs.Add(new WaterfallDialog("cardSelector", new WaterfallStep[] {ChoiceCardStepAsync, ShowCardStepAsync}));
            this.Dialogs.Add(new ChoicePrompt("cardPrompt"));
        }

        private CardsBotAccessors Accessors { get; }

        private DialogSet Dialogs { get; }

        /// <summary>
        /// This controls what happens when an activity gets sent to the bot.
        /// </summary>
        /// <param name="turnContext">Provides the context for the turn of the bot.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A task.</returns>
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (turnContext == null)
            {
                throw new ArgumentNullException(nameof(turnContext));
            }

            switch (turnContext.Activity.Type)
            {
                case ActivityTypes.Message:
                    var dc = await this.Dialogs.CreateContextAsync(turnContext, cancellationToken);
                    await dc.ContinueAsync(cancellationToken);

                    if (!dc.Context.Responded)
                    {
                        await dc.BeginAsync("cardSelector", cancellationToken: cancellationToken);
                    }

                    break;
                case ActivityTypes.ConversationUpdate:
                    // Send a welcome message to the user and tell them what actions they need to perform to use this bot
                    if (turnContext.Activity.MembersAdded.Any())
                    {
                        foreach (var member in turnContext.Activity.MembersAdded)
                        {
                            if (member.Id != turnContext.Activity.Recipient.Id)
                            {
                                await turnContext.SendActivityAsync(
                                    $"Welcome to CardBot {member.Name}. " +
                                    $"This bot will show you different types of Rich Cards.  " +
                                    $"Please type anything to get started.",
                                    cancellationToken: cancellationToken);
                            }
                        }
                    }

                    break;
            }
        }

        private static async Task<DialogTurnResult> ChoiceCardStepAsync(DialogContext dc, WaterfallStepContext step, CancellationToken cancellationToken)
        {
            return await dc.PromptAsync("cardPrompt", GenerateOptions(dc.Context.Activity), cancellationToken);
        }

        private static PromptOptions GenerateOptions(Activity activity)
        {
            // Create options for the prompt
            var options = new PromptOptions()
            {
                Prompt = activity.CreateReply("What card would you like to see? You can click or type the card name"),
                Choices = new List<Choice>(),
            };

            // Add the choices for the prompt.
            options.Choices.Add(new Choice() { Value = "Animation card" });
            options.Choices.Add(new Choice() { Value = "Audio card" });
            options.Choices.Add(new Choice() { Value = "Hero card" });
            options.Choices.Add(new Choice() { Value = "Receipt card" });
            options.Choices.Add(new Choice() { Value = "Signin card" });
            options.Choices.Add(new Choice() { Value = "Thumbnail card" });
            options.Choices.Add(new Choice() { Value = "Video card" });
            options.Choices.Add(new Choice() { Value = "All cards" });

            return options;
        }

        /// <summary>
        /// This method uses the text of the activity to decide which type
        /// of card to resond with and reply with that card to the user.
        /// </summary>
        /// <param name="dc">Provides context for the current dialog.</param>
        /// <param name="step">Provides context for the current waterfall step.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A DialogTurnResult indicating the turn has ended.</returns>
        private static async Task<DialogTurnResult> ShowCardStepAsync(DialogContext dc, WaterfallStepContext step, CancellationToken cancellationToken)
        {
            // Get the text from the activity to use to show the correct card
            var text = dc.Context.Activity.Text.ToLowerInvariant().Split(' ')[0];

            // Replay to the activity we received with an activity
            // .
            var reply = dc.Context.Activity.CreateReply();

            // Cards are sent as Attackments in the Bot Framework.
            // So we need to create a list of attachments on the activity.
            reply.Attachments = new List<Attachment>();

            // Decide which type of card(s) we are going to show the user
            if (text.StartsWith("hero"))
            {
                // Display a HeroCard.
                reply.Attachments.Add(GetHeroCard().ToAttachment());
            }
            else if (text.StartsWith("thumb"))
            {
                // Display a ThumbnailCard.
                reply.Attachments.Add(GetThumbnailCard().ToAttachment());
            }
            else if (text.StartsWith("receipt"))
            {
                // Display a ReceiptCard.
                reply.Attachments.Add(GetReceiptCard().ToAttachment());
            }
            else if (text.StartsWith("sign"))
            {
                // Display a SignInCard.
                reply.Attachments.Add(GetSigninCard().ToAttachment());
            }
            else if (text.StartsWith("animation"))
            {
                // Display an AnimationCard.
                reply.Attachments.Add(GetAnimationCard().ToAttachment());
            }
            else if (text.StartsWith("video"))
            {
                // Display a VideoCard
                reply.Attachments.Add(GetVideoCard().ToAttachment());
            }
            else if (text.StartsWith("Audio"))
            {
                // Display an AudioCard
                reply.Attachments.Add(GetAudioCard().ToAttachment());
            }
            else
            {
                // Display a carousel of all the rich card types.
                reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                reply.Attachments.Add(GetHeroCard().ToAttachment());
                reply.Attachments.Add(GetThumbnailCard().ToAttachment());
                reply.Attachments.Add(GetReceiptCard().ToAttachment());
                reply.Attachments.Add(GetSigninCard().ToAttachment());
                reply.Attachments.Add(GetAnimationCard().ToAttachment());
                reply.Attachments.Add(GetVideoCard().ToAttachment());
                reply.Attachments.Add(GetAudioCard().ToAttachment());
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
                Subtitle = "Microsoft Bot Framework",
                Text = "Build and connect intelligent bots to interact with your users naturally wherever they are," +
                       " from text/sms to Skype, Slack, Office 365 mail and other popular services.",
                Images = new List<CardImage> { new CardImage("https://sec.ch9.ms/ch9/7ff5/e07cfef0-aa3b-40bb-9baa-7c9ef8ff7ff5/buildreactionbotframework_960.jpg") },
                Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Get Started", value: "https://docs.microsoft.com/bot-framework") },
            };

            return heroCard;
        }

        private static ThumbnailCard GetThumbnailCard()
        {
            var heroCard = new ThumbnailCard
            {
                Title = "BotFramework Thumbnail Card",
                Subtitle = "Microsoft Bot Framework",
                Text = "Build and connect intelligent bots to interact with your users naturally wherever they are," +
                       " from text/sms to Skype, Slack, Office 365 mail and other popular services.",
                Images = new List<CardImage> { new CardImage("https://sec.ch9.ms/ch9/7ff5/e07cfef0-aa3b-40bb-9baa-7c9ef8ff7ff5/buildreactionbotframework_960.jpg") },
                Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Get Started", value: "https://docs.microsoft.com/bot-framework") },
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
                    new ReceiptItem(
                        "Data Transfer",
                        price: "$ 38.45",
                        quantity: "368",
                        image: new CardImage(url: "https://github.com/amido/azure-vector-icons/raw/master/renders/traffic-manager.png")),
                    new ReceiptItem(
                        "App Service",
                        price: "$ 45.00",
                        quantity: "720",
                        image: new CardImage(url: "https://github.com/amido/azure-vector-icons/raw/master/renders/cloud-service.png")),
                },
                Tax = "$ 7.50",
                Total = "$ 90.95",
                Buttons = new List<CardAction>
                {
                    new CardAction(
                        ActionTypes.OpenUrl,
                        "More information",
                        "https://account.windowsazure.com/content/6.10.1.38-.8225.160809-1618/aux-pre/images/offer-icon-freetrial.png",
                        "https://azure.microsoft.com/en-us/pricing/"),
                },
            };

            return receiptCard;
        }

        private static SigninCard GetSigninCard()
        {
            var signinCard = new SigninCard
            {
                Text = "BotFramework Sign-in Card",
                Buttons = new List<CardAction> { new CardAction(ActionTypes.Signin, "Sign-in", value: "https://login.microsoftonline.com/") },
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
                    Url = "https://docs.microsoft.com/en-us/bot-framework/media/how-it-works/architecture-resize.png",
                },
                Media = new List<MediaUrl>
                {
                    new MediaUrl()
                    {
                        Url = "http://i.giphy.com/Ki55RUbOV5njy.gif",
                    },
                },
            };

            return animationCard;
        }

        private static VideoCard GetVideoCard()
        {
            var videoCard = new VideoCard
            {
                Title = "Big Buck Bunny",
                Subtitle = "by the Blender Institute",
                Text = "Big Buck Bunny (code-named Peach) is a short computer-animated comedy film by the Blender Institute," +
                       " part of the Blender Foundation. Like the foundation's previous film Elephants Dream," +
                       " the film was made using Blender, a free software application for animation made by the same foundation." +
                       " It was released as an open-source film under Creative Commons License Attribution 3.0.",
                Image = new ThumbnailUrl
                {
                    Url = "https://upload.wikimedia.org/wikipedia/commons/thumb/c/c5/Big_buck_bunny_poster_big.jpg/220px-Big_buck_bunny_poster_big.jpg",
                },
                Media = new List<MediaUrl>
                {
                    new MediaUrl()
                    {
                        Url = "http://download.blender.org/peach/bigbuckbunny_movies/BigBuckBunny_320x180.mp4",
                    },
                },
                Buttons = new List<CardAction>
                {
                    new CardAction()
                    {
                        Title = "Learn More",
                        Type = ActionTypes.OpenUrl,
                        Value = "https://peach.blender.org/",
                    },
                },
            };

            return videoCard;
        }

        private static AudioCard GetAudioCard()
        {
            var audioCard = new AudioCard
            {
                Title = "I am your father",
                Subtitle = "Star Wars: Episode V - The Empire Strikes Back",
                Text = "The Empire Strikes Back (also known as Star Wars: Episode V – The Empire Strikes Back)" +
                       " is a 1980 American epic space opera film directed by Irvin Kershner. Leigh Brackett and" +
                       " Lawrence Kasdan wrote the screenplay, with George Lucas writing the film's story and serving" +
                       " as executive producer. The second installment in the original Star Wars trilogy, it was produced" +
                       " by Gary Kurtz for Lucasfilm Ltd. and stars Mark Hamill, Harrison Ford, Carrie Fisher, Billy Dee Williams," +
                       " Anthony Daniels, David Prowse, Kenny Baker, Peter Mayhew and Frank Oz.",
                Image = new ThumbnailUrl
                {
                    Url = "https://upload.wikimedia.org/wikipedia/en/3/3c/SW_-_Empire_Strikes_Back.jpg",
                },
                Media = new List<MediaUrl>
                {
                    new MediaUrl()
                    {
                        Url = "http://www.wavlist.com/movies/004/father.wav",
                    },
                },
                Buttons = new List<CardAction>
                {
                    new CardAction()
                    {
                        Title = "Read More",
                        Type = ActionTypes.OpenUrl,
                        Value = "https://en.wikipedia.org/wiki/The_Empire_Strikes_Back",
                    },
                },
            };

            return audioCard;
        }
    }
}

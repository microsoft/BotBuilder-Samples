using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;

namespace Bot.Dialogs
{
    [Serializable]
    public sealed class HelpDialog : IDialog<IMessageActivity>
    {
        public async Task StartAsync(IDialogContext context)
        {
            var text = context.UserData.Get<string>(BotConstants.HelpTypeKey);
            await OnHandleHelpMessage(context, text);
        }

        private static async Task OnMessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> activity)
        {
            var message = await activity;
            var text = message.Text.Replace(BotConstants.BotMention, string.Empty);
            await OnHandleHelpMessage(context, text, message);
        }

        private static async Task OnHandleHelpMessage(IDialogContext context, string text,
            IMessageActivity message = null)
        {
            HeroCard card = null;
            var helpString = string.Empty;
            switch (text)
            {
                case BotConstants.HelpNote:
                    helpString = BotConstants.NoteHelpText;
                    break;
                case BotConstants.HelpShow:
                    helpString = BotConstants.ShowHelpText;
                    break;
                case BotConstants.HelpDelete:
                    helpString = BotConstants.DeleteHelpText;
                    break;
                case BotConstants.HelpExport:
                    helpString = BotConstants.ExportHelpText;
                    break;
                case BotConstants.Help:
                    var isGenericHelp = context.UserData.Get<bool>(BotConstants.IsGenericHelpKey);
                    card = OnGetHelpCard(isGenericHelp);
                    card.Buttons = new List<CardAction>
                    {
                        new CardAction(ActionTypes.ImBack, BotConstants.Note, value: BotConstants.HelpNote),
                        new CardAction(ActionTypes.ImBack, BotConstants.Show, value: BotConstants.HelpShow),
                        new CardAction(ActionTypes.ImBack, BotConstants.Delete, value: BotConstants.HelpDelete),
                        new CardAction(ActionTypes.ImBack, BotConstants.Export, value: BotConstants.HelpExport)
                    };
                    break;
                default:
                    context.Done(message);
                    return;
            }

            if (string.IsNullOrEmpty(helpString))
            {
                await OnSendMessage(context, card.ToAttachment());
                context.Wait(OnMessageReceivedAsync);
            }
            else
            {
                await context.PostAsync(helpString);
                context.Done<IMessageActivity>(null);
            }
        }

        private static async Task OnSendMessage(IBotToUser context, Attachment attachment)
        {
            var message = context.MakeMessage();
            message.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            message.Attachments = new List<Attachment> {attachment};
            await context.PostAsync(message);
        }

        private static HeroCard OnGetHelpCard(bool isGeneric)
        {
            return new HeroCard
            {
                Title = isGeneric ? "Help" : "Sorry, I didn’t get that.",
                Text = "Select a command to learn how to use it."
            };
        }
    }
}
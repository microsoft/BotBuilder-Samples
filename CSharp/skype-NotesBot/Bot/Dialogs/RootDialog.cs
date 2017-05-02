using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Bot.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using MongoDB.Driver;

namespace Bot.Dialogs
{
    [Serializable]
    public sealed class RootDialog : IDialog
    {
        [NonSerialized]
        private IMongoDatabase _database;

        private readonly List<string> _commandWords = new List<string>
        {
            BotConstants.Note,
            BotConstants.Show,
            BotConstants.Delete,
            BotConstants.DeleteForce,
            BotConstants.Export,
            BotConstants.ExportEmail,
            BotConstants.Help
        };

        private readonly List<string> _helpCommandWords = new List<string>
        {
            BotConstants.Help,
            BotConstants.HelpNote,
            BotConstants.HelpShow,
            BotConstants.HelpDelete,
            BotConstants.HelpExport
        };

        public RootDialog()
        {
            // No action
        }

        public RootDialog(IMongoDatabase dB)
        {
            _database = dB;
        }

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(OnMessageReceivedAsync);
        }

        private async Task OnMessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> activity)
        {
            var message = await activity;
            if (string.IsNullOrEmpty(message.Text))
            {
                return;
            }

            if (_database == null)
            {
                _database = new MongoClient(BotConstants.DbString).GetDatabase(BotConstants.DbName);
            }

            var fullString = message.Text.Replace(BotConstants.BotMention, string.Empty);
            var individualWords = fullString.Split(' ')
                .Where(word => !string.IsNullOrEmpty(word))
                .Select(word => word.Replace(" ", string.Empty))
                .ToList();
            var isCommand = OnCheckIsStringValidCommand(individualWords.First());

            var canWait = true;
            if (isCommand)
            {
                switch (individualWords.First().ToLower())
                {
                    case BotConstants.Note:
                        await OnAddNoteCommand(context, fullString);
                        break;
                    case BotConstants.Show:
                        await OnShowCommand(context, individualWords);
                        break;
                    case BotConstants.Delete:
                    case BotConstants.DeleteForce:
                        if (individualWords.Count == 2 && BotConstants.DeleteForce.Contains(individualWords[1]))
                        {
                            await OnDeleteForceCommand(context);
                        }
                        else
                        {
                            await OnDeleteCommand(context);
                        }
                        break;
                    case BotConstants.Export:
                    case BotConstants.ExportEmail:
                        if (individualWords.Count >= 2 && BotConstants.ExportEmail.Contains(individualWords[1]))
                        {
                            canWait = await OnExportEmailCommand(context);
                        }
                        else
                        {
                            await OnExportCommand(context);
                        }
                        break;
                    case BotConstants.Help:
                        var isValidHelpCommand = OnCheckIsStringValidHelpCommand(individualWords);
                        context.UserData.SetValue(BotConstants.EmailInputCountKey, -1);
                        var helpType = isValidHelpCommand ? fullString.ToLower().Trim() : BotConstants.Help;
                        context.UserData.SetValue(BotConstants.HelpTypeKey, helpType);
                        context.UserData.SetValue(BotConstants.IsGenericHelpKey, isValidHelpCommand);
                        context.Call(new HelpDialog(), OnResumeAfterHelpDialog);
                        canWait = false;
                        break;
                }
            }
            else
            {
                context.UserData.SetValue(BotConstants.HelpTypeKey, BotConstants.Help);
                context.UserData.SetValue(BotConstants.IsGenericHelpKey, false);
                context.Call(new HelpDialog(), OnResumeAfterHelpDialog);
                canWait = false;
            }

            if (canWait)
            {
                context.Wait(OnMessageReceivedAsync);
            }
        }

        private bool OnCheckIsStringValidCommand(string firstString)
        {
            if (string.IsNullOrEmpty(firstString))
            {
                return false;
            }

            var match = Regex.Match(string.Join(" ", _commandWords),
                @"\b" +
                Regex.Escape(firstString.ToLower().Trim()) +
                @"\b", RegexOptions.IgnoreCase);
            return match.Success;
        }

        private bool OnCheckIsStringValidHelpCommand(IReadOnlyCollection<string> allWords)
        {
            if (allWords == null || allWords.Count == 0 || allWords.Count > 2)
            {
                return false;
            }

            var match = Regex.Match(string.Join(" ,", _helpCommandWords),
                @"\b" +
                Regex.Escape(string.Join(" ", allWords).Trim()) +
                @"\b", RegexOptions.IgnoreCase);
            return match.Success;
        }

        private async Task OnAddNoteCommand(IBotContext context, string noteContent)
        {
            var userId = context.Activity.From.Id;
            var ticks = DateTime.UtcNow.Ticks;
            var commandIndex = noteContent.IndexOf(BotConstants.Note, StringComparison.Ordinal);
            var noteAlone = commandIndex < 0
                ? noteContent
                : noteContent.Remove(commandIndex, BotConstants.Note.Length);
            var tags = OnGetAllTagsFromNoteContent(noteAlone);
            var combinedHashTags = string.Join(", ", tags);
            var note = new Note
            {
                DocumentId = userId + "_" + ticks,
                NoteOwnerId = userId,
                TimestampTicks = ticks,
                NoteContent = noteAlone,
                HashTagsUsed = tags
            };

            var userCollection = _database.GetCollection<User>(BotConstants.AllUsers);
            var userFilter = Builders<User>.Filter.Where(x => x.UserId == userId);
            var userUpdate = Builders<User>.Update.Push(x => x.SavedNotes, note);
            var options = new FindOneAndUpdateOptions<User> {ReturnDocument = ReturnDocument.After};
            var user = await userCollection.FindOneAndUpdateAsync(userFilter, userUpdate, options);
            foreach (var tag in tags)
            {
                var index = user.TaggedNotes.FindIndex(taggedNote => taggedNote.TagValue == tag);
                if (index >= 0)
                {
                    userFilter = Builders<User>.Filter.And(
                        Builders<User>.Filter.Where(x => x.UserId == userId),
                        Builders<User>.Filter.ElemMatch(x => x.TaggedNotes, y => y.TagValue == tag));
                    userUpdate = Builders<User>.Update.Push(x => x.TaggedNotes[index].NoteIds, note.DocumentId);
                    user = await userCollection.FindOneAndUpdateAsync(userFilter, userUpdate,
                        options);
                }
                else
                {
                    var hashTaggedNotes = new TaggedNote
                    {
                        NoteIds = new List<string> {note.DocumentId},
                        TagValue = tag
                    };
                    userFilter = Builders<User>.Filter.Where(x => x.UserId == userId);
                    userUpdate = Builders<User>.Update.Push(x => x.TaggedNotes, hashTaggedNotes);
                    user = await userCollection.FindOneAndUpdateAsync(userFilter, userUpdate, options);
                }
            }
            var card = OnGetNoteAddedSuccessfullyCard(user, combinedHashTags);
            await OnSendMessage(context, card.ToAttachment());
        }

        private static List<string> OnGetAllTagsFromNoteContent(string noteAlone)
        {
            var regex = new Regex(@"(?<=#)\w+");
            var matches = regex.Matches(noteAlone);
            return (from Match match in matches select match.Value).ToList().Distinct().ToList();
        }

        private async Task OnShowCommand(IBotContext context, IEnumerable<string> allWords)
        {
            var searchStrings = allWords.ToList();
            searchStrings.RemoveAt(0);
            var wordCount = searchStrings.Count;
            var user = await OnGetUser(context.Activity.From.Id);
            string reply;
            switch (wordCount)
            {
                case 0:
                    reply = OnGetNotesForShowAll(user);
                    break;
                case 1:
                    reply = searchStrings[0].StartsWith("#")
                        ? OnGetNotesWithTag(user, searchStrings[0].Replace(" ", "").Replace("#", ""))
                        : OnGetNotesWithStrings(user, searchStrings);
                    break;
                default:
                    reply = OnGetNotesWithStrings(user, searchStrings);
                    break;
            }

            await context.PostAsync(reply);
        }

        private static string OnGetNotesForShowAll(User user)
        {
            var totalNotes = user.SavedNotes.Count;
            var title = $"You have **{totalNotes} notes**:";
            var primaryStringBuilder = new StringBuilder(title);
            primaryStringBuilder.Append(new[] {'\n', '\n'});
            foreach (var note in user.SavedNotes)
            {
                var timeOffset = new DateTime(note.TimestampTicks);
                var dateTime = timeOffset.ToString("MMM dd, yyyy hh:mm tt") + " (UTC time)";
                var tempStringBuilder = new StringBuilder(string.Empty);
                tempStringBuilder.Append($"**{dateTime}.**");
                tempStringBuilder.Append(new[] {'\n', '\n'});
                tempStringBuilder.Append(note.NoteContent);
                tempStringBuilder.Append(new[] {'\n', '\n'});
                primaryStringBuilder.Append(tempStringBuilder);
            }

            return primaryStringBuilder.ToString();
        }

        private static string OnGetNotesWithTag(User user, string tag)
        {
            var totalNotes = user.SavedNotes.Count;
            var tagIndex = user.TaggedNotes.FindIndex(x => x.TagValue == tag);
            var noteIds = tagIndex >= 0 ? user.TaggedNotes[tagIndex].NoteIds : null;
            if (noteIds == null)
            {
                return $"You have **{totalNotes} notes**:\r\n\n" +
                       $"None of your **{totalNotes} notes** contain **{tag}**.\r\n\n" +
                       "Please try a different search word.";
            }

            var title = $"**{noteIds.Count} notes** of your {totalNotes} notes contain the string **{tag}**:";
            var primaryStringBuilder = new StringBuilder(title);
            primaryStringBuilder.Append(new[] {'\n', '\n'});
            var tempStringBuilder = new StringBuilder(string.Empty);
            foreach (var noteId in noteIds)
            {
                var note = user.SavedNotes.First(x => x.DocumentId == noteId);
                var timeOffset = new DateTime(note.TimestampTicks);
                var dateTime = timeOffset.ToString("MMM dd, yyyy hh:mm tt") + " (UTC time)";
                tempStringBuilder.Append($"**{dateTime}.**");
                tempStringBuilder.Append(new[] {'\n', '\n'});
                tempStringBuilder.Append(note.NoteContent);
                tempStringBuilder.Append(new[] {'\n', '\n'});
            }

            return primaryStringBuilder.Append(tempStringBuilder.ToString().Replace(tag, $"**{tag}**")).ToString();
        }

        private static string OnGetNotesWithStrings(User user, IReadOnlyCollection<string> searchStrings)
        {
            var totalNotes = user.SavedNotes.Count;
            var combinedTag = string.Join(" ,", searchStrings);
            var primaryStringBuilder = new StringBuilder();
            primaryStringBuilder.Append(new[] {'\n', '\n'});
            var tempStringBuilder = new StringBuilder(string.Empty);
            var matchingNotesCount = 0;
            foreach (var note
                in from note
                in user.SavedNotes
                let containsAny = searchStrings.Any(note.NoteContent.Contains)
                where containsAny
                select note)
            {
                matchingNotesCount++;
                var timeOffset = new DateTime(note.TimestampTicks);
                var dateTime = timeOffset.ToString("MMM dd, yyyy hh:mm tt") + " (UTC time)";
                tempStringBuilder.Append($"**{dateTime}.**");
                tempStringBuilder.Append(new[] {'\n', '\n'});
                tempStringBuilder.Append(note.NoteContent);
                tempStringBuilder.Append(new[] {'\n', '\n'});
            }

            var prependText = matchingNotesCount == 0
                ? $"You have **{totalNotes} notes**:" +
                  $"None of your **{totalNotes} notes** contain **{combinedTag}**." +
                  "Please try a different search word."
                : $"**{matchingNotesCount} notes** of your {totalNotes} notes contain the string **{combinedTag}**:";
            primaryStringBuilder.Insert(0, prependText);

            var reply = tempStringBuilder.ToString();
            return
                primaryStringBuilder.Append(searchStrings.Aggregate(reply,
                        (current, searchString) => current.Replace(searchString, $"**{searchString}**")))
                    .ToString();
        }

        private async Task OnDeleteCommand(IBotContext context)
        {
            var user = await OnGetUser(context.Activity.From.Id);
            if (user.SavedNotes.Count > 0)
            {
                var card = OnGetDeleteCard(user.SavedNotes.Count);
                await OnSendMessage(context, card.ToAttachment());
            }
            else
            {
                await context.PostAsync(BotConstants.NoNotesToDelete);
            }
        }

        private async Task OnExportCommand(IBotContext context)
        {
            var user = await OnGetUser(context.Activity.From.Id);
            if (user.SavedNotes.Count > 0)
            {
                var card = OnGetExportNotesCard(user);
                await OnSendMessage(context, card.ToAttachment());
            }
            else
            {
                await context.PostAsync(BotConstants.NoNotesToExport);
            }
        }

        private async Task<bool> OnExportEmailCommand(IDialogContext context)
        {
            var canWait = true;
            var user = await OnGetUser(context.Activity.From.Id);
            if (user.SavedNotes.Count > 0)
            {
                var notesAsString = OnGetNotesForShowAll(user);
                context.UserData.SetValue(BotConstants.EmailInputCountKey, 0);
                context.UserData.SetValue(BotConstants.UserKey, user);
                context.UserData.SetValue(BotConstants.AllNotesAsString, notesAsString);
                context.Call(new EmailDialog(), OnResumeAfterEmailDialog);
                canWait = false;
            }
            else
            {
                await context.PostAsync(BotConstants.NoNotesToExport);
            }

            return canWait;
        }

        private static async Task OnSendMessage(IBotToUser context, Attachment attachment)
        {
            var message = context.MakeMessage();
            message.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            message.Attachments = new List<Attachment> {attachment};
            await context.PostAsync(message);
        }

        private async Task<User> OnGetUser(string userId)
        {
            User user = null;
            var collection = _database.GetCollection<User>(BotConstants.AllUsers);
            var filter = Builders<User>.Filter.Where(x => x.UserId == userId);
            var userCursor = await collection.FindAsync(filter);
            while (await userCursor.MoveNextAsync())
            {
                var listOfUsers = userCursor.Current;
                var users = listOfUsers as IList<User> ?? listOfUsers.ToList();
                user = !users.Any() ? null : users.First();
            }

            return user;
        }

        private async Task OnDeleteForceCommand(IBotContext context)
        {
            var userId = context.Activity.From.Id;
            var user = await OnGetUser(userId);

            if (user.SavedNotes.Count > 0)
            {
                await context.PostAsync(
                    $"All of your **{user.SavedNotes.Count} notes** have been **permanently deleted**.");
                var userCollection = _database.GetCollection<User>(BotConstants.AllUsers);
                var userFilter = Builders<User>.Filter.Where(x => x.UserId == userId);
                var userUpdate = Builders<User>.Update.Set(x => x.SavedNotes, new List<Note>(0));
                await userCollection.FindOneAndUpdateAsync(userFilter, userUpdate);
            }
            else
            {
                await context.PostAsync(BotConstants.NoNotesToDelete);
            }
        }

        private async Task OnResumeAfterHelpDialog(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            if (message != null)
            {
                await OnMessageReceivedAsync(context, result);
            }
        }

        private async Task OnResumeAfterEmailDialog(IDialogContext context, IAwaitable<bool> result)
        {
            var isSuccess = await result;
            context.Wait(OnMessageReceivedAsync);
        }

        private static HeroCard OnGetNoteAddedSuccessfullyCard(User user, string tag = "")
        {
            return new HeroCard
            {
                Title = "Note added successfully!",
                Subtitle = tag == "" ? "No tags found" : $"Found tags: {tag}",
                Text = user.SavedNotes.Count == 1
                    ? $"You have {user.SavedNotes.Count} note."
                    : $"You have {user.SavedNotes.Count} notes.",
                Buttons = new List<CardAction>
                {
                    new CardAction(ActionTypes.ImBack, "Show notes", value: "show"),
                    new CardAction(ActionTypes.ImBack, "Export notes", value: "export"),
                    new CardAction(ActionTypes.ImBack, "Delete notes", value: "delete")
                }
            };
        }

        private static HeroCard OnGetDeleteCard(int totalNotes)
        {
            return new HeroCard
            {
                Title = "Delete all notes?",
                Text = $"This will permanently delete all of your {totalNotes} notes. Are you sure?",
                Buttons = new List<CardAction>
                {
                    new CardAction(ActionTypes.ImBack, "Yes, delete all", value: "delete force"),
                    new CardAction(ActionTypes.ImBack, "No, show all", value: "show")
                }
            };
        }

        private static HeroCard OnGetExportNotesCard(User user)
        {
            return new HeroCard
            {
                Title = $"You have {user.SavedNotes.Count} notes.",
                Text = "Please choose the format in which you'd like to receive them.",
                Buttons = new List<CardAction>
                {
                    new CardAction(ActionTypes.ImBack, "Email", value: BotConstants.ExportEmail)
                }
            };
        }
    }
}
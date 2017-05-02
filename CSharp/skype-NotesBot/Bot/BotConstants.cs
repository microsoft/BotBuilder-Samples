namespace Bot
{
    public static class BotConstants
    {
        //
        //  IMPORTANT: KEY CONSTANTS SECTION
        //
        //  This section contains all constants that you need to fill,
        //  before you deploy your bot to production.
        //
        public const string BotName = "<>";
        public const string BotId = "<>";
        public const string DbString = "<>";
        public const string WelcomeCardLink = "https://<>/Resources/welcome_card.png";
        public const string BotEmailId = "<>";
        public const string BotEmailPwd = "<>";
        //
        //  END OF KEY CONSTANTS SECTION
        //

        public const string BotMention = "@Notes ";
        public const string DbName = "notes_db";
        public const string AllUsers = "all_users";
        public const string HelpTypeKey = "HelpType";
        public const string UserKey = "user";
        public const string AllNotesAsString = "all notes as string";
        public const string IsGenericHelpKey = "IsGenericHelp";
        public const string EmailInputCountKey = "EmailInputCount";
        public const string Note = "note";
        public const string Show = "show";
        public const string Delete = "delete";
        public const string DeleteForce = "delete force";
        public const string Export = "export";
        public const string ExportEmail = "export email";
        public const string Help = "help";
        public const string HelpNote = "help note";
        public const string HelpShow = "help show";
        public const string HelpDelete = "help delete";
        public const string HelpExport = "help export";
        public const string O365SmtpUrl = "smtp.office365.com";
        public const string EmailSubject = "Here are your notes!";
        public const string NoNotesToDelete = "You have no notes to delete.";
        public const string NoNotesToExport = "You have no notes to export.";
        public const string AskEmailAddress = "Which email address do you want to send this to?";

        public const string NoteHelpText = "Use the **note** command to capture a simple note. For example..." +
                                           "\n\n*note call mom tomorrow*" +
                                           "\n\n*note send the project proposal*";

        public const string ShowHelpText = "Use the **show** command to display all of your notes. " +
                                           "You can also display only notes that contain specific words. For example..." +
                                           "\n\n*show*" +
                                           "\n\n*show call mom*" +
                                           "\n\n*show project*";

        public const string DeleteHelpText = "Use the **delete** command to delete all your notes. " +
                                             "You will be prompted before final deletion.";

        public const string ExportHelpText =
            "Use the **export** command to get your notes back as an email.";
    }
}
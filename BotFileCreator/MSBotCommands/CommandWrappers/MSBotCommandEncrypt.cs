namespace BotFileCreator
{
    public class MSBotCommandEncrypt : MSBotCommandWrapper
    {
        public MSBotCommandEncrypt(MSBotCommandManager commandManager)
        {
            this.commandManager = commandManager;
        }

        public override string GetMSBotCommand()
        {
            return string.Concat(base.GetMSBotCommand(), " --secret --new");
        }
    }
}

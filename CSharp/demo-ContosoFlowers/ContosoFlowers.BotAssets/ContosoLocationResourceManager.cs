namespace ContosoFlowers.BotAssets
{
    using System;
    using Microsoft.Bot.Builder.Location;
    using Properties;

    [Serializable]
    public class ContosoLocationResourceManager : LocationResourceManager
    {
        public override string ConfirmationAsk => Resources.Location_ConfirmationAsk;
    }
}

using Microsoft.Bot.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WelcomeUser.State
{
    /// <summary>
    /// This class holds set of accessors (to specific properties) that the bot uses to access
    /// specific data. These are created as singleton via DI
    /// </summary>
    public class WelcomeUserStateAccessors
    {
        public IStatePropertyAccessor<Boolean>  DidBotWelcomedUser { get; set; }

        // public SemaphoreSlim SemaphoreSlim { get; } = new SemaphoreSlim(1, 1);
    }
}

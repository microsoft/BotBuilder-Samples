using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;

namespace TaskModuleFactorySample.TeamsChannels.Invoke
{
    public abstract class TeamsInvokeHandler<T>
    {
        public abstract Task<T> OnTeamsTaskModuleFetchAsync(ITurnContext context, CancellationToken cancellationToken);

        public abstract Task<T> OnTeamsTaskModuleSubmitAsync(ITurnContext context, CancellationToken cancellationToken);
    }
}

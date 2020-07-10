// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace TaskModuleFactorySample.TeamsChannels.Invoke
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Schema;
    using Microsoft.Bot.Schema.Teams;

    /// <summary>
    /// Interface for handling Fetch/Submit TaskModule
    /// </summary>
    public interface ITeamsTaskModuleHandler<T> : ITeamsFetchActivityHandler<T>, ITeamsSubmitActivityHandler<T>
    {
    }

    public interface ITeamsFetchActivityHandler<T>
    {
        Task<T> OnTeamsTaskModuleFetchAsync(ITurnContext context, CancellationToken cancellationToken);
    }

    public interface ITeamsSubmitActivityHandler<T>
    {
        Task<T> OnTeamsTaskModuleSubmitAsync(ITurnContext context, CancellationToken cancellationToken);
    }
}

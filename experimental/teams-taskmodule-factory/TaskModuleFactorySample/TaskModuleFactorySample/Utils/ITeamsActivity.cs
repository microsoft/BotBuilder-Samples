using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ITSMSkill.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace TaskModuleFactorySample.Utils
{
    /// <summary>
    /// Interface for Teams specific activity
    /// </summary>
    public interface ITeamsActivity<T>
    {
        Task<ResourceResponse> UpdateTaskModuleActivityAsync(
            ITurnContext context,
            ActivityReference activityReference,
            T updateWithValue,
            CancellationToken cancellationToken);
    }
}

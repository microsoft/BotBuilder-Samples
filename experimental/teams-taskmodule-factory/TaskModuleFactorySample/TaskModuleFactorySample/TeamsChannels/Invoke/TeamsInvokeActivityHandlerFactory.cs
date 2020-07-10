// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TaskModuleFactorySample.Extensions;
using TaskModuleFactorySample.Extensions.Teams.TaskModule;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;

namespace TaskModuleFactorySample.TeamsChannels.Invoke
{
    public abstract class TeamsInvokeActivityHandlerFactory
    {
        protected IDictionary<string, Func<ITeamsTaskModuleHandler<TaskModuleContinueResponse>>> TaskModuleFetchSubmitMap { get; set; }
            = new Dictionary<string, Func<ITeamsTaskModuleHandler<TaskModuleContinueResponse>>>();

        /// <summary>
        /// Router for getting Invoke Handler.
        /// </summary>
        /// <returns>TaskResponse</returns>
        public async Task<TaskModuleContinueResponse> HandleTaskModuleActivity(ITurnContext context, CancellationToken cancellationToken)
        {
            if (context.Activity.IsTaskModuleFetchActivity() || context.Activity.IsExtensionActionActivity())
            {
                return await this.GetTaskModuleFetch(context, cancellationToken);
            }

            if (context.Activity.IsTaskModuleSubmitActivity() || context.Activity.IsExtensionActionActivity())
            {
                return await this.GetTaskModuleSubmit(context, cancellationToken);
            }

            return null;
        }

        protected virtual async Task<TaskModuleContinueResponse> GetTaskModuleFetch(ITurnContext context, CancellationToken cancellationToken)
        {
            ITeamsTaskModuleHandler<TaskModuleContinueResponse> taskModuleHandler = this.GetTaskModuleFetchSubmitHandler(context.Activity);
            return await taskModuleHandler.OnTeamsTaskModuleFetchAsync(context, cancellationToken);
        }

        protected virtual async Task<TaskModuleContinueResponse> GetTaskModuleSubmit(ITurnContext context, CancellationToken cancellationToken)
        {
            ITeamsTaskModuleHandler<TaskModuleContinueResponse> taskModuleHandler = this.GetTaskModuleFetchSubmitHandler(context.Activity);
            return await taskModuleHandler.OnTeamsTaskModuleSubmitAsync(context, cancellationToken);
        }

        protected ITeamsTaskModuleHandler<TaskModuleContinueResponse> GetTaskModuleFetchSubmitHandler(Activity activity) =>
            this.GetTaskModuleFetchSubmitHandlerMap(activity.GetTaskModuleMetadata<TaskModuleMetadata>().TaskModuleFlowType);

        /// <summary>
        /// Gets Teams task module handler by registered name.
        /// </summary>
        /// <param name="handlerName">Handler name.</param>
        /// <returns>Message extension handler.</returns>
        /// <exception cref="NotImplementedException">Message Extension flow type undefined for handler.</exception>
        protected ITeamsTaskModuleHandler<TaskModuleContinueResponse> GetTaskModuleFetchSubmitHandlerMap(string handlerName) =>
                this.TaskModuleFetchSubmitMap.TryGetValue(handlerName, out Func<ITeamsTaskModuleHandler<TaskModuleContinueResponse>> handlerFactory)
                    ? handlerFactory()
                    : throw new NotImplementedException($"TaskModule flow type undefined for handler {handlerName}");
    }
}

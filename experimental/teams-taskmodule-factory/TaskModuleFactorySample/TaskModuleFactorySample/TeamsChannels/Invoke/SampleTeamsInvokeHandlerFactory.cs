// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace TaskModuleFactorySample.TeamsChannels.Invoke
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Bot.Schema.Teams;
    using TaskModuleFactorySample.Dialogs.Teams.TicketTaskModule;
    using TaskModuleFactorySample.Extensions.Teams;

    /// <summary>
    /// ITSMTeamsInvokeActivityhandler Factory Class for TaskModules
    /// </summary>
    public class SampleTeamsInvokeHandlerFactory : TeamsInvokeActivityHandlerFactory
    {
        public SampleTeamsInvokeHandlerFactory(IServiceProvider serviceProvider)
        {
            this.TaskModuleFetchSubmitMap = new Dictionary<string, Func<ITeamsTaskModuleHandler<TaskModuleContinueResponse>>>
            {
                {
                    $"{TeamsFlowType.CreateSample_Form}",
                    () => new CreateFormTeamsTaskModuleHandler(serviceProvider)
                },
                {
                    $"{TeamsFlowType.UpdateSample_Form}",
                    () => new UpdateFormTeamsTaskModuleHandler(serviceProvider)
                },
            };
        }
    }
}

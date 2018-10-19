// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Configuration;
using Microsoft.Bot.Schema;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// For each interaction from the user, an instance of this class is called.
    /// This is a Transient lifetime service.  Transient lifetime services are created
    /// each time they're requested. For each Activity received, a new instance of this
    /// class is created. Objects that are expensive to construct, or have a lifetime
    /// beyond the single Turn, should be carefully managed.
    /// </summary>
    public class ProactiveBot : IBot
    {
        public const string JobCompleteEventName = "jobComplete";

        public const string WelcomeText = "Type 'run' or 'run job' to start a new job.\r\n" +
                                          "Type 'show' or 'show jobs' to display the job log.\r\n" +
                                          "Type 'done <jobNumber>' to complete a job.";

        private readonly JobState _jobState;
        private readonly IStatePropertyAccessor<JobLog> _jobLogPropertyAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProactiveBot"/> class.
        /// </summary>
        /// <param name="jobState">The state provider that is independent of user or conversation.</param>
        /// <param name="endpointService">The <see cref="EndpointService"/> portion of the <see cref="BotConfiguration"/>.</param>
        /// <seealso cref="https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-2.1#windows-eventlog-provider"/>
        public ProactiveBot(JobState jobState, EndpointService endpointService)
        {
            _jobState = jobState ?? throw new ArgumentNullException(nameof(jobState));
            _jobLogPropertyAccessor = _jobState.CreateProperty<JobLog>(nameof(JobLog));

            // Validate AppId.
            // Note: For local testing, .bot AppId is empty for the Bot Framework Emulator.
            AppId = string.IsNullOrWhiteSpace(endpointService.AppId) ? "1" : endpointService.AppId;
        }

        /// <summary>Gets the bot's app ID.</summary>
        /// <remarks>AppId required to continue a conversation.
        /// See <see cref="BotAdapter.ContinueConversationAsync"/> for more details.</remarks>
        private string AppId { get; }

        /// <summary>
        /// Every conversation turn will call this method.
        /// Proactive messages use existing conversations (turns) with the user to deliver proactive messages.
        /// Proactive messages can be ad-hoc or dialog-based. This is demonstrating ad-hoc, which doesn't
        /// have to consider an interruption to an existing conversation, which may upset the dialog flow.
        /// Note: The Email channel may send a proactive message outside the context of a active conversation.
        /// </summary>
        /// <param name="turnContext">A <see cref="ITurnContext"/> containing all the data needed
        /// for processing this conversation turn. </param>
        /// <param name="cancellationToken">(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        /// <remarks>
        /// In the scenario, the bot is being called by users as normal (to start jobs and such) and by some
        /// theoretical service (to signal when jobs are complete). The service is sending activities to the
        /// bot on a separate conversation from the user conversations.</remarks>
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            // See https://aka.ms/about-bot-activity-message to learn more about the message and other activity types.
            if (turnContext.Activity.Type != ActivityTypes.Message)
            {
                // Handle non-message activities.
                await OnSystemActivityAsync(turnContext);
            }
            else
            {
                // Get the job log.
                // The job log is a dictionary of all outstanding jobs in the system.
                JobLog jobLog = await _jobLogPropertyAccessor.GetAsync(turnContext, () => new JobLog());

                // Get the user's text input for the message.
                var text = turnContext.Activity.Text.Trim().ToLowerInvariant();
                switch (text)
                {
                    case "run":
                    case "run job":

                        // Start a virtual job for the user.
                        JobLog.JobData job = CreateJob(turnContext, jobLog);

                        // Set the new property
                        await _jobLogPropertyAccessor.SetAsync(turnContext, jobLog);

                        // Now save it into the JobState
                        await _jobState.SaveChangesAsync(turnContext);

                        await turnContext.SendActivityAsync(
                            $"We're starting job {job.TimeStamp} for you. We'll notify you when it's complete.");

                        break;

                    case "show":
                    case "show jobs":

                        // Display information for all jobs in the log.
                        if (jobLog.Count > 0)
                        {
                            await turnContext.SendActivityAsync(
                                "| Job number &nbsp; | Conversation ID &nbsp; | Completed |<br>" +
                                "| :--- | :---: | :---: |<br>" +
                                string.Join("<br>", jobLog.Values.Select(j =>
                                    $"| {j.TimeStamp} &nbsp; | {j.Conversation.Conversation.Id.Split('|')[0]} &nbsp; | {j.Completed} |")));
                        }
                        else
                        {
                            await turnContext.SendActivityAsync("The job log is empty.");
                        }

                        break;

                    default:
                        // Check whether this is simulating a job completed event.
                        string[] parts = text?.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        if (parts != null && parts.Length == 2
                            && parts[0].Equals("done", StringComparison.InvariantCultureIgnoreCase)
                            && long.TryParse(parts[1], out long jobNumber))
                        {
                            if (!jobLog.TryGetValue(jobNumber, out JobLog.JobData jobInfo))
                            {
                                await turnContext.SendActivityAsync($"The log does not contain a job {jobInfo.TimeStamp}.");
                            }
                            else if (jobInfo.Completed)
                            {
                                await turnContext.SendActivityAsync($"Job {jobInfo.TimeStamp} is already complete.");
                            }
                            else
                            {
                                await turnContext.SendActivityAsync($"Completing job {jobInfo.TimeStamp}.");

                                // Send the proactive message.
                                await CompleteJobAsync(turnContext.Adapter, AppId, jobInfo);
                            }
                        }

                        break;
                }

                if (!turnContext.Responded)
                {
                    await turnContext.SendActivityAsync(WelcomeText);
                }
            }
        }

        /// <summary>
        /// On a conversation update activity sent to the bot, the bot will
        /// send a message to the any new user(s) that were added.
        /// </summary>
        /// <param name="turnContext">Provides the <see cref="ITurnContext"/> for the turn of the bot.</param>
        /// or threads to receive notice of cancellation.</param>
        /// <returns>>A <see cref="Task"/> representing the operation result of the Turn operation.</returns>
        private static async Task SendWelcomeMessageAsync(ITurnContext turnContext)
        {
            foreach (var member in turnContext.Activity.MembersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync($"Welcome to SuggestedActionsBot {member.Name}.\r\n{WelcomeText}");
                }
            }
        }

        // Handles non-message activities.
        private async Task OnSystemActivityAsync(ITurnContext turnContext)
        {
            // On a job completed event, mark the job as complete and notify the user.
            if (turnContext.Activity.Type is ActivityTypes.Event)
            {
                var jobLog = await _jobLogPropertyAccessor.GetAsync(turnContext, () => new JobLog());
                var activity = turnContext.Activity.AsEventActivity();
                if (activity.Name == JobCompleteEventName
                    && activity.Value is long timestamp
                    && jobLog.ContainsKey(timestamp)
                    && !jobLog[timestamp].Completed)
                {
                    await CompleteJobAsync(turnContext.Adapter, AppId, jobLog[timestamp]);
                }
            }
            else if (turnContext.Activity.Type is ActivityTypes.ConversationUpdate)
            {
                if (turnContext.Activity.MembersAdded != null)
                {
                    await SendWelcomeMessageAsync(turnContext);
                }
            }
        }

        // Creates and "starts" a new job.
        private JobLog.JobData CreateJob(ITurnContext turnContext, JobLog jobLog)
        {
            JobLog.JobData jobInfo = new JobLog.JobData
            {
                TimeStamp = DateTime.Now.ToBinary(),
                Conversation = turnContext.Activity.GetConversationReference(),
            };

            jobLog[jobInfo.TimeStamp] = jobInfo;

            return jobInfo;
        }

        // Sends a proactive message to the user.
        private async Task CompleteJobAsync(
            BotAdapter adapter,
            string botId,
            JobLog.JobData jobInfo,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await adapter.ContinueConversationAsync(botId, jobInfo.Conversation, CreateCallback(jobInfo), cancellationToken);
        }

        // Creates the turn logic to use for the proactive message.
        private BotCallbackHandler CreateCallback(JobLog.JobData jobInfo)
        {
            return async (turnContext, token) =>
            {
                // Get the job log from state, and retrieve the job.
                JobLog jobLog = await _jobLogPropertyAccessor.GetAsync(turnContext, () => new JobLog());

                // Perform bookkeeping.
                jobLog[jobInfo.TimeStamp].Completed = true;

                // Set the new property
                await _jobLogPropertyAccessor.SetAsync(turnContext, jobLog);

                // Now save it into the JobState
                await _jobState.SaveChangesAsync(turnContext);

                // Send the user a proactive confirmation message.
                await turnContext.SendActivityAsync($"Job {jobInfo.TimeStamp} is complete.");
            };
        }
    }
    }

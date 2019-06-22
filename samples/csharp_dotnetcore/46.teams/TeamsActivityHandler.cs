// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace Microsoft.BotBuilderSamples
{
    public class TeamsActivityHandler : ActivityHandler
    {
        public override Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (turnContext == null)
            {
                throw new ArgumentNullException(nameof(turnContext));
            }

            if (turnContext.Activity == null)
            {
                throw new ArgumentException($"{nameof(turnContext)} must have non-null Activity.");
            }

            if (turnContext.Activity.Type == null)
            {
                throw new ArgumentException($"{nameof(turnContext)}.Activity must have non-null Type.");
            }

            switch (turnContext.Activity.Type)
            {
                case ActivityTypes.MessageReaction:
                    return OnMessageReactionActivityAsync(new DelegatingTurnContext<IMessageReactionActivity>(turnContext), cancellationToken);

                case ActivityTypes.Invoke:
                    return OnInvokeActivityAsync(new DelegatingTurnContext<IInvokeActivity>(turnContext), cancellationToken);

                default:
                    return base.OnTurnAsync(turnContext, cancellationToken);
            }
        }

        protected virtual async Task OnMessageReactionActivityAsync(ITurnContext<IMessageReactionActivity> turnContext, CancellationToken cancellationToken)
        {
            if (turnContext.Activity.ReactionsAdded != null)
            {
                await OnReactionsAddedAsync(turnContext.Activity.ReactionsAdded, turnContext, cancellationToken);
            }
            if (turnContext.Activity.ReactionsRemoved != null)
            {
                await OnReactionsRemovedAsync(turnContext.Activity.ReactionsRemoved, turnContext, cancellationToken);
            }
        }

        protected virtual Task OnReactionsAddedAsync(IList<MessageReaction> messageReactions, ITurnContext<IMessageReactionActivity> turnContext, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
        protected virtual Task OnReactionsRemovedAsync(IList<MessageReaction> messageReactions, ITurnContext<IMessageReactionActivity> turnContext, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected virtual Task OnInvokeActivityAsync(ITurnContext<IInvokeActivity> turnContext, CancellationToken cancellationToken)
        {
            switch (turnContext.Activity.Name)
            {
                case "signin/verifyState":
                    return OnSigninVerifyStateAsync(turnContext, cancellationToken);

                default:
                    return Task.CompletedTask;
            }
        }

        protected virtual Task OnSigninVerifyStateAsync(ITurnContext<IInvokeActivity> turnContext, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        //TODO: REMOVE THE FOLLOWING CODE. (In 4.5 this class will be protected in ActivityHandler and so available to use here in this class.)

        /// <summary>
        /// A TurnContext with a strongly typed Activity property that wraps an untyped inner TurnContext.
        /// </summary>
        /// <typeparam name="T">An IActivity derived type, that is one of IMessageActivity, IConversationUpdateActivity etc.</typeparam>
        private class DelegatingTurnContext<T> : ITurnContext<T>
            where T : IActivity
        {
            private ITurnContext _innerTurnContext;

            /// <summary>
            /// Initializes a new instance of the <see cref="DelegatingTurnContext{T}"/> class.
            /// </summary>
            /// <param name="innerTurnContext">The inner turn context.</param>
            public DelegatingTurnContext(ITurnContext innerTurnContext)
            {
                _innerTurnContext = innerTurnContext;
            }

            /// <summary>
            /// Gets the inner  context's activity, cast to the type parameter of this <see cref="DelegatingTurnContext{T}"/>.
            /// </summary>
            /// <value>The inner context's activity.</value>
            T ITurnContext<T>.Activity => (T)(IActivity)_innerTurnContext.Activity;

            public BotAdapter Adapter => _innerTurnContext.Adapter;

            public TurnContextStateCollection TurnState => _innerTurnContext.TurnState;

            public Activity Activity => _innerTurnContext.Activity;

            public bool Responded => _innerTurnContext.Responded;

            public Task DeleteActivityAsync(string activityId, CancellationToken cancellationToken = default(CancellationToken))
                => _innerTurnContext.DeleteActivityAsync(activityId, cancellationToken);

            public Task DeleteActivityAsync(ConversationReference conversationReference, CancellationToken cancellationToken = default(CancellationToken))
                => _innerTurnContext.DeleteActivityAsync(conversationReference, cancellationToken);

            public ITurnContext OnDeleteActivity(DeleteActivityHandler handler)
                => _innerTurnContext.OnDeleteActivity(handler);

            public ITurnContext OnSendActivities(SendActivitiesHandler handler)
                => _innerTurnContext.OnSendActivities(handler);

            public ITurnContext OnUpdateActivity(UpdateActivityHandler handler)
                => _innerTurnContext.OnUpdateActivity(handler);

            public Task<ResourceResponse[]> SendActivitiesAsync(IActivity[] activities, CancellationToken cancellationToken = default(CancellationToken))
                => _innerTurnContext.SendActivitiesAsync(activities, cancellationToken);

            public Task<ResourceResponse> SendActivityAsync(string textReplyToSend, string speak = null, string inputHint = InputHints.AcceptingInput, CancellationToken cancellationToken = default(CancellationToken))
                => _innerTurnContext.SendActivityAsync(textReplyToSend, speak, inputHint, cancellationToken);

            public Task<ResourceResponse> SendActivityAsync(IActivity activity, CancellationToken cancellationToken = default(CancellationToken))
                => _innerTurnContext.SendActivityAsync(activity, cancellationToken);

            public Task<ResourceResponse> UpdateActivityAsync(IActivity activity, CancellationToken cancellationToken = default(CancellationToken))
                => _innerTurnContext.UpdateActivityAsync(activity, cancellationToken);
        }
    }
}

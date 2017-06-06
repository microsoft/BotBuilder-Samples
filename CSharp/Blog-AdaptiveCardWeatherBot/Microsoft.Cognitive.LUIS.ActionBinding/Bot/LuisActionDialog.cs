namespace Microsoft.Cognitive.LUIS.ActionBinding.Bot
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Internals.Fibers;
    using Microsoft.Bot.Builder.Luis;
    using Microsoft.Bot.Connector;

    public delegate Task LuisActionHandler(IDialogContext context, object actionResult);

    public delegate Task LuisActionActivityHandler(IDialogContext context, IAwaitable<IMessageActivity> message, object actionResult);

    [Serializable]
    public class LuisActionDialog<TResult> : LuisDialog<TResult>
    {
        private readonly LuisActionResolver actionResolver;

        private readonly Action<ILuisAction, object> onContextCreation;

        public LuisActionDialog(IEnumerable<Assembly> assemblies, params ILuisService[] services) : this(assemblies, null, services)
        {
        }

        public LuisActionDialog(IEnumerable<Assembly> assemblies, Action<ILuisAction, object> onContextCreation, params ILuisService[] services) : base(services)
        {
            if (assemblies == null)
            {
                throw new ArgumentNullException(nameof(assemblies));
            }

            this.onContextCreation = onContextCreation;

            this.actionResolver = new LuisActionResolver(assemblies.ToArray());
        }

        protected override async Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> item)
        {
            var message = await item;
            var messageText = await GetLuisQueryTextAsync(context, message);

            var tasks = this.services.Select(s => s.QueryAsync(messageText, context.CancellationToken)).ToArray();
            var results = await Task.WhenAll(tasks);

            var winners = from result in results.Select((value, index) => new { value, index })
                          let resultWinner = this.BestIntentFrom(result.value)
                          where resultWinner != null
                          select new LuisServiceResult(result.value, resultWinner, this.services[result.index]);

            var winner = this.BestResultFrom(winners);

            if (winner == null)
            {
                throw new InvalidOperationException("No winning intent selected from Luis results.");
            }

            var intentName = default(string);
            var luisAction = this.actionResolver.ResolveActionFromLuisIntent(winner.Result, out intentName);
            if (luisAction != null)
            {
                var executionContextChain = new List<ActionExecutionContext> { new ActionExecutionContext(intentName, luisAction) };
                while (LuisActionResolver.IsContextualAction(luisAction))
                {
                    var luisActionDefinition = default(LuisActionBindingAttribute);
                    if (!LuisActionResolver.CanStartWithNoContextAction(luisAction, out luisActionDefinition))
                    {
                        await context.PostAsync($"Cannot start contextual action '{luisActionDefinition.FriendlyName}' without a valid context.");

                        return;
                    }

                    luisAction = LuisActionResolver.BuildContextForContextualAction(luisAction, out intentName);
                    if (luisAction != null)
                    {
                        this.onContextCreation?.Invoke(luisAction, context);

                        executionContextChain.Insert(0, new ActionExecutionContext(intentName, luisAction));
                    }
                }

                var validationResults = default(ICollection<ValidationResult>);
                if (!luisAction.IsValid(out validationResults))
                {
                    var childDialog = new LuisActionMissingEntitiesDialog(winner.LuisService, executionContextChain);

                    context.Call(childDialog, this.LuisActionMissingDialogFinished);
                }
                else
                {
                    await this.DispatchToLuisActionActivityHandler(context, item, intentName, luisAction);
                }
            }
        }

        protected virtual IDictionary<string, LuisActionActivityHandler> GetActionHandlersByIntent()
        {
            return LuisActionDialogHelper.EnumerateHandlers(this).ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        protected virtual async Task<object> PerformActionFulfillment(IDialogContext context, IAwaitable<IMessageActivity> item, ILuisAction luisAction)
        {
            return await luisAction.FulfillAsync();
        }

        protected virtual async Task DispatchToLuisActionActivityHandler(IDialogContext context, IAwaitable<IMessageActivity> item, string intentName, ILuisAction luisAction)
        {
            var actionHandlerByIntent = new Dictionary<string, LuisActionActivityHandler>(this.GetActionHandlersByIntent());

            var handler = default(LuisActionActivityHandler);
            if (!actionHandlerByIntent.TryGetValue(intentName, out handler))
            {
                handler = actionHandlerByIntent[string.Empty];
            }

            if (handler != null)
            {
                await handler(context, item, await this.PerformActionFulfillment(context, item, luisAction));
            }
            else
            {
                throw new Exception($"No default intent handler found.");
            }
        }

        protected virtual async Task LuisActionMissingDialogFinished(IDialogContext context, IAwaitable<ActionExecutionContext> executionContext)
        {
            var messageActivity = (IMessageActivity)context.Activity;

            var executionContextResult = await executionContext;
            await this.DispatchToLuisActionActivityHandler(context, Awaitable.FromItem(messageActivity), executionContextResult.Intent, executionContextResult.Action);
        }

        internal static class LuisActionDialogHelper
        {
            public static IEnumerable<KeyValuePair<string, LuisActionActivityHandler>> EnumerateHandlers(object dialog)
            {
                var type = dialog.GetType();
                var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                foreach (var method in methods)
                {
                    var intents = method.GetCustomAttributes<LuisIntentAttribute>(inherit: true).ToArray();
                    LuisActionActivityHandler intentHandler = null;

                    try
                    {
                        intentHandler = (LuisActionActivityHandler)Delegate.CreateDelegate(typeof(LuisActionActivityHandler), dialog, method, throwOnBindFailure: false);
                    }
                    catch (ArgumentException)
                    {
                        // "Cannot bind to the target method because its signature or security transparency is not compatible with that of the delegate type."
                        // https://github.com/Microsoft/BotBuilder/issues/634
                        // https://github.com/Microsoft/BotBuilder/issues/435
                    }

                    // fall back for compatibility
                    if (intentHandler == null)
                    {
                        try
                        {
                            var handler = (LuisActionHandler)Delegate.CreateDelegate(typeof(LuisActionHandler), dialog, method, throwOnBindFailure: false);

                            if (handler != null)
                            {
                                // thunk from new to old delegate type
                                intentHandler = (context, message, result) => handler(context, result);
                            }
                        }
                        catch (ArgumentException)
                        {
                            // "Cannot bind to the target method because its signature or security transparency is not compatible with that of the delegate type."
                            // https://github.com/Microsoft/BotBuilder/issues/634
                            // https://github.com/Microsoft/BotBuilder/issues/435
                        }
                    }

                    if (intentHandler != null)
                    {
                        var intentNames = intents.Select(i => i.IntentName).DefaultIfEmpty(method.Name);

                        foreach (var intentName in intentNames)
                        {
                            var key = string.IsNullOrWhiteSpace(intentName) ? string.Empty : intentName;
                            yield return new KeyValuePair<string, LuisActionActivityHandler>(intentName, intentHandler);
                        }
                    }
                    else
                    {
                        if (intents.Length > 0)
                        {
                            var msg = $"Handler '{method.Name}' signature is not valid for the following intent/s: {string.Join(";", intents.Select(i => i.IntentName))}";
                            throw new InvalidIntentHandlerException(msg, method);
                        }
                    }
                }
            }
        }

        [Serializable]
        internal class LuisActionMissingEntitiesDialog : IDialog<ActionExecutionContext>
        {
            private readonly ILuisService luisService;

            private string intentName;

            private ILuisAction luisAction;

            private IList<ActionExecutionContext> executionContextChain;

            private QueryValueResult overrunData;

            public LuisActionMissingEntitiesDialog(ILuisService luisService, IList<ActionExecutionContext> executionContextChain)
            {
                if (executionContextChain == null || executionContextChain.Count == 0)
                {
                    throw new ArgumentException("Action chain cannot be null or empty.", nameof(executionContextChain));
                }

                var executionContext = executionContextChain.First();

                SetField.NotNull(out this.luisService, nameof(luisService), luisService);
                SetField.NotNull(out this.intentName, nameof(this.intentName), executionContext.Intent);
                SetField.NotNull(out this.luisAction, nameof(this.luisAction), executionContext.Action);

                executionContextChain.RemoveAt(0);
                if (executionContextChain.Count > 0)
                {
                    this.executionContextChain = executionContextChain;
                }
            }

            public virtual async Task StartAsync(IDialogContext context)
            {
                if (this.executionContextChain != null)
                {
                    var childDialog = new LuisActionMissingEntitiesDialog(this.luisService, this.executionContextChain);

                    // clean executionContextChain - avoid serialization payload
                    this.executionContextChain = null;

                    context.Call(childDialog, this.AfterContextualActionFinished);

                    return;
                }

                await this.MessageReceivedAsync(context, null);
            }

            protected virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> item)
            {
                var nextPromptIdx = 0;

                var validationResults = default(ICollection<ValidationResult>);
                this.luisAction.IsValid(out validationResults);

                if (item != null)
                {
                    var message = await item;

                    var paramName = validationResults.First().MemberNames.First();
                    var paramValue = message.Text;

                    var result = await LuisActionResolver.QueryValueFromLuisAsync(this.luisService, this.luisAction, paramName, paramValue, context.CancellationToken);

                    if (result.Succeed)
                    {
                        nextPromptIdx++;
                    }
                    else if (!string.IsNullOrWhiteSpace(result.NewIntent) && result.NewAction != null)
                    {
                        var currentActionDefinition = LuisActionResolver.GetActionDefinition(this.luisAction);

                        var isContextual = false;
                        if (LuisActionResolver.IsValidContextualAction(result.NewAction, this.luisAction, out isContextual))
                        {
                            var executionContextChain = new List<ActionExecutionContext> { new ActionExecutionContext(result.NewIntent, result.NewAction) };

                            var childDialog = new LuisActionMissingEntitiesDialog(this.luisService, executionContextChain);

                            context.Call(childDialog, this.AfterContextualActionFinished);

                            return;
                        }
                        else if (isContextual & !LuisActionResolver.IsContextualAction(this.luisAction))
                        {
                            var newActionDefinition = LuisActionResolver.GetActionDefinition(result.NewAction);

                            await context.PostAsync($"Cannot execute action '{newActionDefinition.FriendlyName}' in the context of '{currentActionDefinition.FriendlyName}' - continuing with current action");
                        }
                        else if (!this.luisAction.GetType().Equals(result.NewAction.GetType()))
                        {
                            var newActionDefinition = LuisActionResolver.GetActionDefinition(result.NewAction);

                            var valid = LuisActionResolver.UpdateIfValidContextualAction(result.NewAction, this.luisAction, out isContextual);
                            if (!valid && isContextual)
                            {
                                await context.PostAsync($"Cannot switch to action '{newActionDefinition.FriendlyName}' from '{currentActionDefinition.FriendlyName}' due to invalid context - continuing with current action");
                            }
                            else if (currentActionDefinition.ConfirmOnSwitchingContext)
                            {
                                // serialize overrun info
                                this.overrunData = result;

                                PromptDialog.Confirm(
                                    context,
                                    this.AfterOverrunCurrentActionSelected,
                                    $"Do you want to discard the current action '{currentActionDefinition.FriendlyName}' and start executing '{newActionDefinition.FriendlyName}' action?");

                                return;
                            }
                            else
                            {
                                this.intentName = result.NewIntent;
                                this.luisAction = result.NewAction;

                                this.luisAction.IsValid(out validationResults);
                            }
                        }
                    }
                }

                if (validationResults.Count > nextPromptIdx)
                {
                    await context.PostAsync(validationResults.ElementAt(nextPromptIdx).ErrorMessage);
                    context.Wait(this.MessageReceivedAsync);
                }
                else
                {
                    context.Done(new ActionExecutionContext(this.intentName, this.luisAction));
                }
            }

            private async Task AfterOverrunCurrentActionSelected(IDialogContext context, IAwaitable<bool> result)
            {
                if (await result == true)
                {
                    // if switching from contextual to other root
                    if (LuisActionResolver.IsContextualAction(this.luisAction) && !LuisActionResolver.IsContextualAction(this.overrunData.NewAction))
                    {
                        context.Done(new ActionExecutionContext(this.overrunData.NewIntent, this.overrunData.NewAction) { ChangeRootSignaling = true });

                        return;
                    }

                    this.intentName = this.overrunData.NewIntent;
                    this.luisAction = this.overrunData.NewAction;
                }

                // clean overrunData - avoid serialization payload
                this.overrunData = null;

                await this.MessageReceivedAsync(context, null);
            }

            private async Task AfterContextualActionFinished(IDialogContext context, IAwaitable<ActionExecutionContext> executionContext)
            {
                var executionContextResult = await executionContext;

                if (executionContextResult.ChangeRootSignaling)
                {
                    if (LuisActionResolver.IsContextualAction(this.luisAction))
                    {
                        context.Done(executionContextResult);

                        return;
                    }
                    else
                    {
                        this.intentName = executionContextResult.Intent;
                        this.luisAction = executionContextResult.Action;
                    }
                }
                else
                {
                    var result = await executionContextResult.Action.FulfillAsync();
                    if (result is string)
                    {
                        await context.PostAsync(result.ToString());
                    }
                }

                await this.MessageReceivedAsync(context, null);
            }
        }
    }
}
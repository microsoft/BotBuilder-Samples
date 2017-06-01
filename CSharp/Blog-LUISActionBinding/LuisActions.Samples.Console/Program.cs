namespace LuisActions.Samples.Console
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Configuration;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Luis;
    using Microsoft.Bot.Builder.Luis.Models;
    using Microsoft.Cognitive.LUIS.ActionBinding;
    using Samples;

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Query samples:\n");

            Console.WriteLine("- What is the time in Miami?");
            Console.WriteLine("- Search for 5 stars hotels in Barcelona");
            Console.WriteLine("- Tell me the weather in Buenos Aires");
            Console.WriteLine("- Find airport with code PMV");

            while (true)
            {
                Console.Write("\n> Your Query? ");
                var query = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(query))
                {
                    RunQuery(query).Wait();
                }
            }
        }

        private static async Task<ActionExecutionContext> RunActions(ILuisService luisService, IList<ActionExecutionContext> actions)
        {
            if (actions == null || actions.Count == 0)
            {
                Console.WriteLine(">> ERROR: Action chain cannot be null or empty.");

                return null;
            }

            var actionExecutionContext = actions.First();
            var intentAction = actions.First().Action;

            actions.RemoveAt(0);
            if (actions.Count > 0)
            {
                await RunActions(luisService, actions);
            }

            var validationResults = default(ICollection<ValidationResult>);
            bool isValid = intentAction.IsValid(out validationResults);

            while (!isValid)
            {
                var fieldValidation = validationResults.FirstOrDefault();
                if (fieldValidation != null)
                {
                    var paramName = fieldValidation.MemberNames.First();
                    Console.Write("({0}) {1}: ", paramName, fieldValidation.ErrorMessage);
                    var input = Console.ReadLine();

                    var queryResult = await LuisActionResolver.QueryValueFromLuisAsync(luisService, intentAction, paramName, input, CancellationToken.None);

                    if (!queryResult.Succeed && !string.IsNullOrWhiteSpace(queryResult.NewIntent) && queryResult.NewAction != null)
                    {
                        var newActionDefinition = LuisActionResolver.GetActionDefinition(queryResult.NewAction);
                        var currentActionDefinition = LuisActionResolver.GetActionDefinition(intentAction);

                        var isContextual = false;
                        if (LuisActionResolver.IsValidContextualAction(queryResult.NewAction, intentAction, out isContextual))
                        {
                            var executionContextChain = new List<ActionExecutionContext> { new ActionExecutionContext(queryResult.NewIntent, queryResult.NewAction) };

                            var executionContext = await RunActions(luisService, executionContextChain);
                            if (executionContext.ChangeRootSignaling)
                            {
                                if (LuisActionResolver.IsContextualAction(intentAction))
                                {
                                    return executionContext;
                                }
                                else
                                {
                                    intentAction = executionContext.Action;
                                }
                            }
                        }
                        else if (isContextual && !LuisActionResolver.IsContextualAction(intentAction))
                        {
                            Console.WriteLine($"Cannot execute action '{newActionDefinition.FriendlyName}' in the context of '{currentActionDefinition.FriendlyName}' - continuing with current action");
                        }
                        else if (!intentAction.GetType().Equals(queryResult.NewAction.GetType()))
                        {
                            var valid = LuisActionResolver.UpdateIfValidContextualAction(queryResult.NewAction, intentAction, out isContextual);
                            if (!valid && isContextual)
                            {
                                Console.WriteLine($"Cannot switch to action '{newActionDefinition.FriendlyName}' from '{currentActionDefinition.FriendlyName}' due to invalid context - continuing with current action");
                            }
                            else if (currentActionDefinition.ConfirmOnSwitchingContext)
                            {
                                Console.Write($"You are about to discard the current action '{currentActionDefinition.FriendlyName}' and start executing '{newActionDefinition.FriendlyName}'\nContinue? ");
                                var response = Console.ReadLine();

                                if (response.ToUpperInvariant().StartsWith("Y"))
                                {
                                    if (LuisActionResolver.IsContextualAction(intentAction) && !LuisActionResolver.IsContextualAction(queryResult.NewAction))
                                    {
                                        return new ActionExecutionContext(queryResult.NewIntent, queryResult.NewAction) { ChangeRootSignaling = true };
                                    }

                                    intentAction = queryResult.NewAction;
                                }
                            }
                            else
                            {
                                intentAction = queryResult.NewAction;
                            }
                        }
                    }

                    // re-evaluate
                    isValid = intentAction.IsValid(out validationResults);
                }
            }

            var result = await intentAction.FulfillAsync();

            // We just show the ToString() of the result - not care about the result type here
            Console.WriteLine(result != null ? result.ToString() : "Cannot resolve your query");

            return actionExecutionContext;
        }

        private static async Task RunQuery(string query)
        {
            // Process message
            var luisService = new LuisService(new LuisModelAttribute(ConfigurationManager.AppSettings["LUIS_ModelId"], ConfigurationManager.AppSettings["LUIS_SubscriptionKey"]));
            var luisResult = await luisService.QueryAsync(query, CancellationToken.None);

            // Try to resolve intent to action
            var intentName = default(string);
            var intentEntities = default(IList<EntityRecommendation>);
            var intentAction = new LuisActionResolver(typeof(GetTimeInPlaceAction).Assembly)
                .ResolveActionFromLuisIntent(luisResult, out intentName, out intentEntities);

            if (intentAction != null)
            {
                var executionContextChain = new List<ActionExecutionContext> { new ActionExecutionContext(intentName, intentAction) };
                while (LuisActionResolver.IsContextualAction(intentAction))
                {
                    var luisActionDefinition = default(LuisActionBindingAttribute);
                    if (!LuisActionResolver.CanStartWithNoContextAction(intentAction, out luisActionDefinition))
                    {
                        Console.WriteLine($"Cannot start contextual action '{luisActionDefinition.FriendlyName}' without a valid context.");

                        return;
                    }

                    intentAction = LuisActionResolver.BuildContextForContextualAction(intentAction, out intentName);
                    if (intentAction != null)
                    {
                        executionContextChain.Insert(0, new ActionExecutionContext(intentName, intentAction));
                    }
                }

                await RunActions(luisService, executionContextChain);
            }
            else
            {
                Console.WriteLine("Could not understand the input.");
            }
        }
    }
}
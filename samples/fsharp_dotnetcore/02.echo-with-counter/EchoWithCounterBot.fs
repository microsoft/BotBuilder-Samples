// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
namespace Microsoft.BotBuilderSamples
open System.Threading
open System.Threading.Tasks
open Microsoft.Bot.Builder
open Microsoft.Bot.Schema
open Microsoft.Extensions.Logging
open FSharp.Control.Tasks.V2.ContextInsensitive



/// <summary>
/// Represents a bot that processes incoming activities.
/// For each user interaction, an instance of this class is created and the OnTurnAsync method is called.
/// This is a Transient lifetime service.  Transient lifetime services are created
/// each time they're requested. For each Activity received, a new instance of this
/// class is created. Objects that are expensive to construct, or have a lifetime
/// beyond the single turn, should be carefully managed.
/// For example, the <see cref="MemoryStorage"/> object and associated
/// <see cref="IStatePropertyAccessor{T}"/> object are created with a singleton lifetime.
/// </summary>
/// <seealso cref="https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-2.1"/>
type EchoWithCounterBot (accessors:EchoBotAccessors, loggerFactory:ILoggerFactory) =
    let _logger = loggerFactory.CreateLogger<EchoWithCounterBot>()
    do _logger.LogTrace("EchoBot turn start.")
    interface IBot with
        /// <summary>
        /// Initializes a new instance of the <see cref="EchoWithCounterBot"/> class.
        /// </summary>
        /// <param name="accessors">A class containing <see cref="IStatePropertyAccessor{T}"/> used to manage state.</param>
        /// <param name="loggerFactory">A <see cref="ILoggerFactory"/> that is hooked to the Azure App Service provider.</param>
        /// <seealso cref="https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-2.1#windows-eventlog-provider"/>


        /// <summary>
        /// Every conversation turn for our Echo Bot will call this method.
        /// There are no dialogs used, since it's "single turn" processing, meaning a single
        /// request and response.
        /// </summary>
        /// <param name="turnContext">A <see cref="ITurnContext"/> containing all the data needed
        /// for processing this conversation turn. </param>
        /// <param name="cancellationToken">(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> that represents the work queued to execute.</returns>
        /// <seealso cref="BotStateSet"/>
        /// <seealso cref="ConversationState"/>
        /// <seealso cref="IMiddleware"/>
        member this.OnTurnAsync (turnContext : ITurnContext, cancellationToken : CancellationToken ) =
            // Handle Message activity type, which is the main activity type for shown within a conversational interface
            // Message activities may contain text, speech, interactive cards, and binary or unknown attachments.
            // see https://aka.ms/about-bot-activity-message to learn more about the message and other activity types
            task {
                match turnContext.Activity.Type with
                | ActivityTypes.Message ->
                   
                    // Get the conversation state from the turn context.
                    let! state = accessors.CounterState.GetAsync(turnContext,fun () -> new CounterState()) 
 
                    // Bump the turn count for this conversation.
                    state.TurnCount <- state.TurnCount + 1

                    // Set the property using the accessor.
                    do! accessors.CounterState.SetAsync(turnContext, state) 

                    // Save the new turn count into the conversation state.
                    do! accessors.ConversationState.SaveChangesAsync(turnContext) 

                    // Echo back to the user whatever they typed.
                    let responseMessage = sprintf "Turn %i: You sent '%s'\n" state.TurnCount turnContext.Activity.Text
                    do! turnContext.SendActivityAsync(responseMessage) :> Task
                | _ ->
                    do! turnContext.SendActivityAsync(sprintf "{%s} event detected from F#" turnContext.Activity.Type) :> Task     
            } :> _

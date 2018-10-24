// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
namespace Microsoft.BotBuilderSamples
open Microsoft.Bot.Builder
/// <summary>
/// Stores counter state for the conversation.
/// Stored in <see cref="Microsoft.Bot.Builder.ConversationState"/> and
/// backed by <see cref="Microsoft.Bot.Builder.MemoryStorage"/>.
/// </summary>
type CounterState () =  
    /// <summary>
    /// Gets or sets the number of turns in the conversation.
    /// </summary>
    /// <value>The number of turns in the conversation.</value>
    member val TurnCount : int = 0 with get, set
    


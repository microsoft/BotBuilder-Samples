// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder;

namespace EchoBotWithCounter
{
    /// <summary>
    /// Creates the State Accessors used by the EchoBotWithCounter bot. In general usage, this class
    /// is created as a Singleton and passed into the IBot-derived constructor.
    ///  - See EchoWithCounterBot constructor for how that is injected.
    ///  - See the Startup.cs file for more details on creating the Singleton that gets
    ///    injected into the constructor.
    /// </summary>
    public class EchoBotAccessors
    {
        public static string CounterName { get; } = $"{nameof(EchoBotAccessors)}.CounterState";

        public IStatePropertyAccessor<CounterState> CounterState { get; set; }
    }
}

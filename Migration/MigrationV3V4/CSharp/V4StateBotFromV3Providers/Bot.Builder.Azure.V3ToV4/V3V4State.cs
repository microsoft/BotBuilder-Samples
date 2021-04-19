// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.
using Microsoft.Bot.Builder;
using Newtonsoft.Json;

namespace Bot.Builder.Azure.V3V4
{
    /// <summary>
    /// This class inherits from the Bot Builer SDK V4 <see cref="BotState"/> 
    /// and provides an SDK V3 storage key implementation which can be used by the 
    /// SDK V3 storage provider when creating <see cref="V3V4Storage"/>
    /// Keys are based on <see cref="Address"/> which is serialized and returned from
    /// GetStorageKey.  This Address is then used wtihin <see cref="V3V4Storage"/> to
    /// create the actual storage key.
    /// </summary>
    public class V3V4State : BotState
    {
        //// <summary>
        /// Initializes a new instance of the <see cref="V3V4State"/> class.
        /// </summary>
        /// <param name="storage">The V3V4UserDataStorage storage provider to use.</param>
        public V3V4State(V3V4Storage storage)
            : base(storage, nameof(V3V4State))
        {
        }


        /// <summary>
        /// Gets the key to use when reading and writing state to and from storage.
        /// </summary>
        /// <param name="turnContext">The context object for this turn.</param>
        /// <returns>The storage key.</returns>
        protected override string GetStorageKey(ITurnContext turnContext)
        {
            var address = new Address(turnContext.Activity);
            return JsonConvert.SerializeObject(address);
        }
    }
}

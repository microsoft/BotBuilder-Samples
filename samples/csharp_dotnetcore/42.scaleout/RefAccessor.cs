// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// This is an accessor for any object. By definition objects (as opposed to values)
    /// are returned by reference in the GetAsync call on the accessor. As such the SetAsync
    /// call is never used. The actual act of saving any state to an external store therefore
    /// cannot be encapsulated in the Accessor implementation itself. And so to facilitate this
    /// the state itself is available as a public property on this class. The reason its here is
    /// because the caller of the constructor could pass in null for the state, in which case
    /// the factory provided on the GetAsync call will be used.
    /// </summary>
    /// <typeparam name="T">The type of the object this Accessor Gets.</typeparam>
    public class RefAccessor<T> : IStatePropertyAccessor<T>
        where T : class
    {
        public RefAccessor(T value)
        {
            Value = value;
        }

        public T Value { get; private set; }

        public string Name => nameof(T);

        public Task<T> GetAsync(ITurnContext turnContext, Func<T> defaultValueFactory = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (Value == null)
            {
                if (defaultValueFactory == null)
                {
                    throw new KeyNotFoundException();
                }
                else
                {
                    Value = defaultValueFactory();
                }
            }

            return Task.FromResult(Value);
        }

        #region Not Implemented
        public Task DeleteAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task SetAsync(ITurnContext turnContext, T value, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}

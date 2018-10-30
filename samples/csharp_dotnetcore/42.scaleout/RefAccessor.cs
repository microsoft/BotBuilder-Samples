// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;

namespace ScaleoutBot
{
    public class RefAccessor<T> : IStatePropertyAccessor<T> where T : class
    {
        public RefAccessor(T value)
        {
            Value = value;
        }

        public T Value { get; private set; }

        public string Name => nameof(T);

        public Task DeleteAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

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

        public Task SetAsync(ITurnContext turnContext, T value, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }
    }
}

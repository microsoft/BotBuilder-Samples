// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Microsoft.BotBuilderSamples
{
    public class EntityProperty
    {
        public EntityProperty(string name, string value)
        {
            EntityName = name ?? throw new ArgumentNullException(nameof(name));
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string EntityName { get; }

        public object Value { get; }
    }
}

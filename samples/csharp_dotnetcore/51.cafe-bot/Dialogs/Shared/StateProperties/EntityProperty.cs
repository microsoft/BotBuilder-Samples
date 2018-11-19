// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.BotBuilderSamples
{
    public class EntityProperty
    {
        public EntityProperty(string name, object value)
        {
            EntityName = name;
            Value = value;
        }

        public string EntityName { get; }

        public object Value { get; }
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace TaskModuleFactorySample.TeamsChannels
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public class TeamsInvokeAttribute : Attribute
    {
        public string FlowType { get; set; }
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace ITSMSkill.Models
{
    /// <summary>
    /// Activity Reference for keeping track of ActivityId, ThreadId, Object.
    /// </summary>
    public class ActivityReference
    {
        public string ThreadId { get; set; }

        public string ActivityId { get; set; }

        public object Data { get; set; }

        public Dictionary<string, DateTimeOffset> PropertyUpdatedTimeMap { get; set; } = new Dictionary<string, DateTimeOffset>();
    }
}

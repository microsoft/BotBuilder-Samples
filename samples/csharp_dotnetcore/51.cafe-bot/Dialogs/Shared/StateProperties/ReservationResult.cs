// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace Microsoft.BotBuilderSamples
{
    public enum ReservationStatus
    {
        Success,
        Incomplete,
        Unknown,
    }

#pragma warning disable SA1402 // File may only contain a single type
#pragma warning disable SA1649 // File name should match first type name
    public class ReservationOutcome
#pragma warning restore SA1649 // File name should match first type name
#pragma warning restore SA1402 // File may only contain a single type
    {
        public ReservationOutcome(string message, string entity)
        {
            Message = message;
            Entity = entity;
        }

        public string Entity { get; }

        public string Message { get; }
    }

    public class ReservationResult
    {
        public ReservationResult(ReservationProperty property, ReservationStatus status = ReservationStatus.Unknown, ReservationOutcome outcome = null)
        {
            NewReservation = property ?? throw new ArgumentNullException(nameof(property));
            Status = status;
            Outcome = new List<ReservationOutcome>();
            if (outcome != null)
            {
                Outcome.Add(outcome);
            }
        }

        public ReservationProperty NewReservation { get; }

        public ReservationStatus Status { get; set; }

        public List<ReservationOutcome> Outcome { get; }
    }
}

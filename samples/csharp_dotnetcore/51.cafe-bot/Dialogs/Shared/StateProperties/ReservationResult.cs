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
        Unknown
    }

    public class ReservationOutcome
    {
        public ReservationOutcome(string message, string entity)
        {
            Message = message;
            Entity = entity;
        }

        public string Message { get; }

        private readonly string Entity;
    }

    public class ReservationResult
    {
        /**
         * Constructor.
         *
         * @param {reservationProperty} reservationProperty
         * @param {Enum} status
         * @param {Object []} outcome {message:"", property:""}
         */
        public ReservationResult(ReservationProperty property, ReservationStatus status = ReservationStatus.Unknown, ReservationOutcome outcome = null)
        {
            NewReservation = property ?? throw new ArgumentNullException(nameof(property));
            Status = status;
            Outcome = new List<ReservationOutcome>();
            Outcome.Add(outcome);
        }

        public ReservationProperty NewReservation { get; }

        public ReservationStatus Status { get; set; }

        public List<ReservationOutcome> Outcome { get; }
    }
}

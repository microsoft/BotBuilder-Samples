// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
using System.Security.Claims;
using Microsoft.Bot.Schema;

namespace ImmediateAcceptBot.BackgroundQueue
{
    /// <summary>
    /// Activity with Claims which should already have been authenticated via JwtTokenValidation.AuthenticateRequest.
    /// </summary>
    public class ActivityWithClaims
    {
        /// <summary>
        /// <see cref="ClaimsIdentity"/> retrieved from a call to JwtTokenValidation.AuthenticateRequest.
        /// <seealso cref="ImmediateAcceptAdapter"/>
        /// </summary>
        public ClaimsIdentity ClaimsIdentity { get; set; }

        /// <summary>
        /// <see cref="Activity"/> which is to be processed.
        /// </summary>
        public Activity Activity { get; set; }
    }
}

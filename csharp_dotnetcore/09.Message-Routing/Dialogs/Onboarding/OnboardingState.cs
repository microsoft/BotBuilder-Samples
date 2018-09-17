// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace MessageRoutingBot
{
    /// <summary>
    /// State for the <see cref="OnboardingDialog"/>.
    /// This is the user information collected by the <see cref="OnboardingDialog"/>.
    /// </summary>
    public class OnboardingState
    {
        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the email of the user.
        /// </summary>
        /// <value>
        /// The email of the user.
        /// </value>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the location of the user.
        /// </summary>
        /// <value>
        /// The location of the user.
        /// </value>
        public string Location { get; set; }
    }
}

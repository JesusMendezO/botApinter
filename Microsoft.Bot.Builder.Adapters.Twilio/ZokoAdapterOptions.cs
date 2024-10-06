// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Bot.Builder.Adapters.TwilioWhatsapp
{
    /// <summary>
    /// Options for the <see cref="ZokoAdapter"/>.
    /// </summary>
    public class ZokoAdapterOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether incoming requests should be validated as coming from Twilio.
        /// </summary>
        /// <value>
        /// A value indicating whether incoming requests should be validated as coming from Twilio.
        /// </value>
        public bool ValidateIncomingRequests { get; set; } = true;
    }
}

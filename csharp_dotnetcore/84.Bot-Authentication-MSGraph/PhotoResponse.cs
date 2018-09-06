// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// This is a helper class to pass the photo for the ListMeAsync Method.
    /// </summary>
    public class PhotoResponse
    {
        public byte[] Bytes { get; set; }

        public string ContentType { get; set; }

        public string Base64String { get; set; }
    }
}
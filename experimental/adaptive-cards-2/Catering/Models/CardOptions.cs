// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Catering.Models
{
    public class CardOptions
    {
        public int? nextCardToSend { get; set; }
        public int? currentCard { get; set; }
        public string option { get; set; }
        public string custom { get; set; }
    }
}

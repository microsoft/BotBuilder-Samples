// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.IO;
using AdaptiveCards;
using AdaptiveCards.Templating;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;

namespace Catering.Cards
{
    public class CardResource
    {
        private readonly string _fileName;
        private readonly string _filePath;

        public CardResource(string fileName)
        {
            _fileName = fileName;
            _filePath = Path.Combine(".", "Resources", fileName);
        }

        public string AsJson()
        {
            return File.ReadAllText(_filePath);
        }

        public string AsJson<T>(T data)
        {
            var cardJson = AsJson();
            var cardData = JsonConvert.SerializeObject(data);

            var transformer = new AdaptiveTransformer();
            return transformer.Transform(cardJson, cardData);
        }

        public object AsJObject()
        {
            return JsonConvert.DeserializeObject(AsJson());
        }

        public object AsJObject<T>(T data)
        {
            return JsonConvert.DeserializeObject(AsJson(data));
        }

        public Attachment AsAttachment()
        {
            return new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = JsonConvert.DeserializeObject(AsJson()),
            };
        }
        public Attachment AsAttachment<T>(T data)
        {
            return new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = JsonConvert.DeserializeObject(AsJson(data)),
            };
        }
    }
}

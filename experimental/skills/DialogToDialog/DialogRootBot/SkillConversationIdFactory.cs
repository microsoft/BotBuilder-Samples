// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Skills;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;

namespace Microsoft.BotBuilderSamples.DialogRootBot
{
    /// <summary>
    /// A <see cref="SkillConversationIdFactory"/> that uses <see cref="IStorage"/> to store and retrieve <see cref="ConversationReference"/> instances.
    /// </summary>
    public class SkillConversationIdFactory : SkillConversationIdFactoryBase
    {
        private readonly IStorage _storage;

        public SkillConversationIdFactory(IStorage storage)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
        }

        public override async Task<string> CreateSkillConversationIdAsync(ConversationReference conversationReference, CancellationToken cancellationToken)
        {
            if (conversationReference == null)
            {
                throw new ArgumentNullException(nameof(conversationReference));
            }

            if (string.IsNullOrWhiteSpace(conversationReference.Conversation.Id))
            {
                throw new NullReferenceException($"ConversationId in {nameof(conversationReference)} can't be null.");
            }

            if (string.IsNullOrWhiteSpace(conversationReference.ChannelId))
            {
                throw new NullReferenceException($"ChannelId in {nameof(conversationReference)} can't be null.");
            }

            var storageKey = $"{conversationReference.Conversation.Id}{conversationReference.ChannelId}".GetHashCode().ToString(CultureInfo.InvariantCulture);
            var skillConversationInfo = new Dictionary<string, object> { { storageKey, JObject.FromObject(conversationReference) } };
            await _storage.WriteAsync(skillConversationInfo, cancellationToken).ConfigureAwait(false);

            return storageKey;
        }

        public override async Task<ConversationReference> GetConversationReferenceAsync(string skillConversationId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(skillConversationId))
            {
                throw new ArgumentNullException(nameof(skillConversationId));
            }

            var skillConversationInfo = await _storage.ReadAsync(new[] { skillConversationId }, cancellationToken).ConfigureAwait(false);
            if (skillConversationInfo.Any())
            {
                var conversationInfo = ((JObject)skillConversationInfo[skillConversationId]).ToObject<ConversationReference>();
                return conversationInfo;
            }

            return null;
        }

        public override async Task DeleteConversationReferenceAsync(string skillConversationId, CancellationToken cancellationToken)
        {
            await _storage.DeleteAsync(new[] { skillConversationId }, cancellationToken).ConfigureAwait(false);
        }
    }
}

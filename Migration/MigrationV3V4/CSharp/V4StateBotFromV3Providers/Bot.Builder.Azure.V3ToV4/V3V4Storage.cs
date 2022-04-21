// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Newtonsoft.Json;

namespace Bot.Builder.Azure.V3V4
{
    /// <summary>
    /// This is a Bot Builder SDK V4 <see cref="IStorage"/> implementation
    /// which wraps an SDK V3 storage provider <see cref="IBotDataStore"/> 
    /// This enables SDK V3 UserData to be written and read from within an SDK V4 bot.
    /// </summary>
    public class V3V4Storage : IStorage
    {
        private readonly IBotDataStore<BotData> _v3DataStore;
        private readonly BotStoreType _botStoreType;
        private string _statePropertyName;

        /// <summary>
        /// Constructs a new <see cref="V3V4Storage"/>
        /// </summary>
        /// <param name="v3DataStore">A Bot Builder SDK V3 <see cref="IBotDataStore"/> (required)</param>
        /// <param name="botStoreType">THe Type of Data Storage (UserData, ConversationData, PrivateConversationData)
        /// <see cref="BotStoreType"/>Type of storage</param>
        /// <param name="statePropertyName">This is only relavent in code.  The name of the BotState property,
        /// retrieved and accessed through a property accessor created from an instance of <see cref="V3V4State"/>.</param>
        public V3V4Storage(IBotDataStore<BotData> v3DataStore, 
            BotStoreType botStoreType = BotStoreType.BotUserData,
            string statePropertyName = "V3State")
        {
            _v3DataStore = v3DataStore ?? throw new ArgumentNullException(nameof(v3DataStore));
            _botStoreType = botStoreType;
            _statePropertyName = statePropertyName;
        }

        /// <summary>
        /// Delete records from the underlying storage.
        /// </summary>
        /// <param name="keys">Json serialized array of <see cref="Address"/> objects.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public async Task DeleteAsync(string[] keys, CancellationToken cancellationToken = default(CancellationToken))
        {
            foreach (var key in keys)
            {
                var asAddress = JsonConvert.DeserializeObject<Address>(key);

                // Sending a null data, with an eTag, should ensure the object is deleted
                var botData = new BotData(eTag: "delete", data: null);

                await _v3DataStore.SaveAsync(asAddress, _botStoreType, botData, cancellationToken);
            }
        }

        /// <summary>
        /// Retrieve records from the underlying storage.
        /// </summary>
        /// <param name="keys">Json serialized array of <see cref="Address"/> objects.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>A dictionary of string and object.  The key within the dictionary will be a serialized <see cref="Address"/>
        /// and the object will be a <see cref="BotData"/> containing the data field as a Dictionary.</returns>
        public async Task<IDictionary<string, object>> ReadAsync(string[] keys, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (keys.Length > 1)
            {
                throw new InvalidOperationException("V3V4Storage is only setup for single retrieval");
            }

            var results = new Dictionary<string, object>(1);
            var asAddress = JsonConvert.DeserializeObject<Address>(keys[0]);
            var foundData = await _v3DataStore.LoadAsync(asAddress, _botStoreType, cancellationToken);

            // return the first one found (V3State will not retrieve more than one)
            var inner = new Dictionary<string, object>();
            inner.Add(_statePropertyName, foundData);
            results.Add(keys[0], inner);

            return results;
        }

        /// <summary>
        /// Save records into the underlying storage.
        /// </summary>
        /// <param name="changes">A dictionary of keys and BotData object's representing changes to the data.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public async Task WriteAsync(IDictionary<string, object> changes, CancellationToken cancellationToken = default(CancellationToken))
        {
            foreach (var change in changes)
            {
                var asAddress = JsonConvert.DeserializeObject<Address>(change.Key);
                var firstElement = ((IDictionary<string, object>)change.Value).FirstOrDefault();

                // If the data is null, the record should be removed.  Sending a null data, with an eTag, 
                // should ensure the object is deleted
                var botData = firstElement.Value == null
                            ? new BotData(eTag: "delete", data: null)
                            : firstElement.Value as BotData;
                
                await _v3DataStore.SaveAsync(asAddress, _botStoreType, botData, cancellationToken);
            }
        }
    }

}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace BotFileCreator.Repository
{
    using Microsoft.Bot.Configuration;

    public interface IBotConfigurationRepository
    {
        /// <summary>
        /// Load
        /// </summary>
        /// <param name="file">file</param>
        /// <param name="secret">secret</param>
        void Load(string file, string secret = default(string));

        /// <summary>
        /// Save
        /// </summary>
        /// <param name="secret">secret</param>
        void Save(string secret = default(string));

        /// <summary>
        /// ConnectService
        /// </summary>
        /// <param name="service">service</param>
        void ConnectService(ConnectedService service);

        /// <summary>
        /// DisconnectService
        /// </summary>
        /// <param name="serviceId">serviceId</param>
        void DisconnectService(string serviceId);

        /// <summary>
        /// Encrypt
        /// </summary>
        /// <param name="secret">secret</param>
        void Encrypt(string secret);

        /// <summary>
        /// Decrypt
        /// </summary>
        /// <param name="secret">secret</param>
        void Decrypt(string secret);
    }
}

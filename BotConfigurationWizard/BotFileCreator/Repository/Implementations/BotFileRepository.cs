// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace BotFileCreator.Repository
{
    using System.IO;
    using Microsoft.Bot.Configuration;

    public class BotFileRepository : IBotConfigurationRepository
    {
        private BotConfiguration botConfiguration;
        private string path;
        private string secret;
        private string fileName;

        public BotFileRepository(string fileName, string path, string secret = default(string))
        {
            this.fileName = fileName;
            this.path = path;

            if (!string.IsNullOrWhiteSpace(secret))
            {
                this.secret = secret;
            }

            // Initialize the BotConfiguration
            Initialize();
        }

        /// <summary>
        /// Generates a Key
        /// </summary>
        /// <returns>string</returns>
        public static string GenerateKey()
        {
            return BotConfiguration.GenerateKey();
        }

        /// <summary>
        /// Load
        /// </summary>
        /// <param name="file">file</param>
        /// <param name="secret">secret</param>
        public void Load(string file, string secret = default(string))
        {
            var botConfiguration = BotConfiguration.Load(file, secret);

            if (botConfiguration != null)
            {
                this.botConfiguration = botConfiguration;
            }
        }

        /// <summary>
        /// Save
        /// </summary>
        /// <param name="secret">secret</param>
        public void Save(string secret = default(string))
        {
            if (this.botConfiguration != null)
            {
                #pragma warning disable VSTHRD002 // Avoid problematic synchronous waits
                this.botConfiguration.SaveAsAsync(this.GetFullPath(), secret).GetAwaiter().GetResult();
                #pragma warning restore VSTHRD002 // Avoid problematic synchronous waits
            }
        }

        /// <summary>
        /// ConnectService
        /// </summary>
        /// <param name="service">service</param>
        public void ConnectService(ConnectedService service)
        {
            if (this.botConfiguration != null)
            {
                this.botConfiguration.ConnectService(service);
            }
        }

        /// <summary>
        /// DisconnectService
        /// </summary>
        /// <param name="serviceId">serviceId</param>
        public void DisconnectService(string serviceId)
        {
            if (this.botConfiguration != null)
            {
                this.botConfiguration.DisconnectService(serviceId);
            }
        }

        /// <summary>
        /// Encrypt
        /// </summary>
        /// <param name="secret">secret</param>
        public void Encrypt(string secret)
        {
            if (this.botConfiguration != null)
            {
                this.botConfiguration.Encrypt(secret);
            }
        }

        /// <summary>
        /// Decrypt
        /// </summary>
        /// <param name="secret">secret</param>
        public void Decrypt(string secret)
        {
            if (this.botConfiguration != null)
            {
                this.botConfiguration.Decrypt(secret);
            }
        }

        /// <summary>
        /// Initialize
        /// </summary>
        private void Initialize()
        {
            string fullPath = GetFullPath();

            // If the configuration file exists, load it. If not, create
            // a new instance of BotConfiguration for managing it
            if (File.Exists(fullPath))
            {
                Load(fullPath, this.secret);
            }
            else
            {
                this.botConfiguration = new BotConfiguration();

                // Sets the botConfiguration's name
                SetBotProperties();
            }
        }

        /// <summary>
        /// Sets the Bot's name
        /// </summary>
        private void SetBotProperties()
        {
            if (this.botConfiguration != null)
            {
                this.botConfiguration.Name = this.fileName.Replace(".bot", string.Empty);
                this.botConfiguration.Padlock = string.Empty;
                this.botConfiguration.Description = string.Empty;
            }
        }

        /// <summary>
        /// Get full path
        /// </summary>
        /// <returns>string</returns>
        private string GetFullPath()
        {
            string fullPath = Path.Combine(this.path, this.fileName);

            if (!fullPath.EndsWith(".bot"))
            {
                fullPath = string.Join(string.Empty, fullPath, ".bot");
            }

            return fullPath;
        }
    }
}

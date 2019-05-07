// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json.Linq;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// An implementation of the ETag aware IStore interface against Azure Blob Storage.
    /// </summary>
    public class BlobStore : IStore
    {
        private readonly CloudBlobContainer _container;

        public BlobStore(string accountName, string accountKey, string containerName)
        {
            if (string.IsNullOrWhiteSpace(accountName))
            {
                throw new ArgumentException(nameof(accountName));
            }

            if (string.IsNullOrWhiteSpace(accountKey))
            {
                throw new ArgumentException(nameof(accountKey));
            }

            if (string.IsNullOrWhiteSpace(containerName))
            {
                throw new ArgumentException(nameof(containerName));
            }

            var storageCredentials = new StorageCredentials(accountName, accountKey);
            var cloudStorageAccount = new CloudStorageAccount(storageCredentials, useHttps: true);
            var client = cloudStorageAccount.CreateCloudBlobClient();
            _container = client.GetContainerReference(containerName);
        }

        public async Task<(JObject content, string etag)> LoadAsync(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException(nameof(key));
            }

            var blob = _container.GetBlockBlobReference(key);
            try
            {
                var content = await blob.DownloadTextAsync();
                var obj = JObject.Parse(content);
                var etag = blob.Properties.ETag;
                return (obj, etag);
            }
            catch (StorageException e)
                when (e.RequestInformation.HttpStatusCode == (int)HttpStatusCode.NotFound)
            {
                return (new JObject(), null);
            }
        }

        public async Task<bool> SaveAsync(string key, JObject obj, string etag)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException(nameof(key));
            }

            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            var blob = _container.GetBlockBlobReference(key);
            blob.Properties.ContentType = "application/json";
            var content = obj.ToString();
            if (etag != null)
            {
                try
                {
                    await blob.UploadTextAsync(content, Encoding.UTF8, new AccessCondition {IfMatchETag = etag}, new BlobRequestOptions(), new OperationContext());
                }
                catch (StorageException e)
                    when (e.RequestInformation.HttpStatusCode == (int)HttpStatusCode.PreconditionFailed)
                {
                    return false;
                }
            }
            else
            {
                await blob.UploadTextAsync(content);
            }

            return true;
        }
    }
}

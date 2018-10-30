// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Net;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json.Linq;

namespace ScaleoutBot
{
    public class BlobStore : IStore
    {
        private CloudBlobContainer _container;

        public BlobStore(string myAccountName, string myAccountKey, string containerName)
        {
            var storageCredentials = new StorageCredentials(myAccountName, myAccountKey);
            var cloudStorageAccount = new CloudStorageAccount(storageCredentials, useHttps: true);
            var client = cloudStorageAccount.CreateCloudBlobClient();
            _container = client.GetContainerReference(containerName);
        }

        public async Task<(JObject content, string eTag)> LoadAsync(string key)
        {
            var blob = _container.GetBlockBlobReference(key);
            try
            {
                var content = await blob.DownloadTextAsync();
                var obj = JObject.Parse(content);
                var eTag = blob.Properties.ETag;
                return (obj, eTag);
            }
            catch (StorageException e)
                when (e.RequestInformation.HttpStatusCode == (int)HttpStatusCode.NotFound)
            {
                return (new JObject(), null);
            }
        }

        public async Task<bool> SaveAsync(string key, JObject obj, string eTag)
        {
            var blob = _container.GetBlockBlobReference(key);
            blob.Properties.ContentType = "application/json";
            var content = obj.ToString();
            if (eTag != null)
            {
                try
                {
                    await blob.UploadTextAsync(content, new AccessCondition { IfMatchETag = eTag }, new BlobRequestOptions(), new OperationContext());
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

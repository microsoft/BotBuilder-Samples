// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Model = Catering.Models;

namespace Catering
{
    public class CateringDb : IDisposable
    {
        private string _endpointUri;
        private string _primaryKey;

        // Cosmos client 
        private readonly CosmosClient _cosmosClient;
        private Lazy<Database> _database;
        private Lazy<Container> _container;

        private readonly string DatabaseId = "UserDB";
        private readonly string ContainerId = "Orders";
        private readonly string PartitionKeyValue = "AllUsers";

        public CateringDb(CosmosClient cosmosClient)
        {
            this._cosmosClient = cosmosClient;

            this._database = new Lazy<Database>(() =>
            {
                var task = this._cosmosClient.CreateDatabaseIfNotExistsAsync(DatabaseId);
                task.Wait();
                return task.Result;
            }, isThreadSafe: true);

            this._container = new Lazy<Container>(() =>
            {
                var task = this._database.Value.CreateContainerIfNotExistsAsync(ContainerId, "/partitionKey");
                task.Wait();
                return task.Result;
            }, isThreadSafe: true);
        }

        public IConfiguration Configuration { get; set; }


        
        public async Task<CosmosResult<Model.User>> GetRecentOrdersAsync(string continuationToken = null)
        {
            var sqlQueryText = $"SELECT * FROM c ORDER BY c.lunch.orderTimestamp DESC OFFSET 0 LIMIT 5";

            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            FeedIterator<Model.User> queryResultSetIterator = this._container.Value.GetItemQueryIterator<Model.User>(queryDefinition, continuationToken);

            CosmosResult<Model.User> results = new CosmosResult<Model.User>()
            {
                Items = new List<Model.User>()
            };

            if (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<Model.User> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                results.ContinuationToken = currentResultSet.ContinuationToken;
                foreach (var conversationInfo in currentResultSet)
                {
                    results.Items.Add(conversationInfo);
                }
            }

            return results;
        }

        public async Task<Model.User> UpsertOrderAsync(Model.User user)
        {
            if (user.Id == null)
            {
                user.Id = Guid.NewGuid().ToString("D");
            }

            user.PartitionKey = PartitionKeyValue;
            user.Lunch.OrderTimestamp = DateTime.UtcNow;

            ItemResponse<Model.User> response = await this._container.Value.UpsertItemAsync(user, new PartitionKey(PartitionKeyValue));
            return response.Resource;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _cosmosClient.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~CateringDb()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}

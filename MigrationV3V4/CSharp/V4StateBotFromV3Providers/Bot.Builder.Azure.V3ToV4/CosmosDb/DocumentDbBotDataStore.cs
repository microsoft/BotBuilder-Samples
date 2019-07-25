// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace Bot.Builder.Azure.V3V4.CosmosDb
{
    /// <summary>
    /// <see cref="IBotDataStore{T}"/> Implementation using Azure DocumentDb
    /// From https://github.com/microsoft/BotBuilder-Azure
    /// </summary>
    public class DocumentDbBotDataStore : IBotDataStore<BotData>
    {
        private const string entityKeyParameterName = "@entityKey";

        private static readonly TimeSpan MaxInitTime = TimeSpan.FromSeconds(5);

        private readonly IDocumentClient documentClient;
        private readonly string databaseId;
        private readonly string collectionId;
        private readonly bool enableCrossPartitionQuery;

        /// <summary>
        /// Creates an instance of the <see cref="IBotDataStore{T}"/> that uses the Azure DocumentDb.
        /// </summary>
        /// <param name="documentClient">The DocumentDb client to use.</param>
        /// <param name="databaseId">The name of the DocumentDb database to use.</param>
        /// <param name="collectionId">The name of the DocumentDb collection to use.</param>
        /// <param name="enableCrossPartitionQuery">The query param to execute a cross partition query.</param>
        public DocumentDbBotDataStore(IDocumentClient documentClient, string databaseId = "botdb", string collectionId = "botcollection", bool enableCrossPartitionQuery = false)
        {
            this.databaseId = databaseId ?? throw new ArgumentNullException(nameof(databaseId));
            this.collectionId = collectionId ?? throw new ArgumentNullException(nameof(collectionId));

            this.documentClient = documentClient;
            this.databaseId = databaseId;
            this.collectionId = collectionId;
            this.enableCrossPartitionQuery = enableCrossPartitionQuery;

            CreateDatabaseIfNotExistsAsync().GetAwaiter().GetResult();
            CreateCollectionIfNotExistsAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Creates an instance of the <see cref="IBotDataStore{T}"/> that uses the Azure DocumentDb.
        /// </summary>
        /// <param name="serviceEndpoint">The service endpoint to use to create the client.</param>
        /// <param name="authKey">The authorization key or resource token to use to create the client.</param>
        /// <param name="databaseId">The name of the DocumentDb database to use.</param>
        /// <param name="collectionId">The name of the DocumentDb collection to use.</param>
        /// <param name="enableCrossPartitionQuery">The query param to execute a cross partition query.</param>
        /// <remarks>The service endpoint can be obtained from the Azure Management Portal. If you
        /// are connecting using one of the Master Keys, these can be obtained along with
        /// the endpoint from the Azure Management Portal If however you are connecting as
        /// a specific DocumentDB User, the value passed to authKeyOrResourceToken is the
        /// ResourceToken obtained from the permission feed for the user.
        /// Using Direct connectivity, wherever possible, is recommended.</remarks>
        public DocumentDbBotDataStore(Uri serviceEndpoint, string authKey, string databaseId = "botdb", string collectionId = "botcollection", bool enableCrossPartitionQuery = false)
            : this(new DocumentClient(serviceEndpoint, authKey), databaseId, collectionId, enableCrossPartitionQuery) { }

        async Task<BotData> IBotDataStore<BotData>.LoadAsync(IAddress key, BotStoreType botStoreType,
            CancellationToken cancellationToken)
        {
            try
            {
                var entityKey = DocDbBotDataEntity.GetEntityKey(key, botStoreType);

                // query to retrieve the document if it exists
                SqlQuerySpec querySpec = new SqlQuerySpec(
                                                queryText: $"SELECT * FROM {collectionId} b WHERE (b.id = {entityKeyParameterName})",
                                                parameters: new SqlParameterCollection()
                                                {
                                                    new SqlParameter(entityKeyParameterName, entityKey)
                                                });
                var collectionUri = UriFactory.CreateDocumentCollectionUri(databaseId, collectionId);
                // execute the cross partition query if enableCrossPartitionQuery is true
                var feedOption = new FeedOptions { EnableCrossPartitionQuery = enableCrossPartitionQuery };
                var query = documentClient.CreateDocumentQuery(collectionUri, querySpec, feedOption)
                                          .AsDocumentQuery();
                var feedResponse = await query.ExecuteNextAsync<Document>(CancellationToken.None);
                Document document = feedResponse.FirstOrDefault();

                if (document != null)
                {
                    // The document, of type IDynamicMetaObjectProvider, has a dynamic nature, 
                    // similar to DynamicTableEntity in Azure storage. When casting to a static type, properties that exist in the static type will be 
                    // populated from the dynamic type.
                    DocDbBotDataEntity entity = (dynamic)document;
                    return new BotData(document?.ETag, entity?.Data);
                }
                else
                {
                    // the document does not exist in the database, return an empty BotData object
                    return new BotData(string.Empty, null);
                }
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode.HasValue && e.StatusCode.Value == HttpStatusCode.NotFound)
                {
                    return new BotData(string.Empty, null);
                }

                //throw new HttpException(e.StatusCode.HasValue ? (int)e.StatusCode.Value : 0, e.Message, e);
                throw;
            }
        }

        async Task IBotDataStore<BotData>.SaveAsync(IAddress key, BotStoreType botStoreType, BotData botData,
            CancellationToken cancellationToken)
        {
            try
            {
                var requestOptions = new RequestOptions()
                {
                    AccessCondition = new AccessCondition()
                    {
                        Type = AccessConditionType.IfMatch,
                        Condition = botData.ETag
                    }
                };

                var entity = new DocDbBotDataEntity(key, botStoreType, botData);
                var entityKey = DocDbBotDataEntity.GetEntityKey(key, botStoreType);

                if (string.IsNullOrEmpty(botData.ETag))
                {
                    await documentClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseId, collectionId), entity, requestOptions);
                }
                else if (botData.ETag == "*")
                {
                    if (botData.Data != null)
                    {
                        await documentClient.UpsertDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseId, collectionId), entity, requestOptions);
                    }
                    else
                    {
                        await documentClient.DeleteDocumentAsync(UriFactory.CreateDocumentUri(databaseId, collectionId, entityKey), requestOptions);
                    }
                }
                else
                {
                    if (botData.Data != null)
                    {
                        await documentClient.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(databaseId, collectionId, entityKey), entity, requestOptions);
                    }
                    else
                    {
                        await documentClient.DeleteDocumentAsync(UriFactory.CreateDocumentUri(databaseId, collectionId, entityKey), requestOptions);
                    }
                }
            }
            catch (DocumentClientException e)
            {
                //if (e.StatusCode.HasValue && e.StatusCode.Value == HttpStatusCode.Conflict)
                //{
                //    throw new HttpException((int)HttpStatusCode.PreconditionFailed, e.Message, e);
                //}

                //throw new HttpException(e.StatusCode.HasValue ? (int)e.StatusCode.Value : 0, e.Message, e);

                throw;
            }
        }

        Task<bool> IBotDataStore<BotData>.FlushAsync(IAddress key, CancellationToken cancellationToken)
        {
            // Everything is saved. Flush is no-op
            return Task.FromResult(true);
        }

        private async Task CreateDatabaseIfNotExistsAsync()
        {
            try
            {
                await documentClient.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(databaseId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == HttpStatusCode.NotFound)
                {
                    await documentClient.CreateDatabaseAsync(new Database { Id = databaseId });
                }
                else
                {
                    throw;
                }
            }
        }

        private async Task CreateCollectionIfNotExistsAsync()
        {
            try
            {
                await documentClient.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(databaseId, collectionId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await documentClient.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(databaseId),
                        new DocumentCollection { Id = collectionId });
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
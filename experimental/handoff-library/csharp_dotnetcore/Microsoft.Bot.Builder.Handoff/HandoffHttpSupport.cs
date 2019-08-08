// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Handoff;
using Microsoft.Rest;
using Newtonsoft.Json;

namespace Microsoft.Bot.Builder.Handoff
{
    /// <summary>
    /// Making HTTP calls for handoff. When handoff is integrated
    /// into the SDK proper, this code migrates to Conversations.cs.
    /// </summary>
    public static class HandoffHttpSupport
    {
        public static async Task<HttpOperationResponse<ResourceResponse>> HandoffWithHttpMessagesAsync(IServiceOperations<ConnectorClient> conversations, string conversationId, HandoffParameters handoffParameters, CancellationToken cancellationToken = default)
        {
            if (conversationId == null)
            {
                throw new ArgumentNullException(nameof(conversationId));
            }

            if (handoffParameters == null)
            {
                throw new ArgumentNullException(nameof(handoffParameters));
            }

            // Construct URL
            var baseUrl = conversations.Client.BaseUri.AbsoluteUri;
            if (!baseUrl.EndsWith("/"))
            {
                baseUrl += "/";
            }

            var url = new Uri($"{baseUrl}v3/conversations/{Uri.EscapeDataString(conversationId)}/handoff");

            using (var httpRequest = new HttpRequestMessage())
            {
                httpRequest.Method = HttpMethod.Post;
                httpRequest.RequestUri = url;

                // Serialize Request
                string requestContent = null;
                requestContent = Rest.Serialization.SafeJsonConvert.SerializeObject(handoffParameters, conversations.Client.SerializationSettings);
                httpRequest.Content = new StringContent(requestContent, System.Text.Encoding.UTF8);
                httpRequest.Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json; charset=utf-8");

                cancellationToken.ThrowIfCancellationRequested();

                // Set Credentials
                if (conversations.Client.Credentials != null)
                {
                    await conversations.Client.Credentials.ProcessHttpRequestAsync(httpRequest, cancellationToken).ConfigureAwait(false);
                }

                using (var httpResponse = await conversations.Client.HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    string responseContent;
                    if (httpResponse.StatusCode != HttpStatusCode.OK)
                    {
                        var ex = new ErrorResponseException(string.Format("Operation returned an invalid status code '{0}'", httpResponse.StatusCode));

                        responseContent = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                        ErrorResponse errorBody = Rest.Serialization.SafeJsonConvert.DeserializeObject<ErrorResponse>(responseContent, conversations.Client.DeserializationSettings);
                        if (errorBody != null)
                        {
                            ex.Body = errorBody;
                        }

                        ex.Request = new HttpRequestMessageWrapper(httpRequest, requestContent);
                        ex.Response = new HttpResponseMessageWrapper(httpResponse, responseContent);
                        throw ex;
                    }

                    // Create Result
                    var result = new HttpOperationResponse<ResourceResponse>
                    {
                        Request = httpRequest,
                        Response = httpResponse,
                    };

                    // Deserialize Response
                    responseContent = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                    try
                    {
                        result.Body = Rest.Serialization.SafeJsonConvert.DeserializeObject<ResourceResponse>(responseContent, conversations.Client.DeserializationSettings);
                    }
                    catch (JsonException ex)
                    {
                        throw new SerializationException("Unable to deserialize the response.", responseContent, ex);
                    }

                    return result;
                }
            }
        }

        public static async Task<HttpOperationResponse<string>> GetHandoffStatusWithHttpMessagesAsync(IServiceOperations<ConnectorClient> conversations, string conversationId, CancellationToken cancellationToken = default)
        {
            if (conversationId == null)
            {
                throw new ArgumentNullException(nameof(conversationId));
            }

            // Construct URL
            var baseUrl = conversations.Client.BaseUri.AbsoluteUri;
            if (!baseUrl.EndsWith("/"))
            {
                baseUrl += "/";
            }

            var url = new Uri($"{baseUrl}v3/conversations/{Uri.EscapeDataString(conversationId)}/handoff");

            using (var httpRequest = new HttpRequestMessage())
            {
                httpRequest.Method = HttpMethod.Get;
                httpRequest.RequestUri = url;

                cancellationToken.ThrowIfCancellationRequested();

                // Set Credentials
                if (conversations.Client.Credentials != null)
                {
                    await conversations.Client.Credentials.ProcessHttpRequestAsync(httpRequest, cancellationToken).ConfigureAwait(false);
                }

                // Send Request
                using (var httpResponse = await conversations.Client.HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    string responseContent;
                    if (httpResponse.StatusCode != HttpStatusCode.OK && httpResponse.StatusCode != HttpStatusCode.NotFound)
                    {
                        var ex = new ErrorResponseException(string.Format("Operation returned an invalid status code '{0}'", httpResponse.StatusCode));

                        responseContent = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                        ErrorResponse errorBody = Rest.Serialization.SafeJsonConvert.DeserializeObject<ErrorResponse>(responseContent, conversations.Client.DeserializationSettings);

                        if (errorBody != null)
                        {
                            ex.Body = errorBody;
                        }

                        ex.Request = new HttpRequestMessageWrapper(httpRequest, null);
                        ex.Response = new HttpResponseMessageWrapper(httpResponse, responseContent);
                        throw ex;
                    }

                    // Create Result
                    var result = new HttpOperationResponse<string>
                    {
                        Request = httpRequest,
                        Response = httpResponse,
                    };

                    // Deserialize Response
                    responseContent = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                    try
                    {
                        result.Body = Rest.Serialization.SafeJsonConvert.DeserializeObject<string>(responseContent, conversations.Client.DeserializationSettings);
                    }
                    catch (JsonException ex)
                    {
                        throw new SerializationException("Unable to deserialize the response.", responseContent, ex);
                    }

                    return result;
                }
            }
        }
    }
}

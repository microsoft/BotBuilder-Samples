using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using Microsoft.Rest;
using Microsoft.Rest.Serialization;
using Newtonsoft.Json;

namespace BotTest
{
    public sealed class TestConversations : IConversations
    {
        public TestConversations(ConnectorClient client)
        {
            Client = client;
        }

        private ConnectorClient Client { get; }

        public Task<HttpOperationResponse<object>> CreateConversationWithHttpMessagesAsync(
            ConversationParameters parameters, Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return null;
        }

        public async Task<HttpOperationResponse<object>> SendToConversationWithHttpMessagesAsync(Activity activity,
            string conversationId, Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (activity == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "activity");
            }
            if (conversationId == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "conversationId");
            }

            // Construct URL
            var baseUrl = Client.BaseUri.AbsoluteUri;
            var url = new Uri(new Uri(baseUrl + (baseUrl.EndsWith("/") ? "" : "/")),
                "v3/conversations/{conversationId}/activities").ToString();
            url = url.Replace("{conversationId}", Uri.EscapeDataString(conversationId));
            // Create HTTP transport objects
            var httpRequest = new HttpRequestMessage
            {
                Method = new HttpMethod("POST"),
                RequestUri = new Uri(url)
            };

            var cred = new MicrosoftAppCredentials("c9cae410-3386-485d-8d93-78cbcf304d3a", "6z7iOj81affTvNrR092YGQf");
            var token = await cred.GetTokenAsync();
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            // Set Headers
            if (customHeaders != null)
            {
                foreach (var header in customHeaders)
                {
                    if (httpRequest.Headers.Contains(header.Key))
                    {
                        httpRequest.Headers.Remove(header.Key);
                    }
                    httpRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            // Serialize Request
            var requestContent = SafeJsonConvert.SerializeObject(activity, Client.SerializationSettings);
            httpRequest.Content = new StringContent(requestContent, Encoding.UTF8);
            httpRequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
            // Set Credentials
            if (Client.Credentials != null)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await Client.Credentials.ProcessHttpRequestAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            }
            // Send Request
            cancellationToken.ThrowIfCancellationRequested();
            var httpResponse = await Client.HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            var statusCode = httpResponse.StatusCode;
            cancellationToken.ThrowIfCancellationRequested();
            string responseContent;
            if ((int) statusCode != 200 && (int) statusCode != 201 && (int) statusCode != 202 &&
                (int) statusCode != 400 && (int) statusCode != 401 && (int) statusCode != 403 &&
                (int) statusCode != 404 && (int) statusCode != 500 && (int) statusCode != 503)
            {
                var ex = new HttpOperationException(
                    $"Operation returned an invalid status code '{statusCode}'");
                responseContent = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                ex.Request = new HttpRequestMessageWrapper(httpRequest, requestContent);
                ex.Response = new HttpResponseMessageWrapper(httpResponse, responseContent);
                httpRequest.Dispose();
                httpResponse.Dispose();
                throw ex;
            }
            // Create Result
            var result = new HttpOperationResponse<object>
            {
                Request = httpRequest,
                Response = httpResponse
            };

            responseContent = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            try
            {
                result.Body =
                    SafeJsonConvert.DeserializeObject<ResourceResponse>(responseContent,
                        Client.DeserializationSettings);
            }
            catch (JsonException ex)
            {
                httpRequest.Dispose();
                httpResponse.Dispose();
                throw new SerializationException("Unable to deserialize the response.", responseContent, ex);
            }
            return result;
        }

        public Task<HttpOperationResponse<object>> UpdateActivityWithHttpMessagesAsync(string conversationId,
            string activityId, Activity activity,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return null;
        }

        public Task<HttpOperationResponse<object>> ReplyToActivityWithHttpMessagesAsync(string conversationId,
            string activityId, Activity activity,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return null;
        }

        public Task<HttpOperationResponse<ErrorResponse>> DeleteActivityWithHttpMessagesAsync(string conversationId,
            string activityId, Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return null;
        }

        public Task<HttpOperationResponse<object>> GetConversationMembersWithHttpMessagesAsync(string conversationId,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return null;
        }

        public Task<HttpOperationResponse<object>> GetActivityMembersWithHttpMessagesAsync(string conversationId,
            string activityId, Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return null;
        }

        public Task<HttpOperationResponse<object>> UploadAttachmentWithHttpMessagesAsync(string conversationId,
            AttachmentData attachmentUpload,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return null;
        }
    }
}
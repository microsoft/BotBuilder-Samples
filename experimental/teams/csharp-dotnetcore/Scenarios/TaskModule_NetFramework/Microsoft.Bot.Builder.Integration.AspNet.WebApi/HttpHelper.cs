// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Schema;

namespace Microsoft.Bot.Builder.Integration.AspNet.WebApi
{
    internal static class HttpHelper
    {
        public static readonly MediaTypeFormatter[] BotMessageMediaTypeFormatters = new[]
        {
            new JsonMediaTypeFormatter
            {
                SerializerSettings = MessageSerializerSettings.Create(),
                SupportedMediaTypes =
                {
                    new System.Net.Http.Headers.MediaTypeHeaderValue("application/json") { CharSet = "utf-8" },
                    new System.Net.Http.Headers.MediaTypeHeaderValue("text/json") { CharSet = "utf-8" },
                },
            },
        };

        public static async Task<Activity> ReadRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var activity = await request.Content.ReadAsAsync<Activity>(BotMessageMediaTypeFormatters, cancellationToken).ConfigureAwait(false);
            return activity;
        }

        public static void WriteResponse(HttpRequestMessage request, HttpResponseMessage response, InvokeResponse invokeResponse)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            response.RequestMessage = request;

            if (invokeResponse == null)
            {
                response.StatusCode = HttpStatusCode.OK;
            }
            else
            {
                response.StatusCode = (HttpStatusCode)invokeResponse.Status;

                if (invokeResponse.Body != null)
                {
                    //response.Content = request.CreateResponse((HttpStatusCode)invokeResponse.Status, invokeResponse.Body, BotMessageMediaTypeFormatters[0]).Content;

                    response.Content = new ObjectContent(
                        invokeResponse.Body.GetType(),
                        invokeResponse.Body,
                        BotMessageMediaTypeFormatters[0]);
                }
            }
        }
    }
}

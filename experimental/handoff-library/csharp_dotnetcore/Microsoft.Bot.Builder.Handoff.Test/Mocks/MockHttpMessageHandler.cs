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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Microsoft.Rest;
using Newtonsoft.Json;

namespace Microsoft.Bot.Builder.Handoff.UnitTests
{
    internal class MockHttpMessageHandler : HttpMessageHandler
    {
        public MockHttpMessageHandler()
        {
            StatusCode = HttpStatusCode.OK;
        }

        public HttpMethod LastRequestMethod { get; set; }
        public Uri LastRequestUri { get; set; }
        public string LastRequestContent { get; set; }

        public string ResponseContent { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequestMethod = request.Method;
            LastRequestUri = request.RequestUri;
            LastRequestContent = request.AsFormattedString();
            return Task.FromResult(new HttpResponseMessage() { StatusCode = StatusCode, Content = new StringContent(ResponseContent ?? string.Empty) });
        }
    }
}

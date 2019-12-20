// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

﻿using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Bot_Auth_DL_Secure_Site_MVC.Models;
using Newtonsoft.Json;

namespace Bot_Authentication_Controller.Controllers
{
    public class HomeController : Controller
    {
        private const string secret = "<TODO: DirectLine secret>";
        private const string dlUrl = "https://directline.botframework.com/v3/directline/tokens/generate";

        public async Task<ActionResult> Index()
        {
            HttpClient client = new HttpClient();
            var userId = $"dl_{Guid.NewGuid()}";

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, dlUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", secret);
            request.Content = new StringContent(
                JsonConvert.SerializeObject(
                    new { User = new { Id = userId } }),
                    Encoding.UTF8,
                    "application/json");

            var response = await client.SendAsync(request);

            string token = String.Empty;
            if (response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                token = JsonConvert.DeserializeObject<DirectLineToken>(body).token;
            }

            var config = new ChatConfig()
            {
                Token = token,
                UserId = userId
            };

            return View(config);
        }
    }
}

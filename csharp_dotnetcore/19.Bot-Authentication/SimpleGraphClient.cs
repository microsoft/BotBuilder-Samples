// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Graph;

namespace Bot_Authentication
{
    // This class is a wrapper for the Microsoft Graph API
    // See: https://developer.microsoft.com/en-us/graph
    public class SimpleGraphClient
    {
        private readonly string _token;

        public SimpleGraphClient(string token)
        {
            this._token = token;
        }

        // Sends an email on the users behlaf using the Microsoft Graph API
        public async Task<bool> SendMailAsync(string toAddress, string subject, string content)
        {
            try
            {
                var graphClient = this.GetAuthenticatedClient();

                var recipients = new List<Recipient>
                {
                    new Recipient
                    {
                        EmailAddress = new EmailAddress
                        {
                            Address = toAddress,
                        },
                    },
                };

                // Create the message.
                var email = new Message
                {
                    Body = new ItemBody
                    {
                        Content = content,
                        ContentType = BodyType.Text,
                    },
                    Subject = subject,
                    ToRecipients = recipients,
                };

                // Send the message.
                await graphClient.Me.SendMail(email, true).Request().PostAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Gets recent unread mail for the user using the Microsoft Graph API
        public async Task<Message[]> GetRecentUnreadMailAsync()
        {
            var graphClient = this.GetAuthenticatedClient();
            var messages =
                await graphClient.Me.MailFolders.Inbox.Messages.Request().GetAsync();
            var from = DateTime.Now.Subtract(TimeSpan.FromMinutes(60));
            var unreadMessages = new List<Message>();

            var done = false;
            while (messages?.Count > 0 && !done)
            {
                foreach (var message in messages)
                {
                    if (message.ReceivedDateTime.HasValue && message.ReceivedDateTime.Value >= from)
                    {
                        if (message.IsRead.HasValue && !message.IsRead.Value)
                        {
                            unreadMessages.Add(message);
                            if (unreadMessages.Count >= 5)
                            {
                                done = true;
                            }
                        }
                    }
                    else
                    {
                        done = true;
                    }
                }

                messages = await messages.NextPageRequest.GetAsync();
            }

            return unreadMessages.ToArray();
        }

        // Get information about the user.
        public async Task<User> GetMeAsync()
        {
            var graphClient = this.GetAuthenticatedClient();
            var me = await graphClient.Me.Request().GetAsync();
            return me;
        }

        // gets information about the user's manager
        public async Task<User> GetManagerAsync()
        {
            var graphClient = this.GetAuthenticatedClient();
            var manager = await graphClient.Me.Manager.Request().GetAsync() as User;
            return manager;
        }

        // Gets the user's photo
        public async Task<PhotoResponse> GetPhotoAsync()
        {
            var photoResponse =
                await new HttpClient().GetStreamWithAuthAsync(
                    this._token,
                    "https://graph.microsoft.com/v1.0/me/photo/$value");
            if (photoResponse != null)
            {
                photoResponse.Base64String = $"data:{photoResponse.ContentType};base64," +
                                             Convert.ToBase64String(photoResponse.Bytes);
            }

            return photoResponse;
        }

        // Get an Authenticated Microsoft Graph client using the token issued to the user.
        private GraphServiceClient GetAuthenticatedClient()
        {
            var graphClient = new GraphServiceClient(
                new DelegateAuthenticationProvider(
                    requestMessage =>
                    {
                        // Append the access token to the request.
                        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", _token);

                        // Get event times in the current time zone.
                        requestMessage.Headers.Add("Prefer", "outlook.timezone=\"" + TimeZoneInfo.Local.Id + "\"");

                        return Task.CompletedTask;
                    }));
            return graphClient;
        }
    }
}

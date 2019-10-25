// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.BotFramework;
using Microsoft.Bot.Builder.Integration.AspNet.WebApi;
using Microsoft.BotBuilderSamples.Bots;
using Microsoft.BotBuilderSamples.Dialogs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Microsoft.BotBuilderSamples
{
    [Route("api/messages")]
    public class BotController : ApiController
    {
        private static LoggerFactory _loggerFactory;
        private static IBotFrameworkHttpAdapter _adapter;
        private static ConversationState _conversationState;
        private static UserState _userState;
        private static MainDialog _dialog;

        // In this example we will be using a static constructor on the Controller as these objects
        // should be singletons. Most likely a production application will be using one of the
        // Dependency Injection systems from NuGet such as Autofac, Unity, Ninject etc.
        static BotController()
        {
            _loggerFactory = new LoggerFactory();

            // create the User and ConversationState objects (in this case, for testing, both based on the same memory store)
            var storage = new MemoryStorage();
            _conversationState = new ConversationState(storage);
            _userState = new UserState(storage);
            
            // create the BotAdapter we will be using
            var credentialProvider = new ConfigurationCredentialProvider();
            _adapter = new AdapterWithErrorHandler(credentialProvider, _loggerFactory.CreateLogger<BotFrameworkHttpAdapter>(), _conversationState);

            // read the old style Web.Config settings and construct a new style dot net core IConfiguration object
            var appsettings = ConfigurationManager.AppSettings.AllKeys.SelectMany(
                ConfigurationManager.AppSettings.GetValues,
                (k, v) => new KeyValuePair<string, string>(k, v));

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(appsettings)
                .Build();

            // LUIS recognizer and BookingDialog are used by the MainDialog
            var bookingRecognizer = new FlightBookingRecognizer(configuration);
            var bookingDialog = new BookingDialog();
            // create the Dialog this bot will run - we need configuration because this Dialog will call LUIS
            _dialog = new MainDialog(bookingRecognizer, bookingDialog, _loggerFactory.CreateLogger<MainDialog>());
        }

        [HttpPost]
        [HttpGet]
        public async Task<HttpResponseMessage> PostAsync()
        {
            var response = new HttpResponseMessage();

            var bot = new DialogAndWelcomeBot<MainDialog>(_conversationState, _userState, _dialog, _loggerFactory.CreateLogger<DialogBot<MainDialog>>());

            await _adapter.ProcessAsync(Request, response, bot);

            return response;
        }
    }
}

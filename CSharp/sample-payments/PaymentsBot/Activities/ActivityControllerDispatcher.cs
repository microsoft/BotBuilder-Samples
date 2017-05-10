namespace PaymentsBot.Activities
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Web.Http;
    using Microsoft.Bot.Builder.Internals.Fibers;
    using Microsoft.Bot.Builder.Scorables;
    using Microsoft.Bot.Connector;

    public sealed class ActivityControllerDispatcher : Dispatcher
    {
        private readonly ApiController controller;
        private readonly Activity activity;
        private readonly HttpResponseMessage response;

        public ActivityControllerDispatcher(ApiController controller, Activity activity, HttpResponseMessage response)
        {
            SetField.NotNull(out this.controller, nameof(controller), controller);
            SetField.NotNull(out this.activity, nameof(activity), activity);
            SetField.NotNull(out this.response, nameof(response), response);
        }

        protected override Type MakeType()
        {
            return this.controller.GetType();
        }

        protected override IReadOnlyList<object> MakeServices()
        {
            var credentials = new MicrosoftAppCredentials();
            var connector = new ConnectorClient(new Uri(this.activity.ServiceUrl), credentials);

            StateClient storage;
            if (this.activity.ChannelId == "emulator")
            {
                storage = new StateClient(new Uri(this.activity.ServiceUrl), credentials);
            }
            else
            {
                storage = new StateClient(credentials);
            }

            return new object[] { this.controller, this.activity, this.response, connector, storage };
        }
    }
}
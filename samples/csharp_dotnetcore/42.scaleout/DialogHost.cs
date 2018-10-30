// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ScaleoutBot
{
    public class DialogHost
    {
        private static readonly JsonSerializer StateJsonSerializer = new JsonSerializer() { TypeNameHandling = TypeNameHandling.All };

        public static async Task<Tuple<Activity[], JObject>> RunAsync(Dialog rootDialog, Activity activity, JObject oldState)
        {
            // A custom adapter and corresponding TurnContext that buffers any messages sent.
            var adapter = new DialogHostAdapter();
            var turnContext = new TurnContext(adapter, activity);

            // Run the dialog using this TurnContext with the existing state.
            JObject newState = await RunTurnAsync(rootDialog, turnContext, oldState);

            // The result is a set of activities to send and a replacement state.
            return Tuple.Create(adapter.Activities.ToArray(), newState);
        }

        private static async Task<JObject> RunTurnAsync(Dialog rootDialog, TurnContext turnContext, JObject state)
        {
            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                // If we have some state, deserialize it. (This mimics the shape produced by BotState.cs.)
                var dialogState = state?[nameof(DialogState)]?.ToObject<DialogState>(StateJsonSerializer);

                // A custom accessor is used to pass a handle on the state to the dialog system.
                var accessor = new RefAccessor<DialogState>(dialogState);

                // The following is regular dialog driver code.
                var dialogs = new DialogSet(accessor);
                dialogs.Add(rootDialog);

                var dialogContext = await dialogs.CreateContextAsync(turnContext);
                var results = await dialogContext.ContinueDialogAsync();

                if (results.Status == DialogTurnStatus.Empty)
                {
                    await dialogContext.BeginDialogAsync("root");
                }

                // Serialize the result, and put its value back into a new JObject.
                return new JObject { { nameof(DialogState), JObject.FromObject(accessor.Value, StateJsonSerializer) } };
            }

            return state;
        }
    }
}

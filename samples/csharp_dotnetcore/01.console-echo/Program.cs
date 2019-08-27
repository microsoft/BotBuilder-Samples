// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Console_EchoBot
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Console EchoBot is online. I will repeat any message you send me!");
            Console.WriteLine("Say \"quit\" to end.");

            // Create the Console Adapter, and add Conversation State
            // to the Bot. The Conversation State will be stored in memory.
            var adapter = new ConsoleAdapter();

            // Create the instance of our Bot.
            var echoBot = new EchoBot();

            // Connect the Console Adapter to the Bot.
            adapter.ProcessActivityAsync(
                async (turnContext, cancellationToken) => await echoBot.OnTurnAsync(turnContext)).Wait();
        }
    }
}

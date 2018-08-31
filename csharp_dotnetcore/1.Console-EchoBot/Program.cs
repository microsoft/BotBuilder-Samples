// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Console_EchoBot
{
    using System;
    using System.Threading;

    /// <summary>
    /// Main program for EchoBot.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Main for the Console EchoBot.
        /// </summary>
        /// <param name="args">Arguments passed to the program.  Ignored.</param>
        public static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the EchoBot. Type something to get started.");

            // Create the Console Adapter, and add Conversation State
            // to the Bot. The Conversation State will be stored in memory.
            var adapter = new ConsoleAdapter();

            // Create the instance of our Bot.
            var echoBot = new EchoBot();

            // Connect the Console Adapter to the Bot.
            adapter.ProcessActivityAsync(
                async (context, ct) => await echoBot.OnTurnAsync(context, default(CancellationToken))).Wait();
        }
    }
}

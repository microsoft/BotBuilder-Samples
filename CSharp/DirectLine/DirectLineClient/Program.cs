namespace DirectLineSampleClient
{
    using System;
    using System.Configuration;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Bot.Connector.DirectLine;
    using Microsoft.Bot.Connector.DirectLine.Models;
    using Models;
    using Newtonsoft.Json;

    public class Program
    {
        private static string directLineSecret = ConfigurationManager.AppSettings["DirectLineSecret"];
        private static string botId = ConfigurationManager.AppSettings["BotId"];
        private static string fromUser = "DirectLineSampleClientUser";

        public static void Main(string[] args)
        {
            StartBotConversation().Wait();
        }

        private static async Task StartBotConversation()
        {
            DirectLineClient client = new DirectLineClient(directLineSecret);
            
            var conversation = await client.Conversations.NewConversationAsync();

            new System.Threading.Thread(async () => await ReadBotMessagesAsync(client, conversation.ConversationId)).Start();

            Console.Write("Command> ");

            while (true)
            {
                string input = Console.ReadLine().Trim();

                if (input.ToLower() == "exit")
                {
                    break;
                }
                else
                {
                    if (input.Length > 0)
                    {
                        Message userMessage = new Message
                        {
                            FromProperty = fromUser,
                            Text = input
                        };

                        await client.Conversations.PostMessageAsync(conversation.ConversationId, userMessage);
                    }
                }
            }
        }

        private static async Task ReadBotMessagesAsync(DirectLineClient client, string conversationId)
        {
            string watermark = null;
            
            while (true)
            {
                var messages = await client.Conversations.GetMessagesAsync(conversationId, watermark);
                watermark = messages?.Watermark;

                var messagesFromBotText = from x in messages.Messages
                                   where x.FromProperty == botId
                                   select x;

                foreach (Message message in messagesFromBotText)
                {
                    Console.WriteLine(message.Text);

                    if (message.ChannelData != null)
                    {
                        var channelData = JsonConvert.DeserializeObject<DirectLineChannelData>(message.ChannelData.ToString());

                        switch (channelData.ContentType)
                        {
                            case "application/vnd.microsoft.card.hero":
                                RenderHeroCard(channelData);
                                break;

                            case "image/png":
                                Console.WriteLine($"Opening the requested image '{channelData.ContentUrl}'");

                                Process.Start(channelData.ContentUrl);
                                break;
                        }  
                    }

                    Console.Write("Command> ");
                }

                await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);
            }
        }

        private static void RenderHeroCard(DirectLineChannelData channelData)
        {
            const int Width = 70;
            Func<string, string> contentLine = (content) => string.Format($"{{0, -{Width}}}", string.Format("{0," + ((Width + content.Length) / 2).ToString() + "}", content));

            Console.WriteLine("/{0}", new string('*', Width + 1));
            Console.WriteLine("*{0}*", contentLine(channelData.Content.Title));
            Console.WriteLine("*{0}*", new string(' ', Width));
            Console.WriteLine("*{0}*", contentLine(channelData.Content.Text));
            Console.WriteLine("{0}/", new string('*', Width + 1));
        }
    }
}

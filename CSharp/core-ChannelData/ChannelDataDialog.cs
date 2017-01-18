namespace ChannelDataBot
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using Models;

    [Serializable]
    public class ChannelDataDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);
        }

        public async virtual Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            await context.PostAsync("Looking into your upcoming flights to see if you can check-in on any of those...");

            var flightAttachment = GetFlightAttachment();

            var reply = context.MakeMessage();

            if (message.ChannelId != "facebook")
            {
                reply.Text = flightAttachment.ToString();
            }
            else
            {
                reply.ChannelData = new FacebookChannelData()
                {
                    Attachment = flightAttachment
                };
            }

            await context.PostAsync(reply);
            context.Wait(this.MessageReceivedAsync);
        }

        private static FacebookAttachment GetFlightAttachment()
        {
            return new FacebookAttachment()
            {
                Payload = new AirlineCheckIn()
                {
                    IntroMessage = "Check-in is available now",
                    Locale = "en_US",
                    PnrNumber = "ABCDEF",
                    CheckInUrl = "http://www.airline.com/check_in",
                    FlightInfo = new[]
                    {
                        new FlightInfo()
                        {
                            FlightNumber = "F001",
                            DepartureAirport = new Airport()
                            {
                                AirportCode = "SFO",
                                City = "San Francisco",
                                Terminal = "T4",
                                Gate = "G8"
                            },
                            ArrivalAirport = new Airport()
                            {
                                AirportCode = "EZE",
                                City = "Buenos Aires",
                                Terminal = "C",
                                Gate = "A2"
                            },
                            FlightSchedule = new FlightSchedule()
                            {
                                BoardingTime = DateTime.Now.AddDays(1).ToString("yyyy-MM-ddTH:mm"),
                                DepartureTime = DateTime.Now.AddDays(1).AddHours(1.5).ToString("yyy-MM-ddTH:mm"),
                                ArrivalTime = DateTime.Now.AddDays(2).ToString("yyyy-MM-ddTH:mm")
                            }
                        }
                    }
                }
            };
        }
    }
}

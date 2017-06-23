namespace BotBuilder.Samples.AdaptiveCards
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class HotelsQuery
    {
        [Required]
        public string Destination { get; set; }

        [Required]
        public DateTime? Checkin { get; set; }

        [Range(1, 60)]
        public int? Nights { get; set; }

        public static HotelsQuery Parse(dynamic o)
        {
            try
            {
                return new HotelsQuery
                {
                    Destination = o.Destination.ToString(),
                    Checkin = DateTime.Parse(o.Checkin.ToString()),
                    Nights = int.Parse(o.Nights.ToString())
                };
            }
            catch
            {
                throw new InvalidCastException("HotelQuery could not be read");
            }
        }
    }
}
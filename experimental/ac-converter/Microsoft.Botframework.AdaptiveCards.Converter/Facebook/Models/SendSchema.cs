using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Microsoft.Botframework.AdaptiveCards.Converter.Facebook.Models.Outgoing
{
    public class FileAttachmentPayload
    {
        /// <summary>
        /// Optional. URL of the file to upload. Max file size is 25MB (after encoding). A Timeout is set to 75 sec for videos and 10 secs for every other file type.
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }

        /// <summary>
        /// Optional. Set to true to make the saved asset sendable to other message recipients. Defaults to false.
        /// </summary>
        [JsonProperty("is_reusable")]
        public bool IsReusable { get; set; }

        [JsonProperty("attachment_id")]
        public string AttachmentId { get; set; }
    }



    /// <summary>
    /// Base class for all facebook template types.
    /// </summary>
    public class FacebookAttachment
    {
        // TODO: just add too parameters for template demo, need to change later.

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("payload")]
        public object Payload { get; set; }
    }

    public abstract class FacebookTemplate
    {
        [JsonProperty("template_type")]
        public string TemplateType { get; set; }
    }

    public class MediaTemplate
    {
        public MediaTemplate()
        {
            TemplateType = Outgoing.TemplateType.Media;
        }

        [JsonProperty("template_type")]
        public string TemplateType { get; set; }

        /// <summary>
        /// An array containing 1 element object that describe the media in the message. A maximum of 1 element is supported.
        /// </summary>
        [JsonProperty("elements")]
        public Element[] Elements { get; set; }

        /// <summary>
        /// Optional. Set to true to enable the native share button in Messenger for the template message. Defaults to false.
        /// </summary>
        [JsonProperty("sharable")]
        public bool Sharable { get; set; }

    }

    public class FlightUpdateTemplate
    {
        public FlightUpdateTemplate()
        {
            TemplateType = Outgoing.TemplateType.Update;
        }

        [JsonProperty("template_type")]
        public string TemplateType { get; set; }

        [JsonProperty("intro_message")]
        public string IntroMessage { get; set; }

        /// <summary>
        /// Optional. Background color of the attachment. Must be a RGB hexadecimal string. Defaults to #009ddc.
        /// </summary>
        [JsonProperty("theme_color")]
        public string ThemeColor { get; set; }

        /// <summary>
        /// Type of update. Must be 'delay', 'gate_change' or 'cancellation'.
        /// </summary>
        [JsonProperty("update_type")]
        public string UpdateType { get; set; }

        /// <summary>
        /// Two-letter language region code. Must be a two-letter ISO 639-1 language code and a ISO 3166-1 alpha-2 region code separated by an underscore character. 
        /// Used to translate field labels (e.g. en_US). 
        /// See this document for more information about Facebook's locale support.
        /// </summary>
        [JsonProperty("locale")]
        public string Locale { get; set; }

        /// <summary>
        /// Optional. The Passenger Name Record number (Booking Number).
        /// </summary>
        [JsonProperty("pnr_number")]
        public string PNRNumber { get; set; }

        [JsonProperty("update_flight_info")]
        public FlightInfo UpdateFilghtInfo { get; set; }
    }

    public class FlightInfo
    {
        [JsonProperty("flight_number")]
        public string FlightNumber { get; set; }

        [JsonProperty("departure_airport")]
        public Airport DepartureAirport { get; set; }

        [JsonProperty("arrival_airport")]
        public Airport ArrivalAirport { get; set; }

        [JsonProperty("flight_schedule")]
        public Schedule FlightSchedule { get; set; }
    }

    public class Airport
    {
        [JsonProperty("airport_code")]
        public string AirportCode { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("termial")]
        public string Terminal { get; set; }

        [JsonProperty("gate")]
        public string Gate { get; set; }
    }

    public class Schedule
    {
        /// <summary>
        /// Optional.Boarding time in departure airport timezone.
        /// Must be in the ISO 8601-based format YYYY-MM-DDThh:mm
        /// (e.g. 2015-09-26T10:30).
        /// </summary>
        [JsonProperty("boarding_time")]
        public string BoardingTime { get; set; }

        /// <summary>
        /// Departure time in departure airport timezone. 
        /// Must be in the ISO 8601-based format YYYY-MM-DDThh:mm 
        /// (e.g. 2015-09-26T10:30).
        /// </summary>
        [JsonProperty("departure_time")]
        public string DepartureTime { get; set; }

        /// <summary>
        /// Optional. Arrival time in arrival airport timezone. 
        /// Must be in the ISO 8601-based format YYYY-MM-DDThh:mm 
        /// (e.g. 2015-09-26T10:30).
        /// </summary>
        [JsonProperty("arrival_time")]
        public string ArrivalTime { get; set; }
    }

    public class GenericTemplate : FacebookTemplate
    {
        public GenericTemplate()
        {
            TemplateType = Outgoing.TemplateType.Generic;
        }

        [JsonProperty("elements")]
        public Element[] Elements { get; set; }
    }

    public class AttachmentType
    {
        public const string Audio = "audio";
        public const string Video = "video";
        public const string Image = "image";
        public const string File = "file";
        public const string Template = "template";
    }

    public class TemplateType
    {
        public const string Generic = "generic";
        public const string Button = "button";
        public const string Receipt = "receipt";
        public const string Media = "media";
        public const string Itinerary = "airline_itinerary";
        public const string CheckIn = "airline_checkin";
        public const string BoardingPass = "airline_boardingpass";
        public const string Update = "airline_update";
    }

    public class Element
    {
        /// <summary>
        /// The type of media being sent - image or video is supported.
        /// </summary>
        [JsonProperty("media_type")]
        public string MediaType { get; set; }

        /// <summary>
        /// The attachment ID of the image or video. Cannot be used if url is set.
        /// </summary>
        [JsonProperty("attachment_id")]
        public string AttachmentId { get; set; }

        /// <summary>
        /// The URL of the image. Cannot be used if attachment_id is set.
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }

        /// <summary>
        /// An array of button objects to be appended to the template. A maximum of 1 button is supported.
        /// </summary>
        [JsonProperty("buttons")]
        public Button[] Buttons { get; set; }
    }

    public enum ButtonType { web_url, postback, phone_number };

    public class ButtonTemplate
    {
        public ButtonTemplate()
        {
            TemplateType = Outgoing.TemplateType.Button;
        }

        [JsonProperty("template_type")]
        public string TemplateType { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("buttons")]
        public Button[] Buttons { get; set; }
    }


    public class Button
    {
        /// <summary>
        /// web_url|postback
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("type")]
        public ButtonType Type { get; set; }

        /// <summary>
        /// For web_url buttons, this URL is opened in a mobile browser when the button is tapped
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }

        /// <summary>
        /// Button title
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// For postback buttons, this data will be sent back to you via webhook
        /// </summary>
        [JsonProperty("payload")]
        public string Payload { get; set; }

        [JsonExtensionData(ReadData = true, WriteData = true)]
        public JObject Properties { get; set; }
    }

    public class Constants
    {
        public const int ButtonTitleLimit = 20;
        public const string ButtonTemplateName = "Button";
    }
}

using System;

namespace Microsoft.Botframework.AdaptiveCards.Converter.LINE.Models
{
    /// <summary>
    /// Tap to send a postback event with date and time selected by user.
    /// https://developers.line.me/en/docs/messaging-api/reference/#datetime-picker-action.
    /// </summary>
    public class DateTimePickerAction : ITemplateAction
    {
        public string Type { get; } = TemplateActionTypes.Datetimepicker;

        /// <summary>
        /// Label for the action
        /// Required for templates other than image carousel. Max: 20 characters
        /// Optional for image carousel templates. Max: 12 characters.
        /// Optional for rich menus. Spoken when the accessibility feature is enabled on the client device. Max: 20 characters.
        /// Supported on LINE iOS version 8.2.0 and later.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// String value in the postback.data
        /// Max: 300 characters.
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// DateTimePicker mode.
        /// </summary>
        public string Mode { get; set; }

        /// <summary>
        /// Initial value of date or time.
        /// </summary>
        public string Initial { get; set; }

        /// <summary>
        /// Largest date or time value that can be selected.
        /// Must be greater than the min value.
        /// </summary>
        public string Max { get; set; }

        /// <summary>
        /// Smallest date or time value that can be selected.
        /// Must be less than the max value.
        /// </summary>
        public string Min { get; set; }

        public DateTimePickerAction(string label, string data, string mode, string initial = null, string min = null, string max = null)
        {
            Initialize(label, data, mode, initial, min, max);
        }

        public DateTimePickerAction()
        {
        }

        public DateTimePickerAction(string label, string data, string mode, DateTime? initial = null, DateTime? min = null, DateTime? max = null)
        {
            var format = GetDateTimeFormat(mode);
            Initialize(label, data, mode,
                initial == null ? null : ((DateTime)initial).ToString(format),
                min == null ? null : ((DateTime)min).ToString(format),
                max == null ? null : ((DateTime)max).ToString(format));
        }

        internal void Initialize(string label, string data, string mode, string initial, string min, string max)
        {
            Label = label?.Substring(0, Math.Min(label.Length, 20));
            Data = data.Substring(0, Math.Min(data.Length, 300));
            Mode = mode;
            Initial = initial;
            Min = min;
            Max = max;
        }

        internal static string GetDateTimeFormat(string mode)
        {
            switch (mode)
            {
                case DateTimePickerMode.Date:
                    return "yyyy-MM-dd";
                case DateTimePickerMode.Time:
                    return "HH:mm";
                case DateTimePickerMode.Datetime:
                default:
                    return "yyyy-MM-ddTHH:mm";
            }
        }

    }
}

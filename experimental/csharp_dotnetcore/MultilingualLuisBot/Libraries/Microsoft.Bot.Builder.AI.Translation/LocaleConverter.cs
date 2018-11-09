// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Recognizers.Text;
using Microsoft.Recognizers.Text.DateTime;

namespace Microsoft.Bot.Builder.AI.Translation
{
    /// <summary>
    /// Locale Converter Class Converts dates and times
    /// between different locales.
    /// </summary>
    public class LocaleConverter : ILocaleConverter
    {
        private static readonly ConcurrentDictionary<string, DateAndTimeLocaleFormat> _mapLocaleToFunction = new ConcurrentDictionary<string, DateAndTimeLocaleFormat>();
        private static LocaleConverter _localeConverter;

        private LocaleConverter()
        {
            InitLocales();
        }

        public static LocaleConverter Converter
        {
            get
            {
                if (_localeConverter == null)
                {
                    _localeConverter = new LocaleConverter();
                }

                return _localeConverter;
            }
        }

        /// <summary>
        /// Check if a specific locale is available.
        /// </summary>
        /// <param name="locale">input locale that we need to check if available.</param>
        /// <returns>true if the locale is found, otherwise false.</returns>
        public bool IsLocaleAvailable(string locale)
        {
            AssertValidLocale(locale);
            return _mapLocaleToFunction.ContainsKey(locale);
        }

        /// <summary>
        /// Convert a message from locale to another locale.
        /// </summary>
        /// <param name="message"> input user message.</param>
        /// <param name="fromLocale">Source Locale.</param>
        /// <param name="toLocale">Target Locale.</param>
        /// <returns>The message converted to the target locale.</returns>
        public string Convert(string message, string fromLocale, string toLocale)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentException("Empty message");
            }

            var dates = ExtractDate(message, fromLocale);
            if (!IsLocaleAvailable(toLocale))
            {
                throw new InvalidOperationException($"Unsupported from locale: {toLocale}");
            }

            var processedMessage = message;
            foreach (var date in dates)
            {
                if (date.Range)
                {
                    if (date.Type == "time")
                    {
                        var timeRange = $"{string.Format(_mapLocaleToFunction[toLocale].TimeFormat, date.DateTime)} - {string.Format(_mapLocaleToFunction[toLocale].TimeFormat, date.EndDateTime)}";
                        processedMessage = Regex.Replace(processedMessage, $"\\b{date.Text}\\b", timeRange, RegexOptions.Singleline | RegexOptions.IgnoreCase);
                    }
                    else if (date.Type == "date")
                    {
                        var dateRange = $"{string.Format(_mapLocaleToFunction[toLocale].DateFormat, date.DateTime)} - {string.Format(_mapLocaleToFunction[toLocale].DateFormat, date.EndDateTime)}";
                        processedMessage = Regex.Replace(processedMessage, $"\\b{date.Text}\\b", dateRange, RegexOptions.Singleline | RegexOptions.IgnoreCase);
                    }
                    else
                    {
                        var convertedStartDate = string.Format(_mapLocaleToFunction[toLocale].DateFormat, date.DateTime);
                        var convertedStartTime = string.Format(_mapLocaleToFunction[toLocale].TimeFormat, date.DateTime);

                        var convertedEndDate = string.Format(_mapLocaleToFunction[toLocale].DateFormat, date.EndDateTime);
                        var convertedEndTime = string.Format(_mapLocaleToFunction[toLocale].TimeFormat, date.EndDateTime);
                        processedMessage = Regex.Replace(processedMessage, $"\\b{date.Text}\\b", $"{convertedStartDate} {convertedStartTime} - {convertedEndDate} {convertedEndTime}", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                    }
                }
                else
                {
                    if (date.Type == "time")
                    {
                        processedMessage = Regex.Replace(processedMessage, $"\\b{date.Text}\\b", string.Format(_mapLocaleToFunction[toLocale].TimeFormat, date.DateTime), RegexOptions.Singleline | RegexOptions.IgnoreCase);
                    }
                    else if (date.Type == "date")
                    {
                        processedMessage = Regex.Replace(processedMessage, $"\\b{date.Text}\\b", string.Format(_mapLocaleToFunction[toLocale].DateFormat, date.DateTime), RegexOptions.Singleline | RegexOptions.IgnoreCase);
                    }
                    else
                    {
                        var convertedDate = string.Format(_mapLocaleToFunction[toLocale].DateFormat, date.DateTime);
                        var convertedTime = string.Format(_mapLocaleToFunction[toLocale].TimeFormat, date.DateTime);
                        processedMessage = Regex.Replace(processedMessage, $"\\b{date.Text}\\b", $"{convertedDate} {convertedTime}", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                    }
                }
            }

            return processedMessage;
        }

        /// <summary>
        /// Get all available locales.
        /// </summary>
        /// <returns>The available locales.</returns>
        public string[] GetAvailableLocales() => _mapLocaleToFunction.Keys.ToArray();

        private static void AssertValidLocale(string locale)
        {
            if (string.IsNullOrWhiteSpace(locale))
            {
                throw new ArgumentNullException(nameof(locale));
            }
        }

        private static string FindCulture(string fromLocale)
        {
            var culture = fromLocale.Split('-')[0];
            if (fromLocale.StartsWith("fr"))
            {
                return Culture.French;
            }
            else if (fromLocale.StartsWith("de"))
            {
                return Culture.German;
            }
            else if (fromLocale.StartsWith("pt"))
            {
                return Culture.Portuguese;
            }
            else if (fromLocale.StartsWith("zh"))
            {
                return Culture.Chinese;
            }
            else if (fromLocale.StartsWith("es"))
            {
                return Culture.Spanish;
            }
            else if (fromLocale.StartsWith("en"))
            {
                return Culture.English;
            }
            else
            {
                throw new InvalidOperationException($"Unsupported from locale: {fromLocale}");
            }
        }

        /// <summary>
        /// Init different locales format,
        /// Supporting English, French, Deutsche and Chinese Locales.
        /// </summary>
        private void InitLocales()
        {
            if (_mapLocaleToFunction.Count > 0)
            {
                return;
            }

            var supportedLocales = new string[]
            {
                "en-us", "en-za", "en-ie", "en-gb", "en-ca", "fr-ca", "zh-cn", "zh-sg", "zh-hk", "zh-mo", "zh-tw",
                "en-au", "fr-be", "fr-ch", "fr-fr", "fr-lu", "fr-mc", "de-at", "de-ch", "de-de", "de-lu", "de-li",
                "es-es",
            };
            foreach (var locale in supportedLocales)
            {
                var cultureInfo = new CultureInfo(locale);
                var dateTimeInfo = new DateAndTimeLocaleFormat()
                {
                    DateFormat = $"{{0:{cultureInfo.DateTimeFormat.ShortDatePattern}}}",
                    TimeFormat = $"{{0:{cultureInfo.DateTimeFormat.ShortTimePattern}}}",
                };
                _mapLocaleToFunction[locale] = dateTimeInfo;
            }
        }

        /// <summary>
        /// Extract date and time from a sentence using Microsoft Recognizer.
        /// </summary>
        /// <param name="message">input user message.</param>
        /// <param name="fromLocale">Source Locale.</param>
        /// <returns>A <see cref="List{T}"/> of <see cref="TextAndDateTime"/> recognized from a sentence.</returns>
        private List<TextAndDateTime> ExtractDate(string message, string fromLocale)
        {
            var fndDates = new List<TextAndDateTime>();

            // extract culture name.
            var cultureName = FindCulture(fromLocale);
            var results = DateTimeRecognizer.RecognizeDateTime(message, cultureName);

            // looping on each result and extracting found date objects from input utterance
            foreach (var result in results)
            {
                var resolutionValues = (IList<Dictionary<string, string>>)result.Resolution["values"];
                var type = result.TypeName.Replace("datetimeV2.", string.Empty);
                DateTime moment;
                string momentType;
                DateTime momentEnd;
                TextAndDateTime curDateTimeText;
                if (type.Contains("range"))
                {
                    if (type.Contains("date") && type.Contains("time"))
                    {
                        momentType = "datetime";
                    }
                    else if (type.Contains("date"))
                    {
                        momentType = "date";
                    }
                    else
                    {
                        momentType = "time";
                    }

                    moment = DateTime.Parse(resolutionValues.First()["start"]);
                    momentEnd = DateTime.Parse(resolutionValues.First()["end"]);
                    curDateTimeText = new TextAndDateTime
                    {
                        DateTime = moment,
                        Text = result.Text,
                        Type = momentType,
                        Range = true,
                        EndDateTime = momentEnd,
                    };
                }
                else
                {
                    if (type.Contains("date") && type.Contains("time"))
                    {
                        momentType = "datetime";
                    }
                    else if (type.Contains("date"))
                    {
                        momentType = "date";
                    }
                    else
                    {
                        momentType = "time";
                    }

                    moment = resolutionValues.Select(v => DateTime.Parse(v["value"])).FirstOrDefault();
                    curDateTimeText = new TextAndDateTime
                    {
                        DateTime = moment,
                        Text = result.Text,
                        Type = momentType,
                        Range = false,
                    };
                }

                fndDates.Add(curDateTimeText);
            }

            return fndDates;
        }
    }
}

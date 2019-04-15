// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Bot.Builder.AI.Translation.Tests
{
    [TestClass]
    public class LocaleConverterTests
    {
        [TestMethod]
        [TestCategory("AI")]
        [TestCategory("Locale Converter")]
        public void LocaleConverter_ConvertFromFrench()
        {
            LocaleConverter localeConverter = LocaleConverter.Converter;

            var convertedMessage = localeConverter.Convert("Set a meeting on 30/9/2017", "fr-fr", "en-us");
            Assert.IsNotNull(convertedMessage);
            Assert.AreEqual("Set a meeting on 9/30/2017", convertedMessage);
        }

        [TestMethod]
        [TestCategory("AI")]
        [TestCategory("Locale Converter")]
        public void LocaleConverter_ConvertTimeFromSpanishSpain()
        {
            LocaleConverter localeConverter = LocaleConverter.Converter;

            var convertedMessage = localeConverter.Convert("Book a meeting for 15:00", "es-es", "en-us");
            Assert.IsNotNull(convertedMessage);
            Assert.AreEqual("Book a meeting for 3:00 PM", convertedMessage);
        }

        [TestMethod]
        [TestCategory("AI")]
        [TestCategory("Locale Converter")]
        public  void LocaleConverter_ConvertToChinese()
        {
            LocaleConverter localeConverter = LocaleConverter.Converter;

            var convertedMessage = localeConverter.Convert("Book me a plane ticket for France on 12/25/2018", "en-us", "zh-cn");
            Assert.IsNotNull(convertedMessage);
            Assert.AreEqual("Book me a plane ticket for France on 2018/12/25", convertedMessage); 
        }

        [TestMethod]
        [TestCategory("AI")]
        [TestCategory("Locale Converter")]
        public void LocaleConverter_InvalidFromLocale()
        {
            LocaleConverter localeConverter = LocaleConverter.Converter;

             Assert.ThrowsException<InvalidOperationException>(() =>
                 localeConverter.Convert("Book me a plane ticket for France on 12/25/2018", "na-na", "en-us")); 
        }   

        [TestMethod]
        [TestCategory("AI")]
        [TestCategory("Locale Converter")]
        public void LocaleConverter_InvalidToLocale()
        {
            LocaleConverter localeConverter = LocaleConverter.Converter;

            Assert.ThrowsException<InvalidOperationException>( ()=>
                 localeConverter.Convert("Book me a plane ticket for France on 12/25/2018", "en-us", "na-na"));
            Assert.ThrowsException<InvalidOperationException>(() =>
                localeConverter.Convert("Book me a plane ticket for France on 12/25/2018", "en-us", "Weird Locale"));
            Assert.ThrowsException<InvalidOperationException>(() =>
                localeConverter.Convert("Book me a plane ticket for France on 12/25/2018", "en-us", "fr-"));
            Assert.ThrowsException<ArgumentNullException>(() =>
                localeConverter.Convert("Book me a plane ticket for France on 12/25/2018", "en-us", string.Empty));
            Assert.ThrowsException<ArgumentNullException>(() =>
                localeConverter.Convert("Book me a plane ticket for France on 12/25/2018", "en-us", null));
        }

        [TestMethod]
        [TestCategory("AI")]
        [TestCategory("Locale Converter")]
        public void LocaleConverter_DateAndTime()
        {
            LocaleConverter localeConverter = LocaleConverter.Converter;

            var convertedMessage = localeConverter.Convert("half past 9 am 02/03/2010", "en-us", "fr-fr");
            Assert.IsNotNull(convertedMessage);
            Assert.AreEqual("03/02/2010 09:30", convertedMessage);
        }

        [TestMethod]
        [TestCategory("AI")]
        [TestCategory("Locale Converter")]
        public void LocaleConverter_DateOnly()
        {
            LocaleConverter localeConverter = LocaleConverter.Converter;

            var convertedMessage = localeConverter.Convert("02/03/2010", "en-us", "fr-fr");
            Assert.IsNotNull(convertedMessage);
            Assert.AreEqual("03/02/2010", convertedMessage);
        }

        [TestMethod]
        [TestCategory("AI")]
        [TestCategory("Locale Converter")]
        public void LocaleConverter_TimeOnly()
        {
            LocaleConverter localeConverter = LocaleConverter.Converter;

            var convertedMessage = localeConverter.Convert("half past 9 am", "en-us", "fr-fr");
            Assert.IsNotNull(convertedMessage);
            Assert.AreEqual("09:30", convertedMessage);
        }

        [TestMethod]
        [TestCategory("AI")]
        [TestCategory("Locale Converter")]
        public void LocaleConverter_DateAndTimeRange()
        {
            LocaleConverter localeConverter = LocaleConverter.Converter;

            var convertedMessage = localeConverter.Convert("from 10/21/2018 9 am to 10/23/2018 1 pm", "en-us", "fr-fr");
            Assert.IsNotNull(convertedMessage);
            Assert.AreEqual("21/10/2018 09:00 - 23/10/2018 13:00", convertedMessage);
        }

        [TestMethod]
        [TestCategory("AI")]
        [TestCategory("Locale Converter")]
        public void LocaleConverter_DateRange()
        {
            LocaleConverter localeConverter = LocaleConverter.Converter;

            var convertedMessage = localeConverter.Convert("from 10/21/2018 to 10/23/2018", "en-us", "fr-fr");
            Assert.IsNotNull(convertedMessage);
            Assert.AreEqual("21/10/2018 - 23/10/2018", convertedMessage);
        }

        [TestMethod]
        [TestCategory("AI")]
        [TestCategory("Locale Converter")]
        public void LocaleConverter_TimeRange()
        {
            LocaleConverter localeConverter = LocaleConverter.Converter;

            var convertedMessage = localeConverter.Convert("from 9 am to 1 pm", "en-us", "fr-fr");
            Assert.IsNotNull(convertedMessage);
            Assert.AreEqual("09:00 - 13:00", convertedMessage);
        }
    }
}

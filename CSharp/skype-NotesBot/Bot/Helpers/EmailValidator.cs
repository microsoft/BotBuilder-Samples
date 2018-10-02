using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Bot.Interfaces;

namespace Bot.Helpers
{
    public class EmailValidator : IEmailValidator
    {
        private static bool _invalid;

        public bool OnCheckIsValidEmail(string emailString)
        {
            _invalid = false;
            var strippedEmail = Regex.Replace(emailString, "<.*?>", string.Empty);
            if (string.IsNullOrEmpty(strippedEmail))
            {
                return false;
            }

            try
            {
                strippedEmail = Regex.Replace(strippedEmail, @"(@)(.+)$", DomainMapper,
                    RegexOptions.None, TimeSpan.FromMilliseconds(200));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }

            if (_invalid)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(strippedEmail,
                    @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                    @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        private static string DomainMapper(Match match)
        {
            var domainName = match.Groups[2].Value;
            try
            {
                domainName = new IdnMapping().GetAscii(domainName);
            }
            catch (ArgumentException)
            {
                _invalid = true;
            }
            return match.Groups[1].Value + domainName;
        }
    }
}
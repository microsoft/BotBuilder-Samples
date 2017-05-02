using Bot.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace BotTest.Helpers
{
    [TestClass]
    public sealed class EmailValidotorTest
    {
        [Test]
        [TestCase("")]
        [TestCase("contoso@microsoft")]
        [TestCase("@microsoft.com")]
        [TestCase("contoso@@microsoft.com")]
        [TestCase("this is invalid email")]
        [TestCase("contoso@microsoft.com")]
        public void CheckStringIsValidEmail(string emailString)
        {
            // Arrange
            var mockEmailValidator = new Mock<EmailValidator>();

            // Act
            var isValidEmail = mockEmailValidator.Object.OnCheckIsValidEmail(emailString);

            // Assert
            switch (emailString)
            {
                case "contoso@microsoft.com":
                    Assert.True(isValidEmail);
                    break;
                default:
                    Assert.False(isValidEmail);
                    break;
            }
        }
    }
}
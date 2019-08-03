// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with CoreBot .NET Template version __vX.X.X__

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Moq;

namespace CoreBot.Tests.Common
{
    /// <summary>
    /// Contains utility methods for creating simple mock objects based on <see href="https://github.com/moq/moq">moq</see>/>.
    /// </summary>
    public static class SimpleMockFactory
    {
        /// <summary>
        /// Creates a simple mock dialog.
        /// </summary>
        /// <typeparam name="T">A <see cref="Dialog"/> derived type.</typeparam>
        /// <param name="expectedResult">An object containing the results returned by the dialog ind the Dialog in the <see cref="DialogTurnResult"/>.</param>
        /// <param name="constructorParams">Optional constructor parameters for the dialog.</param>
        /// <returns>A <see cref="Mock{T}"/> object for the desired dialog type.</returns>
        public static Mock<T> CreateMockDialog<T>(object expectedResult = null, params object[] constructorParams)
            where T : Dialog
        {
            var mockDialog = new Mock<T>(constructorParams);
            var mockDialogNameTypeName = typeof(T).Name;
            mockDialog
                .Setup(x => x.BeginDialogAsync(It.IsAny<DialogContext>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .Returns(async (DialogContext dialogContext, object options, CancellationToken cancellationToken) =>
                {
                    await dialogContext.Context.SendActivityAsync($"{mockDialogNameTypeName} mock invoked", cancellationToken: cancellationToken);

                    return await dialogContext.EndDialogAsync(expectedResult, cancellationToken);
                });

            return mockDialog;
        }

        /// <summary>
        /// Creates a simple <see cref="IRecognizer"/> mock object that returns the desired <see cref="IRecognizerConvert"/> result.
        /// </summary>
        /// <typeparam name="TRecognizer">The type of <see cref="IRecognizer"/>to create.</typeparam>
        /// <typeparam name="TReturns">Type type of <see cref="IRecognizerConvert"/> to return.</typeparam>
        /// <param name="returns">The value to return when <see cref="IRecognizer.RecognizeAsync{T}"/> gets called.</param>
        /// <param name="constructorParams">Optional constructor parameters for the recognizer.</param>
        /// <returns>A <see cref="Mock{TRecognizer}"/> instance.</returns>
        public static Mock<TRecognizer> CreateMockLuisRecognizer<TRecognizer, TReturns>(TReturns returns, params object[] constructorParams)
            where TRecognizer : class, IRecognizer
            where TReturns : IRecognizerConvert, new()
        {
            var mockRecognizer = new Mock<TRecognizer>(constructorParams);
            mockRecognizer
                .Setup(x => x.RecognizeAsync<TReturns>(It.IsAny<ITurnContext>(), It.IsAny<CancellationToken>()))
                .Returns(() => Task.FromResult(returns));
            return mockRecognizer;
        }

        /// <summary>
        /// Creates a simple <see cref="IRecognizer"/> mock object that returns the desired <see cref="RecognizerResult"/> result.
        /// </summary>
        /// <typeparam name="TRecognizer">The type of <see cref="IRecognizer"/>to create.</typeparam>
        /// <param name="returns">The value to return when <see cref="IRecognizer.RecognizeAsync"/> gets called.</param>
        /// <param name="constructorParams">Optional constructor parameters for the recognizer.</param>
        /// <returns>A <see cref="Mock{TRecognizer}"/> instance.</returns>
        public static Mock<TRecognizer> CreateMockLuisRecognizer<TRecognizer>(RecognizerResult returns, params object[] constructorParams)
            where TRecognizer : class, IRecognizer
        {
            var mockRecognizer = new Mock<TRecognizer>(constructorParams);
            mockRecognizer
                .Setup(x => x.RecognizeAsync(It.IsAny<ITurnContext>(), It.IsAny<CancellationToken>()))
                .Returns(() => Task.FromResult(returns));
            return mockRecognizer;
        }
    }
}

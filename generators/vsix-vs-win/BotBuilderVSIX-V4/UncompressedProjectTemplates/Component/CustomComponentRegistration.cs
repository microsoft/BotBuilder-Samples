// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio Bot Framework Custom Component v$templateversion$

using AdaptiveExpressions.Converters;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs.Debugging;
using Microsoft.Bot.Builder.Dialogs.Declarative;
using Microsoft.Bot.Builder.Dialogs.Declarative.Resources;
using Newtonsoft.Json;
using System.Collections.Generic;
using $safeprojectname$.Actions;

namespace $safeprojectname$
{
    /// <summary>
    /// <see cref="ComponentRegistration"/> for DoWhile enabling discovering assets.
    /// </summary>
    /// <remarks>
    /// To make your components available to the system you derive from ComponentRegistration
    /// and implement appropriate interfaces which register functionality. These components
    /// then are consumed in appropriate places by the systems that need them. 
    /// </remarks>
    public class CustomComponentRegistration : ComponentRegistration, IComponentDeclarativeTypes
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomComponentRegistration"/> class.
        /// </summary>
        public CustomComponentRegistration()
        {
        }

        /// <summary>
        /// Gets adaptive <see cref="DeclarativeType"/> resources.
        /// </summary>
        /// <param name="resourceExplorer"><see cref="ResourceExplorer"/> with expected path to get all schema resources.</param>
        /// <returns>Adaptive <see cref="DeclarativeType"/> resources.</returns>
        public virtual IEnumerable<DeclarativeType> GetDeclarativeTypes(ResourceExplorer resourceExplorer)
        {
            yield return new DeclarativeType<CustomAction>(CustomAction.Kind);
        }

        /// <summary>
        /// Gets adaptive <see cref="JsonConverter"/> resources.
        /// </summary>
        /// <param name="resourceExplorer">ResourceExplorer to use to resolve references.</param>
        /// <param name="sourceContext">SourceContext to build debugger source map.</param>
        /// <returns>Adaptive <see cref="JsonConverter"/> resources.</returns>
        public virtual IEnumerable<JsonConverter> GetConverters(ResourceExplorer resourceExplorer, SourceContext sourceContext)
        {
            yield return new ObjectExpressionConverter<object>();
        }
    }
}

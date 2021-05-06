// Licensed under the MIT License.
// Copyright (c) Microsoft Corporation. All rights reserved.

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using AdaptiveExpressions;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Conditions;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;

namespace Microsofts.Bot.Component.Samples.MemberUpdates
{
    /// <summary>
    /// Actions triggered when ConversationUpdateActivity is received with Activity.MembersRemoved > 0.
    /// </summary>
    public class OnMembersRemoved : OnActivity
    {
        /// <summary>
        /// Gets the unique name (class identifier) of this trigger.
        /// </summary>
        /// <remarks>
        /// There should be at least a .schema file of the same name.  There can optionally be a
        /// .uischema file of the same name that describes how Composer displays this trigger.
        /// </remarks>
        [JsonProperty("$kind")]
        public new const string Kind = "Microsoft.OnMembersRemoved";

        /// <summary>
        /// Initializes a new instance of the <see cref="OnMembersRemoved"/> class.
        /// </summary>
        /// <param name="actions">Optional, list of <see cref="Dialog"/> actions.</param>
        /// <param name="condition">Optional, condition which needs to be met for the actions to be executed.</param>
        /// <param name="callerPath">Optional, source file full path.</param>
        /// <param name="callerLine">Optional, line number in source file.</param>
        [JsonConstructor]
        public OnMembersRemoved(List<Dialog> actions = null, string condition = null, [CallerFilePath] string callerPath = "", [CallerLineNumber] int callerLine = 0)
            : base(type: ActivityTypes.ConversationUpdate, actions: actions, condition: condition, callerPath: callerPath, callerLine: callerLine)
        {
        }

        /// <inheritdoc/>
        protected override Expression CreateExpression()
        {
            // The Activity.MembersRemoved list must have more than 0 items.
            return Expression.AndExpression(Expression.Parse($"count({TurnPath.Activity}.MembersRemoved) > 0"), base.CreateExpression());
        }
    }
}

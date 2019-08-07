// <copyright file="AdditionalProperties.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Schema.Teams
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Content type for <see cref="O365ConnectorCard"/>.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Using one file for all additional properties.")]
    public partial class O365ConnectorCard
    {
        /// <summary>
        /// Content type to be used in the type property.
        /// </summary>
        public const string ContentType = "application/vnd.microsoft.teams.card.o365connector";
    }

    /// <summary>
    /// Content type for <see cref="O365ConnectorCardViewAction"/>.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Using one file for all additional properties.")]
    public partial class O365ConnectorCardViewAction
    {
        /// <summary>
        /// Content type to be used in the @type property.
        /// </summary>
        public const string Type = "ViewAction";
    }

    /// <summary>
    /// Content type for <see cref="O365ConnectorCardOpenUri"/>.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Using one file for all additional properties.")]
    public partial class O365ConnectorCardOpenUri
    {
        /// <summary>
        /// Content type to be used in the @type property.
        /// </summary>
        public const string Type = "OpenUri";
    }

    /// <summary>
    /// Content type for <see cref="O365ConnectorCardHttpPOST"/>.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Using one file for all additional properties.")]
    public partial class O365ConnectorCardHttpPOST
    {
        /// <summary>
        /// Content type to be used in the @type property.
        /// </summary>
        public const string Type = "HttpPOST";
    }

    /// <summary>
    /// Content type for <see cref="O365ConnectorCardActionCard"/>.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Using one file for all additional properties.")]
    public partial class O365ConnectorCardActionCard
    {
        /// <summary>
        /// Content type to be used in the @type property.
        /// </summary>
        public const string Type = "ActionCard";
    }

    /// <summary>
    /// Content type for <see cref="O365ConnectorCardTextInput"/>.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Using one file for all additional properties.")]
    public partial class O365ConnectorCardTextInput
    {
        /// <summary>
        /// Content type to be used in the @type property.
        /// </summary>
        public const string Type = "TextInput";
    }

    /// <summary>
    /// Content type for <see cref="O365ConnectorCardDateInput"/>.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Using one file for all additional properties.")]
    public partial class O365ConnectorCardDateInput
    {
        /// <summary>
        /// Content type to be used in the @type property.
        /// </summary>
        public const string Type = "DateInput";
    }

    /// <summary>
    /// Content type for <see cref="O365ConnectorCardMultichoiceInput"/>.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Using one file for all additional properties.")]
    public partial class O365ConnectorCardMultichoiceInput
    {
        /// <summary>
        /// Content type to be used in the @type property.
        /// </summary>
        public const string Type = "MultichoiceInput";
    }

    /// <summary>
    /// Content type for <see cref="FileConsentCard"/>.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Using one file for all additional properties.")]
    public partial class FileConsentCard
    {
        /// <summary>
        /// Content type to be used in the type property.
        /// </summary>
        public const string ContentType = "application/vnd.microsoft.teams.card.file.consent";
    }

    /// <summary>
    /// Content type for <see cref="FileDownloadInfo"/>.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Using one file for all additional properties.")]
    public partial class FileDownloadInfo
    {
        /// <summary>
        /// Content type to be used in the type property.
        /// </summary>
        public const string ContentType = "application/vnd.microsoft.teams.file.download.info";
    }

    /// <summary>
    /// Content type for <see cref="FileConsentCard"/>.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Using one file for all additional properties.")]
    public partial class FileInfoCard
    {
        /// <summary>
        /// Content type to be used in the type property.
        /// </summary>
        public const string ContentType = "application/vnd.microsoft.teams.card.file.info";
    }
}

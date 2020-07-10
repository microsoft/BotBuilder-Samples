using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace TaskModuleFactorySample.Extensions.Teams
{
    /// <summary>
    /// Enum for different Handler of TaskModule.
    /// </summary>
    public enum TeamsFlowType
    {
        /// <summary>
        /// Task Module will display create form
        /// </summary>
        [EnumMember(Value = "createsample_form")]
        CreateSample_Form,

        /// <summary>
        /// Task Module will display update ticket form
        /// </summary>
        [EnumMember(Value = "updatesample_form")]
        UpdateSample_Form,

        /// <summary>
        /// Task Module will display delete ticket form
        /// </summary>
        [EnumMember(Value = "deleteticket_form")]
        DeleteSample_Form
    }
}

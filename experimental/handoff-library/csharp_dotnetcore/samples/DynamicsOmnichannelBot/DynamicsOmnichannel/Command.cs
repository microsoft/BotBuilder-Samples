using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DynamicsOmnichannelBot.DynamicsOmnichannel
{
    /// <summary>
    /// Command types that bot can send to Omni-Channel
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CommandType
    {
        [EnumMember(Value = "Escalate")]
        Escalate = 0,
        [EnumMember(Value = "EndConversation")]
        EndConversation = 1,
    }
    /// <summary>
    /// Action
    /// </summary>
    [DataContract]
    public class Command
    {
        /// <summary>
        /// Type of action that bot can send to Omni-Channel
        /// </summary>
        [DataMember(Name = "type")]
        public CommandType Type { get; set; }

        /// <summary>
        /// Dictionary of Workstream Context variable and value pairs to be sent to Omni-Channel 
        /// </summary>
        [DataMember(Name = "context")]
        public Dictionary<string, object> Context { get; set; }
    }
}

// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Bot.Builder.Azure.V3V4
{
    /// <summary>
    /// From https://github.com/microsoft/BotBuilder-Azure
    /// </summary>
    public partial class BotData
    {
        public BotData()
        {
            CustomInit();
        }

        public BotData(string eTag = default(string), object data = default(object))
        {
            if(data is JObject jobj)
            {
                Data = jobj.ToObject<IDictionary<string, object>>();
            }
            else if(data == null)
            {
                Data = new Dictionary<string, object>();
            }
            else
            {
                Data = data;
            }
            ETag = eTag;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "data")]
        public object Data { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "eTag")]
        public string ETag { get; set; }

        /// <summary>
        /// Get a property from a BotData recorded retrieved using the REST API
        /// </summary>
        /// <param name="property">property name to change</param>
        /// <returns>property requested or default for type</returns>
        public TypeT GetProperty<TypeT>(string property)
        {
            if (this.Data == null)
                this.Data = new JObject();

            dynamic data = this.Data;
            try
            {
                if (data[property] == null)
                    return default(TypeT);
            }
            catch (System.Collections.Generic.KeyNotFoundException)
            {
                // Property doesn't exist
                return default(TypeT);
            }

            var propValue = data[property];
            if (propValue is TypeT)
            {
                return propValue;
            }

            // convert jToken (JArray or JObject) to the given typeT
            return (TypeT)(data[property].ToObject(typeof(TypeT)));
        }


        /// <summary>
        /// Set a property on a BotData record retrieved using the REST API
        /// </summary>
        /// <param name="property">property name to change</param>
        /// <param name="data">new data</param>
        public void SetProperty<TypeT>(string property, TypeT data)
        {
            if (this.Data == null)
                this.Data = new JObject();

            // convert (object or array) to JToken (JObject/JArray)
            if (data == null)
                ((IDictionary<string,object>)this.Data)[property] = null;
            else
                ((IDictionary<string, object>)this.Data)[property] = JToken.FromObject(data);
        }

        /// <summary>
        /// Remove a property from the BotData record
        /// </summary>
        /// <param name="property">property name to remove</param>
        public void RemoveProperty(string property)
        {
            if (this.Data == null)
                this.Data = new JObject();

            ((IDictionary<string, object>)this.Data).Remove(property);
        }
    }
}

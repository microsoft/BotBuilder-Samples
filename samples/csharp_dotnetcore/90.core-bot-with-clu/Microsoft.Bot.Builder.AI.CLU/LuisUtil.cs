// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Microsoft.Bot.Builder.AI.CLU
{
    // Utility functions used to extract and transform data from Luis SDK
    internal static class LuisUtil
    {
        internal const string MetadataKey = "$instance";
        internal const string GeoV2 = "builtin.geographyV2.";
        internal static readonly HashSet<string> _dateSubtypes = new HashSet<string> { "date", "daterange", "datetime", "datetimerange", "duration", "set", "time", "timerange" };
        internal static readonly HashSet<string> _geographySubtypes = new HashSet<string> { "poi", "city", "countryRegion", "continent", "state" };
        
        internal static string NormalizedIntent(string intent) => intent.Replace('.', '_').Replace(' ', '_');

        internal static IDictionary<string, IntentScore> GetIntents(JObject luisResult)
        {
            var result = new Dictionary<string, IntentScore>();
            var intents = (JObject)luisResult["intents"];
            if (intents != null)
            {
                foreach (var intent in intents)
                {
                    result.Add(NormalizedIntent(intent.Key), new IntentScore { Score = intent.Value["score"] == null ? 0.0 : intent.Value["score"].Value<double>() });
                }
            }

            return result;
        }

        // Remove role and ensure that dot and space are not a part of entity names since we want to do JSON paths.
        internal static string NormalizeEntity(string entity)
        {
            // Type::Role -> Role
            var type = entity.Split(':').Last();
            return type.Replace('.', '_').Replace(' ', '_');
        }

        internal static JToken MapProperties(JToken source, bool inInstance)
        {
            var result = source;
            if (source is JObject obj)
            {
                var nobj = new JObject();

                // Fix datetime by reverting to simple timex
                if (!inInstance && obj.TryGetValue("type", out var type) && type.Type == JTokenType.String && _dateSubtypes.Contains(type.Value<string>()))
                {
                    var timexs = obj["values"];
                    var arr = new JArray();
                    if (timexs != null)
                    {
                        var unique = new HashSet<string>();
                        foreach (var elt in timexs)
                        {
                            unique.Add(elt["timex"]?.Value<string>());
                        }

                        foreach (var timex in unique)
                        {
                            arr.Add(timex);
                        }

                        nobj["timex"] = arr;
                    }

                    nobj["type"] = type;
                }
                else
                {
                    // Map or remove properties
                    foreach (var property in obj.Properties())
                    {
                        var name = NormalizeEntity(property.Name);
                        var isObj = property.Value.Type == JTokenType.Object;
                        var isArray = property.Value.Type == JTokenType.Array;
                        var isString = property.Value.Type == JTokenType.String;
                        var isInt = property.Value.Type == JTokenType.Integer;
                        var val = MapProperties(property.Value, inInstance || property.Name == MetadataKey);
                        if (name == "datetime" && isArray)
                        {
                            nobj.Add("datetimeV1", val);
                        }
                        else if (name == "datetimeV2" && isArray)
                        {
                            nobj.Add("datetime", val);
                        }
                        else if (inInstance)
                        {
                            // Correct $instance issues
                            if (name == "length" && isInt)
                            {
                                nobj.Add("endIndex", property.Value.Value<int>() + property.Parent["startIndex"].Value<int>());
                            }
                            else if (!((isInt && name == "modelTypeId") ||
                                       (isString && name == "role")))
                            {
                                nobj.Add(name, val);
                            }
                        }
                        else
                        {
                            // Correct non-$instance values
                            if (name == "unit" && isString)
                            {
                                nobj.Add("units", val);
                            }
                            else
                            {
                                nobj.Add(name, val);
                            }
                        }
                    }
                }

                result = nobj;
            }
            else if (source is JArray arr)
            {
                var narr = new JArray();
                foreach (var elt in arr)
                {
                    // Check if element is geographyV2
                    var isGeographyV2 = string.Empty;
                    foreach (var props in elt.Children())
                    {
                        var tokenProp = props as JProperty;
                        if (tokenProp == null)
                        {
                            break;
                        }

                        if (tokenProp.Name.Contains("type") && _geographySubtypes.Contains(tokenProp.Value.ToString()))
                        {
                            isGeographyV2 = tokenProp.Value.ToString();
                            break;
                        }
                    }

                    if (!inInstance && !string.IsNullOrEmpty(isGeographyV2))
                    {
                        var geoEntity = new JObject();
                        foreach (var props in elt.Children())
                        {
                            var tokenProp = props as JProperty;
                            if (tokenProp.Name.Contains("value"))
                            {
                                geoEntity.Add("location", tokenProp.Value);
                            }
                        }

                        geoEntity.Add("type", isGeographyV2);
                        narr.Add(geoEntity);
                    }
                    else
                    {
                        narr.Add(MapProperties(elt, inInstance));
                    }
                }

                result = narr;
            }

            return result;
        }

        internal static JObject ExtractEntitiesAndMetadata(JObject prediction)
        {
            var entities = (JObject)JObject.FromObject(prediction["entities"]);
            return (JObject)MapProperties(entities, false);
        }

        internal static void AddProperties(JObject luis, RecognizerResult result)
        {
            var sentiment = luis["sentiment"];
            if (luis["sentiment"] != null)
            {
                result.Properties.Add("sentiment", new JObject(
                    new JProperty("label", sentiment["label"]),
                    new JProperty("score", sentiment["score"])));
            }
        }
    }
}

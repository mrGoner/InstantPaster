using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace InstantPaster
{
    internal class ConfigurationSerializer
    {
        public const string PConfiguration = "configuration";
        public const string PCombination = "combination";
        public const string PDescription = "description";
        public const string PActionType = "type";
        public const string PActionContent = "content";

        public string Serialize(Configuration _configuration)
        {
            if (_configuration == null)
                throw new ArgumentNullException(nameof(_configuration));

            try
            {
                var jHotKeys = new JArray();

                foreach (var hotKey in _configuration.HotKeys)
                {
                    var jCombination = new JObject
                    {
                        new JProperty(PDescription, hotKey.Combination),
                        new JProperty(PCombination, hotKey.Combination),
                        new JProperty(PActionType, hotKey.ActionType.ToString()),
                        new JProperty(PActionContent, hotKey.ActionContent)
                    };

                    jHotKeys.Add(jCombination);
                }

                var jObject = new JObject {new JProperty(PConfiguration, jHotKeys)};

                return jObject.ToString();
            }
            catch (Exception ex)
            {
                throw new ConfigurationSerializerException("Error occured while serialization", ex);
            }
        }

        public Configuration Deserialize(string _data)
        {
            try
            {
                var jObject = JObject.Parse(_data);

                if (jObject[PConfiguration] is JArray jHotKeys)
                {
                    var hotKeys = new List<HotKeySettings>();

                    foreach (var jHotKey in jHotKeys)
                    {
                        var description = jHotKey[PDescription].Value<string>();

                        var actionTypeRaw = jHotKey[PActionType].Value<string>();

                        if (!Enum.TryParse(actionTypeRaw, out ActionType actionType))
                            throw new InvalidOperationException($"Failed to parse actionType <{actionTypeRaw}>");

                        var actionContent = jHotKey[PActionContent].Value<string>();

                        var combination = jHotKey[PCombination].Value<string>();

                        hotKeys.Add(new HotKeySettings(combination, description, actionType, actionContent));
                    }

                    return new Configuration(hotKeys);
                }

                throw new InvalidOperationException($"{PConfiguration} not found");
            }
            catch (Exception ex)
            {
                throw new ConfigurationSerializerException("Failed to deserialize configuration", ex);
            }
        }
    }
}

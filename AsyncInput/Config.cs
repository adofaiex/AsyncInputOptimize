using ModsTagLib.FileFormat.Json;
using ModsTagLib.Win32;
using System.Collections.Generic;

namespace AsyncInput
{
    internal static class Config
    {
        public const string FILE_NAME = "Config.json";

        internal static List<VirtualKeys> SelectKeys = new();
        internal static bool BlackListMode = false;

        internal static JObject JObject
        {
            get
            {
                JObject obj = new();
                JArray keys = new JArray();
                foreach (VirtualKeys vk in SelectKeys)
                {
                    keys.Add((int)vk);
                }
                obj.Add("Keys", keys);
                obj.Add(nameof(BlackListMode), BlackListMode);
                return obj;
            }
            set
            {
                JArray keys = value.GetArray("Keys");
                foreach (JValue jv in keys)
                {
                    if (jv.IsInt)
                        SelectKeys.Add((VirtualKeys)jv.Int);
                }
                BlackListMode = value.GetBool(nameof(BlackListMode));
            }
        }
    }
}

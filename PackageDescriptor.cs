using System;
using System.IO;
using Newtonsoft.Json;

namespace CarbyScript
{
    [Serializable]
    internal class PackageDescriptor
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("main")]
        public string Main { get; set; }

        public static PackageDescriptor Load(string path)
        {
            return JsonConvert.DeserializeObject<PackageDescriptor>(File.ReadAllText(path));
        }
    }
}
